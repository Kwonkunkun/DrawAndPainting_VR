using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component moves the current <b>Transform</b> in a spiral pattern.</summary>
	[ExecuteInEditMode]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dSpiral")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Spiral")]
	public class P3dSpiral : MonoBehaviour
	{
		public Vector3 Position { set { position = value; } get { return position; } } [SerializeField] private Vector3 position;

		public Vector3 Rotation { set { rotation = value; } get { return rotation; } } [SerializeField] private Vector3 rotation;

		public float Radius { set { radius = value; } get { return radius; } } [SerializeField] private float radius = 10.0f;

		public float RadiusAngle { set { radiusAngle = value; } get { return radiusAngle; } } [SerializeField] private float radiusAngle;

		public float RadiusSpeed { set { radiusSpeed = value; } get { return radiusSpeed; } } [SerializeField] private float radiusSpeed = 5.0f;

		public float Offset { set { offset = value; } get { return offset; } } [SerializeField] private float offset = 1.0f;

		public float OffsetAngle { set { offsetAngle = value; } get { return offsetAngle; } } [SerializeField] private float offsetAngle;

		public float OffsetSpeed { set { offsetSpeed = value; } get { return offsetSpeed; } } [SerializeField] private float offsetSpeed = 1.0f;

		protected virtual void Update()
		{
			if (Application.isPlaying == true)
			{
				radiusAngle += radiusSpeed * Time.deltaTime;
				offsetAngle += offsetSpeed * Time.deltaTime;
			}

			var o = Mathf.Sin(offsetAngle * Mathf.Deg2Rad) * offset;
			var x = Mathf.Sin(radiusAngle * Mathf.Deg2Rad) * (radius + o);
			var z = Mathf.Cos(radiusAngle * Mathf.Deg2Rad) * (radius + o);

			var matrix = Matrix4x4.TRS(position, Quaternion.Euler(rotation), Vector3.one);

			transform.localPosition = matrix.MultiplyPoint(new Vector3(x, 0.0f, z));
		}
#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			var matrix = Matrix4x4.TRS(position, Quaternion.Euler(rotation), new Vector3(1.0f, 0.0f, 1.0f));

			if (transform.parent != null)
			{
				matrix = transform.localToWorldMatrix * matrix;
			}

			Gizmos.matrix = matrix;

			Gizmos.DrawWireSphere(Vector3.zero, radius - offset);
			Gizmos.DrawWireSphere(Vector3.zero, radius         );
			Gizmos.DrawWireSphere(Vector3.zero, radius + offset);
		}
#endif
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CustomEditor(typeof(P3dSpiral))]
	public class P3dSpiral_Editor : P3dEditor<P3dSpiral>
	{
		protected override void OnInspector()
		{
			Draw("position", "");
			Draw("rotation", "");

			Separator();

			Draw("radius", "");
			Draw("radiusAngle", "");
			Draw("radiusSpeed", "");
			
			Separator();

			Draw("offset", "");
			Draw("offsetAngle", "");
			Draw("offsetSpeed", "");
		}
	}
}
#endif