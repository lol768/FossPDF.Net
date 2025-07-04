Usage:

```c#
// face and buffer are HarfBuzz instances
var builder = new FontSubsetBuilder();
builder.SetFace(face);
builder.AddGlyphs(buffer.GlyphInfos); // glyphs to subset down to

await File.WriteAllBytesAsync("/tmp/out.ttf", builder.Build());
```
