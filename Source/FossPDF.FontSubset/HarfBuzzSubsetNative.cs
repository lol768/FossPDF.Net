namespace FossPDF.FontSubset;

using System;
using System.Runtime.InteropServices;

using hb_blob_t = System.IntPtr;
using hb_face_t = System.IntPtr;
using hb_set_t = System.IntPtr;
using hb_subset_input_t = System.IntPtr;


internal static partial class HarfBuzzSubsetNative
{
#if __IOS__ || __TVOS__
    private const string HARFBUZZ = "@rpath/libHarfBuzzSharp.framework/libHarfBuzzSharp";
#else
    private const string HARFBUZZ = "libHarfBuzzSharp";
#endif

    [LibraryImport(HARFBUZZ)]
    internal static partial hb_subset_input_t hb_subset_input_create_or_fail();

    [LibraryImport(HARFBUZZ)]
    internal static partial hb_set_t hb_subset_input_glyph_set(hb_subset_input_t input);

    [LibraryImport(HARFBUZZ)]
    internal static partial void hb_set_add(hb_set_t set, uint value);

    [LibraryImport(HARFBUZZ)]
    internal static partial hb_face_t hb_subset_or_fail(hb_face_t source, hb_subset_input_t input);

    [LibraryImport(HARFBUZZ)]
    internal static partial hb_blob_t hb_face_reference_blob(hb_face_t face);

    [LibraryImport(HARFBUZZ)]
    internal static partial void hb_subset_input_destroy(hb_subset_input_t input);

    [LibraryImport(HARFBUZZ)]
    internal static partial hb_face_t hb_font_get_face(IntPtr font);
}
