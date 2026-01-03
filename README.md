# Vulkan.NET Bindings - Getting Started

---
## Overview
This project provides bindings for the **Vulkan API** in **.NET**, allowing developers to leverage modern graphics and compute capabilities within .NET applications. This wrapper simplifies interaction with Vulkan while maintaining functionality.

--- 

## Prerequisites
- **[Vulkan SDK](https://vulkan.lunarg.com/sdk/home)** installed on your system (ensure it includes headers and libraries).
  Download and extract to a directory like `C:/VulkanSDK`.
- **.NET 9+**.

--- 

## Installation & Build
### Quick Start
Run the following commands:
```bash
./build.sh
```
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

## Dependencies
- Vulkan being present on the system and that's it.

### Tools
- **BindGen**: Generates `.cs` bindings. Ensure it is in your `PATH`.

--- 

