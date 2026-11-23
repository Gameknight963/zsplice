#ifndef ZSPLICE_H
#define ZSPLICE_H

#include <cstdint>

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
    const char* Name = "Unnamed Mod";
    const char* Version = "1.0.0";
    const char* Author = "<no author>";
    const int32_t AuthourColor = 0xff525252;
} ZModInfo;

// Required method exports for zsplice mod

// Only use this to return your ZModInfo, don't perform other work
ZSPLICE_EXPORT extern ZModInfo ZSPLICE_CALL ZSplice_GetInfo(void);

// Initialize your mod here
ZSPLICE_EXPORT extern void ZSPLICE_CALL ZSplice_ModInit(void);

#ifdef __cplusplus
}

#endif