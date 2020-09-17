using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component can be added to any ParticleSystem with collisions enabled, and it will fire hits when the particles collide with something.</summary>
	[RequireComponent(typeof(ParticleSystem))]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dHitParticles")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Hit/Hit Particles")]
	public class P3dHitParticles : MonoBehaviour
	{
		public enum OrientationType
		{
			WorldUp,
			CameraUp
		}

		public enum NormalType
		{
			ParticleVelocity,
			CollisionNormal
		}

		public enum PressureType
		{
			Constant,
			Distance,
			Speed
		}

		/// <summary>How should the hit point be oriented?
		/// WorldUp = It will be rotated to the normal, where the up vector is world up.
		/// CameraUp = It will be rotated to the normal, where the up vector is world up.</summary>
		public OrientationType Orientation { set { orientation = value; } get { return orientation; } } [SerializeField] private OrientationType orientation;

		/// <summary>Orient to a specific camera?
		/// None = MainCamera.</summary>
		public Camera Camera { set { _camera = value; } get { return _camera; } } [SerializeField] private Camera _camera;

		/// <summary>Which normal should the hit point rotation be based on?</summary>
		public NormalType Normal { set { normal = value; } get { return normal; } } [SerializeField] private NormalType normal;

		/// <summary>If you want the raycast hit point to be offset from the surface a bit, this allows you to set by how much in world space.</summary>
		public float Offset { set { offset = value; } get { return offset; } } [SerializeField] private float offset;

		/// <summary>If you have too many particles, then painting can slow down. This setting allows you to reduce the amount of particles that actually cause hits.
		/// 0 = Every particle will hit.
		/// 5 = Skip 5 particles, then hit using the 6th.</summary>
		public int Skip { set { skip = value; } get { return skip; } } [SerializeField] private int skip;

		/// <summary>Should the particles paint preview paint?</summary>
		public bool Preview { set { preview = value; } get { return preview; } } [SerializeField] private bool preview;

		/// <summary>This allows you to override the order this paint gets applied to the object during the current frame.</summary>
		public int Priority { set { priority = value; } get { return priority; } } [SerializeField] private int priority;

		/// <summary>This allows you to set how the pressure value will be calculated.
		/// Constant = A value of 1 will be used.
		/// Distance = A value will be calculated based on the distance between this emitter and the particle hit point.
		/// Speed = A value will be calculated based on the hit speed of the particle.</summary>
		public PressureType PressureMode { set { pressureMode = value; } get { return pressureMode; } } [SerializeField] private PressureType pressureMode = PressureType.Constant;

		/// <summary>This allows you to specify the distance/speed that gives 0.0 pressure.</summary>
		public float PressureMin { set { pressureMin = value; } get { return pressureMin; } } [UnityEngine.Serialization.FormerlySerializedAs("pressureMinDistance")] [SerializeField] private float pressureMin;

		/// <summary>This allows you to specify the distance/speed that gives 1.0 pressure.</summary>
		public float PressureMax { set { pressureMax = value; } get { return pressureMax; } } [UnityEngine.Serialization.FormerlySerializedAs("pressureMaxDistance")] [SerializeField] private float pressureMax;

		/// <summary>The calculated pressure value will be multiplied by this.</summary>
		public float PressureMultiplier { set { pressureMultiplier = value; } get { return pressureMultiplier; } } [UnityEngine.Serialization.FormerlySerializedAs("pressure")] [SerializeField] [Range(0.0f, 1.0f)] private float pressureMultiplier = 1.0f;

		[System.NonSerialized]
		private ParticleSystem cachedParticleSystem;

		[System.NonSerialized]
		private bool cachedParticleSystemSet;

		[System.NonSerialized]
		private static List<ParticleCollisionEvent> particleCollisionEvents = new List<ParticleCollisionEvent>();

		[System.NonSerialized]
		private P3dHitCache hitCache = new P3dHitCache();

		[System.NonSerialized]
		private int skipCounter;

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

		protected virtual void OnParticleCollision(GameObject hitGameObject)
		{
			if (cachedParticleSystemSet == false)
			{
				cachedParticleSystem    = GetComponent<ParticleSystem>();
				cachedParticleSystemSet = true;
			}

			// Get the collision events array
			var count = cachedParticleSystem.GetSafeCollisionEventSize();

			// Expand collisionEvents list to fit all particles
			for (var i = particleCollisionEvents.Count; i < count; i++)
			{
				particleCollisionEvents.Add(new ParticleCollisionEvent());
			}

			count = cachedParticleSystem.GetCollisionEvents(hitGameObject, particleCollisionEvents);

			// Calculate up vector ahead of time
			var finalUp = orientation == OrientationType.CameraUp ? P3dHelper.GetCameraUp(_camera) : Vector3.up;

			// Paint all locations
			for (var i = 0; i < count; i++)
			{
				if (skip > 0)
				{
					if (skipCounter++ > skip)
					{
						skipCounter = 0;
					}
					else
					{
						continue;
					}
				}

				var collisionEvent = particleCollisionEvents[i];
				var finalPosition  = collisionEvent.intersection + collisionEvent.normal * offset;
				var finalNormal    = normal == NormalType.CollisionNormal ? collisionEvent.normal : -collisionEvent.velocity;
				var finalRotation  = finalNormal != Vector3.zero ? Quaternion.LookRotation(-finalNormal, finalUp) : Quaternion.identity;
				var finalPressure  = pressureMultiplier;

				switch (pressureMode)
				{
					case PressureType.Distance:
					{
						var distance = Vector3.Distance(transform.position, collisionEvent.intersection);

						finalPressure *= Mathf.InverseLerp(pressureMin, pressureMax, distance);
					}
					break;

					case PressureType.Speed:
					{
						var speed = Vector3.SqrMagnitude(collisionEvent.velocity);

						if (speed > 0.0f)
						{
							speed = Mathf.Sqrt(speed);
						}

						finalPressure *= Mathf.InverseLerp(pressureMin, pressureMax, speed);
					}
					break;
				}
				if (pressureMin != pressureMax)
				{
					
				}

				hitCache.InvokePoint(gameObject, preview, priority, finalPressure, finalPosition, finalRotation);
			}
		}

#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.DrawWireSphere(transform.position, pressureMin);
			Gizmos.DrawWireSphere(transform.position, pressureMax);
		}
#endif
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CustomEditor(typeof(P3dHitParticles))]
	public class P3dHitParticles_Editor : P3dEditor<P3dHitParticles>
	{
		protected override void OnInspector()
		{
			Draw("orientation", "How should the hit point be oriented?\n\nNone = It will be treated as a point with no rotation.\n\nWorldUp = It will be rotated to the normal, where the up vector is world up.\n\nCameraUp = It will be rotated to the normal, where the up vector is world up.");
			BeginIndent();
				if (Any(t => t.Orientation == P3dHitParticles.OrientationType.CameraUp))
				{
					Draw("_camera", "Orient to a specific camera?\n\nNone = MainCamera.");
				}
			EndIndent();
			Draw("normal", "Which normal should the hit point rotation be based on?");
			Draw("offset", "If you want the raycast hit point to be offset from the surface a bit, this allows you to set by how much in world space.");
			Draw("skip", "If you have too many particles, then painting can slow down. This setting allows you to reduce the amount of particles that actually cause hits.\n\n0 = Every particle will hit.\n\n5 = Skip 5 particles, then hit using the 6th.");

			Separator();
			
			Draw("preview", "Should the particles paint preview paint?");
			Draw("priority", "This allows you to override the order this paint gets applied to the object during the current frame.");
			Draw("pressureMode", "This allows you to set how the pressure value will be calculated.\n\nConstant = A value of 1 will be used.\n\nDistance = A value will be calculated based on the distance between this emitter and the particle hit point.\n\nVelocity = A value will be calculated based on the hit velocity of the particle.");
			BeginIndent();
				if (Any(t => t.PressureMode == P3dHitParticles.PressureType.Distance))
				{
					Draw("pressureMin", "This allows you to set the world space distance from this emitter where the particle hit point will register as having 0.0 pressure.", "Min");
					Draw("pressureMax", "This allows you to set the world space distance from this emitter where the particle hit point will register as having 1.0 pressure.", "Max");
				}
				if (Any(t => t.PressureMode == P3dHitParticles.PressureType.Speed))
				{
					Draw("pressureMin", "This allows you to set the particle speed where the hit will register as having 0.0 pressure.", "Min");
					Draw("pressureMax", "This allows you to set the particle speed where the hit will register as having 1.0 pressure.", "Max");
				}
				Draw("pressureMultiplier", "The calculated pressure value will be multiplied by this.", "Multiplier");
			EndIndent();

			Separator();

			var point = true;

			Target.HitCache.Inspector(Target.gameObject, point: point);
		}
	}
}
#endif