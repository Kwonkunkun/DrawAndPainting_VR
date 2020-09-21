using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D.Examples
{
	/// <summary>This component can open a URL. This can be done by attaching it to a clickable object, or manually from the Open methods.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dButtonLink")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Button Link")]
	public class P3dButtonLink : MonoBehaviour, IPointerClickHandler
	{
		/// <summary>The URL that will be opened.</summary>
		public string Url { set { url = value; } get { return url; } } [SerializeField] private string url;

		public void OnPointerClick(PointerEventData eventData)
		{
			Open();
		}

		[ContextMenu("This allows you to manually open the current URL.")]
		public void Open()
		{
			Open(url);
		}

		/// <summary>This allows you to open the specified URL.</summary>
		public void Open(string url)
		{
			Application.OpenURL(url);
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dButtonLink))]
	public class P3dButtonLink_Editor : P3dEditor<P3dButtonLink>
	{
		protected override void OnInspector()
		{
			Draw("url", "The URL that will be opened.");
		}
	}
}
#endif