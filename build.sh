#!/bin/bash
# DogSC for Windows 构建脚本

set -e

echo "=== DogSC for Windows Build ==="
echo ""

# 检查 .NET SDK
if ! command -v dotnet &> /dev/null; then
    echo "错误: 未找到 dotnet SDK，请先安装 .NET 8.0 SDK"
    exit 1
fi

echo "使用 dotnet: $(dotnet --version)"
echo ""

# 还原依赖
echo "还原 NuGet 包..."
dotnet restore
echo ""

# 构建
echo "构建 Release 版本..."
dotnet build -c Release
echo ""

# 发布
echo "发布自包含版本..."
dotnet publish src/DogSC.App -c Release -o ./dist/DogSC --self-contained true -r win-x64
echo ""

echo "=== 构建完成 ==="
echo "输出目录: ./dist/DogSC"
echo "可执行文件: ./dist/DogSC/DogSC.exe"
