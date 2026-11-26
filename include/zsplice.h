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

typedef enum HarmonyStatus
{
    HARMONY_OK = 0,

    HARMONY_TYPE_NOT_FOUND,
    HARMONY_METHOD_NOT_FOUND,
    HARMONY_AMBIGUOUS_METHOD,
    HARMONY_INVALID_ARGUMENT,

    HARMONY_PATCH_FAILED,
    HARMONY_INTERNAL_ERROR
} HarmonyStatus;

typedef HarmonyStatus (*HarmonyHook_t)(
    const char* AssemblyQualifiedName,
    const char* Method,
    void* pDetour,
    void** ppOriginal
);

typedef void (*Msg_t)(const char* message);

typedef enum RuntimeType {
    RUNTIME_UNKNOWN = 0,
    RUNTIME_MONO,
    RUNTIME_IL2CPP
} RuntimeType;

typedef enum Architecture {
    ARCH_UNKNOWN = 0,
    ARCH_X86,
    ARCH_X64,
    ARCH_ARM32,
    ARCH_ARM64
} Architecture;

// Provides capabilities and information for mods
typedef struct ZModCtx {
    unsigned int StructSize;
    unsigned int ApiVersion[3];
    Msg_t Msg;
    HarmonyHook_t HarmonyHook;
    const char* GameName;
    const char* GameVersion;
    const char* MelonLoaderVersion;
    RuntimeType Runtime;
    Architecture Architecture;
} ZModCtx;

// Represents a user-created mod
typedef struct ZModInfo {
    const char* Name;
    const char* Version;
    const char* Author;
    int32_t AuthorColor;
} ZModInfo;

// Only use this to return your ZModInfo, don't perform other work
ZSPLICE_EXPORT extern ZModInfo ZSPLICE_CALL ZSplice_GetInfo(void);

// Initialize your mod here
ZSPLICE_EXPORT extern void ZSPLICE_CALL ZSplice_ModInit(ZModCtx ctx);

#ifdef __cplusplus
}
#endif

#endif





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

    typedef enum HarmonyStatus
    {
        HARMONY_OK = 0,

        HARMONY_TYPE_NOT_FOUND,
        HARMONY_METHOD_NOT_FOUND,
        HARMONY_AMBIGUOUS_METHOD,
        HARMONY_INVALID_ARGUMENT,

        HARMONY_PATCH_FAILED,
        HARMONY_INTERNAL_ERROR
    } HarmonyStatus;

    typedef HarmonyStatus(*HarmonyHook_t)(
        const char* AssemblyQualifiedName,
        const char* Method,
        void* pDetour,
        void** ppOriginal
        );

    typedef void (*Msg_t)(const char* message);

    typedef enum RuntimeType {
        RUNTIME_UNKNOWN = 0,
        RUNTIME_MONO,
        RUNTIME_IL2CPP
    } RuntimeType;

    typedef enum Architecture {
        ARCH_UNKNOWN = 0,
        ARCH_X86,
        ARCH_X64,
        ARCH_ARM32,
        ARCH_ARM64
    } Architecture;

    // Provides capabilities and information for mods
    typedef struct ZModCtx {
        unsigned int StructSize;
        unsigned int ApiVersion[3];
        Msg_t Msg;
        HarmonyHook_t HarmonyHook;
        const char* GameName;
        const char* GameVersion;
        const char* MelonLoaderVersion;
        RuntimeType Runtime;
        Architecture Architecture;
    } ZModCtx;

    // Represents a user-created mod
    typedef struct ZModInfo {
        const char* Name;
        const char* Version;
        const char* Author;
        int32_t AuthorColor;
    } ZModInfo;

    // Only use this to return your ZModInfo, don't perform other work
    ZSPLICE_EXPORT extern ZModInfo ZSPLICE_CALL ZSplice_GetInfo(void);

    // Initialize your mod here
    ZSPLICE_EXPORT extern void ZSPLICE_CALL ZSplice_ModInit(ZModCtx ctx);

#ifdef __cplusplus
}
#endif

#endif