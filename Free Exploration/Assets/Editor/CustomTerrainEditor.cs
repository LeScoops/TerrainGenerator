using UnityEngine;
using UnityEditor;
using EditorGUITable;

[CustomEditor(typeof(CustomTerrain))]
[CanEditMultipleObjects]
public class CustomTerrainEditor : Editor
{
    // Properties ----------
    SerializedProperty randomHeightRange;

    // Single Perlin -----------
    SerializedProperty perlinXScale;
    SerializedProperty perlinYScale;
    SerializedProperty perlinOffsetX;
    SerializedProperty perlinOffsetY;
    SerializedProperty perlinOctaves;
    SerializedProperty perlinPersistance;
    SerializedProperty perlinHeightScale;
    SerializedProperty resetTerrain;

    // Multiple Perlin --------
    GUITableState perlinParameterTable;
    SerializedProperty perlinParameters;

    // Voronoi ---------------
    SerializedProperty vPeakCount;
    SerializedProperty vFallOff;
    SerializedProperty vDropOff;
    SerializedProperty vMinHeight;
    SerializedProperty vMaxHeight;
    SerializedProperty voronoiType;

    // Midpoint Displacement -------
    SerializedProperty mpdHeightMin;
    SerializedProperty mpdHeightMax;
    SerializedProperty mpdHeightDampenerPower;
    SerializedProperty mpdRoughness;

    // Smooth ----------------------
    SerializedProperty smoothIterations;

    // Texturing ------------------
    GUITableState splatMapTable;
    SerializedProperty splatHeights;

    // Vegetation -----------------
    GUITableState vegMapTable;
    SerializedProperty vegetation;
    SerializedProperty maximumTrees;
    SerializedProperty treeSpacing;

    // Fold outs ----------
    bool showRandom = false;
    bool showPerlin = false;
    bool showMultiplePerlin = false;
    bool showVoronoi = false;
    bool showMPD = false;
    bool showSmooth = false;
    bool showTexturing = false;
    bool showVegetation = false;

    private void OnEnable()
    {
        randomHeightRange = serializedObject.FindProperty("randomHeightRange");
        perlinXScale = serializedObject.FindProperty("perlinXScale");
        perlinYScale = serializedObject.FindProperty("perlinYScale");
        perlinOffsetX = serializedObject.FindProperty("perlinOffsetX");
        perlinOffsetY = serializedObject.FindProperty("perlinOffsetY");
        perlinOctaves = serializedObject.FindProperty("perlinOctaves");
        perlinPersistance = serializedObject.FindProperty("perlinPersistance");
        perlinHeightScale = serializedObject.FindProperty("perlinHeightScale");
        resetTerrain = serializedObject.FindProperty("resetTerrain");
        perlinParameterTable = new GUITableState("perlinParameterTable");
        perlinParameters = serializedObject.FindProperty("perlinParameters");

        vPeakCount = serializedObject.FindProperty("vPeakCount");
        vFallOff = serializedObject.FindProperty("vFallOff");
        vDropOff = serializedObject.FindProperty("vDropOff");
        vMinHeight = serializedObject.FindProperty("vMinHeight");
        vMaxHeight = serializedObject.FindProperty("vMaxHeight");
        voronoiType = serializedObject.FindProperty("voronoiType");

        mpdHeightMin = serializedObject.FindProperty("mpdHeightMin");
        mpdHeightMax = serializedObject.FindProperty("mpdHeightMax");
        mpdHeightDampenerPower = serializedObject.FindProperty("mpdHeightDampenerPower");
        mpdRoughness = serializedObject.FindProperty("mpdRoughness");

        smoothIterations = serializedObject.FindProperty("smoothIterations");

        splatMapTable = new GUITableState("splatMapTable");
        splatHeights = serializedObject.FindProperty("splatHeights");

        vegMapTable = new GUITableState("vegMapTable"); ;
        vegetation = serializedObject.FindProperty("vegetation");
        maximumTrees = serializedObject.FindProperty("maximumTrees");
        treeSpacing = serializedObject.FindProperty("treeSpacing");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        CustomTerrain terrain = (CustomTerrain)target;
        EditorGUILayout.PropertyField(resetTerrain);

        showRandom = EditorGUILayout.Foldout(showRandom, "Random");
        if (showRandom)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Set heights between random values", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(randomHeightRange);
            if (GUILayout.Button("Random Heights"))
            {
                terrain.RandomTerrain();
            }
        }

        showPerlin = EditorGUILayout.Foldout(showPerlin, "Single Perlin Noise");
        if (showPerlin)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Single Perlin Noise", EditorStyles.boldLabel);
            EditorGUILayout.Slider(perlinXScale, 0, 0.01f, new GUIContent("X Scale"));
            EditorGUILayout.Slider(perlinYScale, 0, 0.01f, new GUIContent("Y Scale"));
            EditorGUILayout.IntSlider(perlinOffsetX, 0, 1000, new GUIContent("X Offset"));
            EditorGUILayout.IntSlider(perlinOffsetY, 0, 1000, new GUIContent("Y Offset"));
            EditorGUILayout.IntSlider(perlinOctaves, 1, 10, new GUIContent("Octaves"));
            EditorGUILayout.Slider(perlinPersistance, 1, 10, new GUIContent("Persistance"));
            EditorGUILayout.Slider(perlinHeightScale, 0, 1, new GUIContent("Height Scale"));
            if (GUILayout.Button("Generate"))
            {
                terrain.Perlin();
            }
        }

        showMultiplePerlin = EditorGUILayout.Foldout(showMultiplePerlin, "Multiple Perlin Noise");
        if (showMultiplePerlin)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Multiple Perlin Noise", EditorStyles.boldLabel);
            perlinParameterTable = GUITableLayout.DrawTable(perlinParameterTable, serializedObject.FindProperty("perlinParameters"));
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                terrain.AddNewPerlin();
            }
            if (GUILayout.Button("-"))
            {
                terrain.RemovePerlin();
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Generate"))
            {
                terrain.MultiplePerlinTerrain();
            }
        }

        showVoronoi = EditorGUILayout.Foldout(showVoronoi, "Voronoi");
        if (showVoronoi)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Voronoi", EditorStyles.boldLabel);
            EditorGUILayout.IntSlider(vPeakCount, 1, 10, new GUIContent("Peak Count"));
            EditorGUILayout.Slider(vFallOff, 0, 10.0f, new GUIContent("Falloff"));
            EditorGUILayout.Slider(vDropOff, 0, 10.0f, new GUIContent("DropOff"));
            EditorGUILayout.Slider(vMinHeight, 0, 1.0f, new GUIContent("Min Height"));
            EditorGUILayout.Slider(vMaxHeight, 0, 1.0f, new GUIContent("Max Height"));
            EditorGUILayout.PropertyField(voronoiType);
            if (GUILayout.Button("Generate"))
            {
                terrain.Voronoi();
            }
        }

        showMPD = EditorGUILayout.Foldout(showMPD, "Midpoint Displacement");
        if (showMPD)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Midpoint Displacement", EditorStyles.boldLabel);
            EditorGUILayout.IntSlider(mpdHeightMin, -20, 0, new GUIContent("Height Min"));
            EditorGUILayout.IntSlider(mpdHeightMax, 2, 20, new GUIContent("Height Max"));
            EditorGUILayout.IntSlider(mpdHeightDampenerPower, 1, 10, new GUIContent("Height Dampener Power"));
            EditorGUILayout.IntSlider(mpdRoughness, 1, 10, new GUIContent("Roughness"));
            if (GUILayout.Button("Generate"))
            {
                terrain.MidPointDisplacement();
            }
        }

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

        showTexturing = EditorGUILayout.Foldout(showTexturing, "Texturing");
        if (showTexturing)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Texturing", EditorStyles.boldLabel);
            splatMapTable = GUITableLayout.DrawTable(splatMapTable, serializedObject.FindProperty("splatHeights"));
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                terrain.AddNewSplatHeight();
            }
            if (GUILayout.Button("-"))
            {
                terrain.RemoveSplatHeight();
            }
            EditorGUILayout.EndHorizontal();
            
            if (GUILayout.Button("Apply Textures"))
            {
                terrain.SplatMaps();
            }
        }

        showVegetation = EditorGUILayout.Foldout(showVegetation, "Vegetation");
        if (showVegetation)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Vegetation", EditorStyles.boldLabel);
            EditorGUILayout.IntSlider(maximumTrees, 0, 10000, new GUIContent("Maximum Trees"));
            EditorGUILayout.IntSlider(treeSpacing, 2, 20, new GUIContent("Tree Spacing"));
            vegMapTable = GUITableLayout.DrawTable(splatMapTable, serializedObject.FindProperty("vegetation"));
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                terrain.AddNewVegetation();
            }
            if (GUILayout.Button("-"))
            {
                terrain.RemoveVegetation();
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Apply Vegetation"))
            {
                terrain.PlantVegetation();
            }
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        if (GUILayout.Button("Reset Terrain"))
        {
            terrain.ResetTerrain();
        }
        

        serializedObject.ApplyModifiedProperties();
    }
}
