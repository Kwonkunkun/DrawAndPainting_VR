using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Knife.HologramEffect
{
    public class HologramShaderEditor : ShaderGUI
    {
        private List<MaterialProperty> propertiesList = new List<MaterialProperty>();
        private List<MaterialProperty> otherPropetiesList = new List<MaterialProperty>();

        private List<MaterialProperty> propertiesFresnel = new List<MaterialProperty>();
        private List<MaterialProperty> propertiesLine1 = new List<MaterialProperty>();
        private List<MaterialProperty> propertiesLine2 = new List<MaterialProperty>();
        private List<MaterialProperty> propertiesLineGlitch = new List<MaterialProperty>();
        private List<MaterialProperty> propertiesRandomGlitch = new List<MaterialProperty>();
        private List<MaterialProperty> propertiesColorGlitch = new List<MaterialProperty>();
        private List<MaterialProperty> propertiesGrain = new List<MaterialProperty>();
        private List<MaterialProperty> propertiesNormalMap = new List<MaterialProperty>();
        private List<MaterialProperty> propertiesSoftIntersection1 = new List<MaterialProperty>();
        private List<MaterialProperty> propertiesSoftIntersection2 = new List<MaterialProperty>();
        private List<MaterialProperty> propertiesDissolve = new List<MaterialProperty>();
        private List<MaterialProperty> propertiesMask = new List<MaterialProperty>();
        private List<MaterialProperty> propertiesAlphaMask = new List<MaterialProperty>();
        private List<MaterialProperty> propertiesVoxelization = new List<MaterialProperty>();

        private static bool isExpandedFresnel;
        private static bool isExpandedLine1;
        private static bool isExpandedLine2;
        private static bool isExpandedLineGlitch;
        private static bool isExpandedRandomGlitch;
        private static bool isExpandedColorGlitch;
        private static bool isExpandedGrain;
        private static bool isExpandedNormalMap;
        private static bool isExpandedSoftIntersection1;
        private static bool isExpandedSoftIntersection2;
        private static bool isExpandedDissolve;
        private static bool isExpandedMask;
        private static bool isExpandedPosition;
        private static bool isExpandedAlphaMask;
        private static bool isExpandedVoxelization;

        private MaterialProperty mainColorProperty;
        private MaterialProperty fresnelFeature;
        private MaterialProperty line1Feature;
        private MaterialProperty line2Feature;
        private MaterialProperty lineBothFeature;
        private MaterialProperty lineGlitchFeature;
        private MaterialProperty randomGlitchFeature;
        private MaterialProperty colorGlitchFeature;
        private MaterialProperty grainFeature;
        private MaterialProperty normalMapFeature;
        private MaterialProperty softIntersection1Feature;
        private MaterialProperty softIntersection2Feature;
        private MaterialProperty dissolveFeature;
        private MaterialProperty maskFeature;
        private MaterialProperty maskLocalFeature;
        private MaterialProperty positionFeature;
        private MaterialProperty positionSpaceFeature;
        private MaterialProperty alphaMaskFeature;
        private MaterialProperty voxelizationFeature;
        private MaterialProperty positionDirectionProperty;
        private MaterialProperty randomOffsetProperty;
        private MaterialProperty zWriteProperty;
        private MaterialProperty cullModeProperty;
        private MaterialProperty alphaProperty;

        /*
			#pragma shader_feature _FRESNELFEATURE_ON
			#pragma shader_feature _LINE1FEATURE_ON
			#pragma shader_feature _LINE2FEATURE_ON
			#pragma shader_feature _LINEBOTHFEATURE_ON
			#pragma shader_feature _LINEGLITCHFEATURE_ON
			#pragma shader_feature _RANDOMGLITCHFEATURE_ON
			#pragma shader_feature _COLORGLITCHFEATURE_ON
			#pragma shader_feature _GRAINFEATURE_ON
			#pragma shader_feature _NORMALMAPFEATURE_ON
			#pragma shader_feature _SOFTINTERSECTIONFEATURE_OFF _SOFTINTERSECTIONFEATURE_ALPHA _SOFTINTERSECTIONFEATURE_COLOR
			#pragma shader_feature _DISSOLVEFEATURE_ON
			#pragma shader_feature _MASKFEATURE_ON
            
			#pragma shader_feature _POSITIONSPACEFEATURE_WORLD _POSITIONSPACEFEATURE_LOCAL _POSITIONSPACEFEATURE_CUSTOM
			#pragma shader_feature _POSITIONFEATURE_X _POSITIONFEATURE_Y _POSITIONFEATURE_Z
         */

        private string[] softIntersection1Keywords = { "_SOFTINTERSECTION1FEATURE_OFF", "_SOFTINTERSECTION1FEATURE_ALPHA", "_SOFTINTERSECTION1FEATURE_COLOR" };
        private string[] softIntersection2Keywords = { "_SOFTINTERSECTION2FEATURE_OFF", "_SOFTINTERSECTION2FEATURE_ALPHA", "_SOFTINTERSECTION2FEATURE_COLOR" };
        private string[] softIntersectionNames = { "Off", "Alpha", "Color" };
        private int[] softIntersectionValues = { 0, 1, 2 };

        private string[] positionKeywords = { "_POSITIONFEATURE_X", "_POSITIONFEATURE_Y", "_POSITIONFEATURE_Z" };
        private string[] positionNames = { "X", "Y", "Z" };
        private int[] positionValues = { 0, 1, 2 };

        private string[] positionSpaceKeywords = { "_POSITIONSPACEFEATURE_WORLD", "_POSITIONSPACEFEATURE_LOCAL", "_POSITIONSPACEFEATURE_CUSTOM" };
        private string[] positionSpaceNames = { "World", "Local", "Custom" };
        private int[] positionSpaceValues = { 0, 1, 2 };

        private MaterialEditor materialEditor;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            try
            {
                this.materialEditor = materialEditor;

                propertiesList.Clear();
                otherPropetiesList.Clear();
                propertiesList.AddRange(properties);

                propertiesFresnel.Clear();
                propertiesLine1.Clear();
                propertiesLine2.Clear();
                propertiesLineGlitch.Clear();
                propertiesRandomGlitch.Clear();
                propertiesColorGlitch.Clear();
                propertiesGrain.Clear();
                propertiesNormalMap.Clear();
                propertiesSoftIntersection1.Clear();
                propertiesSoftIntersection2.Clear();
                propertiesDissolve.Clear();
                propertiesMask.Clear();
                propertiesAlphaMask.Clear();
                propertiesVoxelization.Clear();

                const string feature = "Feature";

                EditorGUI.BeginChangeCheck();
                foreach (var p in propertiesList)
                {
                    string propName = p.displayName;

                    if (propName.Equals("Main Color"))
                    {
                        mainColorProperty = p;
                    } else if(propName.Equals("Random Offset"))
                    {
                        randomOffsetProperty = p;
                    } else if (propName.Equals("Line Both Feature"))
                    {
                        lineBothFeature = p;
                    } else if (propName.Equals("Position Feature"))
                    {
                        positionFeature = p;
                    } else if (propName.Equals("Position Space Feature"))
                    {
                        positionSpaceFeature = p;
                    } else if (propName.Equals("Position Direction"))
                    {
                        positionDirectionProperty = p;
                    } else if(propName.Equals("ZWrite"))
                    {
                        zWriteProperty = p;
                    } else if(propName.Equals("Cull Mode"))
                    {
                        cullModeProperty = p;
                    } else if(propName.Equals("Alpha"))
                    {
                        alphaProperty = p;
                    }
                    else if (propName.StartsWith("Fresnel"))
                    {
                        if (propName.Contains(feature))
                        {
                            fresnelFeature = p;
                        }
                        else
                        {
                            propertiesFresnel.Add(p);
                        }
                    }
                    else if (propName.StartsWith("Line 1"))
                    {
                        if (propName.Contains(feature))
                        {
                            line1Feature = p;
                        }
                        else
                        {
                            propertiesLine1.Add(p);
                        }
                    }
                    else if (propName.StartsWith("Line 2"))
                    {
                        if (propName.Contains(feature))
                        {
                            line2Feature = p;
                        }
                        else
                        {
                            propertiesLine2.Add(p);
                        }
                    }
                    else if (propName.StartsWith("Line Glitch"))
                    {
                        if (propName.Contains(feature))
                        {
                            lineGlitchFeature = p;
                        }
                        else
                        {
                            propertiesLineGlitch.Add(p);
                        }
                    }
                    else if (propName.StartsWith("Random Glitch"))
                    {
                        if (propName.Contains(feature))
                        {
                            randomGlitchFeature = p;
                        }
                        else
                        {
                            propertiesRandomGlitch.Add(p);
                        }
                    }
                    else if (propName.StartsWith("Color Glitch"))
                    {
                        if (propName.Contains(feature))
                        {
                            colorGlitchFeature = p;
                        }
                        else
                        {
                            propertiesColorGlitch.Add(p);
                        }
                    }
                    else if (propName.StartsWith("Grain"))
                    {
                        if (propName.Contains(feature))
                        {
                            grainFeature = p;
                        }
                        else
                        {
                            propertiesGrain.Add(p);
                        }
                    }
                    else if (propName.StartsWith("Normal"))
                    {
                        if (propName.Contains(feature))
                        {
                            normalMapFeature = p;
                        }
                        else
                        {
                            propertiesNormalMap.Add(p);
                        }
                    }
                    else if (propName.StartsWith("Soft Intersection 1"))
                    {
                        if (propName.Contains(feature))
                        {
                            softIntersection1Feature = p;
                        }
                        else
                        {
                            propertiesSoftIntersection1.Add(p);
                        }
                    }
                    else if (propName.StartsWith("Soft Intersection 2"))
                    {
                        if (propName.Contains(feature))
                        {
                            softIntersection2Feature = p;
                        }
                        else
                        {
                            propertiesSoftIntersection2.Add(p);
                        }
                    }
                    else if (propName.StartsWith("Dissolve"))
                    {
                        if (propName.Contains(feature))
                        {
                            dissolveFeature = p;
                        }
                        else
                        {
                            propertiesDissolve.Add(p);
                        }
                    } else if (propName.StartsWith("Mask"))
                    {
                        if (propName.Contains(feature))
                        {
                            if (propName.Equals("Mask Local Feature"))
                            {
                                maskLocalFeature = p;
                            }
                            else
                            {
                                maskFeature = p;
                            }
                        }
                        else
                        {
                            propertiesMask.Add(p);
                        }
                    } else if (propName.StartsWith("Alpha Mask"))
                    {
                        if (propName.Contains(feature))
                        {
                            alphaMaskFeature = p;
                        }
                        else
                        {
                            propertiesAlphaMask.Add(p);
                        }
                    }
                    else if (propName.StartsWith("Voxelization"))
                    {
                        if (propName.Contains(feature))
                        {
                            voxelizationFeature = p;
                        }
                        else
                        {
                            propertiesVoxelization.Add(p);
                        }
                    }
                    else
                    {
                        otherPropetiesList.Add(p);
                    }
                }

                materialEditor.ColorProperty(mainColorProperty, "Main Color");
                materialEditor.ShaderProperty(alphaProperty, "Alpha");
                if(GUILayout.Button("Randomize"))
                {
                    randomOffsetProperty.floatValue = Random.Range(0f, 100000f);
                }
                materialEditor.FloatProperty(randomOffsetProperty, "Random Offset");
                materialEditor.ShaderProperty(zWriteProperty, "ZWrite");
                materialEditor.ShaderProperty(cullModeProperty, "Cull Mode");


                isExpandedFresnel = EditorGUILayout.Foldout(isExpandedFresnel, "Fresnel");
                if (isExpandedFresnel)
                {
                    EditorGUI.indentLevel++;
                    DrawFeature(fresnelFeature, "_FRESNELFEATURE_ON");
                    if (IsFeatureEnabled(fresnelFeature))
                    {
                        foreach (var p in propertiesFresnel)
                        {
                            DrawProperty(p);
                        }
                    }
                    EditorGUI.indentLevel--;
                }

                isExpandedLine1 = EditorGUILayout.Foldout(isExpandedLine1, "Line 1");
                if (isExpandedLine1)
                {
                    EditorGUI.indentLevel++;
                    DrawFeature(line1Feature, "_LINE1FEATURE_ON");
                    if (IsFeatureEnabled(line1Feature))
                    {
                        foreach (var p in propertiesLine1)
                        {
                            DrawProperty(p);
                        }
                    }
                    EditorGUI.indentLevel--;
                }

                isExpandedLine2 = EditorGUILayout.Foldout(isExpandedLine2, "Line 2");
                if (isExpandedLine2)
                {
                    EditorGUI.indentLevel++;
                    DrawFeature(line2Feature, "_LINE2FEATURE_ON");
                    if (IsFeatureEnabled(line2Feature))
                    {
                        foreach (var p in propertiesLine2)
                        {
                            DrawProperty(p);
                        }
                    }
                    EditorGUI.indentLevel--;
                }

                if (IsFeatureEnabled(line1Feature) && IsFeatureEnabled(line2Feature))
                {
                    lineBothFeature.floatValue = 1f;
                    EnableKeyword("_LINEBOTHFEATURE_ON", true);
                }
                else
                {
                    lineBothFeature.floatValue = 0f;
                    EnableKeyword("_LINEBOTHFEATURE_ON", false);
                }

                isExpandedLineGlitch = EditorGUILayout.Foldout(isExpandedLineGlitch, "Line Glitch");
                if (isExpandedLineGlitch)
                {
                    EditorGUI.indentLevel++;
                    DrawFeature(lineGlitchFeature, "_LINEGLITCHFEATURE_ON");
                    if (IsFeatureEnabled(lineGlitchFeature))
                    {
                        foreach (var p in propertiesLineGlitch)
                        {
                            DrawProperty(p);
                        }
                    }
                    EditorGUI.indentLevel--;
                }

                isExpandedRandomGlitch = EditorGUILayout.Foldout(isExpandedRandomGlitch, "Random Glitch");
                if (isExpandedRandomGlitch)
                {
                    EditorGUI.indentLevel++;
                    DrawFeature(randomGlitchFeature, "_RANDOMGLITCHFEATURE_ON");
                    if (IsFeatureEnabled(randomGlitchFeature))
                    {
                        foreach (var p in propertiesRandomGlitch)
                        {
                            DrawProperty(p);
                        }
                    }
                    EditorGUI.indentLevel--;
                }

                isExpandedColorGlitch = EditorGUILayout.Foldout(isExpandedColorGlitch, "Color Glitch");
                if (isExpandedColorGlitch)
                {
                    EditorGUI.indentLevel++;
                    DrawFeature(colorGlitchFeature, "_COLORGLITCHFEATURE_ON");
                    if (IsFeatureEnabled(colorGlitchFeature))
                    {
                        foreach (var p in propertiesColorGlitch)
                        {
                            DrawProperty(p);
                        }
                    }
                    EditorGUI.indentLevel--;
                }

                isExpandedGrain = EditorGUILayout.Foldout(isExpandedGrain, "Grain");
                if (isExpandedGrain)
                {
                    EditorGUI.indentLevel++;
                    DrawFeature(grainFeature, "_GRAINFEATURE_ON");
                    if (IsFeatureEnabled(grainFeature))
                    {
                        foreach (var p in propertiesGrain)
                        {
                            DrawProperty(p);
                        }
                    }
                    EditorGUI.indentLevel--;
                }

                isExpandedNormalMap = EditorGUILayout.Foldout(isExpandedNormalMap, "Normal Map");
                if (isExpandedNormalMap)
                {
                    EditorGUI.indentLevel++;
                    DrawFeature(normalMapFeature, "_NORMALMAPFEATURE_ON");
                    if (IsFeatureEnabled(normalMapFeature))
                    {
                        foreach (var p in propertiesNormalMap)
                        {
                            DrawProperty(p);
                        }
                    }
                    EditorGUI.indentLevel--;
                }

                isExpandedSoftIntersection1 = EditorGUILayout.Foldout(isExpandedSoftIntersection1, "Soft Intersection 1");
                if (isExpandedSoftIntersection1)
                {
                    EditorGUI.indentLevel++;
                    DrawFeatureEnum(softIntersection1Feature, softIntersectionNames, softIntersectionValues, softIntersection1Keywords);
                    if (GetFeatureEnumValue(softIntersection1Feature) != 0)
                    {
                        foreach (var p in propertiesSoftIntersection1)
                        {
                            DrawProperty(p);
                        }
                    }
                    EditorGUI.indentLevel--;
                }

                isExpandedSoftIntersection2 = EditorGUILayout.Foldout(isExpandedSoftIntersection2, "Soft Intersection 2");
                if (isExpandedSoftIntersection2)
                {
                    EditorGUI.indentLevel++;
                    DrawFeatureEnum(softIntersection2Feature, softIntersectionNames, softIntersectionValues, softIntersection2Keywords);
                    if (GetFeatureEnumValue(softIntersection2Feature) != 0)
                    {
                        foreach (var p in propertiesSoftIntersection2)
                        {
                            DrawProperty(p);
                        }
                    }
                    EditorGUI.indentLevel--;
                }

                isExpandedDissolve = EditorGUILayout.Foldout(isExpandedDissolve, "Dissolve");
                if (isExpandedDissolve)
                {
                    EditorGUI.indentLevel++;
                    DrawFeature(dissolveFeature, "_DISSOLVEFEATURE_ON");
                    if (IsFeatureEnabled(dissolveFeature))
                    {
                        foreach (var p in propertiesDissolve)
                        {
                            DrawProperty(p);
                        }
                    }
                    EditorGUI.indentLevel--;
                }

                isExpandedMask = EditorGUILayout.Foldout(isExpandedMask, "Mask");
                if (isExpandedMask)
                {
                    EditorGUI.indentLevel++;
                    DrawFeature(maskFeature, "_MASKFEATURE_ON");
                    if (IsFeatureEnabled(maskFeature))
                    {
                        DrawFeature(maskLocalFeature, "_MASKLOCALFEATURE_ON", maskLocalFeature.displayName.Replace(" Feature", ""));
                        foreach (var p in propertiesMask)
                        {
                            DrawProperty(p);
                        }
                    }
                    EditorGUI.indentLevel--;
                }

                isExpandedPosition = EditorGUILayout.Foldout(isExpandedPosition, "Position");
                if (isExpandedPosition)
                {
                    EditorGUI.indentLevel++;
                    DrawFeatureEnum(positionSpaceFeature, positionSpaceNames, positionSpaceValues, positionSpaceKeywords);
                    DrawFeatureEnum(positionFeature, positionNames, positionValues, positionKeywords);
                    DrawProperty(positionDirectionProperty);
                    EditorGUI.indentLevel--;
                }

                isExpandedAlphaMask = EditorGUILayout.Foldout(isExpandedAlphaMask, "Alpha Mask");
                if (isExpandedAlphaMask)
                {
                    EditorGUI.indentLevel++;
                    DrawFeature(alphaMaskFeature, "_ALPHAMASKFEATURE_ON");
                    if (IsFeatureEnabled(alphaMaskFeature))
                    {
                        foreach (var p in propertiesAlphaMask)
                        {
                            DrawProperty(p);
                        }
                    }
                    EditorGUI.indentLevel--;
                }

                isExpandedVoxelization = EditorGUILayout.Foldout(isExpandedVoxelization, "Voxelization");
                if (isExpandedVoxelization)
                {
                    EditorGUI.indentLevel++;
                    DrawFeature(voxelizationFeature, "_VOXELIZATIONFEATURE_ON");
                    if (IsFeatureEnabled(voxelizationFeature))
                    {
                        foreach (var p in propertiesVoxelization)
                        {
                            DrawProperty(p);
                        }
                    }
                    EditorGUI.indentLevel--;
                }

                if (EditorGUI.EndChangeCheck())
                {
                    foreach (var t in materialEditor.targets)
                    {
                        Material m = t as Material;

                        EditorUtility.SetDirty(m);
                    }
                }
            } catch (System.Exception ex)
            {
                base.OnGUI(materialEditor, properties);
                throw ex;
            }
        }

        private bool IsFeatureEnabled(MaterialProperty property)
        {
            return (int)property.floatValue == 1;
        }

        private int GetFeatureEnumValue(MaterialProperty property)
        {
            return (int)property.floatValue;
        }

        private void EnableKeyword(string keywordName, bool value)
        {
            if (value)
            {
                foreach (var t in materialEditor.targets)
                {
                    Material m = t as Material;

                    m.EnableKeyword(keywordName);
                }
            }
            else
            {
                foreach (var t in materialEditor.targets)
                {
                    Material m = t as Material;

                    m.DisableKeyword(keywordName);
                }
            }
        }

        private void EnableKeywordEnum(string[] keywords, int index)
        {
            for (int i = 0; i < keywords.Length; i++)
            {
                if(index == i)
                {
                    foreach (var t in materialEditor.targets)
                    {
                        Material m = t as Material;

                        m.EnableKeyword(keywords[i]);
                    }
                }
                else
                {
                    foreach (var t in materialEditor.targets)
                    {
                        Material m = t as Material;

                        m.DisableKeyword(keywords[i]);
                    }
                }
            }
        }

        private void DrawFeature(MaterialProperty property, string keywordName, string propCustomName = "Enabled")
        {
            string propName = property.displayName;
            propName = propCustomName;

            bool hasMixedValue = property.hasMixedValue;

            if (hasMixedValue)
                EditorGUI.showMixedValue = true;

            bool value = (int)property.floatValue == 1;

            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.Toggle(propName, value);
            if (EditorGUI.EndChangeCheck())
            {
                property.floatValue = value ? 1 : 0;

                EnableKeyword(keywordName, value);
            }

            if(hasMixedValue)
                EditorGUI.showMixedValue = false;
        }

        private void DrawFeatureEnum(MaterialProperty property, string[] names, int[] values, string[] keywords)
        {
            string propName = property.displayName;
            propName = propName.Replace("Feature", "");

            bool hasMixedValue = property.hasMixedValue;

            if (hasMixedValue)
                EditorGUI.showMixedValue = true;

            int value = (int)property.floatValue;

            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.IntPopup(propName, value, names, values);

            if (EditorGUI.EndChangeCheck())
            {
                property.floatValue = value;

                EnableKeywordEnum(keywords, value);
            }
            if (hasMixedValue)
                EditorGUI.showMixedValue = false;
        }

        private void DrawProperty(MaterialProperty property)
        {
            string propName = property.displayName;
            switch (property.type)
            {
                case MaterialProperty.PropType.Color:
                    materialEditor.ColorProperty(property, propName);
                    break;
                case MaterialProperty.PropType.Float:
                    materialEditor.FloatProperty(property, propName);
                    break;
                case MaterialProperty.PropType.Range:
                    materialEditor.RangeProperty(property, propName);
                    break;
                case MaterialProperty.PropType.Texture:
                    materialEditor.TexturePropertySingleLine(new GUIContent(propName), property);
                    if(!property.flags.HasFlag((System.Enum)MaterialProperty.PropFlags.NoScaleOffset))
                        materialEditor.TextureScaleOffsetProperty(property);
                    break;
                case MaterialProperty.PropType.Vector:
                    materialEditor.VectorProperty(property, propName);
                    break;
            }
        }
    }
}