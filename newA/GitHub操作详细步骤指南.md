# GitHub 操作详细步骤指南

> 写给新手的 GitHub 操作教程 · 基于 capillary-exercise 项目真实流程

---

## 前置准备：安装与登录

开始之前，确认两件事：

**1. 安装 Git**

在终端里输入：
```bash
git --version
```
如果输出版本号（如 `git version 2.45.0`），说明已安装。如果没有，去 [git-scm.com](https://git-scm.com) 下载安装。

**2. 安装 GitHub CLI（gh）**

```bash
gh --version
```
如果输出版本号，说明已安装。如果没有，去 [cli.github.com](https://cli.github.com) 下载安装。

**3. 登录 GitHub**

```bash
gh auth login
```

按提示选择：
- `GitHub.com` → `HTTPS` → `Login with a web browser`
- 浏览器弹出验证码，输入确认，完成登录。

---

## 操作一：Issue（任务卡片）

### 是什么

Issue 是 GitHub 上的**任务卡片**。它用来描述"要做什么、怎么算做完"。在本项目中，每个 Issue 是一个**可独立交付的工作单元**。

### 在浏览器中创建 Issue

1. 打开仓库页面：`https://github.com/你的用户名/仓库名/issues`
2. 点击绿色按钮 **New issue**
3. 填写：
   - **Title**：简短描述任务（如 `实现领料业务逻辑 PickupService`）
   - **Description**：详细写清三件事：
     - **要做什么**（功能描述）
     - **交付物**（代码 + 测试 + 文档？）
     - **验收标准**（怎么判断做完了？可以用 checkbox 列表）

   **示例描述**（本项目真实 Issue 风格）：

   ```markdown
   ## 要做什么
   实现 PickupService，编排完整的领料流程。

   ## 交付物
   - PickupService 类
   - 覆盖所有成功/失败分支的单元测试

   ## 验收标准
   - [ ] 正常领料流程：MES 查类型 → 库存找货 → PLC 取料 → 扫码 → 上报 → 出料
   - [ ] 库存不足时返回失败，不动硬件
   - [ ] 读码失败时放回仓位并锁定
   - [ ] 所有测试通过（dotnet test）
   ```

4. 点击 **Submit new issue**
5. 记录下 Issue 编号（如 `#7`），后续建分支、提 PR 都会用到

### 用命令行创建 Issue

```bash
gh issue create --title "实现领料业务逻辑 PickupService" --body "## 要做什么
实现 PickupService，编排完整的领料流程。

## 交付物
- PickupService 类
- 单元测试

## 验收标准
- [ ] 正常流程跑通
- [ ] 库存不足拦截
- [ ] 读码失败锁仓位"
```

创建成功后命令行会输出 Issue 链接和编号。

### 创建后的状态

- Issue 默认状态是 **Open**（待处理）
- 分配到某个开发者（Assignees）
- 可以加标签（Labels）区分类型
- PR 合并后 Issue 会自动关闭（通过 `Closes #N` 关联）

---

## 操作二：Branch（分支）

### 是什么

分支是 Git 的核心概念——**在独立的线上改代码，不影响主线**。每个 Issue 对应一条分支，改完后通过 PR 合入 `main`。

### 分支命名规则

本项目统一使用：

```
feature/issue-{编号}-{简短描述}
```

例如：
- `feature/issue-1-project-skeleton`
- `feature/issue-7-pickup-service`
- `feature/issue-10-ci`

### 创建分支（完整步骤）

**第 1 步：确保在 main 分支，且是最新状态**

```bash
# 切换到 main
git checkout main

# 拉取最新代码
git pull origin main
```

**第 2 步：创建并切换到新分支**

```bash
# 创建分支 + 切换，一条命令搞定
git checkout -b feature/issue-7-pickup-service
```

这条命令等效于：
```bash
git branch feature/issue-7-pickup-service    # 创建分支
git checkout feature/issue-7-pickup-service  # 切换到分支
```

**第 3 步：确认当前在正确的分支上**

```bash
git branch
```

有 `*` 标记的就是当前分支：
```
  main
* feature/issue-7-pickup-service
```

### 在分支上开发

在分支上正常写代码、改文件。完成后提交：

```bash
# 查看改了哪些文件
git status

# 将改动加入暂存区
git add src/CapillaryExercise/Services/PickupService.cs
git add test/CapillaryExercise.Tests/

# 提交（写清做了什么）
git commit -m "实现 PickupService 领料编排逻辑

- 正常流程：MES 查类型 → FIFO 找库存 → PLC 取料 → 扫码 → 上报 → 出料
- 异常处理：库存不足 / 读码失败 / MES 失败 / PLC 失败
- 单元测试覆盖全部 11 个用例（TC-01~TC-11）

Closes #7

Co-Authored-By: Claude <noreply@anthropic.com>"
```

> **提交信息规范**（本项目约定）：
> - 第一行：一句话概括做了什么
> - 空一行后：详细说明（可选）
> - 结尾：`Closes #N` + `Co-Authored-By`

**推送到 GitHub：**

```bash
# 第一次推送（把本地分支推上去）
git push -u origin feature/issue-7-pickup-service

# 后续推送（已建立关联后）
git push
```

---

## 操作三：Pull Request（PR）

### 是什么

PR（拉取请求）是**把分支上的改动打包成一个"提案"，请求合并进 `main`**。它是 CI 自动检查和 Code Review 人工审查的共同载体。

### 提 PR 之前

1. **本地测试通过**：`dotnet test` 全绿
2. **提交已推送**：代码在 GitHub 上
3. **想清楚三件事**：做了什么、为什么这么做、怎么验证的

### 用命令行创建 PR

```bash
gh pr create \
  --title "实现 PickupService 领料编排与服务层异常处理" \
  --body "## 做了什么

实现了 PickupService，负责编排完整的领料流程，并处理所有异常分支。

## 为什么这么做

- 业务编排是系统的核心，所有领料路径（成功 + 4 种失败）集中在 Service 层
- 所有依赖走接口注入，方便测试时换 NSubstitute 替身
- 异常/失败用返回值表达，不抛异常（符合设计决策）

## 怎么验证

- 本地 dotnet test：全部测试通过
- 单元测试覆盖 5 条路径：成功、库存不足、读码失败、MES 失败、PLC 取料失败

Closes #7

🤖 Generated with [Claude Code](https://claude.com/claude-code)"
```

`gh pr create` 默认：
- **base 分支**：`main`（合入目标）
- **head 分支**：当前所在分支（改动来源）

如果是第一次推送该分支，需要指定远端：
```bash
gh pr create --base main --head feature/issue-7-pickup-service --title "..." --body "..."
```

### 用浏览器创建 PR

1. push 分支后，打开仓库页面，GitHub 通常会自动弹出黄色提示 "feature/xxx had recent pushes"，点击 **Compare & pull request**
2. 或者手动进入 `https://github.com/你的用户名/仓库名/pull/new/feature/issue-7-pickup-service`
3. 填写标题和描述（格式同上），正文里写 `Closes #7`
4. 点击 **Create pull request**

### 创建之后

PR 创建成功会有几个自动发生的事情：

```
┌────────────────────────── 一个 PR ──────────────────────────┐
│                                                              │
│  PR #24: 实现 PickupService                                 │
│  ┌──────────────────────────────────────────────────────┐   │
│  │ ① GitHub Actions 自动触发 CI                         │   │
│  │    编译 + 跑测试 → 绿 ✓ / 红 ✗                       │   │
│  │    ↓                                                 │   │
│  │ ② CI 绿了 → 进入 Code Review                         │   │
│  │    ↓                                                 │   │
│  │ ③ Review 通过 → 点 Merge pull request                │   │
│  │    ↓                                                 │   │
│  │ ④ Issue #7 自动关闭（因为 PR 正文写了 Closes #7）     │   │
│  └──────────────────────────────────────────────────────┘   │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```

---

## 操作四：CI（GitHub Actions 自动检查）

### 是什么

CI 是**每次 push 或提 PR 时，GitHub 自动在云端跑编译 + 测试**，验证代码质量。配置文件是 `.github/workflows/build-test.yml`。

### 你的角色：不需要操作，只需要看结果

CI 是**自动触发**的，不需要你手动启动。你只需要：

1. **提 PR 后等待**：打开 PR 页面，往下滚，会看到 "Some checks haven't completed yet" 或旋转图标
2. **看结果**：
   - 🟢 **绿色对勾（All checks have passed）**：编译和测试全通过 → 可以进入 Code Review
   - 🔴 **红色叉号（All checks have failed）**：有错误 → 点进去看详情，修复后重新 push

### 如果 CI 红了，怎么排查

**第 1 步：点进失败的 Checks**

在 PR 页面底部，点红色的叉号 → 点 **Details** → 进入 Actions 页面

**第 2 步：展开失败的 Step**

找到标红色的步骤（比如 `Test`），展开看日志：

```
Failed CapillaryExercise.Tests.PickupServiceTests.Pickup_ReadFails_LocksSlot
Duration: 15 ms
Message:
  Assert.False() Failure
  Expected: False
  Actual:   True
```

**第 3 步：在本地重现**

```bash
# 在本地跑同样的测试
dotnet test
```

如果有失败，修改代码，修复后重新提交：

```bash
git add .
git commit -m "修复: 读码失败后仓位锁定逻辑"
git push
```

推送后 CI 会**自动重新跑**，不用手动触发。

### 本项目的 CI 内容

本项目的 `.github/workflows/build-test.yml`（真实配置）：

```yaml
name: Build and Test

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build-test:
    runs-on: windows-latest    # WinForms 需要 Windows
    steps:
      - name: Checkout          # ① 拉代码
        uses: actions/checkout@v4
      - name: Setup .NET        # ② 装 .NET 9
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'
      - name: Restore           # ③ 还原 NuGet 包
        run: dotnet restore
      - name: Build             # ④ 编译（Release 配置）
        run: dotnet build --configuration Release --no-restore
      - name: Test              # ⑤ 运行全部测试
        run: dotnet test --configuration Release --no-build
```

**触发条件**：
- push 到 `main` 分支
- 向 `main` 分支提 PR

**它做的事**（5 步）：
1. 在云端开一台干净的 Windows 虚拟机
2. 拉取代码 → 装 .NET 9 SDK → 还原 NuGet 包 → 编译 → 跑全部测试
3. 全绿 = 允许合并，红了 = 挡住

---

## 操作五：Code Review（代码审查）

### 是什么

CI 通过后，代码还不能直接合并——需要**人工审查**代码的设计、逻辑、可读性。CI 管"能不能跑"，Review 管"好不好"。

### 在 GitHub 上审 PR（多人项目）

1. 打开 PR 页面
2. 点击 **Files changed**，逐文件看改了什么
3. 对某一行有意见：
   - 鼠标悬停到行号左侧，点击蓝色 **+** 号
   - 输入评论，选择 **Start a review**（开始一轮审查）
4. 审完后，点右上角 **Review changes** 绿色按钮：
   - **Comment**：中性意见，不表态通过还是拒绝
   - **Approve**：审查通过，允许合并
   - **Request changes**：有问题必须改，改完再找我
5. 提交审查意见

### 单人项目怎么做 Review（项目真实做法）

单人项目没有第二个审查者，但**仍然值得做**。三个办法：

**办法一：隔夜再回来看**

写完代码当天别急着合并，第二天再打开 PR 的 **Files changed**，以"陌生人"的眼光读一遍——很多当时觉得理所当然的写法，隔一夜再看会觉得不对劲。

**办法二：先读自己的 PR 描述**

PR 描述写了"做了什么、为什么"。如果你自己读着觉得解释不顺、逻辑跳跃——那往往说明代码本身没想清楚。

**办法三：对照 Issue 逐条验收**

把原始 Issue 拉出来，一条条对：
- Issue 要求的功能都实现了吗？
- 验收标准的 checkbox 都能打勾吗？
- 有没有写着写着偏离了初衷、顺手塞了不该加的东西？

### AI 作为"第二双眼睛"

在单人项目中，可以把 PR 的 diff 交给 AI（如 Claude Code）来审：

- 让 AI 指出潜在的遗漏（空值检查、缺少测试覆盖等）
- AI 提供建议，**但最终判断仍然是你自己的事**——AI 不懂你的业务上下文

---

## 总结：完整操作流程

下面是一次完整的 Issue 开发流程中，你用到的所有 GitHub 操作：

```
第 1 步：建 Issue
    → 浏览器 https://github.com/.../issues/new
    → 或 gh issue create --title "..." --body "..."

第 2 步：建分支
    → git checkout main && git pull origin main
    → git checkout -b feature/issue-N-描述

第 3 步：编码 + 提交
    → 写代码
    → git add .
    → git commit -m "做了什么... Closes #N"
    → git push -u origin feature/issue-N-描述

第 4 步：本地测试
    → dotnet test    （必须全绿再提 PR）

第 5 步：提 PR
    → gh pr create --title "..." --body "... Closes #N"
    → 此时 GitHub Actions 自动触发 CI

第 6 步：看 CI 结果
    → 全绿 ✓ → 继续
    → 红了 ✗ → 修代码 → git push → 回到第 4 步

第 7 步：Code Review
    → PR 页面的 Files changed 逐文件审查
    → 有问题提意见，没问题点 Approve

第 8 步：合并 PR
    → 点 Merge pull request
    → Issue 自动关闭（因为 PR 正文有 Closes #N）

第 9 步：切回 main 同步
    → git checkout main
    → git pull origin main
    → 继续下一个 Issue（回到第 1 步）
```

### 本章涉及的 Git 命令速查

| 命令 | 作用 |
|------|------|
| `git checkout main` | 切换到 main 分支 |
| `git pull origin main` | 拉取远端最新代码 |
| `git checkout -b feature/issue-N-xxx` | 创建并切换到新分支 |
| `git status` | 查看当前改动了哪些文件 |
| `git add .` | 把所有改动加入暂存区 |
| `git commit -m "..."` | 提交（本地保存） |
| `git push -u origin feature/issue-N-xxx` | 首次推送分支到 GitHub |
| `git push` | 后续推送（已建立远端关联） |
| `gh issue create` | 命令行创建 Issue |
| `gh pr create` | 命令行创建 Pull Request |
| `dotnet test` | 本地跑测试 |

---

*本指南基于 capillary-exercise 项目的真实开发流程编写。*
