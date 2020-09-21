using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D.Examples
{
	/// <summary>This component will output the total pixels for the specified team to a UI Text component.</summary>
	[RequireComponent(typeof(Text))]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dColorCounterText")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Color Counter Text")]
	public class P3dColorCounterText : MonoBehaviour
	{
		/// <summary>This allows you to specify the counters that will be used.
		/// Zero = All active and enabled counters in the scene.</summary>
		public List<P3dColorCounter> Counters { get { if (counters == null) counters = new List<P3dColorCounter>(); return counters; } } [SerializeField] private List<P3dColorCounter> counters;

		/// <summary>This allows you to set which color will be handled by this component.</summary>
		public P3dColor Color { set { color = value; } get { return color; } } [SerializeField] private P3dColor color;

		/// <summary>Inverse the <b>Count</b> and <b>Percent</b> values?</summary>
		public bool Inverse { set { inverse = value; } get { return inverse; } } [SerializeField] private bool inverse;

		/// <summary>This allows you to set the amount of decimal places when using the percentage output.</summary>
		public int DecimalPlaces { set { decimalPlaces = value; } get { return decimalPlaces; } } [SerializeField] private int decimalPlaces;

		/// <summary>This allows you to set the format of the team text. You can use the following tokens:
		/// {TOTAL} = Total amount of pixels that can be painted.
		/// {COUNT} = Total amount of pixel that have been painted.
		/// {PERCENT} = Percentage of pixels that have been painted.</summary>
		public string Format { set { format = value; } get { return format; } } [Multiline] [SerializeField] private string format = "{PERCENT}";

		[System.NonSerialized]
		private Text cachedText;

		protected virtual void OnEnable()
		{
			cachedText = GetComponent<Text>();
		}

		protected virtual void Update()
		{
			var finalCounters = counters.Count > 0 ? counters : null;
			var total         = P3dColorCounter.GetTotal(finalCounters);
			var count         = P3dColorCounter.GetCount(color, finalCounters);

			if (inverse == true)
			{
				count = total - count;
			}

			var final   = format;
			var percent = P3dHelper.RatioToPercentage(P3dHelper.Divide(count, total), decimalPlaces);

			final = final.Replace("{TOTAL}", total.ToString());
			final = final.Replace("{COUNT}", count.ToString());
			final = final.Replace("{PERCENT}", percent.ToString());

			cachedText.text = final;
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dColorCounterText))]
	public class P3dTeamText_Editor : P3dEditor<P3dColorCounterText>
	{
		protected override void OnInspector()
		{
			Draw("counters", "This allows you to specify the counters that will be used.\n\nZero = All active and enabled counters in the scene.");

			Separator();

			BeginError(Any(t => t.Color == null));
				Draw("color", "This allows you to set which color will be handled by this component.");
			EndError();
			Draw("inverse", "Inverse the Count and Percent values?");
			Draw("decimalPlaces", "This allows you to set the amount of decimal places when using the percentage output.");
			Draw("format", "This allows you to set the format of the team text. You can use the following tokens:\n\n{COLOR} = Name of the color.\n\n{TOTAL} = Total amount of pixels that can be painted.\n\n{COUNT} = Total amount of pixel that have been painted.\n\n{PERCENT} = Percentage of pixels that have been painted.");
		}
	}
}
#endif