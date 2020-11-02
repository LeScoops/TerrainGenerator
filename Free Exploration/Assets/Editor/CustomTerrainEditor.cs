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

    // Details --------------------
    GUITableState detailMapTable;
    SerializedProperty details;
    SerializedProperty maxDetails;
    SerializedProperty detailSpacing;

    // Water ----------------------
    SerializedProperty waterHeight;
    SerializedProperty waterGameObject;
    SerializedProperty shoreLineMaterial;
    SerializedProperty shoreSize;

    // Erosion --------------------
    SerializedProperty erosionType;
    SerializedProperty erosionStrength;
    SerializedProperty erosionAmount;
    SerializedProperty springsPerRiver;
    SerializedProperty solubility;
    SerializedProperty droplets;
    SerializedProperty erosionSmoothAmount;

    // Clouds ---------------------
    SerializedProperty numberOfClouds;
    SerializedProperty particlesPerCloud;
    SerializedProperty cloudParticleSize;
    SerializedProperty cloudScaleMin;
    SerializedProperty cloudScaleMax;
    SerializedProperty cloudMaterial;
    SerializedProperty cloudShadowMaterial;
    SerializedProperty cloudColour;
    SerializedProperty cloudLining;
    SerializedProperty cloudMinSpeed;
    SerializedProperty cloudMaxSpeed;
    SerializedProperty cloudDistanceTravelled;

    // Fold outs ----------
    bool showRandom = false;
    bool showPerlin = false;
    bool showMultiplePerlin = false;
    bool showVoronoi = false;
    bool showMPD = false;
    bool showSmooth = false;
    bool showTexturing = false;
    bool showVegetation = false;
    bool showDetails = false;
    bool showWater = false;
    bool showErosion = false;
    bool showClouds = false;

    private void OnEnable()
    {
        randomHeightRange = serializedObject.FindProperty("randomHeightRange");
        perlinXScale = serializedObject.FindProperty("perlinXScale");
        perlinYScale = serializedObject.FindProperty("perlinYScale");
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

        vegMapTable = new GUITableState("vegMapTable");
        details = serializedObject.FindProperty("details");
        maximumTrees = serializedObject.FindProperty("maximumTrees");
        treeSpacing = serializedObject.FindProperty("treeSpacing");

        detailMapTable = new GUITableState("detailMapTable");
        vegetation = serializedObject.FindProperty("vegetation");
        maxDetails = serializedObject.FindProperty("maxDetails");
        detailSpacing = serializedObject.FindProperty("detailSpacing");

        waterHeight = serializedObject.FindProperty("waterHeight");
        waterGameObject = serializedObject.FindProperty("waterGameObject");
        shoreLineMaterial = serializedObject.FindProperty("shoreLineMaterial");
        shoreSize = serializedObject.FindProperty("shoreSize");

        erosionType = serializedObject.FindProperty("erosionType");
        erosionStrength = serializedObject.FindProperty("erosionStrength");
        erosionAmount = serializedObject.FindProperty("erosionAmount");
        springsPerRiver = serializedObject.FindProperty("springsPerRiver");
        solubility = serializedObject.FindProperty("solubility");
        droplets = serializedObject.FindProperty("droplets");
        erosionSmoothAmount = serializedObject.FindProperty("erosionSmoothAmount");

        numberOfClouds = serializedObject.FindProperty("numberOfClouds");
        particlesPerCloud = serializedObject.FindProperty("particlesPerCloud");
        cloudParticleSize = serializedObject.FindProperty("cloudParticleSize");
        cloudScaleMin = serializedObject.FindProperty("cloudScaleMin");
        cloudScaleMax = serializedObject.FindProperty("cloudScaleMax");
        cloudMaterial = serializedObject.FindProperty("cloudMaterial");
        cloudShadowMaterial = serializedObject.FindProperty("cloudShadowMaterial");
        cloudColour = serializedObject.FindProperty("cloudColour");
        cloudLining = serializedObject.FindProperty("cloudLining");
        cloudMinSpeed = serializedObject.FindProperty("cloudMinSpeed");
        cloudMaxSpeed = serializedObject.FindProperty("cloudMaxSpeed");
        cloudDistanceTravelled = serializedObject.FindProperty("cloudDistanceTravelled");
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
            //EditorGUILayout.IntSlider(perlinOffsetX, 0, 1000, new GUIContent("X Offset"));
            //EditorGUILayout.IntSlider(perlinOffsetY, 0, 1000, new GUIContent("Y Offset"));
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
            vegMapTable = GUITableLayout.DrawTable(vegMapTable, serializedObject.FindProperty("vegetation"));
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

        showDetails = EditorGUILayout.Foldout(showDetails, "Details");
        if (showDetails)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Details", EditorStyles.boldLabel);
            EditorGUILayout.IntSlider(maxDetails, 0, 5000, new GUIContent("Maximum Details"));
            EditorGUILayout.IntSlider(detailSpacing, 1, 20, new GUIContent("Detail Spacing"));
            detailMapTable = GUITableLayout.DrawTable(detailMapTable, serializedObject.FindProperty("details"));
            terrain.GetComponent<Terrain>().detailObjectDistance = maxDetails.intValue;
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                terrain.AddNewDetail();
            }
            if (GUILayout.Button("-"))
            {
                terrain.RemoveDetail();
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Apply Details"))
            {
                terrain.AddDetails();
            }
        }

        showWater = EditorGUILayout.Foldout(showWater, "Water");
        if (showWater)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Water", EditorStyles.boldLabel);
            EditorGUILayout.Slider(waterHeight, 0, 1, new GUIContent("Water Height"));
            EditorGUILayout.PropertyField(waterGameObject);
            if (GUILayout.Button("Apply Water"))
            {
                terrain.AddWater();
            }
            EditorGUILayout.Slider(shoreSize, 1, 20, new GUIContent("Shore Size"));
            EditorGUILayout.PropertyField(shoreLineMaterial);
            if (GUILayout.Button("Draw Shoreline"))
            {
                terrain.DrawShoreLine();
            }
        }

        showErosion = EditorGUILayout.Foldout(showErosion, "Erosion");
        if (showErosion)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Erosion", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(erosionType);
            EditorGUILayout.Slider(erosionStrength, 0, 1.0f, new GUIContent("Strength"));
            EditorGUILayout.Slider(erosionAmount, 0, 1.0f, new GUIContent("Amount"));
            EditorGUILayout.IntSlider(springsPerRiver, 0, 20, new GUIContent("Springs Per River"));
            EditorGUILayout.Slider(solubility, 0.001f, 1, new GUIContent("Solubility"));
            EditorGUILayout.IntSlider(droplets, 0, 1000, new GUIContent("Droplets"));
            EditorGUILayout.IntSlider(erosionSmoothAmount, 0, 20, new GUIContent("Smooth"));
            if (GUILayout.Button("Generate"))
            {
                terrain.Erosion();
            }
        }

        showClouds = EditorGUILayout.Foldout(showClouds, "Clouds");
        if (showClouds)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Clouds", EditorStyles.boldLabel);
            EditorGUILayout.IntSlider(numberOfClouds, 1, 20, new GUIContent("Num Clouds"));
            EditorGUILayout.IntSlider(particlesPerCloud, 1, 100, new GUIContent("Particles Per"));
            EditorGUILayout.Slider(cloudParticleSize, 1, 20, new GUIContent("Particle Size"));
            EditorGUILayout.PropertyField(cloudScaleMin, new GUIContent("Cloud Scale Min"));
            EditorGUILayout.PropertyField(cloudScaleMin, new GUIContent("Cloud Scale Max"));
            EditorGUILayout.PropertyField(cloudMaterial, new GUIContent("Cloud Material"));
            EditorGUILayout.PropertyField(cloudShadowMaterial, new GUIContent("Shadow Material"));
            EditorGUILayout.PropertyField(cloudColour, new GUIContent("Colour"));
            EditorGUILayout.PropertyField(cloudLining, new GUIContent("Lining"));
            EditorGUILayout.Slider(cloudMinSpeed, 0.1f, 1.0f, new GUIContent("Min Speed"));
            EditorGUILayout.Slider(cloudMaxSpeed, 0.1f, 1.0f, new GUIContent("Max Speed"));
            EditorGUILayout.IntSlider(cloudDistanceTravelled, 100, 1000, new GUIContent("Distance Travelled"));

            if (GUILayout.Button("Generate"))
            {
                terrain.GenerateClouds();
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
