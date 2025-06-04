using System.Collections.Generic;
using System.IO;
using TinyPlyNet;
using Xunit;

namespace TinyPlyNet.Tests;

public class PlyFileTests
{
    [Fact]
    public void LoadApplePly_VerifyCounts()
    {
        using var stream = File.OpenRead("apple.ply");
        var file = new PlyFile(stream);
        var vertices = new List<float>();
        var faces = new List<List<int>>();
        int vertexCount = file.RequestPropertyFromElement("vertex", new[] { "x", "y", "z" }, vertices);
        file.RequestListPropertyFromElement("face", "vertex_indices", faces);
        file.Read(stream);
        Assert.Equal(867, vertexCount);
        Assert.Equal(867, vertices.Count / 3);
        Assert.Equal(1704, faces.Count);
    }

    [Fact]
    public void WriteApplePly_HeaderIsAscii()
    {
        using var stream = File.OpenRead("apple.ply");
        var file = new PlyFile(stream);
        var vertices = new List<float>();
        var faces = new List<List<int>>();
        file.RequestPropertyFromElement("vertex", new[] { "x", "y", "z" }, vertices);
        file.RequestListPropertyFromElement("face", "vertex_indices", faces);
        file.Read(stream);

        var writeFile = new PlyFile();
        writeFile.AddPropertiesToElement("vertex", new[] { "x", "y", "z" }, vertices);
        writeFile.AddListPropertyToElement("face", "vertex_indices", faces);
        using var ms = new MemoryStream();
        writeFile.Write(ms);
        ms.Position = 0;
        using var reader = new StreamReader(ms);
        string headerLine1 = reader.ReadLine();
        string headerLine2 = reader.ReadLine();
        Assert.Equal("ply", headerLine1);
        Assert.Equal("format ascii 1.0", headerLine2);
    }
}
