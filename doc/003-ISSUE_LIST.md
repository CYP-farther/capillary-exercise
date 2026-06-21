# Issue 拆解清单

基于 `002-DESIGN.md`，将领料流程拆解为 10 个可独立开发的 GitHub Issues。

---

## Issue 列表

### 阶段一：基础设施（#1-#2）

| Issue | 标题 | 依赖 | 说明 |
|-------|------|------|------|
| [#1](https://github.com/Luoyuetong/capillary-exercise/issues/1) | 搭建项目基础结构 | 无 | 创建 .NET 9 解决方案、主项目、测试项目 |
| [#2](https://github.com/Luoyuetong/capillary-exercise/issues/2) | 实现数据访问层 | #1 | DbHelper, CapillaryRepository, LogRepository |

### 阶段二：接口与 Mock 实现（#3-#6）

| Issue | 标题 | 依赖 | 说明 |
|-------|------|------|------|
| [#3](https://github.com/Luoyuetong/capillary-exercise/issues/3) | 定义硬件接口 | #1 | IPlcController, IScanner |
| [#4](https://github.com/Luoyuetong/capillary-exercise/issues/4) | 实现 TcpPlcClient | #3 | 连接 MockPLC (TCP 9002) |
| [#5](https://github.com/Luoyuetong/capillary-exercise/issues/5) | 实现 TcpScannerClient | #3 | 连接 MockScanner (TCP 9001) |
| [#6](https://github.com/Luoyuetong/capillary-exercise/issues/6) | 实现 IMesService 和 HttpMesClient | #1 | 连接 MockMES (HTTP 9003) |

### 阶段三：业务逻辑与界面（#7-#8）

| Issue | 标题 | 依赖 | 说明 |
|-------|------|------|------|
| [#7](https://github.com/Luoyuetong/capillary-exercise/issues/7) | 实现 PickupService | #2, #3, #4, #5, #6 | 领料流程编排 + 异常处理 |
| [#8](https://github.com/Luoyuetong/capillary-exercise/issues/8) | 实现 PickupForm | #7 | WinForms 界面 + 进度显示 |

### 阶段四：集成与 CI（#9-#10）

| Issue | 标题 | 依赖 | 说明 |
|-------|------|------|------|
| [#9](https://github.com/Luoyuetong/capillary-exercise/issues/9) | 集成测试：端到端流程 | #1-#8 | Mock 程序 + 预置数据 + E2E 测试 |
| [#10](https://github.com/Luoyuetong/capillary-exercise/issues/10) | 搭建 GitHub Actions CI | #1-#9 | 自动编译 + 测试 |

---

## 开发顺序建议

### 第一批（可并行）
- #1: 搭建项目结构
- #2: 数据访问层
- #3: 定义硬件接口

### 第二批（依赖第一批）
- #4: TcpPlcClient
- #5: TcpScannerClient
- #6: HttpMesClient

### 第三批（依赖第二批）
- #7: PickupService

### 第四批（依赖第三批）
- #8: PickupForm

### 第五批（依赖所有）
- #9: 集成测试
- #10: CI 配置

---

## Issue 与设计文档映射

| Issue | 对应设计章节 |
|-------|-------------|
| #1 | 第八节：项目结构 |
| #2 | 第三节：数据模型 + 第四节4.4-4.5 |
| #3 | 第四节4.1-4.2 |
| #4 | 第四节4.1 |
| #5 | 第四节4.2 |
| #6 | 第四节4.3 |
| #7 | 第五节5.1 |
| #8 | 第二节2.1（需求） |
| #9 | 第九节：Mock程序 |
| #10 | 第六节：依赖注入 |

---

## 验收标准汇总

每个 Issue 完成时：
- [ ] 功能实现符合设计文档
- [ ] 单元测试覆盖核心逻辑
- [ ] 代码遵守规范（待定义 `004-CODING_STANDARD.md`）
- [ ] 通过 CI 编译和测试
- [ ] PR 经过 Code Review（自审）

---

## 预估工作量

| 阶段 | Issues | 预估 |
|------|--------|------|
| 基础设施 | #1-#2 | 2-3h |
| 接口与 Mock | #3-#6 | 4-5h |
| 业务与界面 | #7-#8 | 3-4h |
| 集成与 CI | #9-#10 | 2-3h |
| **总计** | 10 Issues | **11-15h** |

> 注：预估基于单人开发、使用 AI 辅助的情况。
