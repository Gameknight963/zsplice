using MelonLoader;
using MelonLoader.Utils;
using System.Runtime.InteropServices;

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

                // Register the mod
                ZModInfo info = lib.GetExport<ZSplice_GetInfo>("ZSplice_GetInfo")();
                nativeMods.Add(new NativeMod(info, lib));
                LogMelonInfo(info);

                // Initialize the mod
                lib.GetExport<ZSplice_ModInit>("ZSplice_ModInit")();
            }
        }

        private static void LogMelonInfo(ZModInfo info)
        {
            MelonLogger.Msg("------------------------------");
            MelonLogger.Msg($"{info.GetName()} v{info.GetVersion()}");
            MelonLogger.MsgPastel(info.GetAuthorColor(), $"by {info.GetAuthor()}");
            MelonLogger.Msg("------------------------------");
        }
    }
}