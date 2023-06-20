using System.Reflection;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MelonLoader;
using ModData;


using BetterBases.Preparer;
using MelonLoader.Utils;

namespace BetterBases
{
    public class BetterBases : MelonMod
    {
        public const string NAME = "Better Bases";
        
        public static readonly string MOD_ASSETS_PATH = Path.Combine(MelonEnvironment.ModsDirectory, "better-bases");
        public static readonly string MOD_DATA_PATH = Path.Combine(MelonEnvironment.ModsDirectory, "mod-data");
        public static readonly string BB_DATA_PATH = Path.Combine(MOD_DATA_PATH, "better-bases");

        public static List<GameObject> placingItems = null;
        public static List<GameObject> subItems = null;
        public static List<GameObject> smallItems = null;
        public static bool isPlacingDynamic = false;

        public static GameObject staticRoot = null;

        public static ModDataManager dataMngr = null;

        public override void OnInitializeMelon()
        {
            Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp<RepairableContainer>();
            Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp<MoveableObject>();
            Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp<ChangeLayer>();

            //Load options
            Settings.OnLoad();

            //Init modules
            dataMngr = new ModDataManager("better_bases");

            RemoveClutter.LoadBreakDownDefinitions();
            PlaceableFurniture.Initialize();
        }

        public static void SceneReady()
        {
            PrepareScene();
        }

        internal static void PrepareScene()
        {
            
            if (BetterBasesUtils.IsNonGameScene() || Settings.options.disableRepairs)
            {
                return;
            }


            ScenePreparers.PrepareScene();
        }        

        internal static void ChangeLayer(GameObject gameObject, int from, int to)
        {
            if (gameObject.layer == from)
            {
                gameObject.layer = to;
            }

            foreach (Transform eachChild in gameObject.transform)
            {
                ChangeLayer(eachChild.gameObject, from, to);
            }
        }

        internal static void Log(string message, bool forceLog = false)
        {
            if (Settings.options.verbose || forceLog)
            {
                MelonLogger.Msg(message);
            }
        }

        internal static void Log(string message, params object[] parameters)
        {
            Log(string.Format(message, parameters));
        }


    }
}