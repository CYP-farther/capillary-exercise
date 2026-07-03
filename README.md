# Capillary Exercise

> Process-driven development practice: pickup flow implementation

## 项目背景

这是一个**流程驱动开发的练习项目**，以劈刀（Capillary）自动存取管理系统为案例，完整走一遍：

**需求整理 → 概要设计 → Issue 拆解 → 测试设计 → 规范底座 → 迭代开发**

的软件工程闭环。

## 当前范围

聚焦**领料流程（Pickup Flow）**：

1. 输入工单号 + 机台号
2. MES 查询所需劈刀类型
3. 系统 FIFO 查找库存
4. PLC 取出劈刀并读码验证
5. 上报 MES
6. 出料

## 开发原则

- **设计先于编码**：每个功能先有 Issue + 设计，再写代码
- **测试驱动思考**：先想清楚如何验证，再写实现
- **流程倒逼习惯**：用 Git 分支、PR、CI 约束开发流程
- **AI 全程协作**：从设计到测试到编码，AI 是思考伙伴，但人主导决策

## 目录结构

```
capillary-exercise/
├── doc/               # 文档（编号：001-XXX.md, 002-XXX.md...）
│   └── 001-REQUIREMENTS.md
├── src/               # 源代码
├── test/              # 测试代码
├── .github/
│   └── workflows/     # CI/CD 配置
└── README.md
```

**文档编号规则**：
- 001-REQUIREMENTS.md — 需求文档
- 002-DESIGN.md — 概要设计
- 003-TEST_PLAN.md — 测试计划
- 004-CODING_STANDARD.md — 代码规范
- ...

## 参考资料

本项目基于 [002-XinJi_AI_Demo](https://github.com/Luoyuetong/002-XinJi_AI_Demo) 的 `doc/design.md` 提炼需求，但从零实现。

## 版本历史

| 版本 | 日期 | 里程碑 |
|------|------|--------|
| v0.1 | 2026-06-21 | 项目初始化，需求文档、目录结构 |
| v0.2 | 2026-06-22 | 硬件/MES 接口定义 + 数据访问层 |
| v0.3 | 2026-06-23 | 进程内 Fakes（PLC/Scanner/MES） |
| v0.4 | 2026-06-24 | PickupService 领料流程编排 |
| v0.5 | 2026-06-25 | PickupForm 界面 + 进度追踪 |
| v0.6 | 2026-06-26 | E2E 测试 + CI 流水线 |
| v0.7 | 2026-07-03 | 迁移至 CYP-farther/capillary-exercise |

## 开发日志

- 2026-06-21: 初始化仓库，建立目录结构，完成需求文档
