using HarmonyLib;
using KSA;

namespace KittenProtoLink;

[HarmonyPatch]
internal static class Patcher
{
    private static Harmony? _harmony = new Harmony("KittenProtoLink");

    public static void Patch()
    {
        Console.WriteLine("Patching KittenProtoLink...");
        _harmony?.PatchAll();
    }

    public static void Unload()
    {
        _harmony?.UnpatchAll(_harmony.Id);
        _harmony = null;
    }
    
    [HarmonyPatch(typeof(ModLibrary), nameof(ModLibrary.LoadAll))]
    [HarmonyPostfix]
    public static void AfterLoad()
    {
        Console.WriteLine("ModLibrary.LoadAll patched by SimpleMod.");
    }
}

