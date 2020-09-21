using UnityEngine;

namespace PaintIn3D
{
	/// <summary>This struct stores information from a <b>RaycastHit</b>.</summary>
	public struct P3dHit
	{
		public P3dHit(RaycastHit hit)
		{
			Root   = hit.transform;
			First  = hit.textureCoord;
			Second = hit.textureCoord2;
		}

		/// <summary>The <b>Transform</b> that was hit.</summary>
		public Transform Root;

		/// <summary>The first UV coord that was hit.</summary>
		public Vector2 First;

		/// <summary>The second UV coord that was hit.</summary>
		public Vector2 Second;
	}
}