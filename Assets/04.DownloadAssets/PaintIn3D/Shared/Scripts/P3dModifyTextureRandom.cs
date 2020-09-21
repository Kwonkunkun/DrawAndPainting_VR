using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This class allows you to randomize the painting texture of the attached component (e.g. P3dPaintDecal).</summary>
	[System.Serializable]
	public class P3dModifyTextureRandom : P3dModifier
	{
		public static string Group = "Texture";

		public static string Title = "Random";

		/// <summary>A random texture will be picked from this list.</summary>
		public List<Texture> Textures { get { if (textures == null) textures = new List<Texture>(); return textures; } } [SerializeField] private List<Texture> textures;

		public override void SetObjects(Object[] objects)
		{
			Textures.Clear(); // NOTE: Property

			if (objects != null)
			{
				foreach (var obj in objects)
				{
					Textures.Add(obj as Texture);
				}
			}
		}

		public override List<Object> GetObjects()
		{
			tempObjects.Clear();

			if (textures != null)
			{
				foreach (var tex in textures)
				{
					tempObjects.Add(tex);
				}
			}

			return tempObjects;
		}

		protected override void OnModifyTexture(ref Texture texture, float pressure)
		{
			if (textures != null && textures.Count > 0)
			{
				var pickedIndex = Random.Range(0, textures.Count);

				texture = textures[pickedIndex];
			}
		}
#if UNITY_EDITOR
		public override void DrawEditorLayout()
		{
			if (textures != null)
			{
				var removeIndex = -1;

				for (var i = 0; i < textures.Count; i++)
				{
					var texture = textures[i];

					EditorGUILayout.BeginHorizontal();
						texture = (Texture)EditorGUILayout.ObjectField(texture, typeof(Texture), true);
						if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(20)) == true)
						{
							removeIndex = i;
						}
					EditorGUILayout.EndHorizontal();

					textures[i] = texture;
				}

				if (removeIndex >= 0)
				{
					textures.RemoveAt(removeIndex);
				}
			}

			var rect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect());

			if (GUI.Button(rect, "Add Texture") == true)
			{
				if (textures == null)
				{
					textures = new List<Texture>();
				}

				textures.Add(default(Texture));
			}
		}
#endif
	}
}