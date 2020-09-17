using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D.Examples
{
	/// <summary>This component allows you to perform the Redo All action. This can be done by attaching it to a clickable object, or manually from the RedoAll method.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dButtonRedoAll")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Button Redo All")]
	public class P3dButtonRedoAll : MonoBehaviour, IPointerClickHandler
	{
		public void OnPointerClick(PointerEventData eventData)
		{
			RedoAll();
		}

		/// <summary>If you want to manually triggger RedoAll, then call this function.</summary>
		[ContextMenu("Redo All")]
		public void RedoAll()
		{
			P3dStateManager.RedoAll();
		}

		protected virtual void Update()
		{
			var group = GetComponent<CanvasGroup>();

			if (group != null)
			{
				group.alpha = P3dStateManager.CanRedo == true ? 1.0f : 0.5f;
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dButtonRedoAll))]
	public class P3dRedoAll_Editor : P3dEditor<P3dButtonRedoAll>
	{
		protected override void OnInspector()
		{
			EditorGUILayout.HelpBox("This component allows you to perform the Redo All action. This can be done by attaching it to a clickable object, or manually from the RedoAll method.", MessageType.Info);
		}
	}
}
#endif