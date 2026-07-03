# Git/GitHub 协作流程实战 —— 分支、PR、合并

> **示例项目**: capillary-exercise
> **远程仓库**: [github.com/CYP-farther/capillary-exercise](https://github.com/CYP-farther/capillary-exercise)
> **日期**: 2026-07-03

---

## 完整流程图

```
main ──●──●──●──●──●──●──●──●──●──●  (开发主线)
            ╲
             ╲ feature/add-version-history
              ●──●                          (功能开发)
            ╱
main ──●──●──●──●──●──●──●──●──●──●──●  (合并后主线，删除分支)
```

---

## 场景

在 README.md 中新增"版本历史"章节，通过 feature 分支 → PR → 合并的完整流程提交。

---

## 一、准备工作：确保 main 干净

```bash
# 1. 查看当前状态
$ git status

On branch main
Your branch is up to date with 'origin/main'.

Changes not staged for commit:
	modified:   GITHUB_UPLOAD_GUIDE.md
	modified:   doc/teaching/004-代码规范-制定遵守与让它生效.md
	modified:   doc/teaching/040-软件演示指南-现场跑通领料流程.md

# 2. 提交现有修改
$ git add -u
$ git commit -m "Update GITHUB_UPLOAD_GUIDE and teaching docs"
$ git push origin main
```

---

## 二、创建 feature 分支

```bash
# 3. 从 main 创建并切换到新分支
$ git checkout -b feature/add-version-history

Switched to a new branch 'feature/add-version-history'

# 4. 确认当前分支
$ git branch -a

* feature/add-version-history    ← 当前分支
  main
  remotes/origin/main
```

> 分支命名规范：`feature/<描述>` 表示功能分支，`fix/<描述>` 表示修复分支。

---

## 三、在新分支上修改代码

```bash
# 5. 编辑 README.md —— 在"开发日志"上方新增"版本历史"表格
```

改动内容（`README.md`）：

```markdown
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
```

---

## 四、提交并推送分支

```bash
# 6. 查看改动
$ git diff

# 7. 暂存并提交
$ git add README.md
$ git commit -m "Add version history section to README"

[feature/add-version-history 9339425] Add version history section to README
 1 file changed, 12 insertions(+)

# 8. 推送分支到 GitHub（首次需要 -u 建立追踪）
$ git push -u origin feature/add-version-history

remote: Create a pull request for 'feature/add-version-history' on GitHub by visiting:
remote:      https://github.com/CYP-farther/capillary-exercise/pull/new/feature/add-version-history

To github.com:CYP-farther/capillary-exercise.git
 * [new branch]      feature/add-version-history -> feature/add-version-history
```

---

## 五、创建 Pull Request

GitHub 推送成功后自动给出了 PR 创建链接。打开：

```
https://github.com/CYP-farther/capillary-exercise/pull/new/feature/add-version-history
```

填写 PR 信息：

| 字段 | 内容 |
|------|------|
| **Title** | 在 README 中添加版本历史 |
| **Description** | 改动说明、具体改动、验证方式（见下方模板） |

**PR 描述模板：**

```markdown
## 改动说明
在 README.md 中新增"版本历史"章节，以表格形式列出 v0.1 到 v0.7 共 7 个版本的里程碑。

## 具体改动
- 新增版本历史表格，每行包含版本号、日期、里程碑描述
- 涵盖从项目初始化到仓库迁移的完整开发历程

## 验证方式
- 查看 README.md 渲染效果，确认表格格式正确
```

点击 **Create pull request**。

---

## 六、合并 Pull Request

在 PR 页面：

1. 检查 **Files changed** 标签 —— 确认改动只有 README.md 一行变动
2. 检查 **Conversation** 标签 —— 确认没有冲突提示
3. 点击 **Merge pull request** → **Confirm merge**

合并方式选择：

| 方式 | 效果 | 适用场景 |
|------|------|----------|
| **Merge commit** | 保留分支所有提交，生成一个合并提交 | 多人协作，需要保留完整历史 |
| **Squash and merge** | 所有提交压缩为一个，再合并到 main | 单个 feature 的小修改（本文档场景） |
| **Rebase and merge** | 将分支提交逐个"嫁接"到 main 顶端 | 需要保持线性历史 |

---

## 七、同步本地并清理分支

```bash
# 9. 切回 main
$ git checkout main

# 10. 拉取 GitHub 上的合并结果
$ git pull origin main

# 或者：如果 GitHub 合并未及时生效，本地合并
$ git merge feature/add-version-history

Updating a95ae20..9339425
Fast-forward
 README.md | 12 ++++++++++++
 1 file changed, 12 insertions(+)

# 11. 推送到远程
$ git push origin main

# 12. 删除本地分支
$ git branch -d feature/add-version-history

Deleted branch feature/add-version-history (was 9339425).

# 13. 删除远程分支
$ git push origin --delete feature/add-version-history

To github.com:CYP-farther/capillary-exercise.git
 - [deleted]         feature/add-version-history
```

---

## 八、验证最终状态

```bash
$ git log --oneline -5

9339425 Add version history section to README
a95ae20 Update GITHUB_UPLOAD_GUIDE and teaching docs
ad3b62a Add GitHub upload operation guide
c273ac4 Update teaching documents
91f845a Add Pull Request teaching material (005) and renumber CI/Review

$ git branch -a

* main
  remotes/origin/HEAD -> origin/main
  remotes/origin/main

$ git status

On branch main
Your branch is up to date with 'origin/main'.
nothing to commit, working tree clean
```

---

## 命令速查表

| 操作 | 命令 |
|------|------|
| 创建分支 | `git checkout -b feature/<name>` |
| 查看所有分支 | `git branch -a` |
| 切换分支 | `git checkout <branch>` |
| 推送新分支 | `git push -u origin feature/<name>` |
| 在 GitHub 创建 PR | 浏览器打开推送后返回的链接 |
| 合并 PR | GitHub 页面点击 Merge |
| 本地合并分支 | `git merge feature/<name>` |
| 删除本地分支 | `git branch -d feature/<name>` |
| 删除远程分支 | `git push origin --delete feature/<name>` |
| 拉取最新 main | `git checkout main && git pull origin main` |
| 强制删除未合并分支 | `git branch -D feature/<name>` |

---

## 常见状态处理

### 有冲突时（Merge Conflict）

```bash
$ git merge feature/xxx
CONFLICT (content): Merge conflict in README.md
Automatic merge failed; fix conflicts and then commit the result.

# 手动编辑 README.md 解决冲突标记（<<<<<<< ======= >>>>>>>）
# 然后：
$ git add README.md
$ git commit -m "Resolve merge conflict in README"
$ git push origin main
```

### 修改已提交的内容

```bash
# 修改最后一次提交（还未 push 时）
$ git add <file>
$ git commit --amend -m "新的提交信息"

# 强制推送（修改了远程已有提交时，谨慎使用）
$ git push --force-with-lease origin feature/<name>
```

### 撤销未合并的修改

```bash
# 撤销工作区改动
$ git restore <file>

# 撤销暂存区的文件
$ git restore --staged <file>

# 回退到某个提交（保留修改在工作区）
$ git reset <commit-hash>
```
