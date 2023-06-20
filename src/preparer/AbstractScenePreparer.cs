using UnityEngine;
using UnityEngine.SceneManagement;

namespace BetterBases.Preparer
{
    internal abstract class AbstractScenePreparer : IScenePreparer
    {
        protected abstract bool Disabled { get; }

        public int PrepareScene(Scene scene)
        {
            if (Disabled)
            {
                return 0;
            }

            int count = 0;

            foreach (GameObject eachGameObject in BetterBasesUtils.GetSceneObjects(scene, new ScenePreparerFilter(Accept)))
            {
                if (Prepare(eachGameObject))
                {
                    count++;
                }
            }

            return count;
        }

        protected abstract SearchResult Accept(GameObject gameObject);

        protected abstract bool Prepare(GameObject gameObject);
    }

    internal class ScenePreparerFilter : GameObjectSearchFilter
    {
        private readonly System.Func<GameObject, SearchResult> filterMethod;

        internal ScenePreparerFilter(System.Func<GameObject, SearchResult> filterMethod)
        {
            this.filterMethod = filterMethod;
        }

        public override SearchResult Filter(GameObject gameObject)
        {
            return filterMethod(gameObject);
        }
    }
}