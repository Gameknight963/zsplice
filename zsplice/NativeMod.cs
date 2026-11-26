using MelonLoader;
using MelonLoader.Logging;
using System.Runtime.InteropServices;

namespace zsplice
{
    public class NativeMod
    {
        public readonly NativeLibrary Library;
        public readonly ZModInfo Info;
        public readonly string Name;
        public readonly string Author;
        public readonly ColorARGB AuthorColor;

        public NativeMod(ZModInfo info, NativeLibrary lib)
        {
            Library = lib;
            Info = info;
            Name = info.GetName();
            Author = info.GetAuthor();
            AuthorColor = info.GetAuthorColor();
        }

        public unsafe ZModCtx CreateCtx()
        {
            var ctx = new ZModCtx
            {
                StructSize = (uint)sizeof(ZModCtx),
                GameName = ZSpliceContextSupport.GameName,
                GameVersion = ZSpliceContextSupport.GameVersion,
                Architecture = RuntimeInformation.ProcessArchitecture switch
                {
                    System.Runtime.InteropServices.Architecture.X86 => Architecture.ARCH_X86,
                    System.Runtime.InteropServices.Architecture.X64 => Architecture.ARCH_X64,
                    System.Runtime.InteropServices.Architecture.Arm => Architecture.ARCH_ARM32,
                    System.Runtime.InteropServices.Architecture.Arm64 => Architecture.ARCH_ARM64,
                    _ => Architecture.ARCH_UNKNOWN
                },
                MelonLoaderVersion = ZSpliceContextSupport.MelonLoaderVersion,
                Runtime = MelonUtils.IsGameIl2Cpp()
                    ? RuntimeType.RUNTIME_IL2CPP
                    : RuntimeType.RUNTIME_MONO,
                Msg = (void*)Marshal.GetFunctionPointerForDelegate(ZSpliceContextSupport.MsgCallback),
                HarmonyHook = (void*)Marshal.GetFunctionPointerForDelegate(ZSpliceContextSupport.HarmonyCallback),            
            };
            ctx.ApiVersion[0] = ZSpliceContextSupport.ApiMajor;
            ctx.ApiVersion[1] = ZSpliceContextSupport.ApiMinor;
            ctx.ApiVersion[2] = ZSpliceContextSupport.ApiPatch;
            return ctx;
        }
    }
}
