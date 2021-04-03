using HarmonyLib;

namespace ValheimAthletics.Patches
{
    /**
     * <summary>This class provides a <see cref="HarmonyPatch"/> to inject the athletic skill definition into skills object.</summary>
     **/
    [HarmonyPatch(typeof(Skills), "GetSkillDef")]
    public static class GetSkillDefPatch
    {
        /**
         * <summary>Injects new skill definition identified by <paramref name="__result"/> into <see cref="Skills"/> object.</summary>
         **/
        public static void Postfix(Skills __instance, Skills.SkillType type, ref Skills.SkillDef __result)
        {
            if (type == Mod.ValheimAthleticsSkill)
            {
                __result = Mod.ValheimAthleticsSkillDef;
            }
        }
    }
}
