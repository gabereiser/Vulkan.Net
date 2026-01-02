#!/bin/sh

cd BindGen
dotnet publish -c Release -o ../
cd ..
./BindGen.exe
cp -rf Vk.cs Vulkan/Vk.cs
cd Vulkan
dotnet publish -c Release -o ../Release
cd ..
echo 'Done'
