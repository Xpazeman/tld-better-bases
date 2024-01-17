using System;
using System.Collections.Generic;
using System.Text;
using Il2Cpp;
using UnityEngine;
using UnityEngine.SceneManagement;

using MelonLoader.TinyJSON;
using MelonLoader;
using System.Linq;

namespace BetterBases
{
	internal class BetterBasesUtils
	{
		internal static RepairedContainers DeserializeSaveProxy(string saveProxyData)
		{
			if (string.IsNullOrEmpty(saveProxyData))
			{
				return null;
			}

			SaveProxy saveProxy = JSON.Load(saveProxyData).Make<SaveProxy>();

			if (string.IsNullOrEmpty(saveProxy.data))
			{
				return null;
			}

            return JSON.Load(saveProxy.data).Make<RepairedContainers>();
        }
        internal static Vector3 MakeVector3(float[] coords)
        {
            Vector3 vector = new Vector3(coords[0], coords[1], coords[2]);

			return vector;
		}

        internal static float[] MakeArrayFromVector3(Vector3 vector)
        {
            float[] coords = new float[3];
            coords[0] = (float)Math.Round(vector.x,3);
            coords[1] = (float)Math.Round(vector.y, 3);
            coords[2] = (float)Math.Round(vector.z, 3);

			return coords;
		}

		internal static void FindChildren(GameObject gameObject, List<GameObject> result, GameObjectSearchFilter filter)
		{
			SearchResult searchResult = filter.Filter(gameObject);

			if (searchResult.IsInclude())
			{
				result.Add(gameObject);
			}

			if (searchResult.IsContinue())
			{
				for (int i = 0; i < gameObject.transform.childCount; i++)
				{
					GameObject child = gameObject.transform.GetChild(i).gameObject;
					FindChildren(child, result, filter);
				}
			}
		}

		internal static Container FindContainerTemplate(GameObject gameObject)
		{
			Transform parentTransform = gameObject.transform.parent;
			if (parentTransform == null)
			{
				return null;
			}

			GameObject parent = parentTransform.gameObject;
			if (!parent.name.Contains("CONTAINER_"))
			{
				return null;
			}

			Container[] containers = parent.GetComponentsInChildren<Container>();
			if (containers == null)
			{
				return null;
			}

			foreach (Container eachContainer in containers)
			{
				if (eachContainer.enabled)
				{
					return eachContainer;
				}
			}

			return null;
		}

		internal static GameObject FindGameObject(string path, Vector3 position)
		{
			for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
			{
				Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

				List<GameObject> targets = GetSceneObjects(scene, new PathGameObjectSearchFilter(path, position));
				if (targets.Count > 0)
				{
					return targets[0];
				}
			}

			return null;
		}

		internal static GameObject GetParent(Component component)
		{
			if (component == null)
			{
				return null;
			}

			return GetParent(component.gameObject);
		}

		internal static GameObject GetParent(GameObject gameObject)
		{
			if (gameObject == null)
			{
				return null;
			}

			Transform parent = gameObject.transform.parent;
			if (parent == null)
			{
				return null;
			}

			return parent.gameObject;
		}

		internal static string GetPath(GameObject gameObject)
		{
			StringBuilder stringBuilder = new StringBuilder();

			Transform current = gameObject.transform;

			while (current != null)
			{
				stringBuilder.Insert(0, current.name);
				stringBuilder.Insert(0, "/");

				current = current.transform.parent;
			}

			return stringBuilder.ToString();
		}

		internal static List<GameObject> GetSceneObjects(Scene scene, GameObjectSearchFilter filter)
		{
			List<GameObject> result = new List<GameObject>();

			foreach (GameObject eachRoot in scene.GetRootGameObjects())
			{
				FindChildren(eachRoot, result, filter);
			}

			return result;
		}

		internal static List<GameObject> GetRootObjects()
		{
			List<GameObject> rootObj = new List<GameObject>();

			string[] ignoreRootPrefixes = new string[] { "skill_", "script_", "pdidtable", "navmesh", "betterbasesroot" };

			for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
			{
				Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

				GameObject[] sceneObj = scene.GetRootGameObjects();

				foreach (GameObject obj in sceneObj)
				{
					if (ignoreRootPrefixes.Any(obj.name.ToLowerInvariant().StartsWith))
					{
						continue;
					}
					rootObj.Add(obj);
				}
			}

			return rootObj;
		}

		internal static void GetChildrenWithName(GameObject obj, string name, List<GameObject> result)
		{

			if (obj.transform.childCount > 0)
			{
				for (int i = 0; i < obj.transform.childCount; i++)
				{
					GameObject child = obj.transform.GetChild(i).gameObject;

					if (child.name.ToLowerInvariant().Contains(name.ToLowerInvariant()))
					{

						if (
							child.GetComponent<Container>() != null
							|| child.GetComponent<Bed>() != null
							|| child.GetComponent<AuroraScreenDisplay>() != null
							|| child.GetComponent<OpenClose>() != null
							|| child.GetComponent<GearItem>() != null
							)
						{
							continue;
						}

						if (
							child.GetComponentInParent<Container>() != null
							|| child.GetComponentInParent<OpenClose>() != null
							|| child.GetComponentInParent<GearItem>() != null
							)
						{
							continue;
						}

						if (
							child.GetComponentInChildren<Container>() != null
							|| child.GetComponentInChildren<Bed>() != null
							|| child.GetComponentInChildren<AuroraScreenDisplay>() != null
							|| child.GetComponentInChildren<OpenClose>() != null
							|| child.GetComponentInChildren<GearItem>() != null
							)
						{
							continue;
						}

						result.Add(child);
						continue;
					}

					GetChildrenWithName(child, name, result);
				}
			}
		}

		internal static void SetLayer(GameObject gameObject, int layer, bool recursive = true)
		{
			ChangeLayer changeLayer = gameObject.AddComponent<ChangeLayer>();
			changeLayer.Layer = layer;
			changeLayer.Recursively = recursive;
		}

		internal static void SetGuid(GameObject target, string guid)
		{
			ObjectGuid objectGuid = target.GetComponent<ObjectGuid>();
			if (objectGuid == null)
			{
				objectGuid = target.AddComponent<ObjectGuid>();
			}

			objectGuid.m_Guid = guid;
		}

		public static T GetComponent<T>(Component component) where T : Component
		{
			return GetComponent<T>(component ? component.gameObject : null);
		}

		public static T GetComponent<T>(GameObject gameObject) where T : Component
		{
			if (gameObject == null) return default(T);
			else return gameObject.GetComponent<T>();
		}

		public static T GetOrCreateComponent<T>(Component component) where T : Component
		{
			return GetOrCreateComponent<T>(component ? component.gameObject : null);
		}

		public static T GetOrCreateComponent<T>(GameObject gameObject) where T : Component
		{
			T result = GetComponent<T>(gameObject);

			if (result == null) result = gameObject.AddComponent<T>();

			return result;
		}

		public static bool IsNonGameScene()
		{
			return GameManager.m_ActiveScene == null || GameManager.m_ActiveScene == "MainMenu" || GameManager.m_ActiveScene == "Boot" || GameManager.m_ActiveScene == "Empty";
		}
	}

	[RegisterTypeInIl2Cpp]
	public class ChangeLayer : MonoBehaviour
	{
		public int Layer;
		public bool Recursively;

		public ChangeLayer(IntPtr intPtr) : base(intPtr) { }

		public void Start()
		{
			this.Invoke("SetLayer", 1);
		}

		internal void SetLayer()
		{
			vp_Layer.Set(this.gameObject, Layer, Recursively);
			Destroy(this);
		}
	}
}