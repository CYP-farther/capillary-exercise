# 将 capillary-exercise 项目上传到 GitHub —— 完整操作记录


## 详细步骤

### Step 1: 环境检查

```bash
# 检查 Git 版本
git --version
# → git version 2.54.0.windows.1

# 测试 SSH 连接
ssh -T git@github.com
# → Hi CYP-farther! You've successfully authenticated...
```

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
---


### Step 4: 创建 GitHub 远程仓库



### Step 5: 更换 remote 地址

```bash
# 将远程地址从旧仓库改为新仓库
git remote set-url origin git@github.com:CYP-farther/capillary-exercise.git

# 确认修改成功
git remote -v
# → origin  git@github.com:CYP-farther/capillary-exercise.git (fetch)
# → origin  git@github.com:CYP-farther/capillary-exercise.git (push)
```
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

### Step 7: 提交并推送

```bash
# 创建提交
git commit -m "Update teaching documents"

# 推送到 GitHub（首次推送需要 -u 建立追踪关系）
git push -u origin main
```

**这步在干什么？**
---

### Step 8: 验证结果

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
