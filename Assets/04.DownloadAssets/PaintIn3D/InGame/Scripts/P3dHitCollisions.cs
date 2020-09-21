using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component can be added to any Rigidbody, and it will fire hit events when it hits something.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dHitCollisions")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Hit/Hit Collisions")]
	public class P3dHitCollisions : MonoBehaviour
	{
		public enum OrientationType
		{
			WorldUp,
			CameraUp
		}

		public enum DrawType
		{
			PointsIn3D    = 0,
			PointsOnUV    = 20,
			TrianglesIn3D = 30
		}

		/// <summary>Orient to a specific camera?
		/// None = MainCamera.</summary>
		public Camera Camera { set { _camera = value; } get { return _camera; } } [SerializeField] private Camera _camera;

		/// <summary>If you want the raycast hit point to be offset from the surface a bit, this allows you to set by how much in world space.</summary>
		public float Offset { set { offset = value; } get { return offset; } } [SerializeField] private float offset;

		/// <summary>The impact strength required for a hit to occur with a pressure of 0.</summary>
		public float ImpactMin { set { impactMin = value; } get { return impactMin; } } [UnityEngine.Serialization.FormerlySerializedAs("speedMin")] [SerializeField] private float impactMin = 50.0f;

		/// <summary>The impact strength required for a hit to occur with a pressure of 1.</summary>
		public float ImpactPressure { set { impactPressure = value; } get { return impactPressure; } } [UnityEngine.Serialization.FormerlySerializedAs("speedPressure")] [SerializeField] private float impactPressure = 100.0f;

		/// <summary>If there are multiple contact points, skip them?</summary>
		public bool OnlyUseFirstContact { set { onlyUseFirstContact = value; } get { return onlyUseFirstContact; } } [SerializeField] private bool onlyUseFirstContact = true;

		/// <summary>The time in seconds between each collision if you want to limit it.</summary>
		public float Delay { set { delay = value; } get { return delay; } } [SerializeField] private float delay;

		/// <summary>This allows you to filter collisions to specific layers.</summary>
		public LayerMask Layers { set { layers = value; } get { return layers; } } [SerializeField] private LayerMask layers = -1;

		/// <summary>How should the hit point be oriented?
		/// WorldUp = It will be rotated to the normal, where the up vector is world up.
		/// CameraUp = It will be rotated to the normal, where the up vector is world up.</summary>
		public OrientationType Orientation { set { orientation = value; } get { return orientation; } } [SerializeField] private OrientationType orientation;

		/// <summary>If you need raycast information (used by components like P3dPaintDirectDecal), then this allows you to set the world space distance from the hit point a raycast will be cast from.
		/// 0 = No raycast.
		/// NOTE: This has a performance penalty, so you should disable it if not needed.</summary>
		public float RaycastDistance { set { raycastDistance = value; } get { return raycastDistance; } } [SerializeField] private float raycastDistance = 0.0001f;

		/// <summary>Should the applied paint be applied as a preview?</summary>
		public bool Preview { set { preview = value; } get { return preview; } } [SerializeField] private bool preview;

		/// <summary>This allows you to override the order this paint gets applied to the object during the current frame.</summary>
		public int Priority { set { priority = value; } get { return priority; } } [SerializeField] private int priority;

		/// <summary>This allows you to control how the drawn shape is filled.
		/// PointsIn3D = Point drawing in 3D.
		/// PointsOnUV = Point drawing on UV (requires non-convex <b>MeshCollider</b>).
		/// TrianglesIn3D = Triangle drawing in 3D.</summary>
		public DrawType Draw { set { draw = value; } get { return draw; } } [SerializeField] private DrawType draw;

		/// <summary>By default hit events are sent to all components attached to the current GameObject, but this setting allows you to override that. This is useful if you want to use multiple <b>P3dHitCollisions</b> components with different settings and results.</summary>
		public GameObject Root { set { ClearHitCache(); root = value; } get { return root; } } [SerializeField] private GameObject root;

		[SerializeField]
		private float cooldown;

		[System.NonSerialized]
		private P3dHitCache hitCache = new P3dHitCache();

		public P3dHitCache HitCache
		{
			get
			{
				return hitCache;
			}
		}

		[ContextMenu("Clear Hit Cache")]
		public void ClearHitCache()
		{
			hitCache.Clear();
		}

		protected virtual void OnCollisionEnter(Collision collision)
		{
			CheckCollision(collision);
		}

		protected virtual void OnCollisionStay(Collision collision)
		{
			CheckCollision(collision);
		}

		protected virtual void Update()
		{
			cooldown -= Time.deltaTime;
		}

		private bool TryGetRaycastHit(ContactPoint contact, ref RaycastHit hit)
		{
			if (raycastDistance > 0.0f)
			{
				var ray = new Ray(contact.point + contact.normal * raycastDistance, -contact.normal);

				if (contact.otherCollider.Raycast(ray, out hit, raycastDistance * 2.0f) == true)
				{
					return true;
				}
			}

			return false;
		}

		private void CheckCollision(Collision collision)
		{
			if (cooldown > 0.0f)
			{
				return;
			}

			var impulse = collision.impulse.magnitude / Time.fixedDeltaTime;

			// Only handle the collision if the impact was strong enough
			if (impulse >= impactMin)
			{
				cooldown = delay;

				// Calculate up vector ahead of time
				var finalUp   = orientation == OrientationType.CameraUp ? P3dHelper.GetCameraUp(_camera) : Vector3.up;
				var contacts  = collision.contacts;
				var pressure  = Mathf.InverseLerp(impactMin, impactPressure, impulse);
				var finalRoot = root != null ? root : gameObject;

				for (var i = contacts.Length - 1; i >= 0; i--)
				{
					var contact = contacts[i];

					if (P3dHelper.IndexInMask(contact.otherCollider.gameObject.layer, layers) == true)
					{
						var finalPosition = contact.point + contact.normal * offset;
						var finalRotation = Quaternion.LookRotation(-contact.normal, finalUp);

						switch (draw)
						{
							case DrawType.PointsIn3D:
							{
								hitCache.InvokePoint(finalRoot, preview, priority, pressure, finalPosition, finalRotation);
							}
							break;

							case DrawType.PointsOnUV:
							{
								var hit = default(RaycastHit);

								if (TryGetRaycastHit(contact, ref hit) == true)
								{
									hitCache.InvokeCoord(finalRoot, preview, priority, pressure, new P3dHit(hit), finalRotation);
								}
							}
							break;

							case DrawType.TrianglesIn3D:
							{
								var hit = default(RaycastHit);

								if (TryGetRaycastHit(contact, ref hit) == true)
								{
									hitCache.InvokeTriangle(gameObject, preview, priority, pressure, hit, finalRotation);
								}
							}
							break;
						}

						if (onlyUseFirstContact == true)
						{
							break;
						}
					}
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dHitCollisions))]
	public class P3dHitCollisions_Editor : P3dEditor<P3dHitCollisions>
	{
		protected override void OnInspector()
		{
			Draw("impactMin", "The impact strength required for a hit to occur with a pressure of 0.");
			Draw("impactPressure", "The impact strength required for a hit to occur with a pressure of 1.");
			Draw("onlyUseFirstContact", "If there are multiple contact points, skip them?");
			BeginError(Any(t => t.Delay < 0.0f));
				Draw("delay", "The time in seconds between each collision if you want to limit it.");
			EndError();
			Draw("layers", "This allows you to filter collisions to specific layers.");
			Draw("orientation", "How should the hit point be oriented?\nNone = It will be treated as a point with no rotation.\n\nWorldUp = It will be rotated to the normal, where the up vector is world up.\n\nCameraUp = It will be rotated to the normal, where the up vector is world up.");
			BeginIndent();
				if (Any(t => t.Orientation == P3dHitCollisions.OrientationType.CameraUp))
				{
					Draw("_camera", "Orient to a specific camera?\nNone = MainCamera.");
				}
			EndIndent();
			Draw("offset", "If you want the raycast hit point to be offset from the surface a bit, this allows you to set by how much in world space.");
			Draw("raycastDistance", "If you need raycast information (used by components like P3dPaintDirectDecal), then this allows you to set the world space distance from the hit point a raycast will be cast from.\n\n0 = No raycast.\n\nNOTE: This has a performance penalty, so you should disable it if not needed.");

			Separator();

			Draw("preview", "Should the applied paint be applied as a preview?");
			Draw("priority", "This allows you to override the order this paint gets applied to the object during the current frame.");

			Separator();

			Draw("draw", "This allows you to control how the drawn shape is filled.\n\nPoints = Point drawing in 3D.\n\nPointsOnUV = Point drawing on UV (requires non-convex MeshCollider).\n\nTrianglesIn3D = Triangle drawing in 3D.");

			Separator();

			Draw("root", "By default hit events are sent to all components attached to the current GameObject, but this setting allows you to override that. This is useful if you want to use multiple P3dHitCollisions components with different settings and results.");
			
			var point    = Target.Draw == P3dHitCollisions.DrawType.PointsIn3D;
			var triangle = Target.Draw == P3dHitCollisions.DrawType.TrianglesIn3D;
			var coord    = Target.Draw == P3dHitCollisions.DrawType.PointsOnUV && Target.RaycastDistance > 0.0f;
			
			Target.HitCache.Inspector(Target.Root != null ? Target.Root : Target.gameObject, point: point, triangle: triangle, coord: coord);
		}
	}
}
#endif