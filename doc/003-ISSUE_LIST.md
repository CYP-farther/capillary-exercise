# Issue 拆解清单

基于 `002-DESIGN.md`，将领料流程拆解为 13 个可独立开发的 GitHub Issues。

**关键优化**：Mock 程序（#11-#13）独立成 Issue，先于客户端实现，便于客户端开发时用真实 Mock 测试。

---

## Issue 列表

### 阶段一：基础设施（#1-#2）

| Issue | 标题 | 依赖 | 说明 |
|-------|------|------|------|
| [#1](https://github.com/Luoyuetong/capillary-exercise/issues/1) | 搭建项目基础结构 | 无 | 创建 .NET 9 解决方案、主项目、测试项目 |
| [#2](https://github.com/Luoyuetong/capillary-exercise/issues/2) | 实现数据访问层 | #1 | DbHelper, CapillaryRepository, LogRepository |

### 阶段二：Mock 程序（#11-#13）

| Issue | 标题 | 依赖 | 说明 |
|-------|------|------|------|
| [#11](https://github.com/Luoyuetong/capillary-exercise/issues/11) | 实现 MockPLC 程序 | #1 | TCP Server (端口 9002) |
| [#12](https://github.com/Luoyuetong/capillary-exercise/issues/12) | 实现 MockScanner 程序 | #1 | TCP Server (端口 9001) |
| [#13](https://github.com/Luoyuetong/capillary-exercise/issues/13) | 实现 MockMES 程序 | #1 | HTTP Server (端口 9003) |

### 阶段三：接口与客户端实现（#3-#6）

| Issue | 标题 | 依赖 | 说明 |
|-------|------|------|------|
| [#3](https://github.com/Luoyuetong/capillary-exercise/issues/3) | 定义硬件接口 | #1 | IPlcController, IScanner |
| [#4](https://github.com/Luoyuetong/capillary-exercise/issues/4) | 实现 TcpPlcClient | #3, #11 | 连接 MockPLC，可用真实 Mock 测试 |
| [#5](https://github.com/Luoyuetong/capillary-exercise/issues/5) | 实现 TcpScannerClient | #3, #12 | 连接 MockScanner，可用真实 Mock 测试 |
| [#6](https://github.com/Luoyuetong/capillary-exercise/issues/6) | 实现 IMesService 和 HttpMesClient | #1, #13 | 连接 MockMES，可用真实 Mock 测试 |

### 阶段四：业务逻辑与界面（#7-#8）

| Issue | 标题 | 依赖 | 说明 |
|-------|------|------|------|
| [#7](https://github.com/Luoyuetong/capillary-exercise/issues/7) | 实现 PickupService | #2, #3, #4, #5, #6 | 领料流程编排 + 异常处理 |
| [#8](https://github.com/Luoyuetong/capillary-exercise/issues/8) | 实现 PickupForm | #7 | WinForms 界面 + 进度显示 |

### 阶段五：集成与 CI（#9-#10）

| Issue | 标题 | 依赖 | 说明 |
|-------|------|------|------|
| [#9](https://github.com/Luoyuetong/capillary-exercise/issues/9) | 集成测试：端到端流程 | #1-#8, #11-#13 | 预置数据 + E2E 测试 |
| [#10](https://github.com/Luoyuetong/capillary-exercise/issues/10) | 搭建 GitHub Actions CI | #1-#9 | 自动编译 + 测试 |

---

## 开发顺序建议

### 第一批（可并行）
- #1: 搭建项目结构
- #2: 数据访问层
- #3: 定义硬件接口

### 第二批（依赖第一批）
- #11: MockPLC 程序
- #12: MockScanner 程序
- #13: MockMES 程序

### 第三批（依赖第二批）
- #4: TcpPlcClient（可用真实 MockPLC 测试）
- #5: TcpScannerClient（可用真实 MockScanner 测试）
- #6: HttpMesClient（可用真实 MockMES 测试）

### 第四批（依赖第三批）
- #7: PickupService

### 第五批（依赖第四批）
- #8: PickupForm

### 第六批（依赖所有）
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
| #11 | 第九节9.2（MockPLC） |
| #12 | 第九节9.1（MockScanner） |
| #13 | 第九节9.3（MockMES，需补充） |

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
| Mock 程序 | #11-#13 | 3-4h |
| 接口与客户端 | #3-#6 | 3-4h |
| 业务与界面 | #7-#8 | 3-4h |
| 集成与 CI | #9-#10 | 2-3h |
| **总计** | 13 Issues | **13-18h** |

> 注：预估基于单人开发、使用 AI 辅助的情况。
