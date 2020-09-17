#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PaintIn3D
{
	public partial class P3dWindow
	{
		[System.Serializable]
		public class SettingsData
		{
			public P3dGroup ColorGroup = 0;

			public int StateLimit = 10;

			public int IconSize = 128;

			public bool OverrideColor = true;

			public Color Color = Color.red;

			public bool OverrideRadius = true;

			public float Radius = 1.0f;

			public bool OverrideCamera = true;

			public float Distance = 10.0f;

			public Transform Observer;

			public P3dTool CurrentTool;

			public P3dMaterial CurrentMaterial;

			public P3dShape CurrentShape;

			public float MoveSpeed = 1.0f;

			public KeyCode MoveForward = KeyCode.W;

			public KeyCode MoveBackward = KeyCode.S;

			public KeyCode MoveLeft = KeyCode.A;

			public KeyCode MoveRight = KeyCode.D;
		}

		public static SettingsData Settings = new SettingsData();

		private Vector2 configScrollPosition;

		private static void SaveSettings()
		{
			EditorPrefs.SetString("PaintIn3D.Settings", EditorJsonUtility.ToJson(Settings));
		}

		private static void LoadSettings()
		{
			if (EditorPrefs.HasKey("PaintIn3D.Settings") == true)
			{
				var json = EditorPrefs.GetString("PaintIn3D.Settings");

				if (string.IsNullOrEmpty(json) == false)
				{
					EditorJsonUtility.FromJsonOverwrite(json, Settings);
				}
			}
		}

		private void DrawConfig()
		{
			configScrollPosition = GUILayout.BeginScrollView(configScrollPosition, GUILayout.ExpandHeight(true));
				P3dHelper.BeginLabelWidth(100);
					Settings.ColorGroup = EditorGUILayout.IntField("Color Group", Settings.ColorGroup);
					Settings.IconSize = EditorGUILayout.IntSlider("Icon Size", Settings.IconSize, 32, 256);
					EditorGUILayout.BeginHorizontal();
						Settings.StateLimit = EditorGUILayout.IntField("State Limit", Settings.StateLimit);
						if (GUILayout.Button(new GUIContent("Apply", "Apply this undo/redo state limit to all P3dPaintableTexture components in the scene?"), EditorStyles.miniButton, GUILayout.ExpandWidth(false)) == true)
						{
							if (EditorUtility.DisplayDialog("Are you sure?", "This will apply this StateLimit to all P3dPaintableTexture components in the scene.", "ok") == true)
							{
								ApplyStateLimit();
							}
						}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.Separator();

					EditorGUILayout.LabelField("Move", EditorStyles.boldLabel);
					Settings.MoveSpeed    = EditorGUILayout.FloatField("Speed", Settings.MoveSpeed);
					Settings.MoveForward  = (KeyCode)EditorGUILayout.EnumPopup("Forward", Settings.MoveForward);
					Settings.MoveBackward = (KeyCode)EditorGUILayout.EnumPopup("Backward", Settings.MoveBackward);
					Settings.MoveLeft     = (KeyCode)EditorGUILayout.EnumPopup("Left", Settings.MoveLeft);
					Settings.MoveRight    = (KeyCode)EditorGUILayout.EnumPopup("Right", Settings.MoveRight);
				P3dHelper.EndLabelWidth();
			GUILayout.EndScrollView();
		}

		private void ApplyStateLimit()
		{
			var paintableTextures = FindObjectsOfType<P3dPaintableTexture>();

			Undo.RecordObjects(paintableTextures, "Apply State Limit");

			foreach (var paintableTexture in paintableTextures)
			{
				if (paintableTexture.UndoRedo != P3dPaintableTexture.UndoRedoType.LocalCommandCopy)
				{
					paintableTexture.UndoRedo   = P3dPaintableTexture.UndoRedoType.FullTextureCopy;
					paintableTexture.StateLimit = Settings.StateLimit;

					EditorUtility.SetDirty(paintableTexture);
				}
			}
		}
	}
}
#endif