﻿using Simple_Game_Framework.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Simple_Game_Framework.Creatures
{
    public static class CreatureConfigLoader
    {
        public static Dictionary<int, Dictionary<string, CreatureConfig>> LoadCreatureConfig(string filePath)
        {
            var creatureConfigs = new Dictionary<int, Dictionary<string, CreatureConfig>>();

            var document = XDocument.Load(filePath);

            foreach (var creatureElement in document.Descendants("Creature"))
            {
                // Get creature ID
                var idAttribute = creatureElement.Attribute("id");
                if (idAttribute == null || !int.TryParse(idAttribute.Value, out int id)) continue;

                // Get creature type
                var typeAttribute = creatureElement.Attribute("type");
                if (typeAttribute == null || !Enum.TryParse(typeAttribute.Value, out CreatureType creatureType)) continue;

                // difficulty levels
                var difficultyConfigs = new Dictionary<string, CreatureConfig>();
                foreach (var difficultyElement in creatureElement.Descendants("Difficulty"))
                {
                    var levelAttribute = difficultyElement.Attribute("level");
                    if (levelAttribute == null) continue;

                    string difficultyLevel = levelAttribute.Value;

                    // base health for chosen difficulty
                    var baseHealthElement = difficultyElement.Element("BaseHealth");
                    if (baseHealthElement == null || !int.TryParse(baseHealthElement.Attribute("value")?.Value, out int baseHealth)) continue;

                    // damage values for current difficulty
                    var damageValues = new Dictionary<DamageType, int>();
                    foreach (var damageElement in difficultyElement.Descendants("Damage"))
                    {
                        var damageTypeAttribute = damageElement.Attribute("type");
                        var damageValueAttribute = damageElement.Attribute("value");

                        if (damageTypeAttribute != null && damageValueAttribute != null &&
                            Enum.TryParse(damageTypeAttribute.Value, out DamageType damageType) &&
                            int.TryParse(damageValueAttribute.Value, out int damageValue))
                        {
                            damageValues[damageType] = damageValue;
                        }
                    }

                    // Add difficulty configuration
                    difficultyConfigs[difficultyLevel] = new CreatureConfig
                    {
                        CreatureType = creatureType,
                        BaseHealth = baseHealth,
                        BaseDamageValues = damageValues
                    };
                }

                // Finally add creature configuration for all difficulty levels
                creatureConfigs[id] = difficultyConfigs;
            }

            return creatureConfigs;
        }

        public static CreatureConfig GetCreatureConfig(int creatureId, string difficultyLevel, Dictionary<int, Dictionary<string, CreatureConfig>> creatureConfigs)
        {
            if (creatureConfigs.TryGetValue(creatureId, out var difficultyConfigs) &&
                difficultyConfigs.TryGetValue(difficultyLevel, out var config))
            {
                return config;
            }

            throw new Exception($"Configuration for creature ID {creatureId} with difficulty '{difficultyLevel}' not found.");
        }
    }

    // Helper class to store creature configuration
    public class CreatureConfig
    {
        public CreatureType CreatureType { get; set; }
        public int BaseHealth { get; set; }
        public Dictionary<DamageType, int> BaseDamageValues { get; set; }
    }
}
