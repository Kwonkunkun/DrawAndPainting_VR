#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace PaintIn3D
{
	/// <summary>This is the base class for all Paint in 3D inspectors.</summary>
	public abstract class P3dEditor<T> : Editor
		where T : Object
	{
		protected T Target;

		protected T[] Targets;

		private static GUIContent customContent = new GUIContent();

		private static GUIStyle expandStyle;

		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();

			Target  = (T)target;
			Targets = targets.Select(t => (T)t).ToArray();

			P3dHelper.ClearStacks();

			Separator();

			OnInspector();

			Separator();

			serializedObject.ApplyModifiedProperties();

			if (EditorGUI.EndChangeCheck() == true)
			{
				GUI.changed = true; Repaint();

				foreach (var t in Targets)
				{
					EditorUtility.SetDirty(t);
				}
			}
		}

		public virtual void OnSceneGUI()
		{
			Target = (T)target;

			OnScene();

			if (GUI.changed == true)
			{
				EditorUtility.SetDirty(target);
			}
		}

		protected void Each(System.Action<T> update, bool dirty = false)
		{
			foreach (var t in Targets)
			{
				update(t);

				if (dirty == true)
				{
					EditorUtility.SetDirty(t);
				}
			}
		}

		protected bool Any(System.Func<T, bool> check)
		{
			foreach (var t in Targets)
			{
				if (check(t) == true)
				{
					return true;
				}
			}

			return false;
		}

		protected bool All(System.Func<T, bool> check)
		{
			foreach (var t in Targets)
			{
				if (check(t) == false)
				{
					return false;
				}
			}

			return true;
		}

		protected virtual void Separator()
		{
			EditorGUILayout.Separator();
		}

		protected void BeginIndent()
		{
			EditorGUI.indentLevel += 1;
		}

		protected void EndIndent()
		{
			EditorGUI.indentLevel -= 1;
		}

		protected bool Button(string text)
		{
			var rect = P3dHelper.Reserve();

			return GUI.Button(rect, text);
		}

		protected bool HelpButton(string helpText, UnityEditor.MessageType type, string buttonText, float buttonWidth)
		{
			var clicked = false;

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.HelpBox(helpText, type);

				var style = new GUIStyle(GUI.skin.button); style.wordWrap = true;

				clicked = GUILayout.Button(buttonText, style, GUILayout.ExpandHeight(true), GUILayout.Width(buttonWidth));
			}
			EditorGUILayout.EndHorizontal();

			return clicked;
		}

		protected void BeginMixed(bool mixed = true)
		{
			EditorGUI.showMixedValue = mixed;
		}

		protected void EndMixed()
		{
			EditorGUI.showMixedValue = false;
		}

		protected void BeginDisabled(bool disabled = true)
		{
			EditorGUI.BeginDisabledGroup(disabled);
		}

		protected void EndDisabled()
		{
			EditorGUI.EndDisabledGroup();
		}

		protected void BeginError(bool error = true)
		{
			P3dHelper.BeginColor(error);
		}

		protected void EndError()
		{
			P3dHelper.EndColor();
		}

		protected bool DrawExpand(ref bool expand, string propertyPath, string overrideTooltip = null, string overrideText = null)
		{
			var rect     = P3dHelper.Reserve();
			var property = serializedObject.FindProperty(propertyPath);

			customContent.text    = string.IsNullOrEmpty(overrideText   ) == false ? overrideText    : property.displayName;
			customContent.tooltip = string.IsNullOrEmpty(overrideTooltip) == false ? overrideTooltip : property.tooltip;

			if (expandStyle == null)
			{
				expandStyle = new GUIStyle(EditorStyles.miniLabel); expandStyle.alignment = TextAnchor.MiddleRight;
			}

			if (EditorGUI.DropdownButton(new Rect(rect.position + Vector2.left * 15, new Vector2(15.0f, rect.height)), new GUIContent(expand ? "-" : "+"), FocusType.Keyboard, expandStyle) == true)
			{
				expand = !expand;
			}

			EditorGUI.BeginChangeCheck();

			EditorGUI.PropertyField(rect, property, customContent, true);

			var changed = EditorGUI.EndChangeCheck();

			return changed;
		}

		protected bool Draw(string propertyPath, string overrideTooltip = null, string overrideText = null)
		{
			var property = serializedObject.FindProperty(propertyPath);

			customContent.text    = string.IsNullOrEmpty(overrideText   ) == false ? overrideText    : property.displayName;
			customContent.tooltip = string.IsNullOrEmpty(overrideTooltip) == false ? overrideTooltip : property.tooltip;

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.PropertyField(property, customContent, true);

			return EditorGUI.EndChangeCheck();
		}

		protected bool DrawMinMax(string propertyPath, float min, float max, string overrideTooltip = null, string overrideText = null)
		{
			var property = serializedObject.FindProperty(propertyPath);
			var value    = property.vector2Value;

			customContent.text    = string.IsNullOrEmpty(overrideText   ) == false ? overrideText    : property.displayName;
			customContent.tooltip = string.IsNullOrEmpty(overrideTooltip) == false ? overrideTooltip : property.tooltip;

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.MinMaxSlider(customContent, ref value.x, ref value.y, min, max);

			if (EditorGUI.EndChangeCheck() == true)
			{
				property.vector2Value = value;

				return true;
			}

			return false;
		}

		protected virtual void OnInspector()
		{
		}

		protected virtual void OnScene()
		{
		}
	}
}
#endif