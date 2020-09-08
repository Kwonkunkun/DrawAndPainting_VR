using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(LightShaft))]
public class LightShaftEditor : Editor 
{
	private LightShaft _target;

	// Slider limit setting
	[System.NonSerialized]
	private Vector2 _rotationSliderLimits = new Vector2(0, 360);
	[System.NonSerialized]
	private Vector2 _distanceSliderLimits = new Vector2(1, 5);
//	[System.NonSerialized]
//	private Vector2 _sizeSliderLimits = new Vector2(.5f, 3);


	// Update message settings
	private static bool _updateVisible = false;
	private static float _startUpdateTimeFader = 0;
	private static float _updateFadingValue = 0;
	private static float _updateFadingTime = 2;
	private static float _updateWaitRatio = .85f;

	void OnEnable()
	{
		// Get the target
		_target = (LightShaft)target;

		// Register editor update method
		EditorApplication.update += OnEditorUpdate;
	}

	protected virtual void OnDisable()
	{
		// Unregister editor update method
		EditorApplication.update -= OnEditorUpdate;
	}
	protected virtual void OnEditorUpdate()
	{
		// Update message timing
		if (_updateVisible) 
		{
			float timePassed = Time.realtimeSinceStartup - _startUpdateTimeFader;
			float waitTime = _updateFadingTime * _updateWaitRatio;

			if (timePassed > waitTime) 
			{
				// Calculate fading value
				float fadeTime = _updateFadingTime - waitTime;
				float fadeTimePassed = timePassed - waitTime;

				_updateFadingValue = 1-(fadeTimePassed / fadeTime);

				// Disable update message when target time is reached
				if (timePassed > _updateFadingTime) 
					_updateVisible = false;
				
				// SetDirty so the UI will update
				EditorUtility.SetDirty(target);
			}
		}
	}

	public override void OnInspectorGUI()
	{
		// HEADER
		// Get texture and its aspect ratio
		var tex = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Farm Life/FarmLife_Editable/Plugins/Light Shaft/Editor/beffio_logo.png");
		float imageAspect = tex.height/tex.width;

		// Make GUIContent from texture
		GUIContent content = new GUIContent(tex);

		// Calculate width and height clamped to the width of the inspector
		float width = EditorGUIUtility.currentViewWidth -40;
		float height = width * imageAspect;

		// Save background color
		Color oldCol = GUI.backgroundColor;
		// Set transparent background color
		GUI.backgroundColor = new Color(0,0,0,0);

		// Draw Header branding
		GUILayout.Box(content, GUILayout.Width(width), GUILayout.Height(height));

		// Reset old color
		GUI.backgroundColor = oldCol;

		// Header label
		EditorGUILayout.LabelField("Light Shaft properties", EditorStyles.boldLabel);

		// Disable all settings when a preset texture is used in mobile mode.
		EditorGUI.BeginDisabledGroup (_target.PresetTexture != null && _target.Mobile);

		// ROTATION
		EditorGUI.BeginChangeCheck ();
		float rotation = EditorGUILayout.Slider("Rotation", _target.Rotation, _rotationSliderLimits.x, _rotationSliderLimits.y);
		if (EditorGUI.EndChangeCheck ()) 
		{
			Undo.RecordObject (_target, "Edit Light Shaft Rotation");
			_target.Rotation = rotation;
		}

		// DISTANCE
		EditorGUI.BeginChangeCheck ();
		float distance = EditorGUILayout.Slider("Distance", _target.Distance, _distanceSliderLimits.x, _distanceSliderLimits.y);
		if (EditorGUI.EndChangeCheck ()) 
		{
			Undo.RecordObject (_target, "Edit Light Shaft Distance");
			_target.Distance = distance;
		}

		// SIZE
		EditorGUI.BeginChangeCheck ();
		float size = EditorGUILayout.Slider("Size", _target.Size, .5f, Mathf.Max(1, _target.Distance+1));
		if (EditorGUI.EndChangeCheck ()) 
		{
			Undo.RecordObject (_target, "Edit Light Shaft Size");
			_target.Size = size;
		}
		EditorGUI.EndDisabledGroup(); // _target.PresetTexture != null && _target.Mobile

		// HIGHLIGHT
		EditorGUI.BeginChangeCheck ();
		float highlight = EditorGUILayout.Slider("Highlight", _target.Highlight, 0, 1);
		if (EditorGUI.EndChangeCheck ()) 
		{
			Undo.RecordObject (_target, "Edit Light Shaft Highlight");
			_target.Highlight = highlight;
		}


		// Disable all settings when a preset texture is used in mobile mode.
		EditorGUI.BeginDisabledGroup (_target.PresetTexture != null && _target.Mobile);

		// GRADIENT COLORS
		SerializedObject serializedGradient = new SerializedObject(_target);
		SerializedProperty colorGradient = serializedGradient.FindProperty("Gradient");

		EditorGUI.BeginChangeCheck ();
		EditorGUILayout.PropertyField(colorGradient, true, null);
		if (EditorGUI.EndChangeCheck ()) 
		{
			serializedGradient.ApplyModifiedProperties();
			Undo.RecordObject (_target, "Edit Light Shaft Color Gradient");
		}

		// LIGHTSHAFT TYPE
		EditorGUI.BeginChangeCheck ();
		GradientType lightShaftType = (GradientType)EditorGUILayout.EnumPopup("Gradient Type", _target.LightShaftType);
		if (EditorGUI.EndChangeCheck ()) 
		{
			Undo.RecordObject (_target, "Edit Light Shaft Type");
			_target.LightShaftType = lightShaftType;
		}

		// BLEND MODE
		EditorGUI.BeginChangeCheck ();
		BlendMode lighstShaftBlend = (BlendMode)EditorGUILayout.EnumPopup("Blend Mode", _target.LightShaftBlend);
		if (EditorGUI.EndChangeCheck ()) 
		{
			Undo.RecordObject (_target, "Edit Light Shaft Blend Mode");
			_target.LightShaftBlend = lighstShaftBlend;
		}

		// Header label
		EditorGUILayout.LabelField("Mobile properties", EditorStyles.boldLabel);

		EditorGUI.EndDisabledGroup(); // _target.PresetTexture != null && _target.Mobile

		// MOBILE & BAKE
		EditorGUILayout.BeginHorizontal();

		// Mobile toggle
		EditorGUILayout.PrefixLabel("Mobile");
		EditorGUI.BeginChangeCheck ();
		bool mobile = EditorGUILayout.Toggle(_target.Mobile);
		if (EditorGUI.EndChangeCheck ()) 
		{
			Undo.RecordObject (_target, "Toggle Mobile Light Shaft");
			_target.Mobile = mobile;
		}

		// Show updated message when the texture is updated
		if (_updateVisible) 
		{
			EditorGUILayout.BeginFadeGroup(_updateFadingValue);
			EditorGUILayout.LabelField("Updated", new GUIStyle(), GUILayout.Width(50));
			EditorGUILayout.EndFadeGroup(); // _updateFadingValue
		}
		
		// Disable all settings when a preset texture is used in mobile mode.
		EditorGUI.BeginDisabledGroup (_target.PresetTexture != null && _target.Mobile);



		// Bake button
		if(GUILayout.Button("Bake Texture"))
		{
			Undo.RecordObject(_target, "Bake Light Shaft Texture");
			_target.GenerateTexture();
			EditorUtility.SetDirty (target);

			// Show update message
			_updateVisible = true;
			_updateFadingValue = 1;
			// Set target time to 2 seconds in the future
			_startUpdateTimeFader = Time.realtimeSinceStartup;
		}

		EditorGUILayout.EndHorizontal();

		// DIMENSIONS
		EditorGUI.BeginChangeCheck ();
		Vector2 textureDimensions = EditorGUILayout.Vector2Field("Texture Dimensions", _target.TextureDimensions);
		if (EditorGUI.EndChangeCheck ()) 
		{
			Undo.RecordObject (_target, "Edit Light Shaft Texture Dimensions");
			_target.TextureDimensions = textureDimensions;
		}

		EditorGUI.EndDisabledGroup(); // _target.PresetTexture != null && _target.Mobile

		// PRESET TEXTURE
		EditorGUI.BeginChangeCheck ();
		Texture2D presetTexture = EditorGUILayout.ObjectField("Preset Texture", _target.PresetTexture, typeof(Texture2D), false) as Texture2D;
		if (EditorGUI.EndChangeCheck ()) 
		{
			Undo.RecordObject (_target, "Set Light Shaft Preset Texture");
			_target.PresetTexture = presetTexture;
		}

		// LIGHT SHAFT SHADER
		EditorGUI.BeginChangeCheck ();
		Shader lightShaftShader = EditorGUILayout.ObjectField("Light Shaft Shader", _target.LightShaftShader, typeof(Shader), false) as Shader;
		if (EditorGUI.EndChangeCheck ()) 
		{
			Undo.RecordObject (_target, "Edit Light Shaft Shader");
			_target.LightShaftShader = lightShaftShader;
		}

		// OVERLAY SHADER
		EditorGUI.BeginChangeCheck ();
		Shader overlayShader = EditorGUILayout.ObjectField("Overlay Shader", _target.OverlayShader, typeof(Shader), false) as Shader;
		if (EditorGUI.EndChangeCheck ()) 
		{
			Undo.RecordObject (_target, "Edit Light Shaft Overlay Shader");
			_target.OverlayShader = overlayShader;
		}

		// Mark object as changed
		if (GUI.changed)
			EditorUtility.SetDirty(target);

	}
}
