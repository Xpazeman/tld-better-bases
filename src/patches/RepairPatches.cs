using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using Il2Cpp;
using MelonLoader;

namespace BetterBases
{
    [HarmonyPatch(typeof(GameManager), "InstantiatePlayerObject")]
    internal class GameManager_InstantiatePlayerObject
    {
        internal static void Prefix()
        {
            BetterBases.SceneReady();
        }
    }

    [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.UpdateHUDText), new Type[] { typeof(Panel_HUD) })]
    internal class PlayerManager_UpdateHUDText
    {
        public static void Postfix(PlayerManager __instance, Panel_HUD hud)
        {
            GameObject? interactiveObject = null;
            string hoverText = "";

            try
            {
                interactiveObject = __instance.GetInteractiveObjectUnderCrosshairs(5);
            }
            catch
            {
            
            }

            if (interactiveObject == null)
            {
                return;
            }

            RepairableContainer repairable = interactiveObject.GetComponent<RepairableContainer>();
            if (repairable != null)
            {
                hoverText = repairable.GetInteractiveObjectDisplayText();
                hud.SetHoverText(hoverText, interactiveObject, HoverTextState.CanInteract);
            }

            
        }
    }

    [HarmonyPatch(typeof(PlayerManager), "InteractiveObjectsProcessInteraction")]
    internal class PlayerManager_InteractiveObjectsProcessInteraction
    {
        public static bool Prefix(PlayerManager __instance, ref bool __result)
        {
            if (__instance.m_PickupGearItem || GameManager.GetPlayerAnimationComponent().GetState() == PlayerAnimation.State.Throwing || GameManager.GetPlayerManagerComponent().GetControlMode() == PlayerControlMode.InConversation)
            {
                return true;
            }

            GameObject interactiveObject = __instance.GetInteractiveObjectUnderCrosshairs(100);

            if (interactiveObject == null)
            {
                return true;
            }

            BetterBases.Log("Checking if repairable");
            RepairableContainer repairable = interactiveObject.GetComponent<RepairableContainer>();
            if (repairable != null)
            {
                BetterBases.Log("Is repairable");
                __result = repairable.ProcessInteraction();
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(SaveGameSystem), "LoadSceneData", new Type[] { typeof(string), typeof(string) })]
    internal class SaveGameSystem_LoadSceneData
    {
        public static void Prefix(SaveGameSystem __instance, string name, string sceneSaveName)
        {
            if (BetterBases.placingItems != null && BetterBases.placingItems.Count > 0)
            {
                BetterBases.placingItems.Clear();
            }
            else
            {
                BetterBases.placingItems = new List<GameObject>();
            }

            if (BetterBases.subItems != null && BetterBases.subItems.Count > 0)
            {
                BetterBases.subItems.Clear();
            }
            else
            {
                BetterBases.subItems = new List<GameObject>();
            }

            if (BetterBases.smallItems != null && BetterBases.smallItems.Count > 0)
            {
                BetterBases.smallItems.Clear();
            }
            else
            {
                BetterBases.smallItems = new List<GameObject>();
            }

            if (!Settings.options.disableRepairs)
            {
                RepairManager.LoadRepairs(name, sceneSaveName);
            }

            BetterBases.staticRoot = new GameObject();
            BetterBases.staticRoot.name = "BetterBasesRoot";

            if (!Settings.options.disableRemoveClutter)
            {

                if (InterfaceManager.IsMainMenuEnabled() || (GameManager.IsOutDoorsScene(GameManager.m_ActiveScene) && !RemoveClutter.notReallyOutdoors.Contains(GameManager.m_ActiveScene)))
                {
                    //Deactivate remove clutter when outside
                    if (Settings.options.disableOutsideClutter)
                    {
                        return;
                    }
                }

                RemoveClutter.LoadBreakDownData(name, sceneSaveName);
            }
        }
    }

    [HarmonyPatch(typeof(SaveGameSystem), "SaveSceneData")]
    internal class SaveGameSystem_SaveSceneData
    {
        //public static void Prefix(SaveSlotType gameMode, string name, string sceneSaveName)
        public static void Prefix(SlotData slot, string sceneSaveName)
        {
            RepairManager.SaveRepairs(slot, sceneSaveName);
            RemoveClutter.SaveBreakDownData(slot, sceneSaveName);
        }
    }

    [HarmonyPatch(typeof(BreakDown), "SerializeAll")]
    internal class BreakDown_SerializeAll
    {
        public static void Prefix(BreakDown __instance)
        {
            Il2CppSystem.Collections.Generic.List<BreakDown> okItems = new Il2CppSystem.Collections.Generic.List<BreakDown>();

            for (int i = 0; i < BreakDown.m_BreakDownObjects.Count; i++)
            {
                if (PlaceableFurniture.IsPlacableFurniture(__instance))
                {
                    continue;
                }

                BreakDown breakDown = BreakDown.m_BreakDownObjects[i];
                if (breakDown != null)
                {
                    okItems.Add(breakDown);
                }
            }

            BreakDown.m_BreakDownObjects = okItems;
        }
    }
}