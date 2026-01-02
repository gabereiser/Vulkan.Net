using System.Runtime.InteropServices;
using CppAst;
using CppAst.CodeGen.Common;
using CppAst.CodeGen.CSharp;
using Zio.FileSystems;

namespace BindGen;

class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Generating bindings...");

        try
        {
            var headerDirectories = new[]
            {
                "include",
                "include/vulkan",
                "dependencies/vulkan-headers/include/vulkan"
            };

            AcceptHeaderDirectory(headerDirectories, "Vk", "Vulkan");
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to generate bindings:");
            Console.WriteLine(e);
        }

        Console.WriteLine("Done.");
    }

    public static void AcceptHeaderDirectory(IEnumerable<string> headerDirectories, string outputClass, string outputNamespace)
    {
        // Validate directories
        var validDirectories = new List<string>();
        foreach (var dir in headerDirectories)
        {
            if (Directory.Exists(dir))
                validDirectories.Add(Path.GetFullPath(dir));
        }

        if (validDirectories.Count < 1)
            throw new DirectoryNotFoundException("No valid Vulkan headers found.");

        var options = new CSharpConverterOptions()
        {
            DefaultNamespace = outputNamespace,
            DefaultClassLib = outputClass,
            DefaultOutputFilePath = $"{outputClass}.cs",
            GenerateEnumItemAsFields = false,
            AllowFixedSizeBuffers = true,
            TypedefCodeGenKind = CppTypedefCodeGenKind.Wrap,
            DefaultDllImportNameAndArguments = "\"vulkan\"",
            DefaultMarshalForBool = new CSharpMarshalAsAttribute(UnmanagedType.U1),
            DefaultMarshalForString = new CSharpMarshalAsAttribute(UnmanagedType.LPUTF8Str),
            UseLibraryImport = true,
            ParseSystemIncludes = false,
            DefaultCharSet = CharSet.Unicode,
            DetectOpaquePointers = true,
            MappingRules =
            {
                r => r.MapAll<CppEnumItem>().CSharpAction((converter, element) =>
                {
                    if (element is not CSharpEnumItem item)
                            return;

                    //remove unchecked(int) casts
                    CSharpEnum? parent = item.Parent as CSharpEnum;
                    if (item.Value.StartsWith("unchecked((int)"))
                                item.Value = item.Value[15..^1];
                    // remove redundancy in enum members names
                    string enumName = parent?.Name;
                    if (item.Name.StartsWith(enumName))
                        item.Name = item.Name.Substring(enumName.Length);


                }),
                r => r.MapAll<CppElement>().CSharpAction((converter, element) =>
                {
                    if (element is not CSharpStruct item)
                        return;
                    //remove underscore name prefixes.
                    if(item.Name.StartsWith("_"))
                        item.Name = item.Name[1..];

                    Console.Write(".");
                }),
            }
        };

        // --- Process header files ---
        var headerFiles = validDirectories.SelectMany(dir =>
            Directory.EnumerateFiles(dir, "*.h", SearchOption.AllDirectories))
            .ToList();
        foreach (var h in headerFiles)
        {
            Console.WriteLine("Found {0}", h);
        }
        if (OperatingSystem.IsWindows())
        {
            options.ConfigureForWindowsMsvc(CppTargetCpu.X86_64);

            options.IncludeFolders.AddRange([
                "./dependencies/vulkan-headers/include",
                @"C:\Program Files (x86)\Windows Kits\10\Include\10.0.22621.0\um",
            @"C:\Program Files (x86)\Windows Kits\10\Include\10.0.22621.0\shared",
            @"C:\Program Files (x86)\Windows Kits\10\Include\10.0.22621.0\ucrt",
            @"C:\Program Files\Microsoft Visual Studio\2022\BuildTools\VC\Tools\MSVC\14.38.33130\include"
            ]);


            options.Defines.AddRange([
                "WIN32",
                "_WIN32",
                "_MSVC_",
                "WIN32_LEAN_AND_MEAN",
                "VULKAN_FUCHSIA_H_",
                "VULKAN_DIRECTFB_H_",
                "VULKAN_GGP_H_",
                "VULKAN_XCB_H_",
                "VULKAN_XLIB_H_",
                "VULKAN_XLIB_XRANDR_H_",
                "VK_USE_PLATFORM_WIN32_KHR"
            ]);
        }
        options.IncludeFolders.AddRange(headerDirectories);

        CSharpCompilation compilation = CSharpConverter.Convert(headerFiles.ToList(), options)!;
        Console.WriteLine("\r\n---------------------");
        if (compilation.HasErrors)
        {
            Console.WriteLine("Failed to generate bindings due to:");
            foreach (var message in compilation.Diagnostics.Messages)
                Console.WriteLine(message);
            return;
        }

        // --- Generate output ---
        using var fileSystem = new PhysicalFileSystem();
        using var subFileSystem = new SubFileSystem(fileSystem, fileSystem.ConvertPathFromInternal("."));
        var writer = new CodeWriter(new CodeWriterOptions(subFileSystem));

        compilation.DumpTo(writer);
    }
}