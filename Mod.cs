using BepInEx;
using HarmonyLib;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using ValheimAthletics.Util;

namespace ValheimAthletics
{
    /**
     * This class provides the basic mod. It derives from BaseUnityPlugin so uses unity callbacks.
     **/
    [BepInPlugin("ValheimAthletics.Skill", "ValheimAthleticsSkill", "1.0.0.0")]
    [BepInProcess("valheim.exe")]
    class Mod : BaseUnityPlugin
    {
        // The mod identifier
        public const string ID = "ValheimAthletics.Skill";
        // The numeric mod identifier
        public static readonly int ValheimAthleticsSkillId = 900;
        // The skill object
        public static Skills.SkillType ValheimAthleticsSkill = (Skills.SkillType)Mod.ValheimAthleticsSkillId;
        // The skill definitions
        public static Skills.SkillDef ValheimAthleticsSkillDef;
        
        // Property which controls the level raise interval
        private const float SKILL_RAISE_INTERVAL = 0.015f;
        // Property to define the minimum distance to move based on vector
        private const float MIN_MOVE_DIRECTION_MAGNITUDE = 0.1f;
        // The lower bound stamina value where leveling is not possible
        private const float MIN_STAMINA = 0.0f;
        // The minimum number of seconds between level raise
        private const int MIN_TIME_DIFF_SECONDS = 5;

        // When the counter starts
        private static DateTime? StartOfCounting = null;

        private static bool pullingVagon = false;
        public static bool PullingVagon { get { return pullingVagon; } set { pullingVagon = value; } }

        /**
         * <summary>
         * Callback which will be executed after loading of mod.
         * </summary>
         * <seealso cref="MonoBehaviour">https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html</seealso>
         **/
        public void Awake()
        {
            Data.ModId = ID;
            Data.Folder = Path.GetDirectoryName(this.Info.Location);
            Log.Logging = BepInEx.Logging.Logger.CreateLogSource(Data.ModId);
            Texture2D texture = Data.LoadTextureFromAssets("skill_athletics.png");
            
            Sprite sprite = Sprite.Create(texture, 
                new Rect(
                    0.0f,
                    0.0f, 
                    (float)((Texture)texture).width, 
                    (float)((Texture)texture).height),
                new Vector2(0.5f, 0.5f)
            );

            Mod.ValheimAthleticsSkillDef = new Skills.SkillDef()
            {
                m_skill = (Skills.SkillType)Mod.ValheimAthleticsSkillId,
                m_icon = (Sprite)sprite,
                m_description = Localizer.Instance.Translate("skill_900_description"),
                m_increseStep = 1.0f
            };
            new Harmony(ID).PatchAll();
        }

        /**
         * <summary>
         * Callback which will be executed frame-rate independent.
         * </summary>
         * 
         * <seealso cref="MonoBehaviour">https://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html</seealso>
         * 
         * <remarks>
         * First it is checked, whether or not the player moves during physically demanding.<br/>
         * In case of it is true for the first time after non demanding phase, the start counter will be set to current time.<br/>
         * When its not the first time, the start counter will be subtracted from current time to find difference as time span.<br/>
         * If the time span reached a defined threshed of <see cref="MIN_TIME_DIFF_SECONDS"/> and there is stamina of higher than<br/>
         * <see cref="MIN_STAMINA"/> as well moving arround which is detected by checking the move direction magnitude higher than<br/>
         * <see cref="MIN_MOVE_DIRECTION_MAGNITUDE"/>, the athletic skill level is raised using <see cref="SKILL_RAISE_INTERVAL"/>.
         * </remarks>
         **/
        public void FixedUpdate()
        {
            Player localPlayer = Player.m_localPlayer;
            if (localPlayer == null)
                return;

            // Does carry weight exceed threshold or our character is pulling a wagon?
            if(!localPlayer.IsEncumbered() && !pullingVagon)
            {
                return;
            }
            DateTime? now = DateTime.UtcNow;
            if ( StartOfCounting == null )
            {
                StartOfCounting = now;
                return;
            }

            if (now.Value.Subtract(StartOfCounting.Value).TotalSeconds > MIN_TIME_DIFF_SECONDS &&
                localPlayer.GetMoveDir().magnitude > MIN_MOVE_DIRECTION_MAGNITUDE && 
                localPlayer.GetStamina() > MIN_STAMINA)
            {
                StartOfCounting = now;
                Task.Run(() => {
                    localPlayer.RaiseSkill(Mod.ValheimAthleticsSkill, SKILL_RAISE_INTERVAL);
                });
            }
        }
    }
}
