using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component constantly draws lines between the two specified points.</summary>
	[ExecuteInEditMode]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dHitThrough")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Hit/Hit Through")]
	public class P3dHitThrough : P3dConnectableLines
	{
		public enum PhaseType
		{
			Update,
			FixedUpdate
		}

		public enum OrientationType
		{
			WorldUp,
			CameraUp
		}

		/// <summary>Where in the game loop should this component hit?</summary>
		public PhaseType PaintIn { set { paintIn = value; } get { return paintIn; } } [SerializeField] private PhaseType paintIn;

		/// <summary>The time in seconds between each hit.
		/// 0 = Every frame.
		/// -1 = Manual only.</summary>
		public float Interval { set { interval = value; } get { return interval; } } [UnityEngine.Serialization.FormerlySerializedAs("delay")] [SerializeField] private float interval = 0.05f;

		/// <summary>The start point of the raycast.</summary>
		public Transform PointA { set { pointA = value; } get { return pointA; } } [SerializeField] private Transform pointA;

		/// <summary>The end point of the raycast.</summary>
		public Transform PointB { set { pointB = value; } get { return pointB; } } [SerializeField] private Transform pointB;

		/// <summary>How should the hit point be oriented?
		/// WorldUp = It will be rotated to the normal, where the up vector is world up.
		/// CameraUp = It will be rotated to the normal, where the up vector is world up.</summary>
		public OrientationType Orientation { set { orientation = value; } get { return orientation; } } [SerializeField] private OrientationType orientation;

		/// <summary>Orient to a specific camera?
		/// None = MainCamera.</summary>
		public Camera Camera { set { _camera = value; } get { return _camera; } } [SerializeField] private Camera _camera;

		/// <summary>This allows you to control the pressure of the painting. This could be controlled by a VR trigger or similar for more advanced effects.</summary>
		public float Pressure { set { pressure = value; } get { return pressure; } } [Range(0.0f, 1.0f)] [SerializeField] private float pressure = 1.0f;

		/// <summary>Should the applied paint be applied as a preview?</summary>
		public bool Preview { set { preview = value; } get { return preview; } } [SerializeField] private bool preview;

		/// <summary>This allows you to override the order this paint gets applied to the object during the current frame.</summary>
		public int Priority { set { priority = value; } get { return priority; } } [SerializeField] private int priority;

		/// <summary>If you want to draw a line between the start point and the his point then you can set the line here.</summary>
		public LineRenderer Line { set { line = value; } get { return line; } } [SerializeField] private LineRenderer line;

		[System.NonSerialized]
		private float current;

		/// <summary>This method will immediately submit a non-preview hit. This can be used to apply real paint to your objects.</summary>
		[ContextMenu("Manually Hit Now")]
		public void ManuallyHitNow()
		{
			SubmitHit(false);
		}

		protected override void Update()
		{
			base.Update();

			if (preview == true)
			{
				SubmitHit(true);
			}
			else if (paintIn == PhaseType.Update)
			{
				UpdateHit();
			}
		}

		protected virtual void LateUpdate()
		{
			UpdatePointAndLine();
		}

		protected virtual void FixedUpdate()
		{
			if (preview == false && paintIn == PhaseType.FixedUpdate)
			{
				UpdateHit();
			}
		}

		private void SubmitHit(bool preview)
		{
			if (pointA != null && pointB != null)
			{
				var camera    = P3dHelper.GetCamera(_camera);
				var positionA = pointA.position;
				var positionB = pointB.position;
				var finalUp   = orientation == OrientationType.CameraUp && camera != null ? camera.transform.up : Vector3.up;
				var rotation  = Quaternion.LookRotation(positionB - positionA, finalUp);

				SubmitLine(preview, priority, pointA.position, pointB.position, rotation, pressure, this);
			}
		}

		private void UpdateHit()
		{
			current += Time.deltaTime;

			if (interval > 0.0f)
			{
				if (current >= interval)
				{
					current %= interval;

					SubmitHit(false);
				}
			}
			else if (interval == 0.0f)
			{
				SubmitHit(false);
			}
		}

		private void UpdatePointAndLine()
		{
			if (pointA != null && pointB != null)
			{
				var a = pointA.position;
				var b = pointB.position;

				if (line != null)
				{
					line.positionCount = 2;

					line.SetPosition(0, a);
					line.SetPosition(1, b);
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dHitThrough))]
	public class P3dHitThrough_Editor : P3dConnectableLines_Editor<P3dHitThrough>
	{
		protected override void OnInspector()
		{
			Draw("paintIn", "Where in the game loop should this component hit?");
			Draw("interval", "The time in seconds between each hit.\n\n0 = Every frame.\n\n-1 = Manual only.");

			Separator();

			BeginError(Any(t => t.PointA == null));
				Draw("pointA", "The start point of the raycast.");
			EndError();
			BeginError(Any(t => t.PointB == null));
				Draw("pointB", "The end point of the raycast.");
			EndError();
			Draw("orientation", "How should the hit point be oriented?\n\nWorldUp = It will be rotated to the normal, where the up vector is world up.\n\nCameraUp = It will be rotated to the normal, where the up vector is world up.");
			BeginIndent();
				if (Any(t => t.Orientation == P3dHitThrough.OrientationType.CameraUp))
				{
					Draw("_camera", "Orient to a specific camera?\nNone = MainCamera.");
				}
			EndIndent();

			base.OnInspector();

			Separator();

			Draw("preview", "Should the applied paint be applied as a preview?");
			Draw("priority", "This allows you to override the order this paint gets applied to the object during the current frame.");
			Draw("pressure", "This allows you to control the pressure of the painting. This could be controlled by a VR trigger or similar for more advanced effects.");

			Separator();

			Draw("line", "If you want to draw a line between the start point and the his point then you can set the line here");

			Separator();

			var line = true;
			var quad = line == true && Target.ConnectHits == true;

			Target.HitCache.Inspector(Target.gameObject, line: line, quad: quad);
		}
	}
}
#endif