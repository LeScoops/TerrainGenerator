using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(Terrain))]
[ExecuteInEditMode]
public class BiomeCreator : MonoBehaviour
{
    private TerrainData terrainData;
    private float[,] heightMap;

    [Header("----------")]
    [Header("Terrain Generation")]
    [Header("----------")]
    [SerializeField] bool resetTerrain = true;
    [Header("Perlin Noise")]
    [SerializeField] bool testPerlin = false;
    [SerializeField] float perlinXScale = 0.01f;
    [SerializeField] float perlinYScale = 0.01f;
    [SerializeField] int perlinOctaves = 3;
    [SerializeField] float perlinPersistance = 8;
    [SerializeField] float perlinHeightScale = 0.09f;
    [SerializeField] bool createPerlinSO = false;
    [SerializeField] string soName = "SO_PerlinValues_";

    private void Start()
    {
        terrainData = GetComponent<Terrain>().terrainData;
    }

    private void Update()
    {
        // Perlin Noise
        if (testPerlin)
        {
            SO_PerlinValues perlinValues = (SO_PerlinValues)ScriptableObject.CreateInstance("SO_PerlinValues");
            perlinValues.SetValues(perlinXScale, perlinYScale, perlinOctaves, perlinPersistance, perlinHeightScale);
            perlinValues.GenerateTerrain(terrainData, GetHeightMap());
            testPerlin = false;
        }
        if (createPerlinSO)
        {
            GeneratePerlinSO(new SO_PerlinValues(), soName, perlinXScale, perlinYScale, perlinOctaves, perlinPersistance, perlinHeightScale);
            createPerlinSO = false;
        }
    }


    void GeneratePerlinSO(SO_PerlinValues perlinValues, string soName, float perlinXScale = 0.01f, float perlinYScale = 0.01f, int perlinOctaves = 3,
    float perlinPersistance = 8, float perlinHeightScale = 0.09f)
    {
        //TODO Add values to asset before saving
        AssetDatabase.CreateAsset(perlinValues, "Assets/Resources/ScriptableObjects/Perlin/" + /*perlinValues.name.Replace(" ", "")*/ soName + ".asset");
        AssetDatabase.SaveAssets();
    }

    float[,] GetHeightMap()
    {
        if (!resetTerrain)
        {
            return terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        }
        else
        {
            return new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        }
    }
}
