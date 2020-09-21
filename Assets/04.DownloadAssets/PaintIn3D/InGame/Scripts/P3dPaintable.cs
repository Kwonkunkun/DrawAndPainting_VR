using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component marks the current GameObject as being paintable.
	/// NOTE: This GameObject must has a MeshFilter + MeshRenderer, or a SkinnedMeshRenderer.
	/// To actually paint your object, you must also add at least one <b>P3dPaintableTexture</b> component to specify which texture you want to paint.</summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Renderer))]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintable")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Paintable")]
	public class P3dPaintable : P3dModel
	{
		public enum ActivationType
		{
			Awake,
			OnEnable,
			Start,
			OnFirstUse
		}

		public override P3dPaintable Paintable { set {  } get { paintable = this; return paintable; } }

		/// <summary>This allows you to control when this component actually activates and becomes ready for painting. You probably don't need to change this.</summary>
		public ActivationType Activation { set { activation = value; } get { return activation; } } [SerializeField] private ActivationType activation = ActivationType.Start;

		/// <summary>If you want the paintable texture width/height to be multiplied by the scale of this GameObject, this allows you to set the scale where you want the multiplier to be 1.</summary>
		public Vector3 BaseScale { set { baseScale = value; } get { return baseScale; } } [SerializeField] private Vector3 baseScale;

		/// <summary>If this material is used in multiple renderers, you can specify them here. This usually happens with different LOD levels.</summary>
		public List<Renderer> OtherRenderers { set { otherRenderers = value; } get { return otherRenderers; } } [SerializeField] private List<Renderer> otherRenderers;

		/// <summary>This event will be invoked after this component is activated (usually via <b>P3dPaintable</b>).</summary>
		public UnityEvent OnActivated { get { if (onActivated == null) onActivated = new UnityEvent(); return onActivated; } } [SerializeField] private UnityEvent onActivated;

		[SerializeField]
		private bool activated;

		[System.NonSerialized]
		private List<P3dPaintableTexture> paintableTextures = new List<P3dPaintableTexture>();

		[System.NonSerialized]
		private static List<P3dMaterialCloner> materialCloners = new List<P3dMaterialCloner>();

		/// <summary>This lets you know if this paintable has been activated.
		/// Being activated means each associated P3dMaterialCloner and P3dPaintableTexture has been Activated.
		/// NOTE: If you manually add P3dMaterialCloner or P3dPaintableTexture components after activation, then you must manually Activate().</summary>
		public bool Activated
		{
			get
			{
				return activated;
			}
		}

		/// <summary>This gives you a list of all P3dPaintableTexture components that have been activated.</summary>
		public List<P3dPaintableTexture> PaintableTextures
		{
			get
			{
				return paintableTextures;
			}
		}

		/// <summary>This method will remove all <b>P3dPaintable</b>, <b>P3dMaterialCloner</b>, and <b>P3dPaintableTexture</b> components from this GameObject.</summary>
		public void RemoveComponents()
		{
			var paintableTextures = GetComponents<P3dPaintableTexture>();

			for (var i = paintableTextures.Length - 1; i >= 0; i--)
			{
				var paintableTexture = paintableTextures[i];

				paintableTexture.Deactivate();

				P3dHelper.Destroy(paintableTexture);
			}

			var materialCloners = GetComponents<P3dMaterialCloner>();

			for (var i = materialCloners.Length - 1; i >= 0; i--)
			{
				var materialCloner = materialCloners[i];

				materialCloner.Deactivate();

				P3dHelper.Destroy(materialCloner);
			}

			P3dHelper.Destroy(this);
		}

		/// <summary>This will scale the specified width and height values based on the current BaseScale setting.</summary>
		public void ScaleSize(ref int width, ref int height)
		{
			if (baseScale != Vector3.zero)
			{
				var scale = transform.localScale.magnitude / baseScale.magnitude;

				width  = Mathf.CeilToInt(width  * scale);
				height = Mathf.CeilToInt(height * scale);
			}
		}
#if UNITY_EDITOR
		[ContextMenu("Activate", true)]
		private bool ActivateValidate()
		{
			return Application.isPlaying == true && activated == false;
		}
#endif
		/// <summary>This allows you to manually activate all attached P3dMaterialCloner and P3dPaintableTexture components.</summary>
		[ContextMenu("Activate")]
		public void Activate()
		{
			// Activate material cloners
			GetComponents(materialCloners);

			for (var i = materialCloners.Count - 1; i >= 0; i--)
			{
				materialCloners[i].Activate();
			}

			// Activate textures
			GetComponents(paintableTextures);

			for (var i = paintableTextures.Count - 1; i >= 0; i--)
			{
				paintableTextures[i].Activate();
			}

			activated = true;

			if (onActivated != null)
			{
				onActivated.Invoke();
			}
		}
#if UNITY_EDITOR
		[ContextMenu("Deactivate", true)]
		private bool DeactivateValidate()
		{
			return activated == true;
		}
#endif
		/// <summary>This reverses the material cloning.</summary>
		[ContextMenu("Deactivate")]
		public void Deactivate()
		{
			if (activated == true)
			{
				activated = false;

				foreach (var paintableTexture in paintableTextures)
				{
					if (paintableTexture != null)
					{
						paintableTexture.Deactivate();
					}
				}

				paintableTextures.Clear();

				foreach (var materialCloner in materialCloners)
				{
					if (materialCloner != null)
					{
						materialCloner.Deactivate();
					}
				}

				materialCloners.Clear();
			}
		}

		/// <summary>This allows you to clear the pixels of all activated P3dPaintableTexture components associated with this P3dPaintable with the specified color.</summary>
		public void ClearAll(Color color)
		{
			ClearAll(default(Texture), color);
		}

		/// <summary>This allows you to clear the pixels of all activated P3dPaintableTexture components associated with this P3dPaintable with the specified color and texture.</summary>
		public void ClearAll(Texture texture, Color color)
		{
			if (activated == true)
			{
				for (var i = paintableTextures.Count - 1; i >= 0; i--)
				{
					paintableTextures[i].Clear(texture, color);
				}
			}
		}

		/// <summary>This allows you to manually register a P3dPaintableTexture.</summary>
		public void Register(P3dPaintableTexture paintableTexture)
		{
			for (var i = paintableTextures.Count - 1; i >= 0; i--)
			{
				if (paintableTextures[i] == paintableTexture)
				{
					return;
				}
			}

			paintableTextures.Add(paintableTexture);
		}

		/// <summary>This allows you to manually unregister a P3dPaintableTexture.</summary>
		public void Unregister(P3dPaintableTexture paintableTexture)
		{
			for (var i = paintableTextures.Count - 1; i >= 0; i--)
			{
				if (paintableTextures[i] == paintableTexture)
				{
					paintableTextures.RemoveAt(i);
				}
			}
		}

		protected virtual void Awake()
		{
			if (activation == ActivationType.Awake && activated == false)
			{
				Activate();
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			if (activation == ActivationType.OnEnable && activated == false)
			{
				Activate();
			}

			P3dPaintableManager.GetOrCreateInstance();
		}

		protected virtual void Start()
		{
			if (activation == ActivationType.Start && activated == false)
			{
				Activate();
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dPaintable))]
	public class P3dPaintable_Editor : P3dEditor<P3dPaintable>
	{
		private bool expandAdvanced;

		protected override void OnInspector()
		{
			if (Any(t => t.Activated == true))
			{
				EditorGUILayout.HelpBox("This component has been activated.", MessageType.Info);
			}

			if (Any(t => t.Activated == true && Application.isPlaying == false))
			{
				EditorGUILayout.HelpBox("This component shouldn't be activated during edit mode. Deactive it from the component conext menu.", MessageType.Error);
			}

			Draw("activation", "This allows you to control when this component actually activates and becomes ready for painting. You probably don't need to change this.");

			Separator();

			if (Any(t => t.GetComponent<P3dPaintableTexture>() == null))
			{
				EditorGUILayout.HelpBox("Your paintable doesn't have any paintable textures!", MessageType.Warning);
			}

			if (Button("Add Material Cloner") == true)
			{
				Each(t => t.gameObject.AddComponent<P3dMaterialCloner>());
			}

			if (Button("Add Paintable Texture") == true)
			{
				Each(t => t.gameObject.AddComponent<P3dPaintableTexture>());
			}

			if (Button("Analyze Mesh") == true)
			{
				P3dMeshAnalysis.OpenWith(Target.gameObject);
			}

			Separator();

			expandAdvanced = EditorGUILayout.Foldout(expandAdvanced, "Advanced");

			if (expandAdvanced == true)
			{
				EditorGUI.indentLevel++;
					Draw("baseScale", "If you want the paintable texture width/height to be multiplied by the scale of this GameObject, this allows you to set the scale where you want the multiplier to be 1.");
					Draw("includeScale", "Transform the mesh with its position, rotation, and scale? Some skinned mesh setups require this to be disabled.");
					Draw("otherRenderers", "If this material is used in multiple renderers, you can specify them here. This usually happens with different LOD levels.");
					Draw("onActivated");
				EditorGUI.indentLevel--;
			}
		}
	}
}
#endif