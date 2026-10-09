# 贡献指南

感谢你考虑为 DogSC for Windows 做贡献！

## 如何贡献

1. Fork 本仓库
2. 创建特性分支: `git checkout -b feature/amazing-feature`
3. 提交改动: `git commit -m 'Add amazing feature'`
4. 推送到分支: `git push origin feature/amazing-feature`
5. 提交 Pull Request

## 代码规范

- 遵循 C# 通用命名规范
- 使用 PascalCase 命名类、方法、公共属性
- 使用 camelCase 命名局部变量和私有字段
- 私有字段前缀使用 `_`
- 每行不超过 120 字符
- 添加必要的 XML 文档注释

## 提交信息

建议使用 Conventional Commits 格式：
- `feat: 新功能`
- `fix: 修复 bug`
- `docs: 文档更新`
- `refactor: 代码重构`
- `perf: 性能优化`
- `test: 测试相关`
- `chore: 构建/工具链`

## 报告问题

提交 Issue 时请包含：
- Windows 版本
- .NET 版本
- 复现步骤
- 预期行为
- 实际行为
- 截图/录屏（如适用）

## 开发路线

优先贡献以下方向：
1. Windows Graphics Capture 原生集成
2. WASAPI 音频录制
3. Direct3D 渲染管线
4. 屏幕 3D 效果
5. 项目文件格式实现
