using MelonLoader;
using MelonLoader.Logging;
using MelonLoader.Utils;
using System.Runtime.InteropServices;
using zsplice.Extensions;

[assembly: MelonInfo(typeof(zsplice.Core), "zsplice", "1.0.0", "gameknight963")]
[assembly: MelonAuthorColor(255, 86, 65, 157)]

namespace zsplice
{
    public class Core : MelonPlugin
    {
        public readonly string NativeModsPath = 
            Path.Combine(MelonEnvironment.MelonBaseDirectory, "NativeMods");
        
        public IReadOnlyList<NativeMod> NativeMods => nativeMods;
        private readonly List<NativeMod> nativeMods = new();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate ZModInfo ZSplice_GetInfo();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate ZModInfo ZSplice_ModInit();

        public override void OnPreModsLoaded()
        {
            foreach (string path in Directory.GetFiles(NativeModsPath, "*.dll"))
            {
                NativeLibrary lib = NativeLibrary.Load(path);

                if (lib.TryGetExport("ZSplice_GetInfo", out ZSplice_GetInfo export))
                {
                    ZModInfo info;
                    info = export();
                    nativeMods.Add(new NativeMod(info, lib));
                    LogMelonInfo(info, path);
                    lib.GetExport<ZSplice_ModInit>("ZSplice_ModInit")();
                    continue;
                }
                LogMelonInfo(path);
            }
        }

        private static void LogMelonInfo(string? libFile = null)
        {
            MelonLogger.MsgDirect(ColorARGB.Green, "------------------------------");
            MelonLogger.MsgDirect(ColorARGB.LightGray, $"{Path.GetFileNameWithoutExtension(libFile)}");
            MelonLogger.MsgDirect(ColorARGB.Gray, $"by <unknown>");
            if (libFile != null) MelonLogger.MsgDirect(ColorARGB.Gray, $"Library file: {Path.GetFileName(libFile)}");
            MelonLogger.MsgDirect(ColorARGB.Green, "------------------------------");
        }

        private static void LogMelonInfo(ZModInfo info, string? libFile = null)
        {
            MelonLogger.MsgDirect(ColorARGB.Green, "------------------------------");
            MelonLogger.MsgDirect(ColorARGB.LightGray, $"{info.GetName()} v{info.GetVersion()}");
            MelonLogger.MsgDirect(info.GetAuthorColor(), $"by {info.GetAuthor()}");
            if (libFile != null) MelonLogger.MsgDirect(ColorARGB.Gray, $"Library file: {Path.GetFileName(libFile)}");
            MelonLogger.MsgDirect(ColorARGB.Green, "------------------------------");
        }
    }
}