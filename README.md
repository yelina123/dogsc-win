# DogSC for Windows

macOS 原生录屏与轻量编辑工具 [DogSC](https://github.com/laogou717/dogsc) 的 Windows 移植版。

录制、剪辑、运镜，到导出成片。

## 功能特性

### 录制
- 全屏、窗口、区域录制
- 同步录制麦克风、系统声音
- 可选摄像头画中画
- 录制光标及点击效果
- 悬浮录制条，实时显示时长
- 快捷键标记（Ctrl+Alt+M），录制后定位剪辑点
- 悬浮备忘录，录制时记笔记

### 编辑与运镜
- 多轨时间线剪辑：分割、裁短、重排、变速
- 缩略图/波形显示，边界自动吸附
- 开场动效、自动跟随、手动缩放
- 屏幕 3D：倾斜、透视、聚焦虚化
- 效果复制粘贴（Ctrl+C/V 复制完整效果，Ctrl+Shift+V 粘贴属性）
- 光标样式、点击反馈、静止自动隐藏
- 摄像头布局与动画
- 画面比例、裁切、圆角、阴影、样机外框
- 贴图、画面柔化、场景预设

### 导出
- MP4 / MKV 视频导出
- M4A 音频导出
- 支持原始分辨率、1080p、1440p、4K
- 30 / 60 FPS
- 实时进度显示，可随时取消

## 技术栈

- **.NET 8.0** + WPF
- **C#**
- **FFmpeg**（视频编解码）
- Windows Graphics Capture（屏幕录制）
- WASAPI（音频录制）

## 构建

### 前提条件
- .NET 8.0 SDK 或更新版本
- FFmpeg（已加入 PATH，或放在项目目录）
- Visual Studio 2022 / Rider / VS Code

### 编译
```bash
git clone https://github.com/laogou717/dogsc-win.git
cd dogsc-win
dotnet build
```

### 运行
```bash
dotnet run --project src/DogSC.App
```

## 项目结构

```
DogSC-Win/
├── DogSC.sln
├── src/
│   ├── DogSC.Core/          # 核心数据模型、时间线、项目逻辑
│   │   ├── Models/          # 基础数据类型
│   │   ├── Timeline/        # 时间线模型与编辑操作
│   │   ├── Project/         # 项目模型与验证
│   │   ├── Effects/        # 运镜、缩放、聚焦等效果
│   │   ├── Export/          # 导出设置
│   │   ├── Diagnostics/     # 性能与录制诊断
│   │   └── Utils/           # 工具类
│   ├── DogSC.Capture/       # 录制模块
│   │   ├── Screen/         # 屏幕录制
│   │   ├── Audio/           # 音频录制
│   │   ├── Camera/          # 摄像头录制
│   │   └── Utils/           # 区域选择、光标追踪
│   ├── DogSC.Editor/        # 编辑器 UI 控件
│   │   ├── Timeline/        # 时间线控件
│   │   ├── Preview/         # 预览画布
│   │   └── Inspector/       # 检查器面板
│   ├── DogSC.Export/        # 导出模块
│   └── DogSC.App/           # 主应用
│       ├── Windows/         # 主窗口、设置、导出对话框
│       └── Controls/        # 悬浮录制条等控件
└── docs/                    # 文档
```

## 快捷键

| 快捷键 | 功能 |
|--------|------|
| Ctrl+R | 开始/停止录制 |
| Ctrl+Alt+M | 添加标记 |
| Ctrl+E | 导出 |
| S | 分割片段 |
| Delete | 删除选中片段 |
| Ctrl+C | 复制效果 |
| Ctrl+V | 粘贴效果 |
| Ctrl+Shift+V | 粘贴属性 |
| 空格 | 播放/暂停 |

## 路线图

- [x] 项目骨架与核心数据模型
- [x] 屏幕录制（全屏/区域）
- [x] 音频录制（麦克风/系统声音）
- [x] 摄像头录制
- [x] 时间线编辑器
- [x] 预览画布
- [x] 检查器面板
- [x] FFmpeg 导出
- [ ] Windows Graphics Capture 原生集成
- [ ] WASAPI 低延迟音频
- [ ] Metal → Direct3D 渲染管线移植
- [ ] 屏幕 3D 效果
- [ ] 自动跟随运镜
- [ ] 聚焦虚化
- [ ] 项目文件格式兼容（.dogscproject）
- [ ] 自动更新
- [ ] DMG/MSI 安装包

## 与 macOS 版的差异

| 功能 | macOS 版 | Windows 版 |
|------|----------|------------|
| 录制框架 | ScreenCaptureKit | Windows Graphics Capture |
| 音频 | CoreAudio | WASAPI |
| 渲染 | Metal + Core Image | Direct3D / WPF |
| UI | SwiftUI + AppKit | WPF |
| 更新 | Sparkle | 待实现 |
| 系统要求 | Apple Silicon, macOS 14+ | Windows 10 1809+ |

## 许可

MIT License - 原项目 © laogou717，Windows 移植版贡献者。

## 致谢

- 原项目 [laogou717/dogsc](https://github.com/laogou717/dogsc)
- FFmpeg 社区
- .NET / WPF 团队
