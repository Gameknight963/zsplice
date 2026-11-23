using MelonLoader;
using MelonLoader.Logging;

namespace zsplice
{
    public readonly struct NativeMod(ZModInfo info, NativeLibrary lib)
    {
        public readonly NativeLibrary Library = lib;
        public readonly ZModInfo Info = info;
        public readonly string Name = info.GetName();
        public readonly string Author = info.GetAuthor();
        public readonly ColorARGB AuthorColor = info.GetAuthorColor();
    }
}
