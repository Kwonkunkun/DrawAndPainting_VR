using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D.Examples
{
	/// <summary>This component allows you to enable/disable the target component while the specified key is held down.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dToggleScript")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Toggle Script")]
	public class P3dToggleScript : MonoBehaviour
	{
		/// <summary>The key that must be held for this component to activate.
		/// None = Any mouse button or finger.</summary>
		public KeyCode Key { set { key = value; } get { return key; } } [SerializeField] private KeyCode key = KeyCode.Mouse0;

		/// <summary>The component that will be enabled or disabled.</summary>
		public MonoBehaviour Target { set { target = value; } get { return target; } } [SerializeField] private MonoBehaviour target;

		/// <summary>Should painting triggered from this component be eligible for being undone?</summary>
		public bool StoreStates { set { storeStates = value; } get { return storeStates; } } [SerializeField] protected bool storeStates;

		protected virtual void Update()
		{
			if (target != null)
			{
				if (P3dInputManager.IsPressed(key) == true)
				{
					if (storeStates == true && target.enabled == false)
					{
						P3dStateManager.StoreAllStates();
					}

					target.enabled = true;
				}
				else
				{
					target.enabled = false;
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dToggleScript))]
	public class P3dKeyControl_Editor : P3dEditor<P3dToggleScript>
	{
		protected override void OnInspector()
		{
			Draw("key", "The key that must be held for this component to activate.\n\nNone = Any mouse button or finger.");
			BeginError(Any(t => t.Target == null));
				Draw("target", "The component that will be enabled or disabled.");
			EndError();
			Draw("storeStates", "Should painting triggered from this component be eligible for being undone?");
		}
	}
}
#endif