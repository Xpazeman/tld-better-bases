using UnityEngine;

namespace BetterBases
{
    public class PathGameObjectSearchFilter : GameObjectSearchFilter
    {
        private string targetPath;
        private Vector3 targetPosition;

        public PathGameObjectSearchFilter(string targetPath, Vector3 targetPosition)
        {
            this.targetPath = targetPath;
            this.targetPosition = targetPosition;
        }

        public override SearchResult Filter(GameObject gameObject)
        {
            string path = BetterBasesUtils.GetPath(gameObject);

            if (!targetPath.StartsWith(path))
            {
                return SearchResult.SKIP_CHILDREN;
            }

            if (targetPath.Equals(path) && Vector3.Distance(gameObject.transform.position, targetPosition) < 0.05f)
            {
                return SearchResult.INCLUDE_SKIP_CHILDREN;
            }

            return SearchResult.CONTINUE;
        }
    }
}