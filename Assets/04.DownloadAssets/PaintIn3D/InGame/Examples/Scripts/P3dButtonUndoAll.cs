using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D.Examples
{
	/// <summary>This component allows you to perform the Undo All action. This can be done by attaching it to a clickable object, or manually from the RedoAll method.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dButtonUndoAll")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Button Undo All")]
	public class P3dButtonUndoAll : MonoBehaviour, IPointerClickHandler
	{
		public void OnPointerClick(PointerEventData eventData)
		{
			UndoAll();
		}

		/// <summary>If you want to manually trigger UndoAll, then call this function.</summary>
		[ContextMenu("Undo All")]
		public void UndoAll()
		{
			P3dStateManager.UndoAll();
		}

		protected virtual void Update()
		{
			var group = GetComponent<CanvasGroup>();

			if (group != null)
			{
				group.alpha = P3dStateManager.CanUndo == true ? 1.0f : 0.5f;
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dButtonUndoAll))]
	public class P3dUndoAll_Editor : P3dEditor<P3dButtonUndoAll>
	{
		protected override void OnInspector()
		{
			EditorGUILayout.HelpBox("This component allows you to perform the Undo All action. This can be done by attaching it to a clickable object, or manually from the UndoAll method.", MessageType.Info);
		}
	}
}
#endif