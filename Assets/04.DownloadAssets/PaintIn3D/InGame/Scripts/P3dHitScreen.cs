using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component will perform a raycast under the mouse or finger as it moves across the screen. It will then send hit events to components like <b>P3dPaintDecal</b>, allowing you to paint the scene.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dHitScreen")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Hit/Hit Screen")]
	public class P3dHitScreen : P3dConnectablePoints
	{
		// This stores extra information for each finger unique to this component
		class Link
		{
			public object        Owner;
			public float         Distance;
			public Rect          Area;
			public List<Vector2> History = new List<Vector2>();

			public void Record(Vector2 point)
			{
				if (History.Count == 0)
				{
					Area = new Rect(point, Vector2.zero);
				}
				else
				{
					Area.min = Vector2.Min(Area.min, point);
					Area.max = Vector2.Max(Area.max, point);
				}

				History.Add(point);
			}

			public void Clear()
			{
				Owner    = null;
				Distance = 0.0f;

				History.Clear();
			}
		}

		public enum RotationType
		{
			Normal,
			World,
			ThisRotation,
			ThisLocalRotation,
			CustomRotation,
			CustomLocalRotation
		}

		public enum RelativeType
		{
			WorldUp,
			CameraUp
		}

		public enum DirectionType
		{
			HitNormal,
			RayDirection,
			CameraDirection
		}

		public enum DrawType
		{
			PointsIn3D       = 0,
			PointsIn3DFilled = 10,
			PointsOnUV       = 20,
			TrianglesIn3D    = 30
		}

		/// <summary>Orient to a specific camera?
		/// None = MainCamera.</summary>
		public Camera Camera { set { _camera = value; } get { return _camera; } } [SerializeField] private Camera _camera;

		/// <summary>If you want the paint to continuously apply while moving the mouse, this allows you to set how many pixels are between each step.
		/// 0 = Paint once when released.
		/// -1 = Paint every frame.</summary>
		public float Spacing { set { spacing = value; } get { return spacing; } } [SerializeField] private float spacing = 5.0f;

		/// <summary>The layers you want the raycast to hit.</summary>
		public LayerMask Layers { set { layers = value; } get { return layers; } } [SerializeField] private LayerMask layers = Physics.DefaultRaycastLayers;

		/// <summary>The key that must be held for this component to activate on desktop platforms.
		/// None = Any mouse button.</summary>
		public KeyCode Key { set { key = value; } get { return key; } } [SerializeField] private KeyCode key = KeyCode.Mouse0;

		/// <summary>This allows you to control how the paint is rotated.
		/// Normal = The rotation will be based on a normal direction, and rolled relative to an up axis.
		/// World = The rotation will be aligned to the world, or given no rotation.
		/// ThisRotation = The current <b>Transform.rotation</b> will be used.
		/// ThisLocalRotation = The current <b>Transform.localRotation</b> will be used.
		/// CustomRotation = The specified <b>Transform.rotation</b> will be used.
		/// CustomLocalRotation = The specified <b>Transform.localRotation</b> will be used.</summary>
		public RotationType RotateTo { set { rotateTo = value; } get { return rotateTo; } } [SerializeField] private RotationType rotateTo;

		/// <summary>Which direction should the hit point rotation be based on?</summary>
		public DirectionType NormalDirection { set { normalDirection = value; } get { return normalDirection; } } [UnityEngine.Serialization.FormerlySerializedAs("normal")] [SerializeField] private DirectionType normalDirection = DirectionType.CameraDirection;

		/// <summary>Based on the normal direction, what should the rotation be rolled relative to?
		/// WorldUp = It will be rolled so the up vector is world up.
		/// CameraUp = It will be rolled so the up vector is camera up.</summary>
		public RelativeType NormalRelativeTo { set { normalRelativeTo = value; } get { return normalRelativeTo; } } [UnityEngine.Serialization.FormerlySerializedAs("orientation")] [SerializeField] private RelativeType normalRelativeTo = RelativeType.CameraUp;

		/// <summary>This allows you to specify the <b>Transform</b> when using <b>RotateTo = CustomRotation/CustomLocalRotation</b>.</summary>
		public Transform CustomTransform { set { customTransform = value; } get { return customTransform; } } [SerializeField] private Transform customTransform;

		/// <summary>If you want the raycast hit point to be offset from the surface a bit, this allows you to set by how much in world space.</summary>
		public float Offset { set { offset = value; } get { return offset; } } [SerializeField] private float offset;

		/// <summary>If you want the hit point to be offset upwards when using touch input, this allows you to specify the physical distance the hit will be offset by on the screen. This is useful if you find paint hard to see because it's underneath your finger.</summary>
		public float TouchOffset { set { touchOffset = value; } get { return touchOffset; } } [SerializeField] private float touchOffset;

		/// <summary>Should painting triggered from this component be eligible for being undone?</summary>
		public bool StoreStates { set { storeStates = value; } get { return storeStates; } } [SerializeField] private bool storeStates = true;

		/// <summary>Show a painting preview under the mouse?</summary>
		public bool ShowPreview { set { showPreview = value; } get { return showPreview; } } [SerializeField] private bool showPreview = true;

		/// <summary>This allows you to override the order this paint gets applied to the object during the current frame.</summary>
		public int Priority { set { priority = value; } get { return priority; } } [SerializeField] private int priority;

		/// <summary>This allows you to control how the drawn shape is filled.
		/// PointsIn3D = Point drawing in 3D.
		/// PointsIn3DFilled = Point drawing in 3D, then the drawn shape will be filled in with points in a regular grid.
		/// PointsOnUV = Point drawing on UV (requires non-convex <b>MeshCollider</b>).
		/// TrianglesIn3D = Triangle drawing in 3D.</summary>
		public DrawType Draw { set { draw = value; } get { return draw; } } [UnityEngine.Serialization.FormerlySerializedAs("fill")] [SerializeField] private DrawType draw;

		/// <summary>This allows you to set the pixel distance between each grid point.
		/// NOTE: The lower you set this, the lower the performance will be.</summary>
		public float FillSpacing { set { fillSpacing = value; } get { return fillSpacing; } } [SerializeField] private float fillSpacing = 5.0f;

		[System.NonSerialized]
		private List<Link> links = new List<Link>();

		[System.NonSerialized]
		private P3dInputManager inputManager = new P3dInputManager();

		protected void LateUpdate()
		{
			inputManager.Update(key);

			// Use mouse hover preview?
			if (showPreview == true)
			{
				if (P3dInputManager.MouseExists == true && inputManager.Fingers.Count == 0 && P3dInputManager.PointOverGui(P3dInputManager.MousePosition) == false)
				{
					PaintAt(null, P3dInputManager.MousePosition, true, 1.0f, this);
				}
				else
				{
					BreakHits(this);
				}
			}

			for (var i = inputManager.Fingers.Count - 1; i >= 0; i--)
			{
				var finger = inputManager.Fingers[i];

				if (finger.StartedOverGui == false)
				{
					Paint(finger, finger.Down, finger.Up);
				}
			}
		}

		private void Paint(P3dInputManager.Finger finger, bool down, bool up)
		{
			var link = GetLink(finger);

			if (spacing > 0.0f)
			{
				var head = finger.GetSmoothScreenPosition(0.0f);

				if (down == true)
				{
					if (storeStates == true)
					{
						P3dStateManager.StoreAllStates();
					}

					PaintAt(link, head, false, finger.Pressure, link);
				}

				var steps = Mathf.Max(1, Mathf.FloorToInt(finger.SmoothScreenPositionDelta));
				var step  = P3dHelper.Reciprocal(steps);

				for (var i = 0; i <= steps; i++)
				{
					var next = finger.GetSmoothScreenPosition(Mathf.Clamp01(i * step));
					var dist = Vector2.Distance(head, next);
					var gaps = Mathf.FloorToInt((link.Distance + dist) / spacing);

					for (var j = 0; j < gaps; j++)
					{
						var remainder = spacing - link.Distance;

						head = Vector2.MoveTowards(head, next, remainder);

						PaintAt(link, head, false, finger.Pressure, link);

						dist -= remainder;

						link.Distance = 0.0f;
					}

					link.Distance += dist;
					head = next;
				}
			}
			else
			{
				var preview = true;

				if (showPreview == true)
				{
					if (spacing == 0.0f) // Once
					{
						if (up == true)
						{
							if (storeStates == true)
							{
								P3dStateManager.StoreAllStates();
							}

							preview = false;
						}
					}
					else // Every frame
					{
						if (storeStates == true && down == true)
						{
							P3dStateManager.StoreAllStates();
						}

						preview = false;
					}
				}
				else
				{
					if (spacing == 0.0f) // Once
					{
						if (down == true)
						{
							if (storeStates == true)
							{
								P3dStateManager.StoreAllStates();
							}
						}
						else
						{
							return;
						}
					}
					else // Every frame
					{
						if (storeStates == true && down == true)
						{
							P3dStateManager.StoreAllStates();
						}
					}

					preview = false;
				}

				PaintAt(link, finger.PositionA, preview, finger.Pressure, link);
			}

			if (up == true)
			{
				BreakHits(link);

				if (draw == DrawType.PointsIn3DFilled)
				{
					PaintGridOfPoints(link);
				}

				link.Clear();
			}
		}

		private void PaintGridOfPoints(Link link)
		{
			var rect = link.Area;

			rect.xMin += fillSpacing * 0.25f;
			rect.yMin += fillSpacing * 0.25f;
			rect.xMax -= fillSpacing * 0.25f;
			rect.yMax -= fillSpacing * 0.25f;

			if (fillSpacing > 0.0f && rect.width > 0.0f && rect.height > 0.0f && link.History.Count > 0)
			{
				var stepsH = Mathf.CeilToInt(rect.width  / fillSpacing);
				var stepsV = Mathf.CeilToInt(rect.height / fillSpacing);
				var corner = rect.center - new Vector2(stepsH, stepsV) * fillSpacing * 0.5f;

				for (var y = 0; y <= stepsV; y++)
				{
					for (var x = 0; x <= stepsH; x++)
					{
						var p = corner + new Vector2(x, y) * fillSpacing;

						if (Contains(link.History, p) == true)
						{
							PaintAt(null, p, false, 1.0f, null);
						}
					}
				}
			}
		}

		private static double LineSide(Vector2 a, Vector2 b, Vector2 p)
		{
			return (b.y - a.y) * (p.x - a.x) - (b.x - a.x) * (p.y - a.y);
		}

		private static bool Contains(List<Vector2> points, Vector2 xy)
		{
			var pointA = points[0];
			var total  = 0;

			for (var j = points.Count - 1; j >= 0; j--)
			{
				var pointB = points[j];

				if (pointA.y <= xy.y)
				{
					if (pointB.y > xy.y && LineSide(pointA, pointB, xy) > 0.0f) total += 1;
				}
				else
				{
					if (pointB.y <= xy.y && LineSide(pointA, pointB, xy) < 0.0f) total -= 1;
				}

				pointA = pointB;
			}

			return total != 0;
		}

		private void PaintAt(Link link, Vector2 screenPosition, bool preview, float pressure, object owner)
		{
			if (link != null)
			{
				link.Record(screenPosition);
			}

			var camera = P3dHelper.GetCamera(_camera);

			if (camera != null)
			{
				if (touchOffset != 0.0f && P3dInputManager.TouchCount > 0)
				{
					screenPosition.y += touchOffset * P3dInputManager.ScaleFactor;
				}

				var ray = camera.ScreenPointToRay(screenPosition);
				var hit = default(RaycastHit);

				if (Physics.Raycast(ray, out hit, float.PositiveInfinity, layers) == true)
				{
					var finalPosition = hit.point + hit.normal * offset;
					var finalRotation = Quaternion.identity;

					switch (rotateTo)
					{
						case RotationType.Normal:
						{
							var finalNormal = default(Vector3);

							switch (normalDirection)
							{
								case DirectionType.HitNormal: finalNormal = hit.normal; break;
								case DirectionType.RayDirection: finalNormal = -ray.direction; break;
								case DirectionType.CameraDirection: finalNormal = -camera.transform.forward; break;
							}

							var finalUp = default(Vector3);

							switch (normalRelativeTo)
							{
								case RelativeType.WorldUp: finalUp = Vector3.up; break;
								case RelativeType.CameraUp: finalUp = camera.transform.up; break;
							}

							finalRotation = Quaternion.LookRotation(-finalNormal, finalUp);
						}
						break;
						case RotationType.World: finalRotation = Quaternion.identity; break;
						case RotationType.ThisRotation: finalRotation = transform.rotation; break;
						case RotationType.ThisLocalRotation: finalRotation = transform.localRotation; break;
						case RotationType.CustomRotation: if (customTransform != null) finalRotation = customTransform.rotation; break;
						case RotationType.CustomLocalRotation: if (customTransform != null) finalRotation = customTransform.localRotation; break;
					}

					switch (draw)
					{
						case DrawType.PointsIn3D:
						case DrawType.PointsIn3DFilled:
						{
							SubmitPoint(preview, priority, pressure, finalPosition, finalRotation, owner);
						}
						break;

						case DrawType.PointsOnUV:
						{
							hitCache.InvokeCoord(gameObject, preview, priority, pressure, new P3dHit(hit), finalRotation);
						}
						break;

						case DrawType.TrianglesIn3D:
						{
							hitCache.InvokeTriangle(gameObject, preview, priority, pressure, hit, finalRotation);
						}
						break;
					}

					return;
				}
			}

			BreakHits(owner);
		}

		private Link GetLink(object owner)
		{
			for (var i = links.Count - 1; i >= 0; i--)
			{
				var link = links[i];

				if (link.Owner == owner)
				{
					return link;
				}
			}

			var newLink = new Link();

			newLink.Owner = owner;

			links.Add(newLink);

			return newLink;
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dHitScreen))]
	public class P3dHitScreen_Editor : P3dConnectablePoints_Editor<P3dHitScreen>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => P3dHelper.GetCamera(t.Camera) == null));
				Draw("_camera", "Orient to a specific camera?\n\nNone = MainCamera.");
			EndError();
			BeginError(Any(t => t.Layers == 0));
				Draw("layers", "The layers you want the raycast to hit.");
			EndError();
			Draw("spacing", "If you want the paint to continuously apply while moving the mouse, this allows you to set how many pixels are between each step.\n\n0 = Paint once when released.\n\n-1 = Paint every frame.");
			Draw("key", "The key that must be held for this component to activate on desktop platforms.\n\nNone = Any mouse button.");

			Separator();

			Draw("rotateTo", "This allows you to control how the paint is rotated.\n\nNormal = The rotation will be based on a normal direction, and rolled relative to an up axis.\n\nWorld = The rotation will be aligned to the world, or given no rotation.\n\nThisRotation = The current Transform.rotation will be used.\n\nThisLocalRotation = The current Transform.localRotation will be used.\n\nCustomRotation = The specified Transform.rotation will be used.\n\nCustomLocalRotation = The specified Transform.localRotation will be used.");
			if (Any(t => t.RotateTo == P3dHitScreen.RotationType.Normal))
			{
				BeginIndent();
					Draw("normalDirection", "Which direction should the hit point rotation be based on?", "Direction");
					Draw("normalRelativeTo", "Based on the normal direction, what should the rotation be rolled relative to?\n\nWorldUp = It will be rolled so the up vector is world up.\n\nCameraUp = It will be rolled so the up vector is camera up.", "Relative To");
				EndIndent();
			}
			if (Any(t => t.RotateTo == P3dHitScreen.RotationType.CustomRotation || t.RotateTo == P3dHitScreen.RotationType.CustomLocalRotation))
			{
				BeginIndent();
					Draw("customTransform", "This allows you to specify the Transform when using RotateTo = CustomRotation/CustomLocalRotation.");
				EndIndent();
			}

			Draw("offset", "If you want the raycast hit point to be offset from the surface a bit, this allows you to set by how much in world space.");
			Draw("touchOffset", "If you want the hit point to be offset upwards when using touch input, this allows you to specify the physical distance the hit will be offset by on the screen. This is useful if you find paint hard to see because it's underneath your finger.");
			Draw("storeStates", "Should painting triggered from this component be eligible for being undone?");

			Separator();

			Draw("showPreview", "Should the applied paint be applied as a preview?");
			Draw("priority", "This allows you to override the order this paint gets applied to the object during the current frame.");

			Separator();

			Draw("draw", "This allows you to control how the drawn shape is filled.\n\nPoints = Point drawing in 3D.\n\nPointsIn3DFilled = Point drawing in 3D, then the drawn shape will be filled in with points in a regular grid.\n\nPointsOnUV = Point drawing on UV (requires non-convex MeshCollider).\n\nTrianglesIn3D = Triangle drawing in 3D.");
			if (Any(t => t.Draw == P3dHitScreen.DrawType.PointsIn3DFilled))
			{
				BeginIndent();
					Draw("fillSpacing", "This allows you to set the pixel distance between each grid point.\n\nNOTE: The lower you set this, the lower the performance will be.", "Spacing");
				EndIndent();
			}

			Separator();

			base.OnInspector();

			var point    = Target.Draw == P3dHitScreen.DrawType.PointsIn3D || Target.Draw == P3dHitScreen.DrawType.PointsIn3DFilled;
			var line     = point == true && Target.ConnectHits == true;
			var triangle = Target.Draw == P3dHitScreen.DrawType.TrianglesIn3D;
			var coord    = Target.Draw == P3dHitScreen.DrawType.PointsOnUV;

			Target.HitCache.Inspector(Target.gameObject, point: point, line: line, triangle: triangle, coord: coord);
		}
	}
}
#endif