using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component grabs paint hits and connected hits, mirrors the data, then re-broadcasts it.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dCloneMirror")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Transform/Clone Mirror")]
	public class P3dCloneMirror : P3dClone
	{
		/// <summary>When a decal is mirrored it will appear backwards, should it be flipped back around?</summary>
		public bool Flip { set { flip = value; } get { return flip; } } [SerializeField] private bool flip;

		public override void Transform(ref Matrix4x4 posMatrix, ref Matrix4x4 rotMatrix)
		{
			var p   = transform.position;
			var r   = transform.rotation;
			var s   = Matrix4x4.Scale(new Vector3(1.0f, 1.0f, -1.0f));
			var tp  = Matrix4x4.Translate(p);
			var rp  = Matrix4x4.Rotate(r);
			var ti  = Matrix4x4.Translate(-p);
			var ri  = Matrix4x4.Rotate(Quaternion.Inverse(r));
			var mat = tp * rp * s * ri * ti;

			if (flip == true)
			{
				posMatrix = mat * posMatrix;
			}
			else
			{
				posMatrix = mat * posMatrix;
				rotMatrix = mat * rotMatrix;
			}
		}
#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.matrix = transform.localToWorldMatrix;

			for (var i = 1; i <= 10; i++)
			{
				Gizmos.DrawWireCube(Vector3.zero, new Vector3(i, i, 0.0f));
			}
		}
#endif
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dCloneMirror))]
	public class P3dCloneMirror_Editor : P3dEditor<P3dCloneMirror>
	{
		protected override void OnInspector()
		{
			Draw("flip", "When a decal is mirrored it will appear backwards, should it be flipped back around?");
		}
	}
}
#endif