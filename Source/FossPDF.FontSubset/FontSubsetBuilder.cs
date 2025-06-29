using HarfBuzzSharp;

namespace FossPDF.FontSubset;

public class FontSubsetBuilder
{
    private Face? _face;
    private readonly HashSet<uint> _glyphCodepoints = [];

    public FontSubsetBuilder SetFace(Face face)
    {
        _face = face;
        return this;
    }

    public FontSubsetBuilder AddGlyph(uint glyphId)
    {
        _glyphCodepoints.Add(glyphId);
        return this;
    }

    public FontSubsetBuilder AddGlyphs(IEnumerable<uint> glyphIds)
    {
        foreach (var cp in glyphIds)
            _glyphCodepoints.Add(cp);
        return this;
    }

    public FontSubsetBuilder AddGlyphs(IEnumerable<GlyphInfo> glyphInfos)
    {
        foreach (var info in glyphInfos)
            _glyphCodepoints.Add(info.Codepoint);
        return this;
    }

    public byte[] Build()
    {
        if (_face == null)
            throw new SubsetException("Face must be set before building subset.");

        var subsetInput = HarfBuzzSubsetNative.hb_subset_input_create_or_fail();
        if (subsetInput == IntPtr.Zero)
            throw new SubsetException("Failed to create subset input.");

        try
        {
            var glyphSet = HarfBuzzSubsetNative.hb_subset_input_glyph_set(subsetInput);
            if (glyphSet == IntPtr.Zero)
                throw new SubsetException("Failed to get glyph set from subset input.");

            foreach (var codepoint in _glyphCodepoints)
                HarfBuzzSubsetNative.hb_set_add(glyphSet, codepoint);

            var subsetFacePtr = HarfBuzzSubsetNative.hb_subset_or_fail(_face.Handle, subsetInput);
            if (subsetFacePtr == IntPtr.Zero)
                throw new SubsetException("Failed to subset the face.");

            var faceType = typeof(Face);
            var faceConstructor = faceType.GetConstructor(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null,
                [typeof(IntPtr)], null);
            if (faceConstructor == null)
                throw new SubsetException("Failed to get Face constructor.");
            using var subsetFace = (Face)faceConstructor.Invoke([subsetFacePtr]);

            var subsetBlobPtr = HarfBuzzSubsetNative.hb_face_reference_blob(subsetFacePtr);
            if (subsetBlobPtr == IntPtr.Zero)
                throw new SubsetException("Failed to reference blob from subset face.");

            var blobType = typeof(Blob);
            var blobConstructor = blobType.GetConstructor(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null,
                [typeof(IntPtr)], null);
            if (blobConstructor == null)
                throw new SubsetException("Failed to get Blob constructor.");
            using var subsetBlob = (Blob)blobConstructor.Invoke([subsetBlobPtr]);

            using var stream = subsetBlob.AsStream();
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }
        finally
        {
            HarfBuzzSubsetNative.hb_subset_input_destroy(subsetInput);
        }
    }
}
