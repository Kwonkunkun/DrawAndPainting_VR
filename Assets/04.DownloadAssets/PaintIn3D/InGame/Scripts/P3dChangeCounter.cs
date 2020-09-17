using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D.Examples
{
	/// <summary>This component will total up all RGBA channels in the specified P3dPaintableTexture that exceed the threshold value.</summary>
	[ExecuteInEditMode]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dChangeCounter")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Change Counter")]
	public class P3dChangeCounter : P3dPaintableTextureMonitorMask
	{
		/// <summary>This stores all active and enabled instances.</summary>
		public static LinkedList<P3dChangeCounter> Instances = new LinkedList<P3dChangeCounter>(); private LinkedListNode<P3dChangeCounter> node;

		/// <summary>The RGBA values must be within this range of a color for it to be counted.</summary>
		public float Threshold { set { threshold = value; } get { return threshold; } } [Range(0.0f, 1.0f)] [SerializeField] private float threshold = 0.1f;

		/// <summary>The texture we want to compare change to.
		/// None/null = white.
		/// NOTE: All pixels in this texture will be tinted by the current <b>Color</b>.</summary>
		public Texture Texture { set { texture = value; } get { return texture; } } [SerializeField] private Texture texture;

		/// <summary>The color we want to compare change to.
		/// NOTE: All pixels in the <b>Texture</b> will be tinted by this.</summary>
		public Color Color { set { color = value; } get { return color; } } [SerializeField] private Color color = Color.white;

		/// <summary>The previously counted amount of pixels with a RGBA value difference above the threshold.</summary>
		public int Count { get { return count; } } [SerializeField] private int count;

		/// <summary>The <b>Count / Total</b> value.</summary>
		public float Ratio { get { return total > 0 ? count / (float)total : 0.0f; } }

		[SerializeField]
		private bool changeDirty;

		[SerializeField]
		private P3dReader changeReader;

		[SerializeField]
		protected NativeArray<Color32> changePixels;

		/// <summary>The <b>Total</b> of the specified counters.</summary>
		public static long GetTotal(ICollection<P3dChangeCounter> counters = null)
		{
			var total = 0L; foreach (var counter in counters ?? Instances) { total += counter.total; } return total;
		}

		/// <summary>The <b>Count</b> of the specified counters.</summary>
		public static long GetCount(ICollection<P3dChangeCounter> counters = null)
		{
			var solid = 0L; foreach (var counter in counters ?? Instances) { solid += counter.count; } return solid;
		}

		/// <summary>The <b>Ratio</b> of the specified counters.</summary>
		public static float GetRatio(ICollection<P3dChangeCounter> counters = null)
		{
			return P3dHelper.Divide(GetCount(counters), GetTotal(counters));
		}

		private void HandleCompleteChange(NativeArray<Color32> pixels)
		{
			if (changePixels.IsCreated == true && changePixels.Length != pixels.Length)
			{
				changePixels.Dispose();
			}

			if (changePixels.IsCreated == false)
			{
				changePixels = new NativeArray<Color32>(pixels.Length, Allocator.Persistent);
			}

			if (changePixels.IsCreated == true)
			{
				NativeArray<Color32>.Copy(pixels, changePixels);
			}
			else
			{
				changePixels = new NativeArray<Color32>(pixels, Allocator.Persistent);
			}

			HandleComplete(changeReader.DownsampleBoost);
		}

		protected override void HandleComplete(int boost)
		{
			if (currentPixels.IsCreated == false || maskPixels.IsCreated == false || changePixels.IsCreated == false || currentPixels.Length != maskPixels.Length || currentPixels.Length != changePixels.Length)
			{
				return;
			}

			var threshold32 = (byte)(threshold * 255.0f);

			count = 0;
			total = 0;

			for (var i = 0; i < currentPixels.Length; i++)
			{
				if (maskPixels[i] > 127)
				{
					total++;

					var currentPixel = currentPixels[i];
					var changePixel  = changePixels[i];
					var distance     = 0;

					distance += System.Math.Abs(changePixel.r - currentPixel.r);
					distance += System.Math.Abs(changePixel.g - currentPixel.g);
					distance += System.Math.Abs(changePixel.b - currentPixel.b);
					distance += System.Math.Abs(changePixel.a - currentPixel.a);

					if (distance <= threshold32)
					{
						count++;
					}
				}
			}

			total *= boost;
			count *= boost;

			InvokeOnUpdated();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			node = Instances.AddLast(this);

			if (changeReader != null)
			{
				changeReader.OnComplete += HandleCompleteChange;
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			Instances.Remove(node); node = null;

			if (changeReader != null)
			{
				changeReader.OnComplete -= HandleCompleteChange;
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			if (changeReader != null)
			{
				changeReader.Release();
			}

			if (changePixels.IsCreated == true)
			{
				changePixels.Dispose();
			}
		}

		protected override void Update()
		{
			base.Update();

			if (changeReader == null)
			{
				changeReader = new P3dReader();

				changeReader.OnComplete += HandleCompleteChange;
			}

			if (changeReader.Requested == false && registeredPaintableTexture != null && registeredPaintableTexture.Activated == true)
			{
				if (P3dReader.NeedsUpdating(changeReader, changePixels, registeredPaintableTexture.Current, downsampleSteps) == true || changeDirty == true)
				{
					changeDirty = false;

					var desc          = registeredPaintableTexture.Current.descriptor; desc.useMipMap = false;
					var renderTexture = P3dHelper.GetRenderTexture(desc);

					P3dCommandReplace.Blit(renderTexture, texture, color);

					// Request new change
					changeReader.Request(renderTexture, DownsampleSteps, Async);

					P3dHelper.ReleaseRenderTexture(renderTexture);
				}
			}

			changeReader.UpdateRequest();
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D.Examples
{
	[CustomEditor(typeof(P3dChangeCounter))]
	public class P3dChangeCounter_Editor : P3dPaintableTextureMonitorMask_Editor<P3dChangeCounter>
	{
		protected override void OnInspector()
		{
			base.OnInspector();

			Separator();

			Draw("threshold", "The RGBA value must be higher than this for it to be counted.");
			DrawTexture();
			DrawColor();

			Separator();

			BeginDisabled();
				EditorGUILayout.IntField("Total", Target.Total);

				DrawChannel("count", "Ratio ", Target.Ratio);
			EndDisabled();
		}

		private void DrawTexture()
		{
			EditorGUILayout.BeginHorizontal();
				Draw("texture", "The texture we want to compare change to.\n\nNone/null = white.\n\nNOTE: All pixels in this texture will be tinted by the current Color.");
				EditorGUI.BeginDisabledGroup(All(t => t.PaintableTexture == null || t.PaintableTexture.Texture == t.Texture));
					if (GUILayout.Button("Copy", EditorStyles.miniButton, GUILayout.ExpandWidth(false)) == true)
					{
						Undo.RecordObjects(targets, "Copy Texture"); Each(t => { if (t.PaintableTexture != null) { t.Texture = t.PaintableTexture.Texture; EditorUtility.SetDirty(t); } });
					}
				EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
		}

		private void DrawColor()
		{
			EditorGUILayout.BeginHorizontal();
				Draw("color", "The color we want to compare change to.\n\nNOTE: All pixels in the Texture will be tinted by this.");
				EditorGUI.BeginDisabledGroup(All(t => t.PaintableTexture == null || t.PaintableTexture.Color == t.Color));
					if (GUILayout.Button("Copy", EditorStyles.miniButton, GUILayout.ExpandWidth(false)) == true)
					{
						Undo.RecordObjects(targets, "Copy Color"); Each(t => { if (t.PaintableTexture != null) { t.Color = t.PaintableTexture.Color; EditorUtility.SetDirty(t); } });
					}
				EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
		}

		private void DrawChannel(string changeTitle, string ratioTitle, float ratio)
		{
			var rect  = P3dHelper.Reserve();
			var rectL = rect; rectL.xMax -= (rect.width - EditorGUIUtility.labelWidth) / 2 + 1;
			var rectR = rect; rectR.xMin = rectL.xMax + 2;

			EditorGUI.PropertyField(rectL, serializedObject.FindProperty(changeTitle));
			EditorGUI.ProgressBar(rectR, ratio, ratioTitle);
		}
	}
}
#endif