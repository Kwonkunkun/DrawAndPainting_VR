using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component allows you to block paint from being applied at the current position using the specified shape.</summary>
	[ExecuteInEditMode]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dMask")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Mask")]
	public class P3dMask : P3dLinkedBehaviour<P3dMask>
	{
		/// <summary>The mask will use this texture shape.</summary>
		public Texture Shape { set { shape = value; } get { return shape; } } [SerializeField] private Texture shape;

		/// <summary>The mask will use pixels from this texture channel.</summary>
		public P3dChannel Channel { set { channel = value; } get { return channel; } } [SerializeField] private P3dChannel channel = P3dChannel.Alpha;

		/// <summary>If you want the sides of the mask to extend farther out, then this allows you to set the scale of the boundary.
		/// 1 = Default.
		/// 2 = Double size.</summary>
		public Vector2 Stretch { set { stretch = value; } get { return stretch; } } [SerializeField] private Vector2 stretch = Vector2.one;

		public Matrix4x4 Matrix
		{
			get
			{
				return transform.worldToLocalMatrix;
			}
		}

		public static P3dMask Find(Vector3 position)
		{
			var mask         = FirstInstance;
			var bestMask     = default(P3dMask);
			var bestDistance = float.PositiveInfinity;

			for (var i = 0; i < InstanceCount; i++)
			{
				var distance = Vector3.SqrMagnitude(position - mask.transform.position);

				if (distance < bestDistance)
				{
					bestMask = mask;
				}

				mask = mask.NextInstance;
			}

			return bestMask;
		}

#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.matrix = transform.localToWorldMatrix;

			Gizmos.DrawWireCube(Vector3.zero, new Vector3(1.0f, 1.0f, 0.0f));
			Gizmos.DrawWireCube(Vector3.zero, new Vector3(1.0f, 1.0f, 1.0f));
			Gizmos.DrawWireCube(Vector3.zero, new Vector3(stretch.x, stretch.y, 0.0f));
			Gizmos.DrawWireCube(Vector3.zero, new Vector3(stretch.x, stretch.y, 1.0f));
		}
#endif
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dMask))]
	public class P3dMask_Editor : P3dEditor<P3dMask>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Shape == null));
				Draw("shape", "The mask will use this texture shape.");
			EndError();
			Draw("channel", "The mask will use pixels from this texture channel.");
			Draw("stretch", "If you want the sides of the mask to extend farther out, then this allows you to set the scale of the boundary.\n\n1 = Default.\n\n2 = Double size.");
		}
	}
}
#endif