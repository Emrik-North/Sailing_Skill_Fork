﻿using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SailingSkill
{
    class ConfigVariable <T>
    {
        object backingStore;

        public T Value
        {
            get
            {
                return Traverse.Create(backingStore).Property("Value").GetValue<T>();
            }
        }

        public ConfigVariable(ConfigFile config, string id, string varName, T defaultValue, string configSection, string configDescription, bool localOnly)
        {
            backingStore = config.Bind(configSection, varName, defaultValue, configDescription);
        }
    }
}
