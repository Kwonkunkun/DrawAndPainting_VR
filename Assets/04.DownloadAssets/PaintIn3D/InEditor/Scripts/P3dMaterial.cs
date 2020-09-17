using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
#endif

namespace PaintIn3D
{
	/// <summary>This component can be used to create material prefabs for in-editor painting. These will automatically appear in the Paint tab's Material list.</summary>
	public class P3dMaterial : MonoBehaviour, IBrowsable
	{
		public string Category { set { category = value; } get { return category; } } [SerializeField] private string category;

		public Texture2D Icon { set { icon = value; } get { return icon; } } [SerializeField] private Texture2D icon;

		private static List<P3dMaterial> cachedMaterials;

		public static List<P3dMaterial> CachedMaterials
		{
			get
			{
				if (cachedMaterials == null)
				{
					cachedMaterials = new List<P3dMaterial>();
#if UNITY_EDITOR
					var guids = AssetDatabase.FindAssets("t:prefab");

					foreach (var guid in guids)
					{
						var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));

						if (prefab != null)
						{
							var material = prefab.GetComponent<P3dMaterial>();

							if (material != null)
							{
								cachedMaterials.Add(material);
							}
						}
					}
#endif
				}

				return cachedMaterials;
			}
		}

		public static void ClearCache()
		{
			cachedMaterials = null;
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
		[MenuItem("Assets/Create/Paint in 3D/Material")]
		private static void CreateAsset()
		{
			var material = new GameObject("Material").AddComponent<P3dMaterial>();
			var guids    = Selection.assetGUIDs;
			var path     = guids.Length > 0 ? AssetDatabase.GUIDToAssetPath(guids[0]) : null;

			if (string.IsNullOrEmpty(path) == true)
			{
				path = "Assets";
			}
			else if (AssetDatabase.IsValidFolder(path) == false)
			{
				path = System.IO.Path.GetDirectoryName(path);
			}

			var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/NewMaterial.prefab");

			var asset = PrefabUtility.SaveAsPrefabAsset(material.gameObject, assetPathAndName);

			DestroyImmediate(material.gameObject);

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
	[CustomEditor(typeof(P3dMaterial))]
	public class P3dMaterial_Editor : P3dEditor<P3dMaterial>
	{
		private static List<Texture2D> textures = new List<Texture2D>();

		private static int size = 512;

		private static string GetTitle(Texture2D texture)
		{
			if (texture != null)
			{
				var title      = texture.name;
				var underscore = title.LastIndexOf("_");

				if (underscore >= 0)
				{
					return title.Substring(underscore + 1);
				}
			}

			return null;
		}

		protected override void OnInspector()
		{
			if (P3dMaterial.CachedMaterials.Contains(Target) == false && P3dHelper.IsAsset(Target) == true)
			{
				P3dMaterial.CachedMaterials.Add(Target);
			}
			
			Draw("category");
			Draw("icon");

			EditorGUILayout.Separator();

			EditorGUILayout.LabelField("Material Builder", EditorStyles.boldLabel);

			DrawMaterialBuilder();

			EditorGUILayout.Separator();

			EditorGUILayout.LabelField("Icon Builder", EditorStyles.boldLabel);

			DrawIconBuilder();
		}

		private void DrawMaterialBuilder()
		{
			for (var i = 0; i < textures.Count; i++)
			{
				var texture = textures[i];
				var title   = GetTitle(texture);

				textures[i] = (Texture2D)EditorGUI.ObjectField(P3dHelper.Reserve(), new GUIContent(title), textures[i], typeof(Texture2D), false);

				EditorGUI.BeginDisabledGroup(true);
					foreach (var groupData in P3dGroupData.CachedInstances)
					{
						foreach (var textureData in groupData.TextureDatas)
						{
							if (textureData.Name == title)
							{
								EditorGUILayout.ObjectField(" ", groupData, typeof(P3dGroupData), false);
							}
						}
					}
				EditorGUI.EndDisabledGroup();
			}

			EditorGUILayout.Separator();

			var newTexture = (Texture2D)EditorGUILayout.ObjectField(default(Texture2D), typeof(Texture2D), false);

			if (newTexture != null)
			{
				textures.Add(newTexture);
			}

			EditorGUILayout.Separator();

			EditorGUI.BeginDisabledGroup(Target.transform.childCount == 0);
				if (GUILayout.Button("Populate From Children") == true)
				{
					textures.Clear();

					for (var i = 0; i < Target.transform.childCount; i++)
					{
						var paintDecal = Target.transform.GetChild(i).GetComponent<P3dPaintDecal>();

						if (paintDecal != null)
						{
							var tex = paintDecal.TileTexture as Texture2D;

							if (tex != null)
							{
								textures.Add(tex);
							}
						}
					}
				}
			EditorGUI.EndDisabledGroup();

			EditorGUI.BeginDisabledGroup(textures.Exists(t => t != null) == false);
				if (GUILayout.Button("Build Material") == true)
				{
					for (var i = Target.transform.childCount - 1; i >= 0; i--)
					{
						DestroyImmediate(Target.transform.GetChild(i).gameObject);
					}

					foreach (var texture in textures)
					{
						var title = GetTitle(texture);

						if (string.IsNullOrEmpty(title) == false)
						{
							var child = new GameObject(title);

							child.transform.SetParent(Target.transform, false);

							foreach (var groupData in P3dGroupData.CachedInstances)
							{
								foreach (var textureData in groupData.TextureDatas)
								{
									if (textureData.Name == title)
									{
										var paintDecal = child.AddComponent<P3dPaintDecal>();

										paintDecal.Group       = groupData.Index;
										paintDecal.BlendMode   = textureData.BlendMode;
										paintDecal.TileTexture = texture;
									}
								}
							}
						}
					}

					EditorSceneManager.MarkSceneDirty(Target.gameObject.scene);
				}
			EditorGUI.EndDisabledGroup();
		}

		private void DrawIconBuilder()
		{
			size = EditorGUILayout.IntField("Size", size);

			EditorGUILayout.Separator();

			if (GUILayout.Button("Build Icon") == true)
			{
				var path      = System.IO.Path.ChangeExtension(PrefabStageUtility.GetPrefabStage(Target.gameObject).prefabAssetPath, "png");
				var target    = new RenderTexture(size, size, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
				var buffer    = new Texture2D(size, size, TextureFormat.ARGB32, false);
				var oldActive = RenderTexture.active;
				var oldTarget = Camera.main.targetTexture;

				Camera.main.targetTexture = target;
					Camera.main.Render();
				Camera.main.targetTexture = oldTarget;

				RenderTexture.active = target;
					buffer.ReadPixels(new Rect(0, 0, size, size), 0, 0, false);
					buffer.Apply();
				RenderTexture.active = oldActive;

				System.IO.File.WriteAllBytes(path, buffer.EncodeToPNG());

				DestroyImmediate(target);
				DestroyImmediate(buffer);

				AssetDatabase.ImportAsset(path);

				var importer = (TextureImporter)AssetImporter.GetAtPath(path);

				importer.filterMode          = FilterMode.Trilinear;
				importer.anisoLevel          = 8;
				importer.textureCompression  = TextureImporterCompression.Uncompressed;
				importer.alphaIsTransparency = true;

				importer.SaveAndReimport();

				Target.Icon = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

				EditorSceneManager.MarkSceneDirty(Target.gameObject.scene);
			}
		}
	}
}
#endif