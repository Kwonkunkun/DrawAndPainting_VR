using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component enables or disables the specified ParticleSystem based on mouse or finger presses.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dToggleParticles")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Toggle Particles")]
	public class P3dToggleParticles : MonoBehaviour
	{
		/// <summary>The key that must be held for this component to activate.
		/// None = Any mouse button or finger.</summary>
		public KeyCode Key { set { key = value; } get { return key; } } [SerializeField] private KeyCode key = KeyCode.Mouse0;

		/// <summary>The particle system that will be enabled/disabled based on mouse/touch.</summary>
		public ParticleSystem Target { set { target = value; } get { return target; } } [SerializeField] private ParticleSystem target;

		/// <summary>Should painting triggered from this component be eligible for being undone?</summary>
		public bool StoreStates { set { storeStates = value; } get { return storeStates; } } [SerializeField] protected bool storeStates = true;

		protected virtual void LateUpdate()
		{
			if (target != null)
			{
				if (P3dInputManager.IsPressed(key) == true)
				{
					if (storeStates == true && target.isPlaying == false)
					{
						P3dStateManager.StoreAllStates();
					}

					target.Play();
				}
				else
				{
					target.Stop();
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dToggleParticles))]
	public class P3dToggleParticles_Editor : P3dEditor<P3dToggleParticles>
	{
		protected override void OnInspector()
		{
			Draw("key", "The key that must be held for this component to activate.\n\nNone = Any mouse button or finger.");
			BeginError(Any(t => t.Target == null));
				Draw("target", "The particle system that will be enabled/disabled based on mouse/touch.");
			EndError();
			Draw("storeStates", "Should painting triggered from this component be eligible for being undone?");
		}
	}
}
#endif