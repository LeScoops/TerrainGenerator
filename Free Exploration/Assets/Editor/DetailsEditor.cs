using EditorGUITable;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GenerateDetails))]
[CanEditMultipleObjects]
public class DetailsEditor : Editor
{
    //TODO DO Something
    SerializedProperty baseDetails;

    SerializedProperty givenLayerMask;

    // Smooth ----------------------
    SerializedProperty smoothIterations;

    // Vegetation -----------------
    SerializedProperty treeValues;

    // Details --------------------
    SerializedProperty detailValues;

    // Ground Textures
    SerializedProperty groundTextureValues;

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

    bool showVegetation = false;
    bool showDetails = false;
    bool showGroundTextures = false;
    bool showWater = false;
    bool showErosion = false;
    bool showClouds = false;

    private void OnEnable()
    {
        //TODO CleanUP
        baseDetails = serializedObject.FindProperty("baseDetails");

        smoothIterations = serializedObject.FindProperty("smoothIterations");
        givenLayerMask = serializedObject.FindProperty("givenLayerMask");

        //treeValues = serializedObject.FindProperty("treeValues");
        //detailValues = serializedObject.FindProperty("detailValues");
        //groundTextureValues = serializedObject.FindProperty("groundTextureValues");

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
        GenerateDetails terrain = (GenerateDetails)target;

        //TODO Clean Up
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("BaseDetails", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(baseDetails);
        if (GUILayout.Button("Apply Details"))
        {
            terrain.Generate();
        }

        EditorGUILayout.PropertyField(givenLayerMask);

        //showVegetation = EditorGUILayout.Foldout(showVegetation, "Vegetation");
        //if (showVegetation)
        //{
        //    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        //    GUILayout.Label("Vegetation", EditorStyles.boldLabel);
        //    EditorGUILayout.PropertyField(treeValues);
        //    if (GUILayout.Button("Apply Vegetation"))
        //    {
        //        terrain.PlantVegetation();
        //    }
        //}

        //showDetails = EditorGUILayout.Foldout(showDetails, "Details");
        //if (showDetails)
        //{
        //    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        //    GUILayout.Label("Details", EditorStyles.boldLabel);
        //    EditorGUILayout.PropertyField(detailValues);
        //    if (GUILayout.Button("Apply Details"))
        //    {
        //        terrain.AddDetails();
        //    }
        //}

        //showGroundTextures = EditorGUILayout.Foldout(showGroundTextures, "Ground Textures");
        //if (showGroundTextures)
        //{
        //    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        //    GUILayout.Label("Ground Textures", EditorStyles.boldLabel);
        //    EditorGUILayout.PropertyField(groundTextureValues);
        //    if (GUILayout.Button("Apply Textures"))
        //    {
        //        terrain.GroundTextures();
        //    }
        //}

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

        serializedObject.ApplyModifiedProperties();
    }
}
