using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D.Examples
{
	/// <summary>This component fills the attached UI Image based on the total amount of pixels that have been painted in the specified <b>P3dChangeCounterFill</b> components.</summary>
	[RequireComponent(typeof(Image))]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dChangeCounterFill")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Change Counter Fill")]
	public class P3dChangeCounterFill : MonoBehaviour
	{
		/// <summary>This allows you to specify the counters that will be used.
		/// Zero = All active and enabled counters in the scene.</summary>
		public List<P3dChangeCounter> Counters { get { if (counters == null) counters = new List<P3dChangeCounter>(); return counters; } } [SerializeField] private List<P3dChangeCounter> counters;

		/// <summary>Inverse the fill?</summary>
		public bool Inverse { set { inverse = value; } get { return inverse; } } [SerializeField] private bool inverse;

		[System.NonSerialized]
		private Image cachedImage;

		protected virtual void OnEnable()
		{
			cachedImage = GetComponent<Image>();
		}

		protected virtual void Update()
		{
			var finalCounters = counters.Count > 0 ? counters : null;
			var ratio         = P3dChangeCounter.GetRatio(finalCounters);

			if (inverse == true)
			{
				ratio = 1.0f - ratio;
			}

			cachedImage.fillAmount = Mathf.Clamp01(ratio);
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dChangeCounterFill))]
	public class P3dChangeCounterFill_Editor : P3dEditor<P3dChangeCounterFill>
	{
		protected override void OnInspector()
		{
			Draw("counters", "This allows you to specify the counters that will be used.\n\nZero = All active and enabled counters in the scene.");

			Separator();

			Draw("inverse", "Inverse the fill?");
		}
	}
}
#endif