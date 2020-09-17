using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D.Examples
{
	/// <summary>This component auatomatically destroys this GameObject after some time.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dDestroyAfterTime")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Destroy After Time")]
	public class P3dDestroyAfterTime : MonoBehaviour
	{
		/// <summary>If this component has been active for this many seconds, the current GameObject will be destroyed.
		/// -1 = DestroyNow must be manually called.</summary>
		public float Seconds { set { seconds = value; } get { return seconds; } } [SerializeField] private float seconds = 5.0f;

		[SerializeField]
		private float age;

		[ContextMenu("Destroy Now")]
		public void DestroyNow()
		{
			Destroy(gameObject);
		}

		protected virtual void Update()
		{
			if (seconds >= 0.0f)
			{
				age += Time.deltaTime;

				if (age >= seconds)
				{
					DestroyNow();
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dDestroyAfterTime))]
	public class P3dDestroyAfterTime_Editor : P3dEditor<P3dDestroyAfterTime>
	{
		protected override void OnInspector()
		{
			Draw("seconds", "If this component has been active for this many seconds, the current GameObject will be destroyed.\n-1 = DestroyNow must be manually called.");
		}
	}
}
#endif