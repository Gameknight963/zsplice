using System.Runtime.InteropServices;

namespace zsplice
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ZModCtx
    {
        public uint StructSize;
        public fixed uint ApiVersion[3];

        public void* Msg;
        public void* HarmonyHook;

        public byte* GameName;
        public byte* GameVersion;
        public byte* MelonLoaderVersion;

        public RuntimeType Runtime;
        public Architecture Architecture;
    }
}
