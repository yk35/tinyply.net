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

    [Fact]
    public void WriteApplePly_HeaderIsBinary()
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
        writeFile.Write(ms, true); // isBinary = true
        ms.Position = 0;
        using var reader = new StreamReader(ms);
        string headerLine1 = reader.ReadLine();
        string headerLine2 = reader.ReadLine();
        Assert.Equal("ply", headerLine1);
        Assert.Equal("format binary_little_endian 1.0", headerLine2);
    }

    [Fact]
    public void AddPropertiesToElement_MultipleCallsSameElement_ShouldSucceed()
    {
        // Test reproducing the issue from the GitHub issue
        var xyz = new List<float> { 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f };  // 2 vertices
        var normals = new List<float> { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f };  // 2 vertices  
        var rgb = new List<byte> { 255, 0, 0, 0, 255, 0 };  // 2 vertices

        var writeFile = new PlyFile();
        
        // This should work - first call creates the vertex element
        writeFile.AddPropertiesToElement("vertex", new[] { "x", "y", "z" }, xyz);
        
        // This should also work - second call adds more properties to existing vertex element
        writeFile.AddPropertiesToElement("vertex", new[] { "nx", "ny", "nz" }, normals);
        
        // This should also work - third call adds color properties to existing vertex element
        writeFile.AddPropertiesToElement("vertex", new[] { "red", "green", "blue" }, rgb);

        // Verify we can write the file without issues
        using var ms = new MemoryStream();
        writeFile.Write(ms);
        
        // Verify the file structure - should have one vertex element with 9 properties
        Assert.Single(writeFile.Elements);
        Assert.Equal("vertex", writeFile.Elements[0].Name);
        Assert.Equal(9, writeFile.Elements[0].Properties.Count); // x,y,z,nx,ny,nz,red,green,blue
        Assert.Equal(2, writeFile.Elements[0].Size); // 2 vertices
    }
}
