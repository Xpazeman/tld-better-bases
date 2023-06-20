using UnityEngine;
using Il2Cpp;

using static BetterBases.BetterBasesUtils;

namespace BetterBases.Preparer
{
    internal class RepairDrawers : AbstractScenePreparer
    {
        protected override bool Disabled => false;

        protected override bool Prepare(GameObject gameObject)
        {
            Container template = BetterBasesUtils.FindContainerTemplate(gameObject);
            if (template == null)
            {
                return false;
            }

            return Prepare(gameObject, template, gameObject.transform.localPosition);
        }

        internal static bool Prepare(GameObject gameObject, Container template, Vector3 referencePoint)
        {
            if (gameObject.GetComponent<RepairableContainer>() != null)
            {
                return false;
            }

            RepairableContainer repairable = GetOrCreateComponent<RepairableContainer>(gameObject);
            repairable.Template = template.gameObject;
            repairable.ParentContainer = GetParent(repairable.Template);
            repairable.TargetPosition = RepairableContainer.GetTargetPosition(repairable.ParentContainer, referencePoint);
            repairable.TargetRotation = Quaternion.identity;
            repairable.RequiresTools = gameObject.CompareTag("Metal");

            GetOrCreateComponent<BoxCollider>(gameObject);

            gameObject.layer = vp_Layer.Container;

            return true;
        }
        protected override SearchResult Accept(GameObject gameObject)
        {
            if (gameObject.layer == vp_Layer.Gear)
            {
                return SearchResult.SKIP_CHILDREN;
            }

            if ((gameObject.GetComponent<Renderer>()?.isPartOfStaticBatch).GetValueOrDefault(false))
            {
                return SearchResult.CONTINUE;
            }

            Container container = gameObject.GetComponent<Container>();
            if (container != null && container.enabled)
            {
                return SearchResult.SKIP_CHILDREN;
            }

            if (gameObject.name.StartsWith("OBJ_DresserDrawer") || gameObject.name.StartsWith("OBJ_DresserTallDrawer") || gameObject.name.StartsWith("OBJ_MetalFileCabinetDrawer"))
            {
                return SearchResult.INCLUDE_SKIP_CHILDREN;
            }

            return SearchResult.CONTINUE;
        }
    }
}