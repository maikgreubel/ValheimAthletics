using HarmonyLib;
using UnityEngine;

namespace ValheimAthletics.Patches
{
    /**
     * <summary>This class provides a <see cref="HarmonyPatch"/> to set marker in case of player attaches to a vagon.</summary>
     **/
    [HarmonyPatch(typeof(Vagon), "AttachTo")]
    public static class VagonAttachPatch
    {
        /**
         * <summary>Will be called in case of vagon attach state has changed.</summary>
         **/
        public static void Postfix(Vagon __instance, GameObject go)
        {
            if(__instance.IsAttached(Player.m_localPlayer))
            {
                Mod.PullingVagon = !Mod.PullingVagon;
            }
        }
    }
}
