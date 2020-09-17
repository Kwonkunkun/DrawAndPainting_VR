using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component automatically updates all P3dModel and P3dPaintableTexture instances at the end of the frame, batching all paint operations together.</summary>
	[DefaultExecutionOrder(100)]
	[DisallowMultipleComponent]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintableManager")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Paintable Manager")]
	public class P3dPaintableManager : P3dLinkedBehaviour<P3dPaintableManager>
	{
		public static P3dPaintableManager GetOrCreateInstance()
		{
			if (InstanceCount == 0)
			{
				var paintableManager = new GameObject(typeof(P3dPaintableManager).Name);

				paintableManager.hideFlags = HideFlags.DontSave;
				
				paintableManager.AddComponent<P3dPaintableManager>();
			}

			return FirstInstance;
		}

		public static void SubmitAll(P3dCommand command, Vector3 position, float radius, int layerMask, P3dGroup group, P3dModel targetModel, P3dPaintableTexture targetTexture)
		{
			DoSubmitAll(command, position, radius, layerMask, group, targetModel, targetTexture);

			// Repeat paint?
			P3dClone.BuildCloners();

			for (var c = 0; c < P3dClone.ClonerCount; c++)
			{
				for (var m = 0; m < P3dClone.MatrixCount; m++)
				{
					var copy = command.SpawnCopy();

					P3dClone.Clone(copy, c, m);

					DoSubmitAll(copy, position, radius, layerMask, group, targetModel, targetTexture);

					copy.Pool();
				}
			}
		}

		private static void DoSubmitAll(P3dCommand command, Vector3 position, float radius, int layerMask, P3dGroup group, P3dModel targetModel, P3dPaintableTexture targetTexture)
		{
			if (targetModel != null)
			{
				if (targetTexture != null)
				{
					Submit(command, targetModel, targetTexture);
				}
				else
				{
					SubmitAll(command, targetModel, group);
				}
			}
			else
			{
				if (targetTexture != null)
				{
					Submit(command, targetTexture.CachedPaintable, targetTexture);
				}
				else
				{
					SubmitAll(command, position, radius, layerMask, group);
				}
			}
		}

		private static void SubmitAll(P3dCommand command, Vector3 position, float radius, int layerMask, P3dGroup group)
		{
			var models = P3dModel.FindOverlap(position, radius, layerMask);

			for (var i = models.Count - 1; i >= 0; i--)
			{
				SubmitAll(command, models[i], group);
			}
		}

		private static void SubmitAll(P3dCommand command, P3dModel model, P3dGroup group)
		{
			var paintableTextures = P3dPaintableTexture.FilterAll(model, group);

			for (var i = paintableTextures.Count - 1; i >= 0; i--)
			{
				Submit(command, model, paintableTextures[i]);
			}
		}

		public static void Submit(P3dCommand command, P3dModel model, P3dPaintableTexture paintableTexture)
		{
			var copy = command.SpawnCopy();

			if (copy.Blend.Index == P3dBlendMode.REPLACE_ORIGINAL)
			{
				copy.Blend.Color   = paintableTexture.Color;
				copy.Blend.Texture = paintableTexture.Texture;
			}

			copy.Model   = model;
			copy.Submesh = model.GetSubmesh(paintableTexture);

			paintableTexture.AddCommand(copy);
		}

		protected virtual void LateUpdate()
		{
			if (this == FirstInstance && P3dModel.InstanceCount > 0)
			{
				ClearAll();
				UpdateAll();
			}
			else
			{
				P3dHelper.Destroy(gameObject);
			}
		}

		private void ClearAll()
		{
			var model = P3dModel.FirstInstance;

			for (var i = 0; i < P3dModel.InstanceCount; i++)
			{
				model.Prepared = false;

				model = model.NextInstance;
			}
		}

		private void UpdateAll()
		{
			var paintableTexture = P3dPaintableTexture.FirstInstance;

			for (var i = 0; i < P3dPaintableTexture.InstanceCount; i++)
			{
				paintableTexture.ExecuteCommands(true);

				paintableTexture = paintableTexture.NextInstance;
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dPaintableManager))]
	public class P3dPaintableManager_Editor : P3dEditor<P3dPaintableManager>
	{
		protected override void OnInspector()
		{
			EditorGUILayout.HelpBox("This component automatically updates all P3dModel and P3dPaintableTexture instances at the end of the frame, batching all paint operations together.", MessageType.Info);
		}
	}
}
#endif