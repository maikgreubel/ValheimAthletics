using HarmonyLib;
using ValheimAthletics.Util;


namespace ValheimAthletics.Patches
{
    /**
     * <summary>This class provides loading the data of the athletic skill for particular player.</summary>
     **/
    [HarmonyPatch(typeof(PlayerProfile), "LoadPlayerData" )]
    public static class LoadSkillsPatch
    {
        /**
         * <summary>Loads the athletic skill data out of given <see cref="PlayerProfile"/> identified by <paramref name="__instance"/> and apply them to particular <see cref="Player"/> identified by <paramref name="player"/>.</summary>
         **/
        public static void Postfix(PlayerProfile __instance, Player player)
        {
            SkillData skillData = __instance.LoadModData<SkillData>();
            Skills.Skill skill = (Skills.Skill)AccessTools.Method(typeof(Skills), "GetSkill", null, null).Invoke(player.GetSkills(), new object[1] {
                Mod.ValheimAthleticsSkill
            });
            skill.m_level = skillData.Level;
            skill.m_accumulator = skillData.Progress;
        }
    }
}
