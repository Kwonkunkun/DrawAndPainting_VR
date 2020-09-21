using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component implements the replace paint mode, which will replace all pixels in the specified texture.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintReplace")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Paint/Paint Replace")]
	public class P3dPaintReplace : MonoBehaviour, IHit, IHitCoord
	{
		/// <summary>Only the <b>P3dPaintableTexture</b> components with a matching group will be painted by this component.</summary>
		public P3dGroup Group { set { group = value; } get { return group; } } [SerializeField] private P3dGroup group;

		/// <summary>The texture that will be painted.</summary>
		public Texture Texture { set { texture = value; } get { return texture; } } [SerializeField] private Texture texture;

		/// <summary>The color of the paint.</summary>
		public Color Color { set { color = value; } get { return color; } } [SerializeField] private Color color = Color.white;

		/// <summary>The color of the paint.</summary>
		public Vector4 Channels { set { channels = value; } get { return channels; } } [SerializeField] private Vector4 channels = Vector4.one;

		/// <summary>This stores a list of all modifiers used to change the way this component applies paint (e.g. <b>P3dModifyColorRandom</b>).</summary>
		public P3dModifierList Modifiers { get { if (modifiers == null) modifiers = new P3dModifierList(); return modifiers; } } [SerializeField] private P3dModifierList modifiers;

		public void HandleHitCoord(bool preview, int priority, float pressure, int seed, P3dHit hit, Quaternion rotation)
		{
			var model = hit.Root.GetComponentInParent<P3dModel>();

			if (model != null)
			{
				var paintableTextures = P3dPaintableTexture.FilterAll(model, group);

				if (paintableTextures.Count > 0)
				{
					var finalColor   = color;
					var finalTexture = texture;

					if (modifiers != null && modifiers.Count > 0)
					{
						P3dHelper.BeginSeed(seed);
							modifiers.ModifyColor(ref finalColor, preview, pressure);
							modifiers.ModifyTexture(ref finalTexture, preview, pressure);
						P3dHelper.EndSeed();
					}

					P3dCommandReplace.Instance.SetState(preview, priority);
					P3dCommandReplace.Instance.SetMaterial(finalTexture, finalColor, channels);

					for (var i = paintableTextures.Count - 1; i >= 0; i--)
					{
						var paintableTexture = paintableTextures[i];

						P3dPaintableManager.Submit(P3dCommandReplace.Instance, model, paintableTexture);
					}
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dPaintReplace))]
	public class P3dPaintReplace_Editor : P3dEditor<P3dPaintReplace>
	{
		protected override void OnInspector()
		{
			Draw("group", "Only the P3dPaintableTexture components with a matching group will be painted by this component.");

			Separator();

			Draw("texture", "The texture that will be painted.");
			Draw("color", "The color of the paint.");

			Separator();

			Target.Modifiers.DrawEditorLayout(serializedObject, target, "Color", "Texture");
		}
	}
}
#endif