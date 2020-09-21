#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PaintIn3D
{
	public partial class P3dWindow
	{
		private Vector2 sceneScrollPosition;

		private static HashSet<Transform> roots = new HashSet<Transform>();

		private static void RunRoots()
		{
			roots.Clear();

			foreach (var transform in Selection.transforms)
			{
				RunRoots(transform);
			}
		}

		private static void RunRoots(Transform t)
		{
			if (t.GetComponent<P3dPaintable>() == null)
			{
				roots.Add(t);
			}

			foreach (Transform child in t)
			{
				RunRoots(child);
			}
		}

		private void DrawScene(P3dPaintable[] paintables, P3dPaintableTexture[] paintableTextures)
		{
			RunRoots();

			var removePaintable = default(P3dPaintable);

			sceneScrollPosition = GUILayout.BeginScrollView(sceneScrollPosition, GUILayout.ExpandHeight(true));
				foreach (var root in roots)
				{
					var mr  = root.GetComponent<MeshRenderer>();
					var smr = root.GetComponent<SkinnedMeshRenderer>();

					if (mr != null || smr != null)
					{
						EditorGUILayout.BeginHorizontal();
							EditorGUI.BeginDisabledGroup(true);
								EditorGUILayout.ObjectField(GUIContent.none, root.gameObject, typeof(GameObject), true, GUILayout.MinWidth(10));
							EditorGUI.EndDisabledGroup();
							if (GUILayout.Button("Make Paintable", EditorStyles.miniButton, GUILayout.ExpandWidth(false)) == true)
							{
								root.gameObject.AddComponent<P3dPaintable>();
							}
						EditorGUILayout.EndHorizontal();
					}
				}
				foreach (var paintable in paintables)
				{
					EditorGUILayout.BeginHorizontal();
						EditorGUI.BeginDisabledGroup(true);
							EditorGUILayout.ObjectField(GUIContent.none, paintable, typeof(P3dPaintable), true, GUILayout.MinWidth(10));
						EditorGUI.EndDisabledGroup();
						if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.ExpandWidth(false)) == true)
						{
							if (EditorUtility.DisplayDialog("Are you sure?", "Remove painting components from this GameObject?", "ok") == true)
							{
								removePaintable = paintable;
							}
						}
					EditorGUILayout.EndHorizontal();
					EditorGUI.indentLevel++;
						DrawMaterials(paintable, paintable.Materials, paintable.GetComponents<P3dPaintableTexture>());
					EditorGUI.indentLevel--;
				}
			GUILayout.EndScrollView();

			if (paintables.Length == 0)
			{
				GUILayout.FlexibleSpace();

				EditorGUILayout.HelpBox("Your scene doesn't contain any paintable objects.", MessageType.Warning);
			}

			DrawSceneFooter(paintableTextures);

			if (removePaintable != null)
			{
				removePaintable.RemoveComponents();
			}
		}

		private void DrawMaterials(P3dPaintable paintable, Material[] materials, P3dPaintableTexture[] paintableTextures)
		{
			for (var i = 0; i < materials.Length; i++)
			{
				var material = materials[i];

				EditorGUILayout.BeginHorizontal();
					EditorGUI.BeginDisabledGroup(true);
						EditorGUILayout.ObjectField(GUIContent.none, material, typeof(Material), true, GUILayout.MinWidth(10));
					EditorGUI.EndDisabledGroup();
					if (GUILayout.Button("+Preset", EditorStyles.miniButton, GUILayout.ExpandWidth(false)) == true)
					{
						var menu = new GenericMenu();
						
						foreach (var cachedPreset in P3dPreset.CachedPresets)
						{
							if (cachedPreset != null && material != null && cachedPreset.Targets(material.shader) == true)
							{
								var preset = cachedPreset;
								var index  = i;

								if (preset.CanAddTo(paintable, index) == true)
								{
									menu.AddItem(new GUIContent(preset.FinalName), false, () => preset.AddTo(paintable, index));
								}
								else
								{
									menu.AddDisabledItem(new GUIContent(preset.FinalName));
								}
							}
						}

						if (menu.GetItemCount() == 0)
						{
							menu.AddDisabledItem(new GUIContent("Failed to find any presets for this material or shader."));
						}

						menu.ShowAsContext();
					}
				EditorGUILayout.EndHorizontal();

				foreach (var paintableTexture in paintableTextures)
				{
					if (paintableTexture.Slot.Index == i)
					{
						EditorGUI.indentLevel++;
							DrawPaintableTexture(paintableTexture, material);
						EditorGUI.indentLevel--;
					}
				}
			}
		}

		private void DrawPaintableTexture(P3dPaintableTexture paintableTexture, Material material)
		{
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(paintableTexture.Slot.GetTitle(material));
				if (GUILayout.Button("Export", EditorStyles.miniButton, GUILayout.ExpandWidth(false)) == true)
				{
					var path = AssetDatabase.GUIDToAssetPath(paintableTexture.Output);
					var name = paintableTexture.name + "_" + paintableTexture.Slot.Name;
					var dir  = string.IsNullOrEmpty(path) == false ? System.IO.Path.GetDirectoryName(path) : "Assets";

					if (string.IsNullOrEmpty(path) == false)
					{
						name = System.IO.Path.GetFileNameWithoutExtension(path);
					}

					path = EditorUtility.SaveFilePanelInProject("Export Texture", name, "png", "Export Your Texture", dir);

					if (string.IsNullOrEmpty(path) == false)
					{
						System.IO.File.WriteAllBytes(path, paintableTexture.GetPngData());

						AssetDatabase.ImportAsset(path);

						Undo.RecordObject(paintableTexture, "Output Changed");

						paintableTexture.Output = AssetDatabase.AssetPathToGUID(path);

						EditorUtility.SetDirty(this);
					}
				}
			EditorGUILayout.EndHorizontal();

			P3dHelper.BeginLabelWidth(100);
				EditorGUI.indentLevel++;
					EditorGUILayout.BeginHorizontal();
						var outputTexture    = paintableTexture.OutputTexture;
						var newOutputTexture = EditorGUILayout.ObjectField(outputTexture, typeof(Texture2D), false);

						EditorGUI.BeginDisabledGroup(outputTexture == null || paintableTexture.Activated == false);
							if (GUILayout.Button("Load", EditorStyles.miniButton, GUILayout.ExpandWidth(false)) == true)
							{
								if (EditorUtility.DisplayDialog("Are you sure?", "This will replace this paintable texture with the currently exported texture state.", "ok") == true)
								{
									paintableTexture.Replace(outputTexture, Color.white);

									paintableTexture.Texture = outputTexture;
								}
							}
						EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndHorizontal();

					if (outputTexture != newOutputTexture)
					{
						Undo.RecordObject(paintableTexture, "Output Changed");

						paintableTexture.Output = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newOutputTexture));

						EditorUtility.SetDirty(this);
					}

					if (paintableTexture.UndoRedo == P3dPaintableTexture.UndoRedoType.None)
					{
						EditorGUILayout.HelpBox("This texture has no UndoRedo set, so you cannot undo or redo.", MessageType.Warning);
					}

					if (outputTexture == null)
					{
						EditorGUILayout.HelpBox("This texture hasn't been exported yet, so you cannot Export All.", MessageType.Warning);
					}
				EditorGUI.indentLevel--;
			P3dHelper.EndLabelWidth();
		}

		private bool CanLoadAll(P3dPaintableTexture[] paintableTextures)
		{
			foreach (var paintableTexture in paintableTextures)
			{
				if (paintableTexture.Activated == true && paintableTexture.OutputTexture != null)
				{
					return true;
				}
			}

			return false;
		}

		private bool CanExportAll(P3dPaintableTexture[] paintableTextures)
		{
			foreach (var paintableTexture in paintableTextures)
			{
				if (paintableTexture.OutputTexture != null)
				{
					return true;
				}
			}

			return false;
		}

		private void DrawSceneFooter(P3dPaintableTexture[] paintableTextures)
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

				EditorGUILayout.Separator();

				EditorGUI.BeginDisabledGroup(CanLoadAll(paintableTextures) == false);
					if (GUILayout.Button("Load All", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)) == true)
					{
						if (EditorUtility.DisplayDialog("Are you sure?", "This will replace all paintable textures with their currently exported texture state.", "ok") == true)
						{
							foreach (var paintableTexture in paintableTextures)
							{
								var outputTexture = paintableTexture.OutputTexture;

								if (outputTexture != null)
								{
									paintableTexture.Replace(outputTexture, Color.white);

									paintableTexture.Texture = outputTexture;
								}
							}
						}
					}
				EditorGUI.EndDisabledGroup();
				EditorGUI.BeginDisabledGroup(CanExportAll(paintableTextures) == false);
					P3dHelper.BeginColor(Color.green);
						if (GUILayout.Button("Export All", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)) == true)
						{
							if (EditorUtility.DisplayDialog("Are you sure?", "This will re-export all paintable textures in your scene.", "ok") == true)
							{
								foreach (var paintableTexture in paintableTextures)
								{
									if (paintableTexture.OutputTexture != null)
									{
										System.IO.File.WriteAllBytes(AssetDatabase.GUIDToAssetPath(paintableTexture.Output), paintableTexture.GetPngData());
									}
								}

								AssetDatabase.Refresh();
							}
						}
					P3dHelper.EndColor();
				EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
		}
	}
}
#endif