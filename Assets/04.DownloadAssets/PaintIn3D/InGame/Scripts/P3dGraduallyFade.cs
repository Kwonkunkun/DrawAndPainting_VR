using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D.Examples
{
	/// <summary>This component allows you to fade the pixels of the specified P3dPaintableTexture.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dGraduallyFade")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Gradually Fade")]
	public class P3dGraduallyFade : MonoBehaviour
	{
		/// <summary>This is the paintable texture whose pixels we will fade.</summary>
		public P3dPaintableTexture PaintableTexture { set { paintableTexture = value; } get { return paintableTexture; } } [SerializeField] private P3dPaintableTexture paintableTexture;

		/// <summary>This component will paint using this blending mode.
		/// NOTE: See <b>P3dBlendMode</b> documentation for more information.</summary>
		public P3dBlendMode BlendMode { set { blendMode = value; } get { return blendMode; } } [SerializeField] private P3dBlendMode blendMode = P3dBlendMode.ReplaceOriginal(Vector4.one);

		/// <summary>The texture that will be faded toward.</summary>
		public Texture Texture { set { texture = value; } get { return texture; } } [SerializeField] private Texture texture;

		/// <summary>The color that will be faded toward.</summary>
		public Color Color { set { color = value; } get { return color; } } [SerializeField] private Color color = Color.white;

		/// <summary>Once this component has accumilated this amount of fade, it will be applied to the <b>PaintableTexture</b>. The lower this value, the smoother the fading will appear, but also the higher the performance cost.</summary>
		public float Threshold { set { threshold = value; } get { return threshold; } } [Range(0.0f, 1.0f)] [SerializeField] private float threshold = 0.02f;

		/// <summary>The speed of the fading.
		/// 1 = 1 Second.
		/// 2 = 0.5 Seconds.</summary>
		public float Speed { set { speed = value; } get { return speed; } } [SerializeField] private float speed = 1.0f;

		[SerializeField]
		private float counter;

		protected virtual void Update()
		{
			if (paintableTexture != null && paintableTexture.Activated == true)
			{
				if (speed > 0.0f)
				{
					counter += speed * Time.deltaTime;
				}

				if (counter >= threshold)
				{
					var step = Mathf.FloorToInt(counter * 255.0f);

					if (step > 0)
					{
						var change = step / 255.0f;

						counter -= change;

						P3dCommandFill.Instance.SetState(false, 0);
						P3dCommandFill.Instance.SetMaterial(blendMode, texture, color, Mathf.Min(change, 1.0f), Mathf.Min(change, 1.0f));

						P3dPaintableManager.Submit(P3dCommandFill.Instance, paintableTexture.CachedPaintable, paintableTexture);
					}
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dGraduallyFade))]
	public class P3dGraduallyFade_Editor : P3dEditor<P3dGraduallyFade>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.PaintableTexture == null));
				Draw("paintableTexture", "This is the paintable texture whose pixels we will count.");
			EndError();
			Draw("blendMode", "This component will paint using this blending mode.\n\nNOTE: See P3dBlendMode documentation for more information.");
			Draw("texture", "The texture that will be faded toward.");
			Draw("color", "The color that will be faded toward.");

			Separator();

			BeginError(Any(t => t.Threshold <= 0.0f));
				Draw("threshold", "Once this component has accumilated this amount of fade, it will be applied to the PaintableTexture. The lower this value, the smoother the fading will appear, but also the higher the performance cost.");
			EndError();
			BeginError(Any(t => t.Speed <= 0.0f));
				Draw("speed", "The speed of the fading.\n\n1 = 1 Second.\n\n2 = 0.5 Seconds.");
			EndError();
		}
	}
}
#endif