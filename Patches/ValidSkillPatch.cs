using HarmonyLib;

namespace ValheimAthletics.Patches
{
    /**
     * <summary>This class provides a <see cref="HarmonyPatch"/> to checking whether a skill is valid.</summary>
     **/
    [HarmonyPatch(typeof(Skills), "IsSkillValid" )]
    public static class ValidSkillPatch
    {
        /**
         * <summary>Will be executed before all default skill checks are performed.</summary>
         **/
        public static bool Prefix(Skills __instance, Skills.SkillType type, ref bool __result)
        {
            if (type != Mod.ValheimAthleticsSkill)
                return true;

            __result = true;
            return false;
        }
    }
}
