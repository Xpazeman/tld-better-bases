using UnityEngine;
using Il2Cpp;

using static BetterBases.BetterBasesUtils;

using MelonLoader.TinyJSON;

namespace BetterBases
{
    public struct SaveProxy
    {
        public string data;
    }

    public class RepairManager
    {
        private const string REPAIRED_CONTAINERS_SUFFIX = "_RepairedContainers";

        private static RepairedContainers repairedContainers = new RepairedContainers();

        internal static void AddRepairedContainer(string guid, GameObject repairableContainer, string scene)
        {
            RepairedContainer repairedContainer = new RepairedContainer(scene, GetPath(repairableContainer), repairableContainer.transform.position, guid);
            repairedContainers.AddRepairedContainer(repairedContainer);
        }

        internal static void LoadRepairs(string saveName, string sceneSaveName)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            repairedContainers.Clear();
            string saveProxyData = BetterBases.dataMngr.Load(sceneSaveName + REPAIRED_CONTAINERS_SUFFIX);
			if(string.IsNullOrEmpty(saveProxyData))
			{
				return;
			}

            RepairedContainers loadedRepairedContainers = DeserializeSaveProxy(saveProxyData);
            if (loadedRepairedContainers == null)
            {
                loadedRepairedContainers = new RepairedContainers();
            }

            foreach (RepairedContainer eachRepairedContainer in loadedRepairedContainers.containers)
            {
                if (eachRepairedContainer.scene == GameManager.m_ActiveScene)
                {
                    RestoreRepairedContainer(eachRepairedContainer);
                }
            }

            stopwatch.Stop();

            BetterBases.Log("Loaded " + loadedRepairedContainers.containers.Count + " repair(s) for scene '" + GameManager.m_ActiveScene + "' in " + stopwatch.ElapsedMilliseconds + " ms");
        }

        internal static void SaveRepairs(SlotData slotData, string sceneSaveName)
        {
            
            SaveProxy saveProxy = new SaveProxy
            {
                data = JSON.Dump(repairedContainers)
            };

            string saveProxyData = JSON.Dump(saveProxy);

            BetterBases.dataMngr.Save(saveProxyData, sceneSaveName + REPAIRED_CONTAINERS_SUFFIX);
        }

        private static void RestoreRepairedContainer(RepairedContainer repairedContainer)
        {
            GameObject target = FindGameObject(repairedContainer.path, repairedContainer.position);
            if (target == null)
            {
                return;
            }

            RepairableContainer repairableContainer = target.GetComponentInChildren<RepairableContainer>();
            if (repairableContainer != null && !repairableContainer.Applied)
            {
                BetterBases.Log("Apply Repair to " + repairableContainer.gameObject.name);

                repairableContainer.ContainerGuid = repairedContainer.guid;
                repairableContainer.Repair();
            }
        }
    }
}