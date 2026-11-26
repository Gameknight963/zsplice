using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using HarmonyLib;

public static unsafe class NativeHookBridge
{
    private static readonly HarmonyLib.Harmony Harmony = 
        new("gameknight963.zsplice.NativeHookBridge");
    private static readonly Dictionary<MethodBase, Type> Bridges = new();
    private static readonly List<Delegate> Roots = new();
    private static readonly ModuleBuilder Module = AssemblyBuilder.DefineDynamicAssembly(
        new AssemblyName("gameknight963.zsplice.HarmonyNativeAdapters"), AssemblyBuilderAccess.Run)
        .DefineDynamicModule("gameknight963.zsplice.HarmonyNativeAdapters");
    private static int _nextId;

    public static void* Hook(MethodInfo target, void* detour)
    {
        if (!Bridges.TryGetValue(target, out Type bridge))
        {
            bridge = CreateBridgeType(target);
            Bridges.Add(target, bridge);
            MethodInfo factory = typeof(NativeHookBridge).GetMethod(
                nameof(PrefixFactory), BindingFlags.Static | BindingFlags.NonPublic);
            Harmony.Patch(target, prefix: new HarmonyMethod(factory) { priority = Priority.Last });
        }

        FieldInfo head = bridge.GetField("Head");
        Delegate previous = (Delegate)head.GetValue(null);
        void* original = (void*)Marshal.GetFunctionPointerForDelegate(previous);
        Delegate callback = Marshal.GetDelegateForFunctionPointer((IntPtr)detour, head.FieldType);
        Roots.Add(callback);
        head.SetValue(null, callback);
        return original;
    }

    private static MethodInfo PrefixFactory(MethodBase target)
    {
        Type bridge = Bridges[target];
        Harmony.CreateReversePatcher(target, new HarmonyMethod(bridge.GetMethod("Body")))
            .Patch(HarmonyReversePatchType.Snapshot);
        return bridge.GetMethod("Prefix");
    }

    private static Type CreateBridgeType(MethodInfo target)
    {
        bool isInstanceMethod = !target.IsStatic;
        int offset = isInstanceMethod ? 1 : 0;
        Type receiver = target.DeclaringType;
        Type returnType = target.ReturnType;
        Type[] parameters = Array.ConvertAll(target.GetParameters(), p => p.ParameterType);
        Type[] managedArguments = new Type[parameters.Length + offset];
        Array.Copy(parameters, 0, managedArguments, offset, parameters.Length);
        if (isInstanceMethod) managedArguments[0] = receiver.IsValueType ? receiver.MakeByRefType() : receiver;
        Type[] nativeArguments = (Type[])managedArguments.Clone();
        if (isInstanceMethod) nativeArguments[0] = typeof(IntPtr);

        Type? il2cppBase = isInstanceMethod ? receiver : null;
        while (il2cppBase != null && il2cppBase.FullName != "Il2CppInterop.Runtime.InteropTypes.Il2CppObjectBase")
            il2cppBase = il2cppBase.BaseType;
        MethodInfo? getPointer = il2cppBase?.GetProperty("Pointer").GetGetMethod();
        MethodInfo? wrapPointer = il2cppBase?.Assembly
            .GetType("Il2CppInterop.Runtime.Runtime.Il2CppObjectPool").GetMethod("Get").MakeGenericMethod(receiver);

        Type delegateType = DelegateTypeFactory.instance.CreateDelegateType(
            returnType, nativeArguments, CallingConvention.Cdecl);
        MethodInfo invoke = delegateType.GetMethod("Invoke");
        TypeBuilder type = Module.DefineType("Bridge_" + ++_nextId,
            TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed);
        FieldBuilder head = type.DefineField("Head", delegateType, FieldAttributes.Public | FieldAttributes.Static);

        MethodBuilder body = type.DefineMethod("Body", MethodAttributes.Public | MethodAttributes.Static,
            returnType, managedArguments);
        body.SetImplementationFlags(MethodImplAttributes.IL | MethodImplAttributes.Managed | MethodImplAttributes.NoInlining);
        ILGenerator bodyIL = body.GetILGenerator();
        bodyIL.Emit(OpCodes.Newobj, typeof(InvalidOperationException).GetConstructor(Type.EmptyTypes));
        bodyIL.Emit(OpCodes.Throw);

        if (isInstanceMethod)
        {
            MethodBuilder tail = type.DefineMethod("Tail", MethodAttributes.Public | MethodAttributes.Static,
                returnType, nativeArguments);
            ILGenerator tailIL = tail.GetILGenerator();
            if (wrapPointer != null)
            {
                tailIL.Emit(OpCodes.Ldarg_0);
                tailIL.Emit(OpCodes.Call, wrapPointer);
            }
            else if (receiver.IsValueType)
            {
                tailIL.Emit(OpCodes.Ldarg_0);
            }
            else
            {
                tailIL.Emit(OpCodes.Ldarga_S, (byte)0);
                tailIL.Emit(OpCodes.Ldind_Ref);
                tailIL.Emit(OpCodes.Castclass, receiver);
            }
            for (int i = 1; i < managedArguments.Length; i++) tailIL.Emit(OpCodes.Ldarg, unchecked((short)i));
            tailIL.Emit(OpCodes.Call, body);
            tailIL.Emit(OpCodes.Ret);
        }

        bool hasResult = returnType != typeof(void);
        int flagIndex = managedArguments.Length + (hasResult ? 1 : 0);
        Type[] prefixArguments = new Type[flagIndex + 1];
        Array.Copy(managedArguments, prefixArguments, managedArguments.Length);
        if (hasResult) prefixArguments[managedArguments.Length] = returnType.MakeByRefType();
        prefixArguments[flagIndex] = typeof(bool);
        MethodBuilder prefix = type.DefineMethod("Prefix", MethodAttributes.Public | MethodAttributes.Static,
            typeof(bool), prefixArguments);
        if (isInstanceMethod) prefix.DefineParameter(1, ParameterAttributes.None, "__instance");
        for (int i = 0; i < parameters.Length; i++)
            prefix.DefineParameter(i + offset + 1, ParameterAttributes.None, "__" + i);
        if (hasResult) prefix.DefineParameter(managedArguments.Length + 1, ParameterAttributes.None, "__result");
        prefix.DefineParameter(flagIndex + 1, ParameterAttributes.None, "__runOriginal");

        ILGenerator prefixIL = prefix.GetILGenerator();
        Label run = prefixIL.DefineLabel();
        prefixIL.Emit(OpCodes.Ldarg, unchecked((short)flagIndex));
        prefixIL.Emit(OpCodes.Brtrue, run);
        prefixIL.Emit(OpCodes.Ldc_I4_1);
        prefixIL.Emit(OpCodes.Ret);
        prefixIL.MarkLabel(run);
        LocalBuilder? pinnedReceiver = null;
        if (isInstanceMethod && getPointer == null)
        {
            pinnedReceiver = prefixIL.DeclareLocal(managedArguments[0], true);
            prefixIL.Emit(OpCodes.Ldarg_0);
            prefixIL.Emit(OpCodes.Stloc, pinnedReceiver);
        }
        if (hasResult) prefixIL.Emit(OpCodes.Ldarg, unchecked((short)managedArguments.Length));
        prefixIL.Emit(OpCodes.Volatile);
        prefixIL.Emit(OpCodes.Ldsfld, head);
        if (isInstanceMethod)
        {
            if (pinnedReceiver != null)
            {
                prefixIL.Emit(OpCodes.Ldloc, pinnedReceiver);
                prefixIL.Emit(OpCodes.Conv_I);
            }
            else
            {
                prefixIL.Emit(OpCodes.Ldarg_0);
                prefixIL.Emit(OpCodes.Call, getPointer);
            }
        }
        for (int i = offset; i < managedArguments.Length; i++) prefixIL.Emit(OpCodes.Ldarg, unchecked((short)i));
        prefixIL.Emit(OpCodes.Callvirt, invoke);
        if (hasResult) prefixIL.Emit(OpCodes.Stobj, returnType);
        if (isInstanceMethod && !receiver.IsValueType)
        {
            if (pinnedReceiver != null) prefixIL.Emit(OpCodes.Ldloc, pinnedReceiver);
            else prefixIL.Emit(OpCodes.Ldarg_0);
            prefixIL.Emit(OpCodes.Call, typeof(GC).GetMethod("KeepAlive", new Type[] { typeof(object) }));
        }
        if (pinnedReceiver != null)
        {
            if (receiver.IsValueType)
            {
                prefixIL.Emit(OpCodes.Ldc_I4_0);
                prefixIL.Emit(OpCodes.Conv_U);
            }
            else prefixIL.Emit(OpCodes.Ldnull);
            prefixIL.Emit(OpCodes.Stloc, pinnedReceiver);
        }
        prefixIL.Emit(OpCodes.Ldc_I4_0);
        prefixIL.Emit(OpCodes.Ret);

        Type generated = type.CreateTypeInfo().AsType();
        Delegate tailCallback = generated.GetMethod(isInstanceMethod ? "Tail" : "Body").CreateDelegate(delegateType);
        Roots.Add(tailCallback);
        generated.GetField("Head").SetValue(null, tailCallback);
        return generated;
    }
}