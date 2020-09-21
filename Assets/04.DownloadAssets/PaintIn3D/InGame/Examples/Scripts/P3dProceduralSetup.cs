using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D.Examples
{
	/// <summary>This component can be added to an empty GameObject, and it will set it up with a procedurally generated quad that is ready for painting.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dProceduralSetup")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Procedural Setup")]
	public class P3dProceduralSetup : MonoBehaviour
	{
		/// <summary>The <b>Material</b> applied to the renderer.</summary>
		public Material Material { set { material = value; } get { return material; } } [SerializeField] private Material material;

		/// <summary>The size of the generated quad in local space.</summary>
		public float Size { set { size = value; } get { return size; } } [SerializeField] private float size = 1.0f;

		[SerializeField]
		private Mesh generatedMesh;

		protected virtual void Awake()
		{
			// Create and populate generatedMesh
			UpdateMesh();

			// Create a MeshFilter and assign generatedMesh
			gameObject.AddComponent<MeshFilter>().sharedMesh = generatedMesh;

			// Create a MeshCollider and assign generatedMesh
			gameObject.AddComponent<MeshCollider>().sharedMesh = generatedMesh;

			// Create a MeshRenderer and assign material
			gameObject.AddComponent<MeshRenderer>().sharedMaterial = material;

			// Make this GameObject paintable
			gameObject.AddComponent<P3dPaintable>();

			// Make it so material 0 gets cloned before painting
			var materialCloner = gameObject.AddComponent<P3dMaterialCloner>();

			materialCloner.Index = 0;

			// Make it so the "_MainTex" of material 0 becomes paintable
			var paintableTexture = gameObject.AddComponent<P3dPaintableTexture>();

			paintableTexture.Slot = new P3dSlot(0, "_MainTex");
		}

		protected virtual void OnDestroy()
		{
			// Destroy the mesh so it doesn't leak
			Destroy(generatedMesh);
		}

		private void UpdateMesh()
		{
			// Create or clear the mesh
			if (generatedMesh == null)
			{
				generatedMesh = new Mesh();
			}
			else
			{
				generatedMesh.Clear();
			}

			// Write quad vertices
			generatedMesh.vertices = new Vector3[] { new Vector3(-size, -size), new Vector3(+size, -size), new Vector3(-size, +size), new Vector3(+size, +size) };

			// Write quad uv coords
			generatedMesh.uv = new Vector2[] { new Vector2(0.0f, 0.0f), new Vector2(1.0f, 0.0f), new Vector2(0.0f, 1.0f), new Vector2(1.0f, 1.0f) };

			// Write quad indices
			generatedMesh.triangles = new int[] { 0, 1, 2, 3, 2, 1 };

			// Generate remaining data
			generatedMesh.RecalculateBounds();
			generatedMesh.RecalculateNormals();
			generatedMesh.RecalculateTangents();
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dProceduralSetup))]
	public class P3dProceduralSetup_Editor : P3dEditor<P3dProceduralSetup>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Material == null));
				Draw("material", "The Material applied to the renderer.");
			EndError();
			BeginError(Any(t => t.Size == 0.0f));
				Draw("size", "The size of the generated quad in local space.");
			EndError();
		}
	}
}
#endif