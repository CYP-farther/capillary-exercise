# 将 capillary-exercise 项目上传到 GitHub —— 完整操作记录

> **日期**: 2026-07-03
> **目标仓库**: [github.com/CYP-farther/capillary-exercise](https://github.com/CYP-farther/capillary-exercise)
> **状态**: ✅ 上传成功

---

## 背景

将本地已有的 `capillary-exercise` 项目（原托管在 `Luoyuetong/capillary-exercise`）迁移到新建的 `CYP-farther/capillary-exercise` 仓库。项目是一个 .NET 9.0 Windows Forms 应用，实际源码约 658 KB，包含 50+ 个 Git 提交。

---

## 步骤总览

| 步骤 | 操作 | 结果 |
|------|------|------|
| 1 | 环境检查 | Git 2.54.0 / Git LFS 3.7.1 / SSH 已认证 |
| 2 | 探索项目现状 | 已确认大小、文件类型、Git 状态 |
| 3 | 安装 GitHub CLI | 已安装 gh 2.96.0 |
| 4 | 创建 GitHub 远程仓库 | 手动在 github.com 创建 |
| 5 | 更换 remote 地址 | 从 Luoyuetong 改为 CYP-farther |
| 6 | 处理未追踪文件 | 排除草稿文件 |
| 7 | 提交并推送 | 成功推送 50+ 提交 |
| 8 | 验证结果 | 远程 main 分支已更新 |

---

## 详细步骤

### Step 1: 环境检查

```bash
# 检查 Git 版本
git --version
# → git version 2.54.0.windows.1

# 检查 Git LFS 是否可用
git lfs version
# → git-lfs/3.7.1

# 测试 SSH 连接
ssh -T git@github.com
# → Hi CYP-farther! You've successfully authenticated...
```

**这些命令在干什么？**

- `git --version`：确认 Git 已安装且版本足够新。Git 是核心工具，所有版本控制操作都依赖它。
- `git lfs version`：Git LFS（Large File Storage）是 Git 的扩展，专门处理大文件（如 CAD 模型、视频等）。对于本项目（源码只有几百 KB）并不需要，但检查一下以备不时之需。
- `ssh -T git@github.com`：测试本机与 GitHub 的 SSH 加密通道是否畅通。`-T` 表示不分配终端（因为 GitHub 不允许远程登录 shell）。如果返回了你的用户名，说明密钥配对成功，后续推送无需输入密码。

---

### Step 2: 探索项目现状

```bash
# 查看项目总大小
du -sh D:\exper\capillary-exercise
# → 72 MB

# 查看 Git 仓库状态
cd D:\exper\capillary-exercise
git status
# → On branch main, 2 modified files, 3 untracked items

# 查看提交历史
git log --oneline
# → 50+ 次提交

# 查看当前远程地址
git remote -v
# → origin  https://github.com/Luoyuetong/capillary-exercise.git
```

**这些命令在干什么？**

- `du -sh`：统计磁盘占用。结果显示 72 MB，但深入分析后发现其中 69 MB 是 `bin/obj/` 目录下的编译产物（`.dll`、`.exe` 等），这些已被 `.gitignore` 排除，不会上传。实际追踪的源码仅约 658 KB。
- `git status`：查看哪些文件被修改了但还没提交、哪些文件从未被 Git 追踪。这是推送前的必要检查——你不希望把草稿文件或临时文件推到远程仓库。
- `git log --oneline`：列出所有历史提交。`--oneline` 让每个提交只显示一行摘要，方便快速浏览。
- `git remote -v`：查看当前关联的远程仓库 URL。`-v`（verbose）会同时显示 fetch 和 push 两个地址。这里发现项目还在指向旧的 `Luoyuetong` 仓库。

---

### Step 3: 安装 GitHub CLI（备选方案）

```bash
# 通过 Windows 包管理器安装
winget install GitHub.cli

# 验证安装
gh --version
# → gh 2.96.0
```

**这步在干什么？**

- `gh`（GitHub CLI）是 GitHub 官方命令行工具。它可以让你在终端中直接创建仓库、管理 Issue、查看 PR，而不需要打开浏览器。理想情况下，用 `gh repo create` 一条命令就能创建仓库。但在本次操作中，由于终端环境 HTTPS 到 GitHub API 的连接受限，实际改为手动在网页端创建仓库，`gh` 作为备用工具保留。
- `winget` 是 Windows 11 自带的包管理器（类似 Linux 的 `apt`），可以一键安装/卸载软件。

---

### Step 4: 创建 GitHub 远程仓库

在浏览器中打开 https://github.com/new：

- **Repository name**: `capillary-exercise`
- **可见性**: Public（公开）
- **不要勾选** "Add a README file" / "Add .gitignore" / "Choose a license"

点击 "Create repository"。

**为什么这样做？**

- 仓库名与项目文件夹名一致，方便识别。
- 选择 Public 是因为这是一个学习/教学项目，公开可以让其他人参考。
- **关键**: 不要初始化任何文件（README、.gitignore、LICENSE）。如果勾选了，GitHub 会生成一个初始提交，当我们尝试推送本地项目时就会产生冲突（两个无关的 Git 历史无法自动合并）。我们需要的是一个**完全空的仓库**，让本地项目的 Git 历史直接推上去。

---

### Step 5: 更换 remote 地址

```bash
# 将远程地址从旧仓库改为新仓库
git remote set-url origin git@github.com:CYP-farther/capillary-exercise.git

# 确认修改成功
git remote -v
# → origin  git@github.com:CYP-farther/capillary-exercise.git (fetch)
# → origin  git@github.com:CYP-farther/capillary-exercise.git (push)
```

**这步在干什么？**

- `git remote set-url origin <新地址>`：修改名为 `origin` 的远程仓库 URL。`origin` 是 Git 对"默认远程仓库"的约定命名（就像"主线"叫 `main` 一样）。
- **为什么用 SSH 地址（`git@github.com:...`）而不是 HTTPS？** 因为本机已经配置了 SSH 密钥，用 SSH 推送不需要每次输入用户名密码，而且在本环境中 SSH 连接比 HTTPS 更稳定。HTTPS 地址格式是 `https://github.com/用户/仓库.git`，SSH 格式是 `git@github.com:用户/仓库.git`。

---

### Step 6: 处理未追踪文件

```bash
# 查看未追踪文件
git status
# Untracked files:
#   doc/teaching/a.txt
#   doc/teaching/新建文本文档.txt
#   newA/

# 只暂存已修改的追踪文件（排除未追踪文件）
git add -u

# 查看即将提交的内容
git status
# Changes to be committed:
#   modified: doc/teaching/000-为什么不能上来就写代码.md
#   modified: doc/teaching/001-Issue与模块-为什么提前规划Issue.md
```

**这步在干什么？**

- `git add -u`：`-u`（update）只会暂存那些**已被 Git 追踪但内容发生了变化**的文件。未追踪的新文件（如草稿 `.txt`、`newA/` 目录）不会被加入。这是一个"安全"的添加方式——只更新现有文件，不引入意外的新文件。
- 对比 `git add .`（添加当前目录下所有文件）和 `git add -A`（添加整个仓库所有变化），`git add -u` 更精确地控制了你想要提交的范围。
- 决定排除 `newA/` 和草稿 `.txt` 文件，因为它们不是项目的核心内容。

---

### Step 7: 提交并推送

```bash
# 创建提交
git commit -m "Update teaching documents"

# 推送到 GitHub（首次推送需要 -u 建立追踪关系）
git push -u origin main
```

**这步在干什么？**

- `git commit -m "..."`：将暂存区的内容保存为一个版本快照。`-m` 后面跟的是提交信息，描述这次改动做了什么。Git 会为这次提交生成一个唯一的 SHA-1 哈希值（如 `c273ac4`）作为"身份证号"。
- `git push -u origin main`：
  - `push`：将本地提交"推"到远程仓库。
  - `-u`（`--set-upstream`）：建立本地 `main` 分支与远程 `origin/main` 的**追踪关系**。之后只需敲 `git push`（不带参数）就会自动推送到对应的远程分支。
  - `origin`：远程仓库名。
  - `main`：要推送的分支名。

**推送成功后**，本地的 50+ 个提交全部上传到了 GitHub。你可以在 https://github.com/CYP-farther/capillary-exercise 看到完整的文件树和提交历史。

---

### Step 8: 验证结果

```bash
# 检查远程仓库是否有 main 分支
git ls-remote --heads origin
# → c273ac4...  refs/heads/main

# 查看最近 5 条提交
git log --oneline -5
# → c273ac4 Update teaching documents
# → 91f845a Add Pull Request teaching material
# → ba901f0 Update demo guide
# → ...
```

**这步在干什么？**

- `git ls-remote --heads origin`：直接查询远程仓库，列出所有分支及其最新提交的哈希值。如果返回的哈希值与本地一致（`c273ac4`），说明推送完全成功。
- `git log --oneline -5`：查看本地最近 5 次提交。与远程对比可以确认没有遗漏。

---

## 补充知识

### Git 的"三个区域"

理解 Git 的核心是理解它的三个区域：

```
工作目录（Working Directory）    ← 你正在编辑的文件
    │  git add
    ▼
暂存区（Staging Area / Index）   ← 准备提交的文件快照
    │  git commit
    ▼
本地仓库（Local Repository）     ← 永久保存的提交历史
    │  git push
    ▼
远程仓库（Remote Repository）    ← GitHub 上的副本
```

- **工作目录**：你在编辑器中看到的文件。
- **暂存区**：`git add` 后，文件被放入"购物车"，等待结账（commit）。
- **本地仓库**：`git commit` 后，购物车的内容被打包成一个不可变的快照。
- **远程仓库**：`git push` 后，本地快照同步到 GitHub。

### 什么时候需要 Git LFS？

- 单个文件 **> 100 MB**：GitHub 会直接拒绝
- 仓库总大小 **> 1 GB**：GitHub 会发警告，> 5 GB 会被拒绝
- 频繁修改的二进制文件（如图片、模型、数据库）：LFS 每次只传输差异部分，效率更高

本项目不需要——源码总计才 658 KB。

### SSH vs HTTPS 的区别

| | SSH | HTTPS |
|------|-----|-------|
| 地址格式 | `git@github.com:用户/仓库.git` | `https://github.com/用户/仓库.git` |
| 认证方式 | 密钥对（公钥+私钥） | 用户名+密码 或 Personal Access Token |
| 安全性 | ★★★★★（非对称加密） | ★★★★☆ |
| 便利性 | 一次配置，永久免密 | 每次或定期需要输入凭据 |

---

## 文件清单

以下是与本次操作直接相关的文件：

| 文件 | 说明 |
|------|------|
| `GITHUB_UPLOAD_GUIDE.md` | 本文档 |
| `.gitignore` | 已存在，配置了 .NET 项目的排除规则 |
| `.git/config` | Git 仓库配置（remote 地址等） |
