# TinyPly.Net - PLY File Processing Library

TinyPly.Net is a .NET library for reading and writing PLY (polygon) files with support for ASCII and binary formats. The solution contains a .NET Standard 2.0 library, .NET Framework 4.8.1 console example, and .NET 8.0 unit tests.

**ALWAYS reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.**

## Working Effectively

### Prerequisites and Setup
- .NET 8.0 SDK is required and available
- Mono runtime is available for running .NET Framework applications
- Solution is located at `tinyply.net/tinyplynet.sln`

### Build and Test the Repository
- Navigate to solution directory: `cd tinyply.net`
- Clean build artifacts: `dotnet clean` -- takes 1 second
- Restore packages: `dotnet restore` -- takes 1 second on clean build
- **NEVER CANCEL**: Build solution: `dotnet build` -- takes 42 seconds. Set timeout to 60+ minutes.
- **NEVER CANCEL**: Run all unit tests: `dotnet test` -- takes 6 seconds. Set timeout to 30+ minutes.

### Running the Example Application
The console example requires special handling due to .NET Framework target:
- Build first: `dotnet build`
- The built executable requires Mono: `mono tinyply.net/TinyPlyNet.Example/bin/Debug/net481/TinyPlyNet.Example.exe <file.ply>`
- Alternative: Create a .NET Core test project (see Validation section)
- Sample PLY file available at: `tinyply.net/TinyPlyNet.Example/apple.ply`

## Validation

### Manual Testing Requirements
**ALWAYS run complete end-to-end validation after making changes:**

1. **Build Validation**: Ensure clean build succeeds without errors
   ```bash
   cd tinyply.net
   dotnet clean && dotnet build
   ```

2. **Unit Test Validation**: All tests must pass
   ```bash
   dotnet test
   ```

3. **Functional Validation**: Test PLY file processing with working example
   ```bash
   # Create temporary test project
   mkdir -p /tmp/tinyply-test
   cd /tmp/tinyply-test
   
   # Create test project file
   cat > TestExample.csproj << EOF
   <Project Sdk="Microsoft.NET.Sdk">
     <PropertyGroup>
       <OutputType>Exe</OutputType>
       <TargetFramework>net8.0</TargetFramework>
     </PropertyGroup>
     <ItemGroup>
       <ProjectReference Include="/home/runner/work/tinyply.net/tinyply.net/tinyply.net/TinyPlyNet/TinyPlyNet.csproj" />
     </ItemGroup>
   </Project>
   EOF
   
   # Create test program
   cat > Program.cs << EOF
   using System;
   using System.Collections.Generic;
   using System.IO;
   using TinyPlyNet;
   
   class Program
   {
       static void Main(string[] args)
       {
           if (args.Length == 0) { Console.WriteLine("Usage: dotnet run <file.ply>"); return; }
           
           using (var stream = new FileStream(args[0], FileMode.Open, FileAccess.Read))
           {
               var f = new PlyFile(stream);
               var xyz = new List<float>();
               f.RequestPropertyFromElement("vertex", new[] { "x", "y", "z" }, xyz);
               var index = new List<List<int>>();
               f.RequestListPropertyFromElement("face", "vertex_indices", index);
               f.Read(stream);
               
               Console.WriteLine(\$"Read {xyz.Count / 3} vertices and {index.Count} faces");
               
               using (var writeStream = new FileStream("test-output.ply", FileMode.Create, FileAccess.Write))
               {
                   var writeFile = new PlyFile();
                   writeFile.AddPropertiesToElement("vertex", new[] { "x", "y", "z" }, xyz);
                   writeFile.AddListPropertyToElement("face", "vertex_indices", index);
                   writeFile.Write(writeStream);
                   Console.WriteLine("Successfully wrote ASCII PLY file");
               }
               
               using (var binaryStream = new FileStream("test-binary.ply", FileMode.Create, FileAccess.Write))
               {
                   var binaryFile = new PlyFile();
                   binaryFile.AddPropertiesToElement("vertex", new[] { "x", "y", "z" }, xyz);
                   binaryFile.AddListPropertyToElement("face", "vertex_indices", index);
                   binaryFile.Write(binaryStream, true);
                   Console.WriteLine("Successfully wrote binary PLY file");
               }
           }
       }
   }
   EOF
   
   # Copy sample file and test
   cp ../../../tinyply.net/TinyPlyNet.Example/apple.ply .
   dotnet run apple.ply
   ```

   **Expected output**: Should display "Read 867 vertices and 1704 faces" and create output files successfully.

### Key Projects in Codebase

1. **TinyPlyNet** (`tinyply.net/TinyPlyNet/`)
   - Core library targeting .NET Standard 2.0
   - Main classes: `PlyFile`, `PlyElement`, `PlyProperty` 
   - Entry point: `PlyFile.cs`

2. **TinyPlyNet.Example** (`tinyply.net/TinyPlyNet.Example/`)
   - Console application targeting .NET Framework 4.8.1
   - Demonstrates reading apple.ply and writing ASCII/binary output
   - Requires Mono runtime for execution

3. **TinyPlyNet.Tests** (`tinyply.net/TinyPlyNet.Tests/`)
   - xUnit test project targeting .NET 8.0
   - Tests PLY reading, writing, and format validation
   - Always run before committing changes

### Important Code Locations

- **Core PLY processing**: `tinyply.net/TinyPlyNet/PlyFile.cs`
- **Helper utilities**: `tinyply.net/TinyPlyNet/Helpers/`
- **Sample PLY file**: `tinyply.net/TinyPlyNet.Example/apple.ply`
- **Unit tests**: `tinyply.net/TinyPlyNet.Tests/UnitTest1.cs`

### Common Development Patterns

- **Reading PLY files**: Create `PlyFile` from stream, request properties/lists, call `Read()`
- **Writing PLY files**: Create empty `PlyFile`, add properties/lists, call `Write()` with optional binary flag
- **Property handling**: Use `RequestPropertyFromElement()` for vertex data, `RequestListPropertyFromElement()` for face indices
- **Stream management**: Always use `using` statements for proper disposal

### Troubleshooting

- **Build warnings**: The codebase has nullable reference warnings in PlyFile.cs - these are existing and do not prevent successful builds
- **Mono dependency**: .NET Framework example requires Mono runtime for execution - install with `sudo apt install mono-runtime`
- **Test failures**: If tests fail, check that apple.ply sample file exists and is accessible
- **Performance**: Build and test times are fast (<1 minute each), but always use generous timeout buffers for CI reliability

## Common Tasks

### Repository Structure
```
.
├── README.md              # Project documentation
├── LICENSE               # Public domain license
├── tinyply.net/          # Main solution directory
│   ├── tinyplynet.sln    # Visual Studio solution
│   ├── TinyPlyNet/       # Core library (.NET Standard 2.0)
│   ├── TinyPlyNet.Example/  # Console app (.NET Framework 4.8.1)
│   └── TinyPlyNet.Tests/    # Unit tests (.NET 8.0)
```

### Build Output Verification
After successful build, verify these artifacts exist:
- `tinyply.net/TinyPlyNet/bin/Debug/netstandard2.0/TinyPlyNet.dll`
- `tinyply.net/TinyPlyNet.Example/bin/Debug/net481/TinyPlyNet.Example.exe`
- `tinyply.net/TinyPlyNet.Tests/bin/Debug/net8.0/TinyPlyNet.Tests.dll`

### Sample PLY File Content
The apple.ply sample contains:
- 867 vertices with x,y,z coordinates
- 1704 triangular faces with vertex indices
- ASCII format with standard PLY header structure