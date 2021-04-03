using HarmonyLib;

namespace ValheimAthletics.Patches
{
    /**
     * <summary>This class provides a <see cref="HarmonyPatch"/> to release marker in case of player detaches from a vagon.</summary>
     **/
    [HarmonyPatch(typeof(Vagon), "Detach")]
    public static class VagonDetachPatch
    {
        /**
         * <summary>Will be called in case player detaches vagon before releasing the attachment.</summary>
         **/
        public static void Prefix(Vagon __instance)
        {
            if(__instance.IsAttached(Player.m_localPlayer))
            {
                Mod.PullingVagon = !Mod.PullingVagon;
            }
        }
    }
}
