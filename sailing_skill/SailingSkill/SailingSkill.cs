﻿// Original GitHub: https://github.com/gaijinx/valheim_mods
// Fork: https://github.com/Emrik-North/Sailing_Skill_Fork

using BepInEx;
using HarmonyLib;
using Pipakin.SkillInjectorMod;
using System;
using UnityEngine;

// Baldur's SteamID: 76561199067080192

namespace SailingSkill
{

    [BepInPlugin("gaijinx.mod.sailing_skill", "SailingSkill", "1.2.2")]
    [BepInDependency("com.pipakin.SkillInjectorMod")]
    public class SailingSkillsPlugin : BaseUnityPlugin
    {
        public const String MOD_ID = "gaijinx.mod.sailing_skill";
        public const int SKILL_TYPE = 1339;
        Harmony harmony;

        private static SailingConfig sailingConfig = new SailingConfig();

        void Awake()
        {

            sailingConfig.InitConfig(MOD_ID, Config);

            harmony = new Harmony(MOD_ID);
            harmony.PatchAll();

            SkillInjector.RegisterNewSkill(SKILL_TYPE, "Sailing", "Describes sailing ability", 1.0f, null, Skills.SkillType.Run);
        }

        public static bool IsPlayerControlling(Ship ship)
        {
            return ship.HaveControllingPlayer() && ship.m_shipControlls.IsLocalUser();
        }


        public static float GetSkillFactorMultiplier(float max)
        {
            return 1.0f + (Player.m_localPlayer.GetSkillFactor((Skills.SkillType)SKILL_TYPE) * max);
        }

        [HarmonyPatch(typeof(Ship), "GetSailForce")]
        static class GetSailForce_Patch
        {
            static void Postfix(Ship __instance, ref Vector3 __result)
            {
                if(IsPlayerControlling(__instance))
                {
                    float degrees = Vector3.Angle(EnvMan.instance.GetWindDir(), __instance.transform.forward);
                    if (degrees < 135f)
                    {
                        // Check if player is Baldur, and apply unique speed increase if so
                        if (Player.m_localPlayer.GetPlayerName() == "Baldur")
                        {
                            // Baldur's boat goes 10% faster compared to other boats
                            __result *= GetSkillFactorMultiplier(sailingConfig.MaxTailwindBoost) + 0.1f;
                        }
                        else
                        {
                            __result *= GetSkillFactorMultiplier(sailingConfig.MaxTailwindBoost);  // Maximum tailwind speed boost, up to 50%
                        }
                    }
                    else
                    {
                        __result *= GetSkillFactorMultiplier(sailingConfig.MaxForewindDampener);  // Maximum forewind speed dampening, up to -50%
                    }
                }
            }
        }

        [HarmonyPatch(typeof(WearNTear), "Damage")]
        static class Damage_Patch
        {
            static void Prefix(WearNTear __instance, ref HitData hit)
            {
                if (__instance.gameObject.GetComponent<Ship>() == null)
                    return;
                if (IsPlayerControlling(__instance.gameObject.GetComponent<Ship>()))
                {
                    // Check if player is Baldur, and apply unique damage reduction if so
                    if (Player.m_localPlayer.GetPlayerName() == "Baldur")
                    {
                        Debug.Log($"Baldur's currently this ship's Captain, so it takes less damage.");
                        // Baldur's boat takes 30% less damage compared to other boats.
                        float baldurMultiplier = GetSkillFactorMultiplier(sailingConfig.MaxDamageReduction) - 0.3f; 
                        MultiplyDamage(ref hit, baldurMultiplier);
                    }
                    else
                    {
                        // up to 50% dmg reduction
                        MultiplyDamage(ref hit, GetSkillFactorMultiplier(sailingConfig.MaxDamageReduction)); 
                    }
                }
            }
        }

        private static void MultiplyDamage(ref HitData hit, float value)
        {
            value = Math.Max(0, value);
            hit.m_damage.m_damage *= value;
            hit.m_damage.m_blunt *= value;
            hit.m_damage.m_slash *= value;
            hit.m_damage.m_pierce *= value;
            hit.m_damage.m_chop *= value;
            hit.m_damage.m_pickaxe *= value;
            hit.m_damage.m_fire *= value;
            hit.m_damage.m_frost *= value;
            hit.m_damage.m_lightning *= value;
            hit.m_damage.m_poison *= value;
            hit.m_damage.m_spirit *= value;
        }


        [HarmonyPatch(typeof(Ship), "FixedUpdate")]
        public static class FixedUpdate_Patch
        {
            private static float m_increase_timer = 0f;
            private static float m_original_backward_force;

            private static void Prefix(ref Ship __instance, ref float ___m_backwardForce)
            {
                if (IsPlayerControlling(__instance)) {
                    Ship.Speed shipSpeed = __instance.GetSpeedSetting();
                    if (shipSpeed == Ship.Speed.Slow || shipSpeed == Ship.Speed.Back)
                    {
                        m_original_backward_force = ___m_backwardForce;
                        ___m_backwardForce *= GetSkillFactorMultiplier(sailingConfig.MaxRudderBoost);
                    }
                }
            }

            private static void Postfix(ref Ship __instance, ref float ___m_backwardForce)
            {
                if (IsPlayerControlling(__instance)) {
                    Ship.Speed shipSpeed = __instance.GetSpeedSetting();
                    if (shipSpeed != Ship.Speed.Stop)
                    {
                        switch (shipSpeed)
                        {
                            case Ship.Speed.Back:
                                m_increase_timer += 1f;
                                break;
                            case Ship.Speed.Slow:
                                m_increase_timer += 1f;
                                break;
                            case Ship.Speed.Half:
                                m_increase_timer += sailingConfig.HalfSailSkillIncreaseMultiplier;
                                break;
                            case Ship.Speed.Full:
                                m_increase_timer += sailingConfig.FullSailSkillIncreaseMultiplier;
                                break;
                        }
                        if (m_increase_timer >= sailingConfig.SkillIncreaseTick)
                        {
                            Player.m_localPlayer.RaiseSkill((Skills.SkillType)SKILL_TYPE, sailingConfig.SkillIncrease);
                            m_increase_timer -= sailingConfig.SkillIncreaseTick;
                        }
                        if (!m_original_backward_force.Equals(null))
                        {
                            ___m_backwardForce = m_original_backward_force;
                        }
                    }
                }
            }
        }
    }

}
