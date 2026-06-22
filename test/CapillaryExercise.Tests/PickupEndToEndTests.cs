using System.Globalization;
using CapillaryExercise.Data;
using CapillaryExercise.Hardware;
using CapillaryExercise.Services;
using Microsoft.Data.Sqlite;

namespace CapillaryExercise.Tests;

/// <summary>
/// 端到端测试（TC-20~TC-23）：进程内组装真实 PickupService + 真实 Repository(SQLite)
/// + 三个进程内 Fake，全程内串联，不 Mock 任何业务依赖。
/// 与 PickupServiceTests 的区别：那里 Mock 掉一切只验证业务分支，这里走真实数据库与 Fake，
/// 验证「组装起来确实能跑通」，断言落在持久化后的真实状态（劈刀状态、日志条数）上。
/// 每个测试用例使用独立临时 SQLite 文件，保证隔离。
/// 用 [Trait("Category","E2E")] 标记，便于 CI 按需筛选（见 CLAUDE.md 环境提示）。
/// </summary>
[Trait("Category", "E2E")]
public class PickupEndToEndTests : IDisposable
{
    private readonly string _dbPath;
    private readonly DbHelper _db;
    private readonly CapillaryRepository _capRepo;
    private readonly LogRepository _logRepo;

    public PickupEndToEndTests()
    {
        // 每个测试一个独立临时文件，互不干扰。
        _dbPath = Path.Combine(Path.GetTempPath(), $"cap-e2e-{Guid.NewGuid():N}.db");
        _db = new DbHelper(_dbPath);
        _db.InitializeSchema();
        _capRepo = new CapillaryRepository(_db);
        _logRepo = new LogRepository(_db);
    }

    public void Dispose()
    {
        // 释放连接池对文件的句柄后再删除，避免 Windows 上文件占用。
        SqliteConnection.ClearAllPools();
        if (File.Exists(_dbPath))
        {
            File.Delete(_dbPath);
        }
    }

    // ---- TC-20：端到端 - 正常领料 ----

    [Fact]
    public async Task EndToEnd_HappyPath_MarksPickedOutAndLogsSuccess()
    {
        // Arrange：MES 预置 WO001→CAP-A；库存有 CAP-A(BC001, A,5,10)；扫码器预置 BC001（匹配）。
        SeedCapillary("BC001", "CAP-A", "A", 5, 10, new DateTime(2026, 1, 1, 8, 0, 0));
        var service = BuildService(
            mes: new FakeMesService().WithType("WO001", "CAP-A"),
            scanner: new FakeScanner("BC001"));
        var reports = new List<string>();

        // Act
        var result = await service.ExecuteAsync("WO001", "M01", new SyncProgress(reports.Add));

        // Assert：成功；劈刀真实落库为已领出(1)，关联工单/机台；日志记一条 Success。
        Assert.True(result.IsSuccess);
        Assert.Contains("BC001", result.Message);

        var cap = _capRepo.GetByBarcode("BC001");
        Assert.NotNull(cap);
        Assert.Equal(1, cap!.Status);
        Assert.Equal("WO001", cap.WorkOrder);
        Assert.Equal("M01", cap.MachineNo);

        Assert.Equal(1, CountLogs("Success"));
        // 进度逐步报告，覆盖各关键步骤。
        Assert.Contains(reports, m => m.Contains("查询劈刀类型"));
        Assert.Contains(reports, m => m.Contains("出料"));
    }

    // ---- TC-21：端到端 - 读码失败 ----

    [Fact]
    public async Task EndToEnd_ScanMismatch_ReturnsToSlotAndLocks()
    {
        // Arrange：库存 BC001，但扫码器返回错误条码 → 读码不匹配。
        SeedCapillary("BC001", "CAP-A", "A", 5, 10, new DateTime(2026, 1, 1, 8, 0, 0));
        var service = BuildService(
            mes: new FakeMesService().WithType("WO001", "CAP-A"),
            scanner: new FakeScanner("BC999"));

        // Act
        var result = await service.ExecuteAsync("WO001", "M01", new SyncProgress(_ => { }));

        // Assert：失败；劈刀落库为锁定(2)，未关联工单；日志记一条 Fail / 读码失败。
        Assert.False(result.IsSuccess);
        Assert.Contains("读码失败", result.Message);

        var cap = _capRepo.GetByBarcode("BC001");
        Assert.NotNull(cap);
        Assert.Equal(2, cap!.Status);
        Assert.Null(cap.WorkOrder);

        Assert.Equal(1, CountLogs("Fail"));
        Assert.Equal(0, CountLogs("Success"));
    }

    // ---- TC-22：端到端 - MES 拒绝 ----

    [Fact]
    public async Task EndToEnd_MesRejects_ReturnsToSlotAndLocks()
    {
        // Arrange：读码匹配，但 FakeMes 配置为拒绝上报。
        SeedCapillary("BC001", "CAP-A", "A", 5, 10, new DateTime(2026, 1, 1, 8, 0, 0));
        var service = BuildService(
            mes: new FakeMesService { ShouldApprovePickup = false }.WithType("WO001", "CAP-A"),
            scanner: new FakeScanner("BC001"));

        // Act
        var result = await service.ExecuteAsync("WO001", "M01", new SyncProgress(_ => { }));

        // Assert：失败；劈刀落库为锁定(2)；日志记一条 Fail / MES拒绝。
        Assert.False(result.IsSuccess);
        Assert.Contains("MES拒绝", result.Message);

        var cap = _capRepo.GetByBarcode("BC001");
        Assert.NotNull(cap);
        Assert.Equal(2, cap!.Status);
        Assert.Null(cap.WorkOrder);

        Assert.Equal(1, CountLogs("Fail"));
    }

    // ---- TC-23：端到端 - 无库存 ----

    [Fact]
    public async Task EndToEnd_NoStock_FailsWithoutTouchingHardwareOrDb()
    {
        // Arrange：MES 预置 WO002→CAP-B，但库存里只有 CAP-A，没有 CAP-B。
        SeedCapillary("BC001", "CAP-A", "A", 5, 10, new DateTime(2026, 1, 1, 8, 0, 0));
        var service = BuildService(
            mes: new FakeMesService().WithType("WO002", "CAP-B"),
            scanner: new FakeScanner("BC001"));

        // Act
        var result = await service.ExecuteAsync("WO002", "M01", new SyncProgress(_ => { }));

        // Assert：失败含"库存不足"；未动硬件 → 既有库存状态不变(0)，无任何日志。
        Assert.False(result.IsSuccess);
        Assert.Contains("库存不足", result.Message);

        var cap = _capRepo.GetByBarcode("BC001");
        Assert.NotNull(cap);
        Assert.Equal(0, cap!.Status);

        Assert.Equal(0, CountLogs());
    }

    // ---- 测试辅助 ----

    /// <summary>
    /// 用真实 Repository 与给定的 Fake 组装出真实 PickupService。
    /// PLC 默认全成功；通过参数注入 MES/扫码器以覆盖各端到端场景。
    /// </summary>
    private PickupService BuildService(FakeMesService mes, FakeScanner scanner)
        => new(new FakePlcController(), scanner, mes, _capRepo, _logRepo);

    /// <summary>
    /// 向 CapillaryInfo 表插入一条在库(Status=0)记录。Repository 不提供插入，故测试直接参数化写库。
    /// </summary>
    private void SeedCapillary(string barcode, string type, string face, int x, int y, DateTime storedTime)
    {
        using var connection = _db.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO CapillaryInfo
                (Barcode, CapillaryType, Face, PosX, PosY, StoredTime, Status, WorkOrder, MachineNo)
            VALUES
                ($barcode, $type, $face, $posX, $posY, $storedTime, 0, NULL, NULL);
            """;
        command.Parameters.AddWithValue("$barcode", barcode);
        command.Parameters.AddWithValue("$type", type);
        command.Parameters.AddWithValue("$face", face);
        command.Parameters.AddWithValue("$posX", x);
        command.Parameters.AddWithValue("$posY", y);
        command.Parameters.AddWithValue("$storedTime", storedTime.ToString("o", CultureInfo.InvariantCulture));
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// 统计 OperationLog 表的记录数；可选按 Result 过滤（"Success"/"Fail"）。
    /// </summary>
    private int CountLogs(string? result = null)
    {
        using var connection = _db.CreateConnection();
        using var command = connection.CreateCommand();
        if (result is null)
        {
            command.CommandText = "SELECT COUNT(*) FROM OperationLog;";
        }
        else
        {
            command.CommandText = "SELECT COUNT(*) FROM OperationLog WHERE Result = $result;";
            command.Parameters.AddWithValue("$result", result);
        }
        return Convert.ToInt32(command.ExecuteScalar());
    }

    /// <summary>
    /// 同步执行回调的 IProgress 实现。默认 Progress&lt;T&gt; 依赖 SynchronizationContext
    /// 异步投递，在无 UI 上下文的测试里会丢报告，故用同步版本确保 Report 立即记录。
    /// </summary>
    private sealed class SyncProgress : IProgress<string>
    {
        private readonly Action<string> _handler;
        public SyncProgress(Action<string> handler) => _handler = handler;
        public void Report(string value) => _handler(value);
    }
}
