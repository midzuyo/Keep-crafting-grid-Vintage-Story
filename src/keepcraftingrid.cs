using System;
using System.Reflection;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace KeepCraftingGrid
{
    public class ModConfig
    {
        public bool KeepCraftingGrid { get; set; } = true;
        public bool KeepMouseSlot    { get; set; } = false;
    }

    public class KeepCraftingGridMod : ModSystem
    {
        private Harmony? harmony;
        private const string HarmonyId = "keepcraftinggrid.mod";

        public override bool ShouldLoad(EnumAppSide forSide) => forSide == EnumAppSide.Client;

        public override void StartClientSide(ICoreClientAPI api)
        {
            ModConfig config = api.LoadModConfig<ModConfig>("keepcraftinggrid.json") ?? new ModConfig();
            api.StoreModConfig(config, "keepcraftinggrid.json");

            harmony = new Harmony(HarmonyId);

            if (config.KeepCraftingGrid)
                PatchMethod(api, "Vintagestory.Client.NoObf.GuiDialogInventory", "OnGuiClosed");

            if (config.KeepMouseSlot)
                PatchMethod(api, "Vintagestory.Common.PlayerInventoryManager", "DropMouseSlotItems");
        }

        private void PatchMethod(ICoreClientAPI api, string typeName, string methodName)
        {
            Type? type = AccessTools.TypeByName(typeName);
            if (type == null) { api.Logger.Warning($"[KeepCraftingGrid] Type not found: {typeName}"); return; }

            MethodInfo? method = AccessTools.Method(type, methodName);
            if (method == null) { api.Logger.Warning($"[KeepCraftingGrid] Method not found: {typeName}.{methodName}"); return; }

            harmony!.Patch(method, prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.ReturnFalse)));
            api.Logger.Notification($"[KeepCraftingGrid] Patched {typeName}.{methodName}");
        }

        public override void Dispose()
        {
            harmony?.UnpatchAll(HarmonyId);
        }
    }

    public static class Patches
    {
        public static bool ReturnFalse() => false;
    }
}
