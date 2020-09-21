using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This struct allows you to specify a group index with a group dropdown selector.</summary>
	[System.Serializable]
	public struct P3dGroup
	{
		[SerializeField]
		private int index;

		public P3dGroup(int newIndex)
		{
			index = newIndex;
		}

		public static implicit operator int(P3dGroup group)
		{
			return group.index;
		}

		public static implicit operator P3dGroup(int index)
		{
			return new P3dGroup(index);
		}

		public override string ToString()
		{
			return index.ToString();
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CustomPropertyDrawer(typeof(P3dGroup))]
	public partial class P3dGroup_Drawer : PropertyDrawer
	{
		public static void Draw(Rect position, SerializedProperty property)
		{
			var sPro      = property.FindPropertyRelative("index");
			var groupData = P3dGroupData.GetGroupData(sPro.intValue);

			P3dHelper.BeginColor(groupData == null);
				if (GUI.Button(position, groupData != null ? groupData.name : "MISSING: " + sPro.intValue, EditorStyles.popup) == true)
				{
					var menu        = new GenericMenu();
					var groupDatas  = P3dGroupData.CachedInstances.OrderBy(d => d.Index);
					var editorCount = 0;

					foreach (var cachedGroupData in groupDatas)
					{
						if (cachedGroupData != null)
						{
							if (cachedGroupData.Index >= 0)
							{
								AddMenuItem(menu, cachedGroupData, sPro, cachedGroupData.Index);
							}
							else
							{
								editorCount++;
							}
						}
					}

					if (editorCount > 0)
					{
						menu.AddDisabledItem(new GUIContent(""));
						menu.AddDisabledItem(new GUIContent("EDITOR"));
						menu.AddDisabledItem(new GUIContent(""));

						foreach (var cachedGroupData in groupDatas)
						{
							if (cachedGroupData != null && cachedGroupData.Index < 0)
							{
								AddMenuItem(menu, cachedGroupData, sPro, cachedGroupData.Index);
							}
						}
					}

					menu.DropDown(position);
				}
			P3dHelper.EndColor();
		}

		private static void AddMenuItem(GenericMenu menu, P3dGroupData groupData, SerializedProperty sPro, int index)
		{
			var content = new GUIContent(groupData.GetName(true));

			menu.AddItem(content, sPro.intValue == index, () => { sPro.intValue = index; sPro.serializedObject.ApplyModifiedProperties(); });
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var right = position; right.xMin += EditorGUIUtility.labelWidth;

			EditorGUI.LabelField(position, label);

			Draw(right, property);
		}
	}
}
#endif