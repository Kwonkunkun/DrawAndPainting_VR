using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component can be used to create shape prefabs for in-editor painting. These will automatically appear in the Paint tab's Shape list.</summary>
	public class P3dShape : MonoBehaviour, IBrowsable
	{
		public string Category { set { category = value; } get { return category; } } [SerializeField] private string category;

		public Texture2D Icon { set { icon = value; } get { return icon; } } [SerializeField] private Texture2D icon;

		private static List<P3dShape> cachedShapes;

		public static List<P3dShape> CachedShapes
		{
			get
			{
				if (cachedShapes == null)
				{
					cachedShapes = new List<P3dShape>();
#if UNITY_EDITOR
					var guids = AssetDatabase.FindAssets("t:prefab");

					foreach (var guid in guids)
					{
						var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));

						if (prefab != null)
						{
							var brush = prefab.GetComponent<P3dShape>();

							if (brush != null)
							{
								cachedShapes.Add(brush);
							}
						}
					}
#endif
				}

				return cachedShapes;
			}
		}

		public static void ClearCache()
		{
			cachedShapes = null;
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
		[MenuItem("Assets/Create/Paint in 3D/Shape")]
		private static void CreateAsset()
		{
			var brush = new GameObject("Shape").AddComponent<P3dShape>();
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

			var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/NewBrush.prefab");

			var asset = PrefabUtility.SaveAsPrefabAsset(brush.gameObject, assetPathAndName);

			DestroyImmediate(brush.gameObject);

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
	[CustomEditor(typeof(P3dShape))]
	public class P3dShape_Editor : P3dEditor<P3dShape>
	{
		protected override void OnInspector()
		{
			if (P3dShape.CachedShapes.Contains(Target) == false && P3dHelper.IsAsset(Target) == true)
			{
				P3dShape.CachedShapes.Add(Target);
			}

			Draw("category");
			Draw("icon");
		}
	}
}
#endif