using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ModConfigEnforcer;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using ValheimAthletics.Patches;
using ValheimAthletics.Util;

namespace ValheimAthletics
{
    /**
     * This class provides the basic mod. It derives from BaseUnityPlugin so uses unity callbacks.
     **/
    [BepInPlugin("ValheimAthletics.Skill", ModName, "1.0.5")]
    [BepInProcess("valheim.exe")]
    class Mod : BaseUnityPlugin
    {
        // The name of mod
        public const string ModName = "ValheimAthleticsSkill";
        // The mod identifier
        public const string ID = "ValheimAthletics.Skill";
        // The numeric mod identifier
        public static readonly int ValheimAthleticsSkillId = 900;
        // The skill object
        public static Skills.SkillType ValheimAthleticsSkill = (Skills.SkillType)Mod.ValheimAthleticsSkillId;
        // The skill definitions
        public static Skills.SkillDef ValheimAthleticsSkillDef;

        private Harmony harmony;
        
        // Default value of level raise interval
        private const float SKILL_RAISE_INTERVAL = 0.015f;
        // Default value of magnitude of vector
        private const float MIN_MOVE_DIRECTION_MAGNITUDE = 0.1f;
        // Default value of lower bound of stamina left
        private const float MIN_STAMINA = 0.0f;
        // Default value of seconds between raises
        private const int MIN_TIME_DIFF_SECONDS = 5;
        // Default value of factor multiplying level to add to current max carry weight
        private const float MAX_CARRY_WEIGHT_FACTOR = 2.0f;

        // When the counter starts
        private static DateTime? StartOfCounting = null;

        private static bool pullingVagon = false;
        public static bool PullingVagon { get { return pullingVagon; } set { pullingVagon = value; } }

        // Property which controls the level raise interval
        private ConfigEntry<float> SkillRaiseInterval;
        // Property to define the minimum distance to move based on vector
        private ConfigEntry<float> MinimumMoveDirectionMagnitude;
        // The lower bound stamina value where leveling is not possible
        private ConfigEntry<float> MinimumStamina;
        // The minimum number of seconds between level raise
        private ConfigEntry<int> MinimumTimeDifferenceInSeconds;
        // The factor multiplying on level to add to current max carry weight
        private ConfigEntry<float> MaxCarryWeightFactor;

        /**
         * <summary>
         * Callback which will be executed after loading of mod.
         * </summary>
         * <seealso cref="MonoBehaviour">https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html</seealso>
         **/
        public void Awake()
        {
            InitConfig();

            InitUtil();

            InitSkill();

            ModifyCarryWeightPatch.MaxCarryWeightFactor = MaxCarryWeightFactor.Value;

            harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), Mod.ID);
        }

        private void InitSkill()
        {
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
        }

        private void InitUtil()
        {
            Data.ModId = ID;
            Data.Folder = Path.GetDirectoryName(this.Info.Location);
            Log.Logging = BepInEx.Logging.Logger.CreateLogSource(Data.ModId);
        }

        private void InitConfig()
        {
            ConfigManager.RegisterMod(ModName, Config, null);

            MinimumStamina = Config.Bind<float>("Config", "MinimumStamina", MIN_STAMINA, "The lower bound stamina value where leveling is not possible");
            MinimumMoveDirectionMagnitude = Config.Bind<float>("Config", "MinimumMoveDirectionMagnitude", MIN_MOVE_DIRECTION_MAGNITUDE, "Property to define the minimum distance to move based on vector");
            MinimumTimeDifferenceInSeconds = Config.Bind<int>("Config", "MinimumTimeDifferenceInSeconds", MIN_TIME_DIFF_SECONDS, "The minimum number of seconds between level raise");
            SkillRaiseInterval = Config.Bind<float>("Config", "SkillRaiseInterval", SKILL_RAISE_INTERVAL, "Property which controls the level raise interval");
            MaxCarryWeightFactor = Config.Bind<float>("Config", "MaxCarryWeightFactor", MAX_CARRY_WEIGHT_FACTOR, "Property which controls the factor for multiplying on skill to add to max carry weight");
        }

        /**
         * <summary>Callback which will executed on exit of game.</summary>
         **/
        private void OnDestroy()
        {
            if(harmony != null)
            {
                harmony.UnpatchAll(Mod.ID);
            }
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
         * If the time span reached a defined threshed of <see cref="MinimumTimeDifferenceInSeconds"/> and there is stamina of higher than<br/>
         * <see cref="MinimumStamina"/> as well moving arround which is detected by checking the move direction magnitude higher than<br/>
         * <see cref="MinimumMoveDirectionMagnitude"/>, the athletic skill level is raised using <see cref="SkillRaiseInterval"/>.
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

            if (now.Value.Subtract(StartOfCounting.Value).TotalSeconds > MinimumTimeDifferenceInSeconds.Value &&
                localPlayer.GetMoveDir().magnitude > MinimumMoveDirectionMagnitude.Value && 
                localPlayer.GetStamina() > MinimumStamina.Value)
            {
                StartOfCounting = now;
                Task.Run(() => {
                localPlayer.RaiseSkill(Mod.ValheimAthleticsSkill, SkillRaiseInterval.Value);
                });
            }
        }
    }
}
