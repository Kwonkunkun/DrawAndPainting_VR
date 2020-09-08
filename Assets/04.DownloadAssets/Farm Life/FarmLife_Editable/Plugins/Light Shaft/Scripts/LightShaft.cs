using UnityEngine;
using System.IO;
using System.Collections;
using System.Security.AccessControl;

#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

// Editor specific
#if UNITY_EDITOR
using UnityEditor;
#endif



public enum GradientType
{
	Linear,
	Radial
}
public enum BlendMode
{
	AlphaBlend,
	Additive
}

[ExecuteInEditMode]
[RequireComponent (typeof(Camera))]


/// <summary>
/// Light Shaft post-processing effect component.
/// Requires to be placed on a GameObject with a Camera component.
/// </summary>
public class LightShaft : MonoBehaviour
{
	// Settings
#region Settings
	[SerializeField]
	private float _rotation = 150.0f;

	/// <summary>
	/// Gets or sets the light shaft rotation around the center.
	/// This value is used to calculate polar coordinates together with the <see cref="Distance"/>.
	/// </summary>
	public float Rotation
	{
		get { return _rotation; }
		set
		{
			_rotation = value;
			#if UNITY_EDITOR
				if (Mobile && _liveMobileEditing)
					GenerateTexture();
			#endif
		}
	}
	[SerializeField]
	private float _distance = 1.4f;

	/// <summary>
	/// Gets or sets the distance from the center of the screen to the center of the light shaft.
	/// This value is used to calculate polar coordinates together with the <see cref="Rotation"/>.
	/// </summary>
	public float Distance
	{
		get { return _distance; }
		set
		{
			_distance = value;
			if(_size > _distance+1)
				_size = _distance+1;

			#if UNITY_EDITOR
				if (Mobile && _liveMobileEditing)
					GenerateTexture();
			#endif
		}
	}

	[SerializeField]
	private float _size = 1.0f;

	/// <summary>
	/// Gets or sets the size of the light shaft.
	/// - When in linear mode, the size of the line.
	/// - When in Radial mode, the radius of the ellipse
	/// </summary>
	public float Size
	{
		get { return _size; }
		set
		{
			_size = value;
			#if UNITY_EDITOR
				if (Mobile && _liveMobileEditing)
					GenerateTexture();
			#endif
		}
	}

	[SerializeField]
	private float _highlight = 0.0f;

	/// <summary>
	/// Gets or sets the highlight value.
	/// Will highlight lighter areas in the background image.
	/// </summary>
	public float Highlight
	{
		get { return _highlight; }
		set
		{
			_highlight = value;
			#if UNITY_EDITOR
				if (Mobile && _liveMobileEditing)
					GenerateTexture();
			#endif
		}
	}

	public Gradient Gradient;

	[SerializeField]
	private bool _mobile = false;

	/// <summary>
	/// Gets or sets mobile mode of the light shaft effect.
	/// When enabled the light shaft is static and can't be change during run-time.
	/// </summary>
	public bool Mobile
	{
		get { return _mobile; }
		set
		{
			_mobile = value;
			#if UNITY_EDITOR
				if (Mobile && _liveMobileEditing)
					GenerateTexture();
			#endif

			// Set blending mode again, so
			LightShaftBlend = _lightShaftBlend;
		}
	}

	[SerializeField]
	private GradientType _lightShaftType = GradientType.Linear;

	/// <summary>
	/// Gets or sets the gradient type of the light shaft.
	/// - Linear.
	/// - Radial.
	/// </summary>
	public GradientType LightShaftType
	{
		get { return _lightShaftType; }
		set
		{
			_lightShaftType = value;
			#if UNITY_EDITOR
				if (Mobile && _liveMobileEditing)
					GenerateTexture();
			#endif

			// Setup material keywords
			if(_lightShaftType == GradientType.Linear)
			{
				_lightShaftMaterial.EnableKeyword("LINEAR");
				_lightShaftMaterial.DisableKeyword("RADIAL");
			}
			else if(_lightShaftType == GradientType.Radial)
			{
				_lightShaftMaterial.EnableKeyword("RADIAL");
				_lightShaftMaterial.DisableKeyword("LINEAR");
			}
		}
	}

	[SerializeField]
	private BlendMode _lightShaftBlend = BlendMode.AlphaBlend;

	/// <summary>
	/// Gets or sets the blending mode of the light shaft.
	/// - Alpha-blend.
	/// - Additive.
	/// </summary>
	public BlendMode LightShaftBlend
	{
		get { return _lightShaftBlend; }
		set
		{
			_lightShaftBlend = value;
			#if UNITY_EDITOR
				if (Mobile && _liveMobileEditing)
					GenerateTexture();
			#endif

			// Setup blending mode
			if(_lightShaftBlend == BlendMode.AlphaBlend)
			{
				_overlayMaterial.EnableKeyword("ALPHABLEND");
				_overlayMaterial.DisableKeyword("ADDITIVE");
				_lightShaftMaterial.EnableKeyword("ALPHABLEND");
				_lightShaftMaterial.DisableKeyword("ADDITIVE");
			}
			else if(_lightShaftBlend == BlendMode.Additive)
			{
				_overlayMaterial.EnableKeyword("ADDITIVE");
				_overlayMaterial.DisableKeyword("ALPHABLEND");
				_lightShaftMaterial.EnableKeyword("ADDITIVE");
				_lightShaftMaterial.DisableKeyword("ALPHABLEND");
			}
		}
	}

	[SerializeField]
	private Vector2 _textureDimensions = new Vector2(800, 600);

	/// <summary>
	/// Gets or sets the dimensions of the mobile light shaft overlay texture.
	/// </summary>
	public Vector2 TextureDimensions
	{
		get { return _textureDimensions; }
		set
		{
			_textureDimensions = value;
			#if UNITY_EDITOR
				if (Mobile && _liveMobileEditing)
					GenerateTexture();
			#endif
		}
	}
#endregion

	// Shaders & Materials
	/// <summary>
	/// The light shaft shader, required for rendering the light shaft and generating the mobile light shaft textures.
	/// </summary>
	public Shader LightShaftShader;
	/// <summary>
	/// The overlay shader, required for the mobile texture blending only.
	/// </summary>
	public Shader OverlayShader;
	private Material _lightShaftMaterial;
	private Material _overlayMaterial;

	private Camera _cam;

	private Color[] _colors = new Color[8];
	private Vector4[] _alphas = new Vector4[8];

	// Mobile texture
	#if UNITY_EDITOR
		// The location of where to save the mobile light shaft textures.
		private static string _mobileTexturePath = "/Light Shaft/Textures/";

		private string _currentTexturePath = string.Empty;

		// Live editing of the mobile texture will always save the texture with each change.
		// This may result in laggy or choppy controls.
		private bool _liveMobileEditing = false;
	#endif

	[SerializeField]
	private Texture _lightShaftTexture;
	/// <summary>
	/// Gets or sets the currently used light shaft texture.
	/// </summary>
	public Texture LightShaftTexture { get { return _lightShaftTexture; } set { _lightShaftTexture = value; } }

	[SerializeField]
	private Texture2D _presetTexture = null;
	/// <summary>
	/// Gets or sets the preset texture.
	/// </summary>
	public Texture2D PresetTexture { get { return _presetTexture; } set { _presetTexture = value; } }

	private void Start ()
	{
		// Create shader or disable
		CheckShaderAndCreateMaterial();

		// Setup keywords
		LightShaftBlend = _lightShaftBlend;
		LightShaftType = _lightShaftType;

		if(_cam == null)
			_cam = GetComponent<Camera>();
	}

	private void OnEnable()
	{
		if(_cam == null)
			_cam = GetComponent<Camera>();
	}

	#if UNITY_EDITOR
	private void OnPreRender()
	{
		if (Mobile && LightShaftTexture == null)
		{
			GenerateTexture();
		}
	}
	#endif

	private void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		// If there is no shader or material possible,
		// just return the input
		if (!CheckShaderAndCreateMaterial())
		{
			Graphics.Blit(source, destination);
			return;
		}

		// Render mobile light shaft with texture
		if (Mobile)
		{
		#if UNITY_EDITOR
			if(_lightShaftTexture == null)
				GenerateTexture();
		#endif

			// Will use the preset texture if it exists, otherwise the texture it generated itself
			_overlayMaterial.SetTexture("_OverlayTex", (_presetTexture == null ? _lightShaftTexture : _presetTexture));
			// Set highlight parameter
			_overlayMaterial.SetFloat("_highlight", _highlight);
			Graphics.Blit(source, destination, _overlayMaterial, 0);

		}
		// Render regular light shaft with shader computation
		else
		{
			// Setup light shaft shader parameter and color input
			SetGradientColors();
			SetParameters();

			source.wrapMode = TextureWrapMode.Clamp;
			Graphics.Blit(source, destination, _lightShaftMaterial, 0);
		}
	}

	/// <summary>
	/// Setups the gradient values to not be all white and opaque.
	/// </summary>
	private void SetupGradient()
	{
		Gradient = new Gradient
		{
			alphaKeys = new[] {new GradientAlphaKey(1, 0), new GradientAlphaKey(0, 1)},
			colorKeys = new[] {new GradientColorKey(Color.red, 0), new GradientColorKey(Color.yellow, 1)}
		};
	}

	/// <summary>
	/// Passes through the input gradient colors and alpha values to the light shaft material.
	/// </summary>
	private void SetGradientColors()
	{
		// Setups the gradient values to not be all white and opaque.
		if(Gradient == null)
			SetupGradient();

	#if UNITY_5_4_OR_NEWER
		for(int i = 0; i < 8; ++i)
		{
			if(i >= Gradient.colorKeys.Length)
				_colors[i] = ShaderInputFromColorKey(Gradient.colorKeys[Gradient.colorKeys.Length-1]);
			else
				_colors[i] = ShaderInputFromColorKey(Gradient.colorKeys[i]);

			if (i >= Gradient.alphaKeys.Length)
				_alphas[i] = ShaderInputFromAlphaKey(Gradient.alphaKeys[Gradient.alphaKeys.Length-1]);
			else
				_alphas[i] = ShaderInputFromAlphaKey(Gradient.alphaKeys[i]);
		}
		_lightShaftMaterial.SetColorArray("_Color", _colors);
		_lightShaftMaterial.SetVectorArray("_Alpha", _alphas);
	#else
		for(int i = 0; i < 8; ++i)
		{
			// Pass through gradient color (with the alpha as position)
			if(i >= Gradient.colorKeys.Length)
				_lightShaftMaterial.SetColor(string.Format("_Color{0}",i),
					ShaderInputFromColorKey(Gradient.colorKeys[Gradient.colorKeys.Length-1]));
			else
				_lightShaftMaterial.SetColor(string.Format("_Color{0}",i),
					ShaderInputFromColorKey(Gradient.colorKeys[i]));

			// Pass through gradient alpha (with the y as position)
			if (i >= Gradient.alphaKeys.Length)
				_lightShaftMaterial.SetVector(string.Format("_Alpha{0}",i),
					ShaderInputFromAlphaKey(Gradient.alphaKeys[Gradient.alphaKeys.Length-1]));
			else
				_lightShaftMaterial.SetVector(string.Format("_Alpha{0}",i),
					ShaderInputFromAlphaKey(Gradient.alphaKeys[i]));
		}
	#endif
	}
	/// <summary>
	/// Passes through the parameters determining distance, rotation, size and aspect ratio
	/// to the light shaft material.
	/// </summary>
	private void SetParameters()
	{
		// Setup and pass through light shaft shader parameter input
		_lightShaftMaterial.SetVector("_params", new Vector4(_distance, _rotation, _size, (float) _cam.pixelHeight/ _cam.pixelWidth));
		_lightShaftMaterial.SetFloat("_highlight", _highlight);
	}

	/// <summary>
	/// Creates a gradient color output the shader can parse,
	/// where the alpha value is the keytime within the gradient.
	/// </summary>
	/// <returns>A color with rgb values and a key time as alpha value.</returns>
	/// <param name="key">The gradient color key.</param>
	private Color ShaderInputFromColorKey(GradientColorKey key)
	{
		Color output = key.color;
		output.a = key.time;
		return output;
	}
	/// <summary>
	/// Creates a Vector4 variable the shader can parse,
	/// with each pair (x,y & z,w) containing an alpha value and a key time value.
	/// </summary>
	/// <returns>A Vector4 with 2 alpha/key-time pairs.</returns>
	/// <param name="key">The first gradient alpha key.</param>
	private Vector4 ShaderInputFromAlphaKey(GradientAlphaKey key)
	{
		return new Vector2(key.alpha, key.time);
	}


	/// <summary>
	/// Checks the shader and creates material.
	/// </summary>
	/// <returns><c>true</c>, if shader and material could be created and/or exist, <c>false</c> otherwise.</returns>
	private bool CheckShaderAndCreateMaterial ()
	{
		// Check if shader exists
		if (!OverlayShader)
		{
			Debug.LogErrorFormat("LightShaft.CheckShaderAndCreateMaterial() - No Overlay shader specified.");
			return false;
		}

		// Check if shader is supported
		if (!OverlayShader.isSupported)
		{
			Debug.LogErrorFormat("LightShaft.CheckShaderAndCreateMaterial() - Shader {0} not supported on the current platform.", OverlayShader);
			enabled = false;
			return false;
		}

		// Check if material already exists
		if (_overlayMaterial == null)
		{
			// Create material
			_overlayMaterial = new Material(OverlayShader);
			// Don't save to prevent memory leaks
			_overlayMaterial.hideFlags = HideFlags.DontSave;
		}

		// Check if shader exists
		if (!LightShaftShader)
		{
			Debug.LogErrorFormat("LightShaft.CheckShaderAndCreateMaterial() - No light shaft shader specified.");
			return false;
		}

		// Check if shader is supported
		if (!LightShaftShader.isSupported)
		{
			Debug.LogErrorFormat("LightShaft.Start() - Shader {0} not supported on the current platform.", LightShaftShader);
			enabled = false;
			return false;
		}

		// Check if material already exists
		if (_lightShaftMaterial == null)
		{
			// Create material
			_lightShaftMaterial = new Material(LightShaftShader);
			// Don't save to prevent memory leaks
			_lightShaftMaterial.hideFlags = HideFlags.DontSave;
		}

		return true;
	}

	#if UNITY_EDITOR
	/// <summary>
	/// Bakes the mobile lightshaft texture with the current lightshaft properties and dimensions in the default location.
	/// </summary>
	public void GenerateTexture()
	{
		if (!CheckShaderAndCreateMaterial())
		{
			Debug.LogWarning("LightShaft effect - Can't create texture since the shader is not available.");
			return;
		}

		// Clean old Texture
		if (!_currentTexturePath.Equals(string.Empty))
		{
			File.Delete(string.Format("{0}{1}", Application.dataPath, _currentTexturePath));
		}

		// Create temporary RenderTexture
		RenderTexture rt = RenderTexture.GetTemporary ((int)TextureDimensions.x, (int)TextureDimensions.y, 0, RenderTextureFormat.ARGB32);

		// Setup light shaft shader parameter and color input
		SetGradientColors();
		SetParameters();

		// Render light shaft only (light shaft shader pass 1) to render texture
		Graphics.Blit(null, rt, _lightShaftMaterial, 1);

		// Create temporary texture and copy pixels from render texture
		Texture2D texture = new Texture2D((int)TextureDimensions.x, (int)TextureDimensions.y, TextureFormat.ARGB32, false);
		texture.ReadPixels(new Rect(0,0, (int)TextureDimensions.x, (int)TextureDimensions.y), 0, 0);
		texture.Apply();

		// Release temporary render texture
		RenderTexture.ReleaseTemporary (rt);

		// Save texture to raw png data
		byte[] bytes = texture.EncodeToPNG();
		string mobileTextureExtension = ".png";
		// Delete temporary texture
		DestroyImmediate (texture);

		// Generate name
		#if UNITY_5_3_OR_NEWER
			string sceneName = Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().path);
		#else
			string sceneName = Path.GetFileNameWithoutExtension(EditorApplication.currentScene);
		#endif
		string name = string.Format("LightShaft-{0}.{1}", sceneName, gameObject.GetInstanceID());
		_currentTexturePath = string.Format("{0}{1}{2}", _mobileTexturePath, name, mobileTextureExtension);

		// Check if path exists
		string directoryPath = string.Format("{0}{1}", Application.dataPath, _mobileTexturePath);
		if(!AssetDatabase.IsValidFolder(directoryPath))
			// Create folder if it doesn't exist yet
			Directory.CreateDirectory(directoryPath);

		// Write texture to disk
		File.WriteAllBytes(string.Format("{0}{1}", Application.dataPath, _currentTexturePath), bytes);

		// Import the texture asset
		string assetPath = string.Format("Assets{0}", _currentTexturePath);
		AssetDatabase.ImportAsset(assetPath,ImportAssetOptions.ForceUpdate);

		// Set the texture options (transparancy from alpha)
		TextureImporter imp = TextureImporter.GetAtPath(assetPath) as TextureImporter;
		#if UNITY_5_5_OR_NEWER
			imp.alphaSource = TextureImporterAlphaSource.FromInput;
		#else
			imp.grayscaleToAlpha = false;
		#endif
		imp.alphaIsTransparency = true;

		// Force AssetDatabes to load the texture
		LightShaftTexture = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);

	}
	#endif
}
