using UnityEngine;
using Il2Cpp;
using static BetterBases.BetterBasesUtils;

namespace BetterBases.Preparer
{
    internal class RepairCabinetDoors : AbstractScenePreparer
    {
        protected override bool Disabled => false;

        internal static bool Prepare(GameObject gameObject, Container template, Vector3 referencePoint)
        {
            if (gameObject.GetComponent<RepairableContainer>() != null)
            {
                return false;
            }

            RepairableContainer repairable = gameObject.AddComponent<RepairableContainer>();
            repairable.Template = GetParent(template);
            repairable.ParentContainer = GetParent(repairable.Template);
            repairable.TargetPosition = RepairableContainer.GetTargetPosition(repairable.ParentContainer, referencePoint);
            repairable.TargetRotation = repairable.Template.transform.localRotation;
            repairable.RequiresTools = true;

            if (gameObject.GetComponent<Collider>() == null)
            {
                gameObject.AddComponent<BoxCollider>();
            }

            gameObject.layer = vp_Layer.Container;

            return true;
        }

        protected override bool Prepare(GameObject gameObject)
        {
            Container template = BetterBasesUtils.FindContainerTemplate(gameObject);
            if (template == null)
            {
                return false;
            }

            return Prepare(gameObject, template, gameObject.transform.localPosition);
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

            if (gameObject.GetComponent<Container>() != null)
            {
                return SearchResult.SKIP_CHILDREN;
            }

            if (gameObject.name.StartsWith("OBJ_KitchenCabinetDoor"))
            {
                return SearchResult.INCLUDE_SKIP_CHILDREN;
            }

            return SearchResult.CONTINUE;
        }
    }
}