using System.Runtime.InteropServices;

namespace zsplice
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate ZModInfo ZSplice_GetInfo();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void ZSplice_ModInit(ZModCtx ctx);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate HarmonyStatus HarmonyHook_t(
        byte* AssemblyQualifiedName,
        byte* Method,
        void* pDetour,
        void** ppOriginal);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void Msg_t(byte* message);
}
