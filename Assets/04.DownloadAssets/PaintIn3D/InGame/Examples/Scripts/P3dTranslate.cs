using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D.Examples
{
	/// <summary>This component allows you to move the current <b>Transform</b> using editor events (e.g. UI buttons).</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dTranslate")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Translate")]
	public class P3dTranslate : MonoBehaviour
	{
		/// <summary>This allows you to set the coordinate space the movement will use.</summary>
		public Space Space { set { space = value; } get { return space; } } [SerializeField] private Space space = Space.Self;

		/// <summary>The movement values will be multiplied by this before use.</summary>
		public float Multiplier { set { multiplier = value; } get { return multiplier; } } [SerializeField] private float multiplier = 1.0f;

		/// <summary>If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.
		/// -1 = Instantly change.
		/// 1 = Slowly change.
		/// 10 = Quickly change.</summary>
		public float Dampening { set { dampening = value; } get { return dampening; } } [SerializeField] private float dampening = 10.0f;

		[SerializeField]
		private Vector3 remainingDelta;

		/// <summary>This method allows you to translate along the X axis, with the specified value.</summary>
		public void TranslateX(float magnitude)
		{
			Translate(Vector3.right * magnitude);
		}

		/// <summary>This method allows you to translate along the Y axis, with the specified value.</summary>
		public void TranslateY(float magnitude)
		{
			Translate(Vector3.up * magnitude);
		}

		/// <summary>This method allows you to translate along the Z axis, with the specified value.</summary>
		public void TranslateZ(float magnitude)
		{
			Translate(Vector3.forward * magnitude);
		}

		/// <summary>This method allows you to translate along the specified vector.</summary>
		public void Translate(Vector3 vector)
		{
			if (Space == Space.Self)
			{
				vector = transform.TransformVector(vector);
			}

			TranslateWorld(vector);
		}

		/// <summary>This method allows you to translate along the specified vector in world space.</summary>
		public void TranslateWorld(Vector3 vector)
		{
			remainingDelta += vector * Multiplier;
		}

		protected virtual void Update()
		{
			var factor   = P3dHelper.DampenFactor(Dampening, Time.deltaTime);
			var newDelta = Vector3.Lerp(remainingDelta, Vector3.zero, factor);

			transform.position += remainingDelta - newDelta;

			remainingDelta = newDelta;
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dTranslate))]
	public class P3dTranslate_Editor : P3dEditor<P3dTranslate>
	{
		protected override void OnInspector()
		{
			Draw("space", "This allows you to set the coordinate space the movement will use.");
			Draw("multiplier", "The movement values will be multiplied by this before use.");
			Draw("dampening", "If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.\n\n-1 = Instantly change.\n\n1 = Slowly change.\n\n10 = Quickly change.");
		}
	}
}
#endif