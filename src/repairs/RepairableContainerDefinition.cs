using System;
using System.Collections.Generic;

using UnityEngine;

namespace BetterBases
{
    public struct ObjectDefinition
    {
        public string Path;
        public float[] Position;
    }

    public class RepairableContainerDefinition
    {
        public float[] Reference;
        public ObjectDefinition Target;
        public ObjectDefinition Template;
        public string Type;
    }

    public class RepairableContainerDefinitions
    {
        public Dictionary<String, RepairableContainerDefinition[]> Definitions = new Dictionary<string, RepairableContainerDefinition[]>();

        public RepairableContainerDefinition[] GetDefinitions(string scene)
        {
            RepairableContainerDefinition[] result;
            if (Definitions != null && Definitions.TryGetValue(scene, out result))
            {
                return result;
            }

            return new RepairableContainerDefinition[0];
        }
    }
}