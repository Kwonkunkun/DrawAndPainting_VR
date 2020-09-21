using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component can be used to create tool prefabs for in-editor painting. These will automatically appear in the Paint tab's Tool list.</summary>
	public class P3dTool : MonoBehaviour, IBrowsable
	{
		public string Category { set { category = value; } get { return category; } } [SerializeField] private string category;

		public Texture2D Icon { set { icon = value; } get { return icon; } } [SerializeField] private Texture2D icon;

		private static List<P3dTool> cachedTools;

		public static List<P3dTool> CachedTools
		{
			get
			{
				if (cachedTools == null)
				{
					cachedTools = new List<P3dTool>();
#if UNITY_EDITOR
					var guids = AssetDatabase.FindAssets("t:prefab");

					foreach (var guid in guids)
					{
						var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));

						if (prefab != null)
						{
							var tool = prefab.GetComponent<P3dTool>();

							if (tool != null)
							{
								cachedTools.Add(tool);
							}
						}
					}
#endif
				}

				return cachedTools;
			}
		}

		public static void ClearCache()
		{
			cachedTools = null;
		}

		public string GetCategory()
		{
			return category;
		}

		public string GetTitle()
		{
			return name;
		}

		public Texture2D GetIcon()
		{
			return icon;
		}

		public Object GetObject()
		{
			return this;
		}
#if UNITY_EDITOR
		protected virtual void OnGUI()
		{
			if (P3dWindow.Settings.OverrideCamera == true && P3dWindow.Settings.Observer != null)
			{
				var target   = P3dWindow.Settings.Observer;
				var delta    = Event.current.delta;
				var distance = P3dWindow.Settings.Distance;

				if (Event.current.type == EventType.ScrollWheel)
				{
					var newDistance = delta.y > 0.0f ? distance / 0.9f : distance * 0.9f;

					target.position += target.forward * (distance - newDistance);

					distance = newDistance;
				}
				else if (Event.current.type == EventType.MouseDrag)
				{
					if (Event.current.button == 1)
					{
						var point = target.TransformPoint(0.0f, 0.0f, distance);

						target.RotateAround(point, target.up   , delta.x * 0.5f);
						target.RotateAround(point, target.right, delta.y * 0.5f);

						target.rotation = Quaternion.LookRotation(target.forward, Vector3.up);
					}

					if (Event.current.button == 2)
					{
						target.position -= target.right * delta.x * distance * 0.001f;
						target.position += target.up    * delta.y * distance * 0.001f;
					}
				}
				else if (Event.current.type == EventType.MouseDown)
				{
					if (Event.current.clickCount == 2 && Event.current.button != 0 && Camera.main != null)
					{
						Handles.SetCamera(Camera.main);

						var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
						var hit = default(RaycastHit);

						if (Physics.Raycast(ray, out hit, float.PositiveInfinity) == true)
						{
							target.position = hit.point - target.forward * distance;
						}
					}
				}

				if (Event.current.keyCode == P3dWindow.Settings.MoveForward)
				{
					target.position += target.forward * Time.deltaTime * distance * P3dWindow.Settings.MoveSpeed;
				}

				if (Event.current.keyCode == P3dWindow.Settings.MoveBackward)
				{
					target.position -= target.forward * Time.deltaTime * distance * P3dWindow.Settings.MoveSpeed;
				}

				if (Event.current.keyCode == P3dWindow.Settings.MoveLeft)
				{
					target.position -= target.right * Time.deltaTime * distance * P3dWindow.Settings.MoveSpeed;
				}

				if (Event.current.keyCode == P3dWindow.Settings.MoveRight)
				{
					target.position += target.right * Time.deltaTime * distance * P3dWindow.Settings.MoveSpeed;
				}

				P3dWindow.Settings.Distance = distance;
			}
		}
#endif
#if UNITY_EDITOR
		[MenuItem("Assets/Create/Paint in 3D/Tool")]
		private static void CreateAsset()
		{
			var tool  = new GameObject("Tool").AddComponent<P3dTool>();
			var guids = Selection.assetGUIDs;
			var path  = guids.Length > 0 ? AssetDatabase.GUIDToAssetPath(guids[0]) : null;

			if (string.IsNullOrEmpty(path) == true)
			{
				path = "Assets";
			}
			else if (AssetDatabase.IsValidFolder(path) == false)
			{
				path = System.IO.Path.GetDirectoryName(path);
			}

			var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/NewTool.prefab");

			var asset = PrefabUtility.SaveAsPrefabAsset(tool.gameObject, assetPathAndName);

			DestroyImmediate(tool.gameObject);

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			EditorUtility.FocusProjectWindow();

			Selection.activeObject = asset; EditorGUIUtility.PingObject(asset);
		}
#endif
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dTool))]
	public class P3dTool_Editor : P3dEditor<P3dTool>
	{
		protected override void OnInspector()
		{
			if (P3dTool.CachedTools.Contains(Target) == false && P3dHelper.IsAsset(Target) == true)
			{
				P3dTool.CachedTools.Add(Target);
			}

			Draw("category");
			Draw("icon");
		}
	}
}
#endif