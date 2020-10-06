# dotnet tool for glTF

## How to use

load glTF file

```csharp
using NetGltf;
var gltfFile = "sepbins/CesiumMan/CesiumMan.gltf";
var doc = Document.FromPath(gltfFile);
var res = doc.Parse();
var model = res.Data;
```

load glb file and convert to glTF file

```csharp
using NetGltf;
var glbFile = "CesiumMan.glb";
using(var fs = File.OpenRead(glbFile))
{
    var glbRes = GlbFile.Parse(fs);
    var model = glbRes.Data.ToGltf();
    var doc = Document.Create();
    var path = Path.GetFullPath("testglb.gltf");
    doc.WriteModel(model, path);
}
```