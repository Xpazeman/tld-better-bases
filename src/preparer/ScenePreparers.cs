using System.Collections.Generic;
using Il2Cpp;

using UnityEngine.SceneManagement;

namespace BetterBases.Preparer
{
    internal class ScenePreparers
    {
        private static List<IScenePreparer> preparers = new List<IScenePreparer>();

        static ScenePreparers()
        {
            preparers.Add(new RepairCabinetDoors());
            preparers.Add(new RepairDrawers());
            preparers.Add(new ApplyRepairDefinitions());
        }

        public static void PrepareScene()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            int count = 0;

            BetterBases.Log("Preparing scene...");
            BetterBases.Log("Scene Count: "+ UnityEngine.SceneManagement.SceneManager.sceneCount);
            BetterBases.Log("Scene Loading Count: "+ Il2Cpp.SceneManager.GetInstance().m_LoadingCount);

            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                BetterBases.Log("Preparing scene " + scene.name);

                foreach (IScenePreparer eachScenePreparer in preparers)
                {
                    BetterBases.Log("Running preparer " + eachScenePreparer.GetType().Name);

                    count += eachScenePreparer.PrepareScene(scene);
                }
            }

            stopwatch.Stop();
            BetterBases.Log("Prepared {0} element(s) for scene '{1}' in {2} ms", count, GameManager.m_ActiveScene, stopwatch.ElapsedMilliseconds);
        }
    }
}