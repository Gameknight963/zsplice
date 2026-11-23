using MelonLoader;

namespace zsplice.Extensions
{
    public static class NativeLibraryExtensions
    {
        public static bool TryGetExport<T>(this NativeLibrary This, string name, out T export) where T : Delegate
        {
            export = default!;
            IntPtr address = NativeLibrary.AgnosticGetProcAddress(This.Ptr, name);
            if (address == IntPtr.Zero) return false;
            export = address.GetDelegate<T>();
            return true;
        }
    }
}
