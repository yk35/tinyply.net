# tinyply.net

## Overview 🍰
TinyPly.Net is a .NET library for reading and writing [PLY](https://en.wikipedia.org/wiki/PLY_(file_format)) polygon files. It is a C# adaptation of the original tinyply project and provides utilities for loading PLY meshes, extracting elements and creating new files in either ASCII or binary form.

## Features ✨
- Parse existing PLY files from a stream
- Request typed properties or lists before reading
- Write files in ASCII or binary (little or big endian)
- Supports list properties for faces and other collections
- Example console program and unit tests

## Installation 📦
Clone the repository and build the solution using the .NET SDK:

```bash
git clone <repo-url>
cd tinyply.net
dotnet build tinyply.net/tinyplynet.sln
```

The core library targets **netstandard2.0** so it can be referenced from a variety of .NET projects.

## Quick Start 🚀
Reading a PLY file and writing it back in binary:

```csharp
using var stream = File.OpenRead("apple.ply");
var file = new PlyFile(stream);
var vertices = new List<float>();
file.RequestPropertyFromElement("vertex", new[] { "x", "y", "z" }, vertices);
var faces = new List<List<int>>();
file.RequestListPropertyFromElement("face", "vertex_indices", faces);
file.Read(stream);

var output = new PlyFile();
output.AddPropertiesToElement("vertex", new[] { "x", "y", "z" }, vertices);
output.AddListPropertyToElement("face", "vertex_indices", faces);
using var outStream = File.Create("copy.ply");
output.Write(outStream, true); // true = binary
```

## CLI Options 🛠️
The **TinyPlyNet.Example** project is a minimal console program. Run it with the path to a `.ply` file:

```bash
dotnet run --project tinyply.net/TinyPlyNet.Example <file.ply>
```
It will display vertex values and face counts and also write `writeTest.ply` and `writeTestBinary.ply` in the working directory.

## Contributing 🤝
Issues and pull requests are welcome. Please open an issue to discuss bugs or feature requests before submitting a PR.

## License 📄
This project is released into the public domain. Where that dedication is not recognized you may use it under the terms of the 2-clause BSD license.
