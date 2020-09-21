using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D.Examples
{
	/// <summary>This component allows you to define a color that can later be counted from the P3dColorCounter component.
	/// NOTE: Put this component its own GameObject, so you can give it a unique name.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dColor")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Color")]
	public class P3dColor : P3dLinkedBehaviour<P3dColor>
	{
		[SerializeField]
		private class Contribution
		{
			public P3dColorCounter Counter;
			public int             Solid;
		}

		/// <summary>The color associated with this component and GameObject name.</summary>
		public Color Color { set { color = value; } get { return color; } } [SerializeField] private Color color;

		[SerializeField]
		private List<Contribution> contributions;

		/// <summary>This tells you how many pixels this color could be painted on.</summary>
		public int Total
		{
			get
			{
				var total = 0;

				foreach (var colorCounter in P3dColorCounter.Instances)
				{
					total += colorCounter.Total;
				}

				return total;
			}
		}

		/// <summary>This tells you how many pixels this color has been painted on.</summary>
		public int Solid
		{
			get
			{
				var solid = 0;

				if (contributions != null)
				{
					for (var i = contributions.Count - 1; i >= 0; i--)
					{
						var contribution = contributions[i];

						if (contribution.Counter != null && contribution.Counter.isActiveAndEnabled == true)
						{
							solid += contribution.Solid;
						}
						else
						{
							contributions.RemoveAt(i);
						}
					}
				}

				return solid;
			}
		}

		/// <summary>This is Solid/Total, allowing you to quickly see the percentage of paintable pixels that have been painted by this color.</summary>
		public float Ratio
		{
			get
			{
				var total = Total;

				if (total > 0)
				{
					return Solid / (float)total;
				}

				return 0.0f;
			}
		}

		public void Contribute(P3dColorCounter counter, int solid)
		{
			var contribution = default(Contribution);

			if (TryGetContribution(counter, ref contribution) == false)
			{
				if (solid <= 0)
				{
					return;
				}

				contribution = new Contribution();

				contributions.Add(contribution);

				contribution.Counter = counter;
			}

			contribution.Solid = solid;
		}

		private bool TryGetContribution(P3dColorCounter counter, ref Contribution contribution)
		{
			if (contributions == null)
			{
				contributions = new List<Contribution>();
			}

			for (var i = contributions.Count - 1; i >= 0; i--)
			{
				contribution = contributions[i];

				if (contribution.Counter == counter)
				{
					return true;
				}
			}

			return false;
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D.Examples
{
	[CustomEditor(typeof(P3dColor))]
	public class P3dColor_Editor : P3dEditor<P3dColor>
	{
		protected override void OnInspector()
		{
			Draw("color", "The color associated with this component and GameObject name.");

			EditorGUILayout.Separator();

			EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.IntField(new GUIContent("Total", "This tells you how many pixels this color could be painted on."), Target.Total);
				var rect  = P3dHelper.Reserve();
				var rectL = rect; rectL.xMax -= (rect.width - EditorGUIUtility.labelWidth) / 2 + 1;
				var rectR = rect; rectR.xMin = rectL.xMax + 2;

				EditorGUI.IntField(rectL, new GUIContent("Solid", "This tells you how many pixels this color has been painted on."), Target.Solid);
				EditorGUI.ProgressBar(rectR, Target.Ratio, "Ratio");
			EditorGUI.EndDisabledGroup();
		}
	}
}
#endif