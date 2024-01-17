using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Il2Cpp;
using UnityEngine;

namespace BetterBases
{
    internal class PlaceableFurniture
    {
        private static AssetBundle assetBundle;

        public static float furnitureYOffset;

        private static Dictionary<string, string> prefabNames = new Dictionary<string, string>();

        internal static void Initialize()
        {
            MemoryStream memoryStream;

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BetterBases.resources.better-bases.unity3d"))
            {
                memoryStream = new MemoryStream((int)stream.Length);
                stream.CopyTo(memoryStream);
            }
            if (memoryStream.Length == 0)
            {
                throw new System.Exception("No data loaded!");
            }
            assetBundle = AssetBundle.LoadFromMemory(memoryStream.ToArray());

            foreach (var eachAssetName in assetBundle.GetAllAssetNames())
            {
                prefabNames.Add(GetAssetName(eachAssetName), eachAssetName);
            }
        }

        internal static GameObject GetPrefab(string gameObjectName)
        {
            string cleanName = gameObjectName.Replace("_LOD0", "");
            cleanName = cleanName.Replace("_Shadow", "");
            cleanName = cleanName.Replace("_Prefab", "");

            foreach (string eachPrefabName in prefabNames.Keys)
            {
                if (cleanName.ToLower() == eachPrefabName.ToLower())
                {
                    return assetBundle.LoadAsset<GameObject>(prefabNames[eachPrefabName]);
                }
            }

            foreach (string eachPrefabName in prefabNames.Keys)
            {
                if (gameObjectName.ToLower().StartsWith(eachPrefabName.ToLower()))
                {
                    return assetBundle.LoadAsset<GameObject>(prefabNames[eachPrefabName]);
                }
            }

            return null;
        }

        internal static bool HasPrefab(string gameObjectName)
        {
            string cleanName = gameObjectName.Replace("_LOD0", "");
            cleanName = cleanName.Replace("_Shadow", "");
            cleanName = cleanName.Replace("_Prefab", "");

            foreach (string eachPrefabName in prefabNames.Keys)
            {
                if (cleanName.ToLower() == eachPrefabName.ToLower())
                {
                    return true;
                }
            }

            foreach (string eachPrefabName in prefabNames.Keys)
            {
                if (gameObjectName.ToLower().StartsWith(eachPrefabName.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }        

        private static string GetAssetName(string assetPath)
        {
            string result = assetPath;

            int index = assetPath.LastIndexOf('/');
            if (index != -1)
            {
                result = result.Substring(index + 1);
            }

            index = result.LastIndexOf('.');
            if (index != -1)
            {
                result = result.Substring(0, index);
            }

            return result;
        }

        internal static bool IsPlacableFurniture(BreakDown breakDown)
        {
            return IsPlaceableFurniture(breakDown == null ? null : breakDown.gameObject);
        }

        internal static bool IsPlaceableFurniture(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return false;
            }

            if (Settings.options.disableItemMove)
            {
                return false;
            }

            if (IsSmallItem(gameObject) && Settings.options.disableSmallItems)
            {
                return false;
            }

            //Also check if has breakdown
            if ((gameObject.GetComponentInChildren<Renderer>() != null && !gameObject.GetComponentInChildren<Renderer>().isPartOfStaticBatch) && (gameObject.GetComponent<BreakDown>() != null || gameObject.GetComponentInChildren<BreakDown>() != null))
            {
                return true;
            }

            return PlaceableFurniture.GetPrefab(gameObject.name) != null;
        }

        internal static GameObject GetFurnitureRoot(GameObject gameObject)
        {
            if (gameObject.GetComponent<LODGroup>() != null)
            {
                return gameObject;
            }

            if (gameObject.transform.parent)
            {
                return GetFurnitureRoot(gameObject.transform.parent.gameObject);
            }
            else
            {
                return null;
            }
        }

        internal static void DropObjects(GameObject gameObject)
        {
            BreakDown bdComp = gameObject.GetComponent<BreakDown>();

            if (bdComp != null)
            {
                bdComp.StickSurfaceObjectsToGround(false);
            }
            else
            {
                Transform[] directChildren = new Transform[gameObject.transform.childCount];
                for (int i = 0; i < gameObject.transform.childCount; ++i)
                {
                    directChildren[i] = gameObject.transform.GetChild(i);
                }

                foreach(Transform child in directChildren)
                {
                    BreakDown chBreakDown = child.gameObject.GetComponent<BreakDown>();

                    if (chBreakDown != null)
                    {
                        chBreakDown.StickSurfaceObjectsToGround(false);
                    }
                }
            }
        }

        internal static bool IsSubItem(GameObject gameObject)
        {
            foreach(GameObject go in BetterBases.subItems)
            {
                if (go == gameObject)
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool IsSmallItem(GameObject gameObject)
        {
            if (BetterBases.smallItems == null)
                return false;

            foreach (GameObject go in BetterBases.smallItems)
            {
                if (go == gameObject)
                {
                    return true;
                }
            }

            return false;
        }

        internal static void PreparePlacableFurniture(GameObject gameObject)
        {
            if (gameObject.GetComponentInChildren<Renderer>() == null || !gameObject.GetComponentInChildren<Renderer>().isPartOfStaticBatch)
            {
                return;
            }

            if (IsSmallItem(gameObject) && Settings.options.disableSmallItems)
            {
                return;
            }

            GameObject prefab = PlaceableFurniture.GetPrefab(gameObject.name);
            if (prefab == null)
            {
                return;
            }

            MeshFilter templateMeshFilter = prefab.GetComponentInChildren<MeshFilter>();
            if (templateMeshFilter == null) { return; }

            MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();

            foreach (MeshFilter eachMeshFilter in meshFilters)
            {
                if (!eachMeshFilter.name.ToLower().StartsWith(templateMeshFilter.name.ToLower()))
                {
                    GameObject innerObject = GetFurnitureRoot(eachMeshFilter.gameObject);
                    if (innerObject == null)
                    {
                        continue;
                    }

                    BreakDown innerBD = innerObject.GetComponent<BreakDown>();
                    
                    if (innerBD != null)
                    {
                        if (!BetterBases.placingItems.Contains(innerObject)) {
                            BetterBases.placingItems.Add(innerObject);
                        }
                        BetterBases.subItems.Add(innerObject);
                    }

                    if (innerBD == null || Settings.options.autoDeclutter)
                    {
                        eachMeshFilter.gameObject.SetActive(false);
                    }

                    continue;
                }

                eachMeshFilter.mesh = templateMeshFilter.mesh;
            }

            BoxCollider[] templateBoxColliders = prefab.GetComponentsInChildren<BoxCollider>();
            foreach (BoxCollider eachTemplateBoxCollider in templateBoxColliders)
            {
                BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
                boxCollider.center = eachTemplateBoxCollider.center;
                boxCollider.size = eachTemplateBoxCollider.size;
                boxCollider.material = eachTemplateBoxCollider.material;
            }

            gameObject.transform.localPosition = new Vector3(0, 0, 0);

            if (!BetterBases.placingItems.Contains(gameObject))
            {
                BetterBases.placingItems.Add(gameObject);
            }
        }

        internal static bool IsFurnitureEmpty(GameObject gameObject)
        {
            GameObject prefab = PlaceableFurniture.GetPrefab(gameObject.name);
            if (prefab == null)
            {
                return true;
            }

            MeshFilter templateMeshFilter = prefab.GetComponentInChildren<MeshFilter>();
            MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter eachMeshFilter in meshFilters)
            {
                if (!eachMeshFilter.name.ToLower().StartsWith(templateMeshFilter.name.ToLower()))
                {
                    return false;
                }
            }

            return true;
        }

        internal static void AddFurnitureToPhysicalCollisionMask()
        {
            Utils.m_PhysicalCollisionLayerMask |= 1 << vp_Layer.InteractiveProp;
        }

        internal static void RemoveFurnitureFromPhysicalCollisionMask()
        {
            Utils.m_PhysicalCollisionLayerMask &= ~(1 << vp_Layer.InteractiveProp);
        }

        internal static void AddNpcToPhysicalCollisionMask()
        {
            Utils.m_PhysicalCollisionLayerMask |= 1 << vp_Layer.NPC;
        }

        internal static void RemoveNpcFromPhysiclaCollisionMask()
        {
            Utils.m_PhysicalCollisionLayerMask &= ~(1 << vp_Layer.NPC);
        }

        internal static void RestoreFurnitureLayers(GameObject furniture)
        {
            GameObject root = GetFurnitureRoot(furniture);

            vp_Layer.Set(furniture, vp_Layer.Default, true);

            if (furniture.GetComponent<BreakDown>() != null)
            {
                vp_Layer.Set(furniture, vp_Layer.InteractiveProp);
            }

            if (furniture.GetComponent<Collider>() != null)
            {
                vp_Layer.Set(furniture, vp_Layer.InteractiveProp);
            }

            BreakDown[] breakDownItems = furniture.GetComponentsInChildren<BreakDown>();

            foreach (BreakDown breakDown in breakDownItems)
            {
                if (breakDown != null)
                {
                    vp_Layer.Set(breakDown.gameObject, vp_Layer.InteractiveProp);
                }
            }

            Collider[] colliderItems = furniture.GetComponentsInChildren<Collider>();

            foreach (Collider collider in colliderItems)
            {
                if (collider != null)
                {
                    vp_Layer.Set(collider.gameObject, vp_Layer.InteractiveProp);
                }
            }
        }

        public static List<Mesh> GetAllMeshes(GameObject go)
        {
            List<Mesh> meshes = new List<Mesh>();

            MeshFilter[] meshComponents = go.GetComponentsInChildren<MeshFilter>();

            foreach (MeshFilter meshInChildren in meshComponents)
            {
                if (meshInChildren != null)
                {
                    if (meshInChildren.mesh != null)
                    {
                        meshes.Add(meshInChildren.mesh);
                    }
                    if (meshInChildren.sharedMesh != null)
                    {

                        meshes.Add(meshInChildren.sharedMesh);
                    }
                }
            }
            SkinnedMeshRenderer[] skinnedMeshComponents = go.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer componentInChildren2 in skinnedMeshComponents)
            {
                if (componentInChildren2 != null)
                {
                    meshes.Add(componentInChildren2.sharedMesh);
                }
            }

            return meshes;
        }
    }
}
