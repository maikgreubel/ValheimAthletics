using HarmonyLib;

namespace ValheimAthletics.Patches
{
    /**
     * <summary>This class provides a <see cref="HarmonyPatch"/> to modify the maximum carry weight.</summary>
     **/
    [HarmonyPatch(typeof(Player), "GetMaxCarryWeight")]
    public static class ModifyCarryWeightPatch
    {
        private const float MAX_CARRY_WEIGHT_FACTOR = 2.0f;

        /**
         * <summary>Modifies the maximum carry weight of given <see cref="Player"/> idenfied by <paramref name="__instance"/> by manipulating the value represented by <paramref name="__result"/>.</summary>
         **/
        public static void Postfix(Player __instance, ref float __result)
        {
            Skills.Skill skill = (Skills.Skill)AccessTools.Method(typeof(Skills), "GetSkill", null, null).Invoke(__instance.GetSkills(), new object[1] {
                Mod.ValheimAthleticsSkill
            });

            __result += skill.m_level * MAX_CARRY_WEIGHT_FACTOR;
        }
    }
}
