@ECHO OFF

dotnet pack ../src/NetGltf/NetGltf.csproj -c Release -o ../bin/packages/ -p:Version=1.0.1
dotnet pack ../src/Obj2Gltf/Obj2Gltf.csproj -c Release -o ../bin/packages/ -p:Version=1.0.1