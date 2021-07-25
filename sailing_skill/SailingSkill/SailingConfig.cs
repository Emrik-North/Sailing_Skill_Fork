using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace SailingSkill
{
    public class SailingConfig
    {
        public int NEXUS_ID = 922;

        private ConfigVariable<float> skillIncrease;
        private ConfigVariable<int> skillIncreaseTick;
        private ConfigVariable<float> halfSailSkillIncreaseMultiplier;
        private ConfigVariable<float> fullSailSkillIncreaseMultiplier;
        private ConfigVariable<float> maxTailwindBoost;
        private ConfigVariable<float> maxForewindDampener;
        private ConfigVariable<float> maxDamageReduction;
        private ConfigVariable<float> maxRudderBoost;

        public float SkillIncrease
        {
            get { return skillIncrease.Value; }
        }

        public int SkillIncreaseTick
        {
            get { return skillIncreaseTick.Value; }
        }
        public float HalfSailSkillIncreaseMultiplier
        {
            get { return halfSailSkillIncreaseMultiplier.Value; }
        }

        public float FullSailSkillIncreaseMultiplier
        {
            get { return fullSailSkillIncreaseMultiplier.Value; }
        }        
        public float MaxTailwindBoost
        {
            get { return maxTailwindBoost.Value; }
        }
        public float MaxForewindDampener
        {
            get { return -maxForewindDampener.Value; }
        }
        public float MaxDamageReduction
        {
            get { return -maxDamageReduction.Value; }
        }
        public float MaxRudderBoost
        {
            get { return maxRudderBoost.Value; }
        }

        public void InitConfig(string id, ConfigFile config)
        {
            config.Bind<int>("General", "NexusID", NEXUS_ID, "Nexus mod ID for updates");

            skillIncrease = new ConfigVariable<float>(config, id, "skillIncrease", .5f, "Leveling", "Amount of skill exp granted per skillIncreaseTick", false);
            skillIncreaseTick = new ConfigVariable<int>(config, id, "skillIncreaseTick", 300, "Leveling", "Number of boat update ticks to grant skill increase after, 50 is roughly equivalent to 1 second", false);
            halfSailSkillIncreaseMultiplier = new ConfigVariable<float>(config, id, "halfSailSkillIncreaseMultiplier", 1.5f, "Leveling", "Exp multiplier for half sail sailing speed", false);
            fullSailSkillIncreaseMultiplier = new ConfigVariable<float>(config, id, "fullSailSkillIncreaseMultiplier", 2.0f, "Leveling", "Exp multiplier for full sail sailing speed", false);

            maxTailwindBoost = new ConfigVariable<float>(config, id, "maxTailwindBoost", .5f, "Limits", "Maximum tailwind boost", false);
            maxForewindDampener = new ConfigVariable<float>(config, id, "maxForewindDampener", .5f, "Limits", "Maximum forewind slowdown force dampener", false);
            maxRudderBoost = new ConfigVariable<float>(config, id, "maxRudderBoost", .5f, "Limits", "Maximum rudder speed boost", false);
            maxDamageReduction = new ConfigVariable<float>(config, id, "maxDamageReduction", .5f, "Limits", "Maximum ship damage reduction", false);
        }
    }
}
