using HarmonyLib;
using ValheimAthletics.Util;

namespace ValheimAthletics.Patches
{
    /**
     * <summary>This class provides a <see cref="HarmonyPatch"/> to save skill data.</summary>
     **/
    [HarmonyPatch(typeof(PlayerProfile), "SavePlayerData")]
    public static class SavekillsPatch
    {
        /**
         * <summary>Save skill data for given <see cref="Player"/> identified by <paramref name="player"/> into <see cref="PlayerProfile"/> identified by <paramref name="__instance"/></summary>
         **/
        public static void Postfix(PlayerProfile __instance, Player player)
        {
            Skills.Skill skill = (Skills.Skill)AccessTools.Method(typeof(Skills), "GetSkill", null, null).Invoke(player.GetSkills(), new object[1] { 
                Mod.ValheimAthleticsSkill
            });

            __instance.SaveModData<SkillData>(new SkillData() { 
                Level = (int)skill.m_level,
                Progress = skill.m_accumulator
            });
        }
    }
}
