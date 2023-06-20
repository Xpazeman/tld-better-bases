using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using Il2Cpp;

using MelonLoader.TinyJSON;

namespace BetterBases
{
    [HarmonyPatch(typeof(BreakDown), "Deserialize", new Type[] { typeof(string) })]
    internal class BreakDown_Deserialize
    {
        private static bool Prefix(BreakDown __instance, string text)
        {
            if (text == null || !PlaceableFurniture.IsPlacableFurniture(__instance))
            {
                return true;
            }

            ModBreakDownSaveProxy saveTemp = JSON.Load(text).Make<ModBreakDownSaveProxy>();

            ModBreakDownSaveData saveData = new ModBreakDownSaveData();
            saveData.m_Position = BetterBasesUtils.MakeVector3(saveTemp.m_Position);
            if (saveTemp.m_CurrentPosition != null)
                saveData.m_CurrentPosition = BetterBasesUtils.MakeVector3(saveTemp.m_CurrentPosition);

            if (saveTemp.m_Rotation != null)
                saveData.m_Rotation = BetterBasesUtils.MakeVector3(saveTemp.m_Rotation);
            saveData.m_HasBeenBrokenDown = saveTemp.m_HasBeenBrokenDown;
            saveData.m_Guid = saveTemp.m_Guid;

            if (saveData.m_HasBeenBrokenDown)
            {
                return true;
            }

            __instance.gameObject.SetActive(true);
            
            GameObject root = PlaceableFurniture.GetFurnitureRoot(__instance.gameObject);

            if (root == null)
            {
                return true;
            }

            MoveableObject mObject = __instance.gameObject.GetComponent<MoveableObject>();
            if (mObject != null && saveData.m_CurrentPosition != null && saveData.m_CurrentPosition != Vector3.zero)
            {
                if (saveData.m_CurrentPosition != saveData.m_Position)
                {
                    root.transform.parent = null;
                    PlaceableFurniture.PreparePlacableFurniture(__instance.gameObject);
                }

                root.transform.position = saveData.m_CurrentPosition;

            }
            else
            {
                PlaceableFurniture.PreparePlacableFurniture(__instance.gameObject);
                root.transform.position = saveData.m_Position;
            }            

            if (saveTemp.m_Rotation != null)
                root.transform.rotation = Quaternion.Euler(saveData.m_Rotation);

            return false;
        }
    }

    [HarmonyPatch(typeof(BreakDown), "Serialize")]
    internal class BreakDown_Serialize
    {
        public static bool Prefix(BreakDown __instance, ref string __result)
        {
            if (!PlaceableFurniture.IsPlacableFurniture(__instance))
            {
                return true;
            }

            ModBreakDownSaveData saveData = new ModBreakDownSaveData();
            
            saveData.m_Rotation = __instance.transform.rotation.eulerAngles;
            saveData.m_HasBeenBrokenDown = !__instance.gameObject.activeSelf;
            saveData.m_Guid = ObjectGuid.MaybeGetGuidFromGameObject(__instance.gameObject);

            MoveableObject mObject = __instance.gameObject.GetComponent<MoveableObject>();
            if (mObject != null)
            {
                saveData.m_Position = mObject.m_OriginalPosition;
                saveData.m_CurrentPosition = __instance.transform.position;
            }
            else
            {
                saveData.m_Position = __instance.transform.position;
                saveData.m_CurrentPosition = __instance.transform.position;
            }

            ModBreakDownSaveProxy saveTemp = new ModBreakDownSaveProxy();
            saveTemp.m_Position = BetterBasesUtils.MakeArrayFromVector3(saveData.m_Position);
            saveTemp.m_CurrentPosition = BetterBasesUtils.MakeArrayFromVector3(saveData.m_CurrentPosition);
            saveTemp.m_Rotation = BetterBasesUtils.MakeArrayFromVector3(saveData.m_Rotation);
            saveTemp.m_HasBeenBrokenDown = saveData.m_HasBeenBrokenDown;
            saveTemp.m_Guid = saveData.m_Guid;

            __result = JSON.Dump(saveTemp);

            return false;
        }
    }

    [HarmonyPatch(typeof(GameManager), "UpdateNotPaused")]
    internal class GameManager_UpdateNotPaused
    {
        private static void Prefix(GameManager __instance)
        {
            if (BetterBases.placingItems == null)
            {
                return;
            }

            foreach(GameObject bdObj in BetterBases.placingItems)
            {
                if (bdObj == null) continue;

                MeshRenderer[] meshRenderers = bdObj.GetComponentsInChildren<MeshRenderer>();

                foreach (MeshRenderer rend in meshRenderers)
                {
                    if (!rend.enabled)
                    {
                        rend.enabled = true;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerManager), "CleanUpPlaceMesh")]
    internal class PlayerManager_CleanUpPlaceMesh
    {
        private static void Prefix(PlayerManager __instance)
        {
            var gameObject = __instance.GetObjectToPlace();
            if (PlaceableFurniture.IsPlaceableFurniture(gameObject))
            {
                PlaceableFurniture.AddFurnitureToPhysicalCollisionMask();
                PlaceableFurniture.RestoreFurnitureLayers(gameObject);
                
                
            }

            InterfaceManager.GetPanel<Panel_ActionsRadial>().DisableRadial(false);
        }
    }

    [HarmonyPatch(typeof(InputManager), nameof(InputManager.ExecuteAltFire))]
    internal class InputManager_ExecuteAltFire
    {
        public static bool Prefix(InputManager __instance)
        {
            PlayerManager pm = GameManager.GetPlayerManagerComponent();

            GameObject gameObject = pm.GetInteractiveObjectUnderCrosshairs(5);

            if (PlaceableFurniture.IsPlaceableFurniture(gameObject))
            {
                PlaceableFurniture.PreparePlacableFurniture(gameObject);

                if (!PlaceableFurniture.IsFurnitureEmpty(gameObject))
                {
                    HUDMessage.AddMessage("Furniture isn't empty");
                    return false;
                }

                GameObject root = PlaceableFurniture.GetFurnitureRoot(gameObject);

                if (root != null)
                {
                    pm.StartPlaceMesh(root, 5f, PlaceMeshFlags.None);
                    return false;
                }
                else
                {
                    pm.StartPlaceMesh(gameObject, 5f, PlaceMeshFlags.None);
                    return false;
                }


            }

            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerManager), "StartPlaceMesh", new Type[] { typeof(GameObject), typeof(float), typeof(PlaceMeshFlags) })]
    internal class PlayerManager_StartPlaceMesh
    {
        private static void Postfix(PlayerManager __instance, GameObject objectToPlace, bool __result)
        {
            if (__result)
            {
                if (PlaceableFurniture.IsPlaceableFurniture(objectToPlace))
                {
                    vp_Layer.Set(objectToPlace, vp_Layer.NPC, true);

                    if (Settings.options.dropOnPickup)
                    {
                        PlaceableFurniture.DropObjects(objectToPlace);
                    }

                    PlaceableFurniture.furnitureYOffset = 0;
                    List<Mesh> meshes = PlaceableFurniture.GetAllMeshes(objectToPlace);
                    List<Vector3> vertices = new List<Vector3>();

                    foreach (Mesh mesh in meshes)
                    {
                        if (mesh == null)
                        {
                            continue;
                        }

                        if (mesh.isReadable)
                        {
                            foreach (Vector3 v in mesh.vertices)
                            {
                                vertices.Add(v);
                            }
                        }
                    }

                    float lowest = float.PositiveInfinity;
                    int i = 0;

                    foreach (Vector3 position in vertices)
                    {
                        Vector3 point = objectToPlace.transform.TransformPoint(position);
                        if (point.y < lowest)
                        {
                            lowest = point.y;
                        }
                    }

                    if (!float.IsPositiveInfinity(lowest))
                    {
                        PlaceableFurniture.furnitureYOffset = objectToPlace.transform.position.y - lowest;
                    }
                }
            }
        }

        private static bool Prefix(PlayerManager __instance, GameObject objectToPlace, ref bool __result)
        {
            InterfaceManager.GetPanel<Panel_ActionsRadial>().DisableRadial(true);
            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerManager), "DoPositionCheck")]
    internal class PlayerManager_DoPositionCheck
    {
        public static void Prefix(PlayerManager __instance)
        {
            var gameObject = __instance.GetObjectToPlace();

            if (PlaceableFurniture.IsPlaceableFurniture(gameObject))
            {
                //PlaceableFurniture.RemoveFurnitureFromPhysicalCollisionMask();
            }
        }

        private static void Postfix(PlayerManager __instance, ref MeshLocationCategory __result)
        {
            GameObject gameObject = __instance.GetObjectToPlace();

            if (PlaceableFurniture.IsPlaceableFurniture(gameObject))
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + PlaceableFurniture.furnitureYOffset, gameObject.transform.position.z);
            }
        }
    }

    [HarmonyPatch(typeof(PlayerManager), "ObjectToPlaceOverlapsWithObjectsThatBlockPlacement")]
    internal class PlayerManager_ObjectToPlaceOverlapsWithObjectsThatBlockPlacement
    {
        public static void Postfix(PlayerManager __instance, ref Collider __result)
        {
            PlaceableFurniture.RemoveNpcFromPhysiclaCollisionMask();
        }

        public static bool Prefix(PlayerManager __instance, ref Collider __result)
        {
            PlaceableFurniture.AddNpcToPhysicalCollisionMask();

            var gameObject = __instance.GetObjectToPlace();
            if (!PlaceableFurniture.IsPlaceableFurniture(gameObject))
            {
                return true;
            }

            __result = null;
            return false;
        }
    }
}
