#ifndef ZSPLICE_H
#define ZSPLICE_H

#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif

#if defined(_WIN32)
    #define ZSPLICE_EXPORT __declspec(dllexport)
    #define ZSPLICE_CALL __cdecl
#else
    #define ZSPLICE_EXPORT __attribute__((visibility("default")))
    #define ZSPLICE_CALL
#endif

typedef struct ZModInfo {
    const char* Name;
    const char* Version;
    const char* Author;
    int32_t AuthorColor;
} ZModInfo;

ZModInfo ZSplice_CreateDefaultInfo(void)
{
    ZModInfo info;
    info.Name = "Unnamed Mod";
    info.Version = "1.0.0";
    info.Author = "<no author>";
    info.AuthorColor = 0xff525252;
    return info;
}

// Only use this to return your ZModInfo, don't perform other work
ZSPLICE_EXPORT extern ZModInfo ZSPLICE_CALL ZSplice_GetInfo(void);

// Initialize your mod here
ZSPLICE_EXPORT extern void ZSPLICE_CALL ZSplice_ModInit(void);

#ifdef __cplusplus
}
#endif

#endif