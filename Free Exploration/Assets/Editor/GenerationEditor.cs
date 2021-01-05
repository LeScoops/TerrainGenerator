using UnityEngine;
using UnityEditor;
using EditorGUITable;

[CustomEditor(typeof(GenerateTerrain))]
[CanEditMultipleObjects]
public class GenerationEditor : Editor
{
    //TODO Do Something
    SerializedProperty completeBiomeValues;

    //// Single Perlin -----------
    //SerializedProperty perlinXScale;
    //SerializedProperty perlinYScale;
    //SerializedProperty perlinOctaves;
    //SerializedProperty perlinPersistance;
    //SerializedProperty perlinHeightScale;
    SerializedProperty resetTerrain;
    SerializedProperty perlinValues;

    //// Voronoi ---------------
    //SerializedProperty vPeakCount;
    //SerializedProperty vFallOff;
    //SerializedProperty vDropOff;
    //SerializedProperty vMinHeight;
    //SerializedProperty vMaxHeight;
    //SerializedProperty voronoiType;
    SerializedProperty voronoiValues;

    //// Midpoint Displacement -------
    //SerializedProperty mpdHeightMin;
    //SerializedProperty mpdHeightMax;
    //SerializedProperty mpdHeightDampenerPower;
    //SerializedProperty mpdRoughness;
    SerializedProperty MPDValues;

    // Smooth ----------------------
    SerializedProperty smoothIterations;

    //TODO DO Something
    bool showBiome = false;

    bool showPerlin = false;
    bool showVoronoi = false;
    bool showMPD = false;
    bool showSmooth = false;

    private void OnEnable()
    {
        completeBiomeValues = serializedObject.FindProperty("completeBiomeValues");

        //perlinXScale = serializedObject.FindProperty("perlinXScale");
        //perlinYScale = serializedObject.FindProperty("perlinYScale");
        //perlinOctaves = serializedObject.FindProperty("perlinOctaves");
        //perlinPersistance = serializedObject.FindProperty("perlinPersistance");
        //perlinHeightScale = serializedObject.FindProperty("perlinHeightScale");
        resetTerrain = serializedObject.FindProperty("resetTerrain");
        perlinValues = serializedObject.FindProperty("perlinValues");

        //vPeakCount = serializedObject.FindProperty("vPeakCount");
        //vFallOff = serializedObject.FindProperty("vFallOff");
        //vDropOff = serializedObject.FindProperty("vDropOff");
        //vMinHeight = serializedObject.FindProperty("vMinHeight");
        //vMaxHeight = serializedObject.FindProperty("vMaxHeight");
        //voronoiType = serializedObject.FindProperty("voronoiType");
        voronoiValues = serializedObject.FindProperty("voronoiValues");

        //mpdHeightMin = serializedObject.FindProperty("mpdHeightMin");
        //mpdHeightMax = serializedObject.FindProperty("mpdHeightMax");
        //mpdHeightDampenerPower = serializedObject.FindProperty("mpdHeightDampenerPower");
        //mpdRoughness = serializedObject.FindProperty("mpdRoughness");
        MPDValues = serializedObject.FindProperty("MPDValues");

        smoothIterations = serializedObject.FindProperty("smoothIterations");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GenerateTerrain terrain = (GenerateTerrain)target;
        EditorGUILayout.PropertyField(resetTerrain);

        showBiome = EditorGUILayout.Foldout(showBiome, "Complete Biome");
        if (showBiome)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Biome", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(completeBiomeValues);
            if (GUILayout.Button("Generate"))
            {
                terrain.GenerateTerrains();
            }
        }

        //showPerlin = EditorGUILayout.Foldout(showPerlin, "Single Perlin Noise");
        //if (showPerlin)
        //{
        //    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        //    GUILayout.Label("Single Perlin Noise", EditorStyles.boldLabel);
        //    //EditorGUILayout.Slider(perlinXScale, 0, 0.01f, new GUIContent("X Scale"));
        //    //EditorGUILayout.Slider(perlinYScale, 0, 0.01f, new GUIContent("Y Scale"));
        //    //EditorGUILayout.IntSlider(perlinOctaves, 1, 10, new GUIContent("Octaves"));
        //    //EditorGUILayout.Slider(perlinPersistance, 1, 10, new GUIContent("Persistance"));
        //    //EditorGUILayout.Slider(perlinHeightScale, 0, 1, new GUIContent("Height Scale"));
        //    EditorGUILayout.PropertyField(perlinValues);
        //    if (GUILayout.Button("Generate"))
        //    {
        //        terrain.Perlin();
        //    }
        //}

        //showVoronoi = EditorGUILayout.Foldout(showVoronoi, "Voronoi");
        //if (showVoronoi)
        //{
        //    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        //    GUILayout.Label("Voronoi", EditorStyles.boldLabel);
        //    //EditorGUILayout.IntSlider(vPeakCount, 1, 10, new GUIContent("Peak Count"));
        //    //EditorGUILayout.Slider(vFallOff, 0, 10.0f, new GUIContent("Falloff"));
        //    //EditorGUILayout.Slider(vDropOff, 0, 10.0f, new GUIContent("DropOff"));
        //    //EditorGUILayout.Slider(vMinHeight, 0, 1.0f, new GUIContent("Min Height"));
        //    //EditorGUILayout.Slider(vMaxHeight, 0, 1.0f, new GUIContent("Max Height"));
        //    //EditorGUILayout.PropertyField(voronoiType);
        //    EditorGUILayout.PropertyField(voronoiValues);
        //    if (GUILayout.Button("Generate"))
        //    {
        //        terrain.Voronoi();
        //    }
        //}

        //showMPD = EditorGUILayout.Foldout(showMPD, "Midpoint Displacement");
        //if (showMPD)
        //{
        //    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        //    GUILayout.Label("Midpoint Displacement", EditorStyles.boldLabel);
        //    //EditorGUILayout.IntSlider(mpdHeightMin, -20, 0, new GUIContent("Height Min"));
        //    //EditorGUILayout.IntSlider(mpdHeightMax, 2, 20, new GUIContent("Height Max"));
        //    //EditorGUILayout.IntSlider(mpdHeightDampenerPower, 1, 10, new GUIContent("Height Dampener Power"));
        //    //EditorGUILayout.IntSlider(mpdRoughness, 1, 10, new GUIContent("Roughness"));
        //    EditorGUILayout.PropertyField(MPDValues);
        //    if (GUILayout.Button("Generate"))
        //    {
        //        terrain.MidPointDisplacement();
        //    }
        //}

        showSmooth = EditorGUILayout.Foldout(showSmooth, "Smooth");
        if (showSmooth)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Smooth", EditorStyles.boldLabel);
            EditorGUILayout.IntSlider(smoothIterations, 1, 10, new GUIContent("Smooth Iteration"));
            if (GUILayout.Button("Smooth"))
            {
                terrain.Smooth();
            }
        }

        if (GUILayout.Button("Reset Terrain"))
        {
            terrain.ResetTerrain();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
