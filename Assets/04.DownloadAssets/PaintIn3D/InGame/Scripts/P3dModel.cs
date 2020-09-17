using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component marks the current GameObject as being paintable, as long as this GameObject has a MeshFilter + MeshRenderer, or a SkinnedMeshRenderer.
	/// NOTE: To actually paint, the <b>P3dPaintableTexture</b> component must be on a different object.</summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Renderer))]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dModel")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Model")]
	public class P3dModel : P3dLinkedBehaviour<P3dModel>
	{
		/// <summary>The paintable this separate paintable is associated with.</summary>
		public virtual P3dPaintable Paintable { set { paintable = value; } get { return paintable; } } [SerializeField] protected P3dPaintable paintable;

		/// <summary>Transform the mesh with its position, rotation, and scale? Some skinned mesh setups require this to be disabled.</summary>
		public virtual bool IncludeScale { set { includeScale = value; } get { return includeScale; } } [SerializeField] protected bool includeScale = true;

		[System.NonSerialized]
		private Renderer cachedRenderer;

		[System.NonSerialized]
		private bool cachedRendererSet;

		[System.NonSerialized]
		private SkinnedMeshRenderer cachedSkinned;

		[System.NonSerialized]
		private MeshFilter cachedFilter;

		[System.NonSerialized]
		private bool cachedSkinnedSet;

		[System.NonSerialized]
		private Transform cachedTransform;

		[System.NonSerialized]
		private GameObject cachedGameObject;

		[System.NonSerialized]
		private Material[] materials;

		[System.NonSerialized]
		private bool materialsSet;

		[System.NonSerialized]
		private Mesh bakedMesh;

		[System.NonSerialized]
		private bool bakedMeshSet;

		[System.NonSerialized]
		protected bool prepared;

		[System.NonSerialized]
		private Mesh preparedMesh;

		[System.NonSerialized]
		private Matrix4x4 preparedMatrix;

		[System.NonSerialized]
		private int[] preparedTriangles;

		[System.NonSerialized]
		private Vector3[] preparedPositions;

		[System.NonSerialized]
		private Vector2[] preparedCoord0;

		[System.NonSerialized]
		private Vector2[] preparedCoord1;

		[System.NonSerialized]
		private static List<P3dModel> tempModels = new List<P3dModel>();

		public bool Prepared
		{
			set
			{
				prepared = value;
			}

			get
			{
				return prepared;
			}
		}

		public GameObject CachedGameObject
		{
			get
			{
				return cachedGameObject;
			}
		}

		public Renderer CachedRenderer
		{
			get
			{
				if (cachedRendererSet == false)
				{
					CacheRenderer();
				}

				return cachedRenderer;
			}
		}

		public Material[] Materials
		{
			get
			{
				if (materialsSet == false)
				{
					materials    = CachedRenderer.sharedMaterials;
					materialsSet = true;
				}

				return materials;
			}
		}

		public int GetMaterialIndex(Material material)
		{
			if (material != null)
			{
				var materials = Materials;

				for (var i = materials.Length - 1; i >= 0; i--)
				{
					if (materials[i] == material)
					{
						return i;
					}
				}
			}

			return -1;
		}

		private void CacheRenderer()
		{
			cachedRenderer    = GetComponent<Renderer>();
			cachedRendererSet = true;

			if (cachedRenderer is SkinnedMeshRenderer)
			{
				cachedSkinned    = (SkinnedMeshRenderer)cachedRenderer;
				cachedSkinnedSet = true;
			}
			else
			{
				cachedFilter = GetComponent<MeshFilter>();
			}
		}

		/// <summary>Materials will give you a cached CachedRenderer.sharedMaterials array. If you have updated this array externally then call this to force the cache to update next them it's accessed.</summary>
		[ContextMenu("Dirty Materials")]
		public void DirtyMaterials()
		{
			materialsSet = false;
		}

		/// <summary>This will return a list of all paintables that overlap the specified bounds</summary>
		public static List<P3dModel> FindOverlap(Vector3 position, float radius, int layerMask)
		{
			tempModels.Clear();

			var model = FirstInstance;

			for (var i = 0; i < InstanceCount; i++)
			{
				if (P3dHelper.IndexInMask(model.CachedGameObject.layer, layerMask) == true && model.Paintable != null)
				{
					var bounds    = model.CachedRenderer.bounds;
					var sqrRadius = radius + bounds.extents.magnitude; sqrRadius *= sqrRadius;

					if (Vector3.SqrMagnitude(position - bounds.center) < sqrRadius)
					{
						tempModels.Add(model);

						if (model.paintable.Activated == false)
						{
							model.paintable.Activate();
						}
					}
				}

				model = model.NextInstance;
			}

			return tempModels;
		}

		public void GetPreparedPoints(int triangleIndex, ref Vector3 pointA, ref Vector3 pointB, ref Vector3 pointC)
		{
			if (prepared == true && preparedMesh != null)
			{
				if (preparedPositions == null) preparedPositions = preparedMesh.vertices;
				if (preparedTriangles == null) preparedTriangles = preparedMesh.triangles;

				pointA = preparedPositions[preparedTriangles[triangleIndex * 3 + 0]];
				pointB = preparedPositions[preparedTriangles[triangleIndex * 3 + 1]];
				pointC = preparedPositions[preparedTriangles[triangleIndex * 3 + 2]];
			}
		}

		public void GetPreparedCoords0(int triangleIndex, ref Vector2 coordA, ref Vector2 coordB, ref Vector2 coordC)
		{
			if (prepared == true && preparedMesh != null)
			{
				if (preparedTriangles == null) preparedTriangles = preparedMesh.triangles;
				if (preparedCoord0    == null) preparedCoord0    = preparedMesh.uv;

				coordA = preparedCoord0[preparedTriangles[triangleIndex * 3 + 0]];
				coordB = preparedCoord0[preparedTriangles[triangleIndex * 3 + 1]];
				coordC = preparedCoord0[preparedTriangles[triangleIndex * 3 + 2]];
			}
		}

		public void GetPreparedCoords1(int triangleIndex, ref Vector2 coordA, ref Vector2 coordB, ref Vector2 coordC)
		{
			if (prepared == true && preparedMesh != null)
			{
				if (preparedTriangles == null) preparedTriangles = preparedMesh.triangles;
				if (preparedCoord1    == null) preparedCoord1    = preparedMesh.uv;

				coordA = preparedCoord1[preparedTriangles[triangleIndex * 3 + 0]];
				coordB = preparedCoord1[preparedTriangles[triangleIndex * 3 + 1]];
				coordC = preparedCoord1[preparedTriangles[triangleIndex * 3 + 2]];
			}
		}

		public int GetSubmesh(P3dPaintableTexture paintableTexture)
		{
			var material = paintableTexture.Material;

			if (material == null)
			{
				paintableTexture.UpdateMaterial();

				material = paintableTexture.Material;
			}

			return Paintable.GetMaterialIndex(material);
		}

		public void GetPrepared(ref Mesh mesh, ref Matrix4x4 matrix)
		{
			if (prepared == false)
			{
				prepared = true;

				if (cachedRendererSet == false)
				{
					CacheRenderer();
				}

				if (cachedSkinnedSet == true)
				{
					if (bakedMeshSet == false)
					{
						bakedMesh    = new Mesh();
						bakedMeshSet = true;
					}

					var scaling       = P3dHelper.Reciprocal3(cachedTransform.lossyScale);
					var oldLocalScale = cachedTransform.localScale;

					cachedTransform.localScale = Vector3.one;

					cachedSkinned.BakeMesh(bakedMesh);

					cachedTransform.localScale = oldLocalScale;

					preparedMesh   = bakedMesh;
					preparedMatrix = cachedTransform.localToWorldMatrix;

					if (includeScale == true)
					{
						preparedMatrix *= Matrix4x4.Scale(scaling);
					}
				}
				else
				{
					preparedMesh   = cachedFilter.sharedMesh;
					preparedMatrix = cachedTransform.localToWorldMatrix;
				}
			}

			mesh   = preparedMesh;
			matrix = preparedMatrix;
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			cachedGameObject = gameObject;
			cachedTransform  = transform;
		}

		protected virtual void OnDestroy()
		{
			P3dHelper.Destroy(bakedMesh);
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dModel))]
	public class P3dModel_Editor : P3dEditor<P3dModel>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Paintable == null));
				Draw("paintable", "The paintable this separate paintable is associated with.");
			EndError();
			Draw("includeScale", "Transform the mesh with its position, rotation, and scale? Some skinned mesh setups require this to be disabled.");

			Separator();

			if (Button("Analyze Mesh") == true)
			{
				P3dMeshAnalysis.OpenWith(Target.gameObject);
			}
		}
	}
}
#endif