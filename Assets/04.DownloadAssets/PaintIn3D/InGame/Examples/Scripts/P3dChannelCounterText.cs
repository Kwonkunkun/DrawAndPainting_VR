using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D.Examples
{
	/// <summary>This component allows you to output the totals of all the specified pixel counters to a UI Text component.</summary>
	[RequireComponent(typeof(Text))]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dChannelCounterText")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Channel Counter Text")]
	public class P3dChannelCounterText : MonoBehaviour
	{
		public enum ChannelType
		{
			Red,
			Green,
			Blue,
			Alpha
		}

		/// <summary>This allows you to specify the counters that will be used.
		/// Zero = All active and enabled counters in the scene.</summary>
		public List<P3dChannelCounter> Counters { get { if (counters == null) counters = new List<P3dChannelCounter>(); return counters; } } [SerializeField] private List<P3dChannelCounter> counters;

		/// <summary>This allows you to choose which channel will be output to the UI Text.</summary>
		public ChannelType Channel { set { channel = value; } get { return channel; } } [SerializeField] private ChannelType channel;

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
			var total         = P3dChannelCounter.GetTotal(finalCounters);
			var count         = default(long);

			switch (channel)
			{
				case ChannelType.Red:   count = P3dChannelCounter.GetCountR(finalCounters); break;
				case ChannelType.Green: count = P3dChannelCounter.GetCountG(finalCounters); break;
				case ChannelType.Blue:  count = P3dChannelCounter.GetCountB(finalCounters); break;
				case ChannelType.Alpha: count = P3dChannelCounter.GetCountA(finalCounters); break;
			}

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
	[CustomEditor(typeof(P3dChannelCounterText))]
	public class P3dChannelCounterText_Editor : P3dEditor<P3dChannelCounterText>
	{
		protected override void OnInspector()
		{
			Draw("counters", "This allows you to specify the counters that will be used.\n\nZero = All active and enabled counters in the scene.");

			Separator();

			Draw("channel", "This allows you to choose which channel will be output to the UI Text.");
			Draw("inverse", "Inverse the Count and Percent values?");
			Draw("decimalPlaces", "This allows you to set the amount of decimal places when using the percentage output.");
			Draw("format", "This allows you to set the format of the team text. You can use the following tokens:\n\n{TOTAL} = Total amount of pixels that can be painted.\n\n{COUNT} = Total amount of pixel that have been painted.\n\n{PERCENT} = Percentage of pixels that have been painted.");
		}
	}
}
#endif