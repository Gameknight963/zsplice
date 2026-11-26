using MelonLoader;
using MelonLoader.InternalUtils;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace zsplice
{
    public static unsafe class ZSpliceContextSupport
    {
        public static readonly byte* GameName = AllocCString(UnityInformationHandler.GameName);
        public static readonly byte* GameVersion = AllocCString(UnityInformationHandler.GameVersion);
        public static readonly byte* MelonLoaderVersion = AllocCString(MelonLoader.Properties.BuildInfo.Version);

        public const int ApiMajor = 1;
        public const int ApiMinor = 0;
        public const int ApiPatch = 0;

        public static readonly Msg_t MsgCallback = OnMessage;
        public static readonly HarmonyHook_t HarmonyCallback = OnHarmonyHook;

        private static void OnMessage(byte* pszMessage)
        {
            MelonLogger.Msg(Marshal.PtrToStringUTF8((IntPtr)pszMessage));
        }


        private static unsafe HarmonyStatus OnHarmonyHook(
            byte* pszAssemblyQualifiedName,
            byte* pszMethod,
            void* pDetour,
            void** ppOriginal)
        {
            if (ppOriginal == null)
                return HarmonyStatus.HARMONY_INVALID_ARGUMENT;

            *ppOriginal = null;

            if (pszAssemblyQualifiedName == null ||
                pszMethod == null ||
                pDetour == null)
            {
                return HarmonyStatus.HARMONY_INVALID_ARGUMENT;
            }

            try
            {
                string assemblyQualifiedName =
                    Marshal.PtrToStringUTF8((IntPtr)pszAssemblyQualifiedName);

                string methodName =
                    Marshal.PtrToStringUTF8((IntPtr)pszMethod);

                if (string.IsNullOrWhiteSpace(assemblyQualifiedName) ||
                    string.IsNullOrWhiteSpace(methodName))
                {
                    return HarmonyStatus.HARMONY_INVALID_ARGUMENT;
                }

                Type enclosingType = Type.GetType(assemblyQualifiedName);
                if (enclosingType == null)
                    return HarmonyStatus.HARMONY_TYPE_NOT_FOUND;

                MethodInfo method = enclosingType.GetMethod(
                    methodName,
                    BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.Static | BindingFlags.Instance);

                if (method == null)
                    return HarmonyStatus.HARMONY_METHOD_NOT_FOUND;

                try
                {
                    *ppOriginal = NativeHookBridge.Hook(method, pDetour);
                    return HarmonyStatus.HARMONY_OK;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return HarmonyStatus.HARMONY_PATCH_FAILED;
                }
            }
            catch (AmbiguousMatchException)
            {
                return HarmonyStatus.HARMONY_AMBIGUOUS_METHOD;
            }
            catch (ArgumentException)
            {
                return HarmonyStatus.HARMONY_INVALID_ARGUMENT;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return HarmonyStatus.HARMONY_INTERNAL_ERROR;
            }
        }

        private static byte* AllocCString(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value + '\0');
            IntPtr ptr = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, ptr, bytes.Length);
            return (byte*)ptr;
        }
    }
}
