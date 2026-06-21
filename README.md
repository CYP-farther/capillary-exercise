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

## 参考资料

本项目基于 [002-XinJi_AI_Demo](https://github.com/Luoyuetong/002-XinJi_AI_Demo) 的 `doc/design.md` 提炼需求，但从零实现。

## 开发日志

- 2026-06-21: 初始化仓库，准备开始需求整理
