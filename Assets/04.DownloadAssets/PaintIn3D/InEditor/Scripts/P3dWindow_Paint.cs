#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace PaintIn3D
{
	public partial class P3dWindow
	{
		private Vector2 paintScrollPosition;

		private bool selectingTool;

		private bool selectingMaterial;

		private bool selectingShape;

		private void DrawPaint()
		{
			if (selectingTool == true)
			{
				DrawTool(); return;
			}

			if (selectingMaterial == true)
			{
				DrawMaterial(); return;
			}

			if (selectingShape == true)
			{
				DrawShape(); return;
			}

			paintScrollPosition = GUILayout.BeginScrollView(paintScrollPosition, GUILayout.ExpandHeight(true));
				DrawTop();

				EditorGUILayout.Separator();

				P3dHelper.BeginLabelWidth(100);
					DrawRadius();
				P3dHelper.EndLabelWidth();

				EditorGUILayout.Separator();

				P3dHelper.BeginLabelWidth(100);
					DrawColor();
				P3dHelper.EndLabelWidth();

				EditorGUILayout.Separator();

				P3dHelper.BeginLabelWidth(100);
					DrawCamera();
				P3dHelper.EndLabelWidth();
			GUILayout.EndScrollView();

			GUILayout.FlexibleSpace();

			if (Application.isPlaying == false)
			{
				EditorGUILayout.HelpBox("You must enter play mode to begin painting.", MessageType.Warning);
			}

			UpdatePaint();
		}

		private void DrawTop()
		{
			var toolIcon      = default(Texture2D);
			var toolTitle     = "None";
			var materialIcon  = default(Texture2D);
			var materialTitle = "None";
			var shapeIcon     = default(Texture2D);
			var shapeTitle    = "None";
			var width         = Mathf.FloorToInt((position.width - 30) / 3);

			if (Settings.CurrentTool != null)
			{
				toolIcon  = Settings.CurrentTool.GetIcon();
				toolTitle = Settings.CurrentTool.GetTitle();
			}

			if (Settings.CurrentMaterial != null)
			{
				materialIcon  = Settings.CurrentMaterial.GetIcon();
				materialTitle = Settings.CurrentMaterial.GetTitle();
			}

			if (Settings.CurrentShape != null)
			{
				shapeIcon  = Settings.CurrentShape.GetIcon();
				shapeTitle = Settings.CurrentShape.GetTitle();
			}

			EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				EditorGUILayout.BeginVertical();
					var rectA = DrawIcon(width, toolIcon, toolTitle, Settings.CurrentTool != null, "Tool");
				EditorGUILayout.EndVertical();
				GUILayout.FlexibleSpace();
				EditorGUILayout.BeginVertical();
					var rectB = DrawIcon(width, materialIcon, materialTitle, Settings.CurrentMaterial != null, "Material");
				EditorGUILayout.EndVertical();
				GUILayout.FlexibleSpace();
				EditorGUILayout.BeginVertical();
					var rectC = DrawIcon(width, shapeIcon, shapeTitle, Settings.CurrentShape != null, "Shape");
				EditorGUILayout.EndVertical();
				GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			if (Event.current.type == EventType.MouseDown && rectA.Contains(Event.current.mousePosition) == true)
			{
				if (Event.current.button == 0)
				{
					selectingTool = true;
				}
				else
				{
					P3dHelper.SelectAndPing(Settings.CurrentTool);
				}
			}

			if (Event.current.type == EventType.MouseDown && rectB.Contains(Event.current.mousePosition) == true)
			{
				if (Event.current.button == 0)
				{
					selectingMaterial = true;
				}
				else
				{
					P3dHelper.SelectAndPing(Settings.CurrentMaterial);
				}
			}

			if (Event.current.type == EventType.MouseDown && rectC.Contains(Event.current.mousePosition) == true)
			{
				if (Event.current.button == 0)
				{
					selectingShape = true;
				}
				else
				{
					P3dHelper.SelectAndPing(Settings.CurrentShape);
				}
			}
		}

		private void DrawRadius()
		{
			Settings.OverrideRadius = EditorGUILayout.Toggle("Override Radius", Settings.OverrideRadius);

			if (Settings.OverrideRadius == true)
			{
				EditorGUI.indentLevel++;
					Settings.Radius = LogSlider("Radius", Settings.Radius, -4, 4);
				EditorGUI.indentLevel--;
			}
		}

		private void DrawColor()
		{
			Settings.OverrideColor = EditorGUILayout.Toggle("Override Color", Settings.OverrideColor);

			if (Settings.OverrideColor == true)
			{
				EditorGUI.indentLevel++;
					Settings.Color   = EditorGUILayout.ColorField("Color", Settings.Color);
					Settings.Color.r = Slider("Red", Settings.Color.r, 0.0f, 1.0f);
					Settings.Color.g = Slider("Green", Settings.Color.g, 0.0f, 1.0f);
					Settings.Color.b = Slider("Blue", Settings.Color.b, 0.0f, 1.0f);
					Settings.Color.a = Slider("Alpha", Settings.Color.a, 0.0f, 1.0f);

					float h, s, v; Color.RGBToHSV(Settings.Color, out h, out s, out v);

					h = Slider("Hue"       , h, 0.0f, 1.0f);
					s = Slider("Saturation", s, 0.0f, 1.0f);
					v = Slider("Value"     , v, 0.0f, 1.0f);

					var newColor = Color.HSVToRGB(h, s, v);

					Settings.Color.r = newColor.r;
					Settings.Color.g = newColor.g;
					Settings.Color.b = newColor.b;
				EditorGUI.indentLevel--;
			}
		}

		private void DrawCamera()
		{
			Settings.OverrideCamera = EditorGUILayout.Toggle("Override Camera", Settings.OverrideCamera);

			if (Settings.OverrideCamera == true)
			{
				if (Settings.Observer == null && Camera.main != null)
				{
					Settings.Observer = Camera.main.transform;
				}

				EditorGUI.indentLevel++;
					Settings.Distance = LogSlider("Distance", Settings.Distance, -4, 4);
					Settings.Observer = (Transform)EditorGUILayout.ObjectField("Root", Settings.Observer, typeof(Transform), true);

					if (GUI.Button(EditorGUI.IndentedRect(P3dHelper.Reserve()), "Snap To Scene View", EditorStyles.miniButton) == true)
					{
						var camA = Camera.main;

						if (camA != null && SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null)
						{
							var camB = SceneView.lastActiveSceneView.camera;

							camA.transform.position = camB.transform.position;
							camA.transform.rotation = camB.transform.rotation;
						}
					}
				EditorGUI.indentLevel--;
			}
		}

		private void UpdatePaint()
		{
			if (Settings.CurrentTool != null && toolInstance == null)
			{
				LoadTool(Settings.CurrentTool);
			}

			if (Settings.CurrentMaterial != null && materialInstance == null)
			{
				LoadMaterial(Settings.CurrentMaterial);
			}

			if (toolInstance != null)
			{
				foreach (var connectablePoint in toolInstance.GetComponentsInChildren<P3dConnectablePoints>())
				{
					connectablePoint.ClearHitCache();
				}

				foreach (var connectableLine in toolInstance.GetComponentsInChildren<P3dConnectableLines>())
				{
					connectableLine.ClearHitCache();
				}
			}

			if (materialInstance != null)
			{
				foreach (var paintSphere in materialInstance.GetComponentsInChildren<P3dPaintSphere>())
				{
					if (paintSphere.Group == Settings.ColorGroup)
					{
						if (Settings.OverrideColor == true)
						{
							paintSphere.Color  = Settings.Color;
						}
					}

					if (Settings.OverrideRadius == true)
					{
						paintSphere.Radius = Settings.Radius;
					}
				}

				foreach (var paintDecal in materialInstance.GetComponentsInChildren<P3dPaintDecal>())
				{
					if (paintDecal.Group == Settings.ColorGroup)
					{
						if (Settings.OverrideColor == true)
						{
							paintDecal.Color = Settings.Color;
						}
					}

					if (Settings.OverrideRadius == true)
					{
						paintDecal.Radius = Settings.Radius;
					}

					paintDecal.Shape        = Settings.CurrentShape != null ? Settings.CurrentShape.Icon : null;
					paintDecal.ShapeChannel = P3dChannel.Red;
				}
			}
		}
	}
}
#endif