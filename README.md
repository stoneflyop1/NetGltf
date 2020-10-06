# NetGltf

## glTF tool

loading and validating glTF json file.

- load gltf file
- validate gltf's index and enum values
- gltf2glb

## Obj2Gltf

convert wavefront obj file to glTF file.

- supports polygon that has more than 3 vertices through [earcut](https://github.com/mapbox/earcut)
- supports xyzcolor
- only supports png and jpeg texture files


## Reference

- [glTF – Runtime 3D Asset Delivery](https://github.com/KhronosGroup/glTF/)
- [A crate for loading glTF 2.0](https://github.com/gltf-rs/gltf/)
- [Wavefront obj parser for Rust](https://github.com/simnalamburt/obj-rs)
- [obj2gltf](https://github.com/CesiumGS/obj2gltf)
- [The fastest and smallest JavaScript polygon triangulation library for your WebGL apps](https://github.com/mapbox/earcut)