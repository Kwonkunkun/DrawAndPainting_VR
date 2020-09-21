using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component will spawn and throw Rigidbody prefabs from the camera when you tap the mouse or a finger.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dTapThrow")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Tap Throw")]
	public class P3dTapThrow : MonoBehaviour
	{
		/// <summary>The key that must be held for this component to activate on desktop platforms.
		/// None = Any mouse button.</summary>
		public KeyCode Key { set { key = value; } get { return key; } } [SerializeField] private KeyCode key = KeyCode.Mouse0;

		/// <summary>The prefab that will be thrown.</summary>
		public GameObject Prefab { set { prefab = value; } get { return prefab; } } [SerializeField] private GameObject prefab;

		/// <summary>The speed that the object will be thrown at.</summary>
		public float Speed { set { speed = value; } get { return speed; } } [SerializeField] private float speed = 10.0f;

		/// <summary>Should painting triggered from this component be eligible for being undone?</summary>
		public bool StoreStates { set { storeStates = value; } get { return storeStates; } } [SerializeField] protected bool storeStates;

		[System.NonSerialized]
		private P3dInputManager inputManager = new P3dInputManager();

		protected virtual void Update()
		{
			inputManager.Update(key);

			if (prefab != null)
			{
				var fingers = inputManager.Fingers;

				if (fingers.Count == 1 && fingers[0].Down == true && fingers[0].StartedOverGui == false)
				{
					var camera = P3dHelper.GetCamera();

					if (camera != null)
					{
						if (storeStates == true)
						{
							P3dStateManager.StoreAllStates();
						}

						// Find the ray for this screen position
						var ray      = camera.ScreenPointToRay(P3dInputManager.MousePosition);
						var rotation = Quaternion.LookRotation(ray.direction);

						// Loop through all prefabs and spawn them
						var clone = Instantiate(prefab, ray.origin, rotation);

						clone.SetActive(true);

						// Throw with velocity?
						var cloneRigidbody = clone.GetComponent<Rigidbody>();

						if (cloneRigidbody != null)
						{
							cloneRigidbody.velocity = clone.transform.forward * Speed;
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
	[CustomEditor(typeof(P3dTapThrow))]
	public class P3dTapThrow_Editor : P3dEditor<P3dTapThrow>
	{
		protected override void OnInspector()
		{
			Draw("key", "The key that must be held for this component to activate on desktop platforms.\n\nNone = Any mouse button.");
			BeginError(Any(t => t.Prefab == null));
				Draw("prefab", "The prefab that will be thrown.");
			EndError();
			Draw("speed", "Rotate the decal to the hit normal?");
			Draw("storeStates", "Should painting triggered from this component be eligible for being undone?");
		}
	}
}
#endif