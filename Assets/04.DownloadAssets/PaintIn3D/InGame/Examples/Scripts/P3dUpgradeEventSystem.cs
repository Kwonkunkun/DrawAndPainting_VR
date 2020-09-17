using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D.Examples
{
	/// <summary>This component will automatically update the event system if you switch to using the new <b>InputSystem</b>.</summary>
	[ExecuteInEditMode]
	[AddComponentMenu("Lean/Common/Upgrade EventSystem")]
	public class P3dUpgradeEventSystem : MonoBehaviour
	{
#if UNITY_EDITOR && ENABLE_INPUT_SYSTEM
		protected virtual void Awake()
		{
			var module = FindObjectOfType<UnityEngine.EventSystems.StandaloneInputModule>();

			if (module != null)
			{
				Debug.Log("Replacing old StandaloneInputModule with new InputSystemUIInputModule.", module.gameObject);

				module.gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

				DestroyImmediate(module);
			}
		}
#endif
	}
}

#if UNITY_EDITOR
namespace PaintIn3D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dUpgradeEventSystem))]
	public class P3dUpgradeEventSystem_Editor : P3dEditor<P3dUpgradeEventSystem>
	{
		protected override void OnInspector()
		{
			EditorGUILayout.HelpBox("This component will automatically update the event system if you switch to using the new InputSystem.", MessageType.Info);
		}
	}
}
#endif