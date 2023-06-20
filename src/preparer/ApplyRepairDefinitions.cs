using UnityEngine;
using UnityEngine.SceneManagement;
using Il2Cpp;

using MelonLoader.TinyJSON;

namespace BetterBases.Preparer
{
    internal class ApplyRepairDefinitions : IScenePreparer
    {
        private RepairableContainerDefinitions definitions;

        internal ApplyRepairDefinitions()
        {
            LoadDefinitions();
        }

        public int PrepareScene(Scene scene)
        {
            int count = 0;

            RepairableContainerDefinition[] repairableContainerDefinition = definitions.GetDefinitions(scene.name);

            foreach (RepairableContainerDefinition eachDefinition in repairableContainerDefinition)
            {
                GameObject target = BetterBasesUtils.FindGameObject(eachDefinition.Target.Path, BetterBasesUtils.MakeVector3(eachDefinition.Target.Position));
                if (target == null)
                {
                    BetterBases.Log("Could not find target of definition for " + eachDefinition.Target.Path + " @" + BetterBasesUtils.MakeVector3(eachDefinition.Target.Position).ToString());
                    continue;
                }

                GameObject template = BetterBasesUtils.FindGameObject(eachDefinition.Template.Path, BetterBasesUtils.MakeVector3(eachDefinition.Template.Position));
                if (template == null)
                {
                    BetterBases.Log("Could not find template of definition for " + eachDefinition.Target.Path + " @" + BetterBasesUtils.MakeVector3(eachDefinition.Target.Position).ToString());
                    continue;
                }

                Container container = template.GetComponent<Container>();
                if (container == null)
                {
                    BetterBases.Log("Could not find container of definition for " + eachDefinition.Target.Path + " @" + BetterBasesUtils.MakeVector3(eachDefinition.Target.Position).ToString());
                    continue;
                }

                if ("CabinetDoor" == eachDefinition.Type)
                {
                    if (RepairCabinetDoors.Prepare(target, container, BetterBasesUtils.MakeVector3(eachDefinition.Reference)))
                    {
                        count++;
                    }
                }
                else if ("Drawer" == eachDefinition.Type)
                {
                    if (RepairDrawers.Prepare(target, container, BetterBasesUtils.MakeVector3(eachDefinition.Reference))) {
                        count++;
                    }
                }
                else
                {
                    BetterBases.Log("Unsupported type '" + eachDefinition.Type + "'");
                }
            }

            return count;
        }

        internal void LoadDefinitions()
        {
            System.IO.StreamReader streamReader = new System.IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("BetterBases.resources.repairable-container-definitions.json"));
            string json = streamReader.ReadToEnd();
            streamReader.Close();

            definitions = JSON.Load(json).Make<RepairableContainerDefinitions>();

            if (definitions == null)
            {
                definitions = new RepairableContainerDefinitions();
            }
        }
    }
}