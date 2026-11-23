using MelonLoader.Logging;
using System.Runtime.InteropServices;

namespace zsplice
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe readonly struct ZModInfo
    {
        public readonly string GetName() => Marshal.PtrToStringUTF8((IntPtr)Name);
        public readonly string GetVersion() => Marshal.PtrToStringUTF8((IntPtr)Version);
        public readonly string GetAuthor() => Marshal.PtrToStringUTF8((IntPtr)Author);
        public readonly ColorARGB GetAuthorColor() => 
            ColorARGB.FromArgb(AuthorColor);

        readonly byte* Name;
        readonly byte* Version;
        readonly byte* Author;
        readonly uint AuthorColor;
    }
}
