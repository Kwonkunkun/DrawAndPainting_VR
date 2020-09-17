using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This object allows you to switch P3D material shaders between standard shader usage, and scriptable render pipelines (SRP) shaders.</summary>
	[ExecuteInEditMode]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dRenderingPipeline")]
	public class P3dRenderingPipeline : ScriptableObject
	{
		public const string Title = "Paint in 3D";

		public const string Prefix = "P3D";

		/// <summary>This will be true if this asset has been switched to use a scriptable pipeline.</summary>
		public static bool IsScriptable
		{
			get
			{
				var instance = Instance;

				if (instance != null)
				{
					return instance.text == "True";
				}

				return false;
			}
		}

		/// <summary>This gives you the asset used to define </summary>
		public static TextAsset Instance
		{
			get
			{
				if (instance == null)
				{
					instance = Resources.Load<TextAsset>(typeof(P3dRenderingPipeline).Name);
				}

				return instance;
			}
		}

		private static TextAsset instance;

		#pragma warning disable 67
		public static event System.Action<bool> OnPipelineChanged;
		#pragma warning restore 67

#if UNITY_EDITOR
		[ContextMenu("Apply Standard")]
		public void ApplyStandard()
		{
			Apply(false);
		}

		[ContextMenu("Apply Scriptable")]
		public void ApplyScriptable()
		{
			Apply(true);
		}

		private void Apply(bool srp)
		{
			var map      = new Dictionary<Shader, Shader>();
			var text     = default(string);
			var shaders  = AssetDatabase.FindAssets("t:Shader").Select(g => AssetDatabase.LoadAssetAtPath<Shader>(AssetDatabase.GUIDToAssetPath(g))).Where(s => s.name.Contains(Prefix));
			var shadersA = shaders.Where(s => s.name.EndsWith("SRP") == false);
			var shadersB = shaders.Where(s => s.name.EndsWith("SRP") == true );

			foreach (var shaderA in shadersA)
			{
				var shaderBName = shaderA.name.Substring(shaderA.name.LastIndexOf("/")) + " SRP";
				var shaderB     = shadersB.FirstOrDefault(s => s.name.EndsWith(shaderBName));

				if (shaderB != null)
				{
					if (srp == true)
					{
						map.Add(shaderA, shaderB);
					}
					else
					{
						map.Add(shaderB, shaderA);
					}
				}
			}

			text = "REPLACING";

			foreach (var pair in map)
			{
				text += "\n'" + pair.Key.name + "' with '" + pair.Value.name + "'";
			}

			Debug.Log(text);

			foreach (var material in AssetDatabase.FindAssets("t:Material").Select(g => AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(g))))
			{
				if (material != null)
				{
					var replacement = default(Shader);

					if (map.TryGetValue(material.shader, out replacement) == true)
					{
						material.shader = replacement;
					}
				}
			}

			if (srp == true)
			{
				text = "REIMPORTING";

				foreach (var shaderB in shadersB)
				{
					var path = AssetDatabase.GetAssetPath(shaderB);

					text += "\n" + path;

					AssetDatabase.ImportAsset(path);
				}
			}

			if (Instance != null)
			{
				var path = AssetDatabase.GetAssetPath(instance);

				System.IO.File.WriteAllText(path, srp == true ? "True" : "False");

				AssetDatabase.ImportAsset(path);
			}

			if (OnPipelineChanged != null)
			{
				OnPipelineChanged(srp);
			}

			Debug.Log(srp ? "Finished Switching to Scriptable Pipeline" : "Finished Switching to Standard Pipeline");
		}
#endif
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CustomEditor(typeof(P3dRenderingPipeline))]
	public class P3dRenderingPipeline_Editor : Editor
	{
		public override void OnInspectorGUI()
		{
			var tgt = (P3dRenderingPipeline)target;

			EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.Toggle(new GUIContent("Is Scriptable"), P3dRenderingPipeline.IsScriptable);
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.HelpBox("This tool allows you to quickly switch " + P3dRenderingPipeline.Title + " between using the Standard graphics pipeline, and the scriptable graphics pipeline.", MessageType.Info);

			EditorGUILayout.HelpBox("If you upgrade " + P3dRenderingPipeline.Title + ", then you may need to click the Switch To ___ Pipeline button again.", MessageType.Info);

			EditorGUILayout.Separator();

			if (GUILayout.Button("Switch To Standard Pipeline") == true)
			{
				tgt.ApplyStandard();
				
				EditorUtility.SetDirty(tgt);

				serializedObject.Update();
			}

			if (GUILayout.Button("Switch To Scriptable Pipeline") == true)
			{
				tgt.ApplyScriptable();
				
				EditorUtility.SetDirty(tgt);

				serializedObject.Update();
			}
		}
	}
}
#endif