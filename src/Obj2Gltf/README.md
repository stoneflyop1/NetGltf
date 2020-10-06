# obj2glTF

## How to use

```csharp
using Obj2Gltf;
var objFile = "plants/indoor plant_02.obj";
var gltfFile = "test.gltf";
var options = new ConverterOptions
    {
        SeparateBinary = true,
        SeparateTextures = true
    };
var converter = new ModelConverter(objFile, gltfFile, options);
var model = converter.Run();
```