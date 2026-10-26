Decompiled source (ILSpy) of `Toolbox/Lib/Syroot.NintenTools.NSW.Bfres.dll`, so the library can be patched in this fork.

Changes compared to the upstream DLL:
- `VertexBuffer.SetVertexBufferArrayOffset` pads vertex buffers with the GPU alignment (0x100 in v10 / Switch 2 files) like `ResFileSaver.WriteVertexBuffer` does. Before, saving a v10 BFRES wrote wrong buffer offsets and corrupted the vertex data of every vertex buffer after the first.
- `AnimCurve` reads the cubic curves of version 10 files (Switch 2): a header with the slope scale and offset, then per key an unsigned value (`Delta + raw * Offset`) and slope (`raw * scale + offset` per frame). They are exposed as `IsHermite` with `Keys[i,0]` = value and `Keys[i,1]` = slope and are written back unchanged. Before, the keys were read as 4 coefficients which gave garbage and stretched bones.

Build: `dotnet build -c Release`, copy the DLL to `Toolbox/Lib` and `Toolbox/Lib/Plugins`.
