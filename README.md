# Vulkan.NET Bindings - Getting Started

---
## Overview
This project provides bindings for the **Vulkan API** in **.NET**, allowing developers to leverage modern graphics and compute capabilities within .NET applications. This wrapper simplifies interaction with Vulkan while maintaining functionality.

--- 

## Prerequisites
- **[Vulkan SDK](https://vulkan.lunarg.com/sdk/home)** installed on your system (ensure it includes headers and libraries).
  Download and extract to a directory like `C:/VulkanSDK`.
- **.NET Core 3.1+ / .NET 5+** or **.NET Framework 4.8+**.

--- 

## Installation & Build
### Quick Start
Run the following commands:
```bash
cd BindGen && dotnet publish -c Release -o ../
cd .. && ./BindGen.exe
cp -rf Vk.cs Vulkan/Vk.cs
cd Vulkan && dotnet publish -c Release -o ../Release
cd ..
echo "Build completed successfully!"
```

### Manual Setup Steps
1. **Clone the Repository**:
   ```bash
git clone https://github.com/your-repo/Vulkan.NET-Bindings.git
   cd Vulkan.NET-Bindings
   ```
2. **Install Dependencies**:
   - Install **.NET SDK** (3.1+ or 5+).
   - Ensure the **Vulkan SDK** is installed and accessible.

--- 

## Setup Instructions
### Environment Variables
Set these environment variables:
```bash
# Linux/macOS
export VK_INSTANCE_EXTENSIONS="VK_KHR_SURFACE,VK_KHR_WIN32_SURFACE"

# Windows (PowerShell)
$env:VK_INSTANCE_EXTENSIONS = "VK_KHR_SURFACE,VK_KHR_WIN32_SURFACE"
```

### Linking Vulkan Libraries
Ensure `vulkan-1.dll` is in your system `PATH`. If not, specify its location.

--- 

## Usage Examples
### Initializing a Vulkan Instance
```csharp /Vulkan/Vk.cs
using System;
using Vulkan;

class Program
{
    static void Main()
    {
        var appInfo = new VkApplicationInfo("MyApp", "1.0", null);
        var instance = new VkInstance(appInfo, new[] { VK_KHR_SURFACE });
        Console.WriteLine("Vulkan instance created successfully!");
    }
}
```

### Creating a Swap Chain
```csharp /Vulkan/Vk.cs
var surfaceCreateInfo = new VkSurfaceCreateInfoKHR(VK_SURFACE_CREATE_FLAGS, null, null);
var swapChain = instance.CreateSwapChain(surfaceCreateInfo);
Console.WriteLine("Swap chain created successfully!");
```

--- 

## Dependencies
### Required Libraries
- **SharpVk**: A .NET wrapper for Vulkan headers.
  Install via NuGet:
  ```bash
dotnet add package SharpVk
```

### Tools
- **BindGen**: Generates `.cs` bindings. Ensure it is in your `PATH`.

--- 

## Troubleshooting
### Common Issues
1. **Missing DLLs**: Ensure `vulkan-1.dll` is accessible.
   - Place it in a directory included in your system `PATH`.
2. **Initialization Errors**: Verify:
   - Vulkan SDK installation and headers are correct.
   - Environment variables for extensions are set properly.

### Debugging Steps
Run `vkcube --version` to verify Vulkan:
```bash
vkcube --version
```
If this fails, check your Vulkan SDK installation.


