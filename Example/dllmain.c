#include <windows.h>
#include "stdio.h"
#include "zsplice.h"

BOOL APIENTRY DllMain(
    HMODULE hModule, 
    DWORD ul_reason_for_call, 
    LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

// Only use this to return your ZModInfo, don't perform other work
ZSPLICE_EXPORT ZModInfo ZSPLICE_CALL ZSplice_GetInfo(void)
{
    ZModInfo info;
    info.Name = "Unnamed Mod";
    info.Version = "1.0.0";
    info.Author = "<no author>";
    info.AuthorColor = 0xff525252;
    return info;
}

// Initialize your mod here
ZSPLICE_EXPORT void ZSPLICE_CALL ZSplice_ModInit(ZModCtx ctx)
{
    ctx.Msg("hello");
}