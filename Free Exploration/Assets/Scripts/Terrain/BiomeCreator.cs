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
    [Range(0, 20)]
    [SerializeField] int perlinSmoothIterations = 1;
    [SerializeField] bool createPerlinSO = false;
    [SerializeField] string perlinSOName = "SO_PerlinValues_";

    [Header("Voronoi")]
    [SerializeField] bool testVoronoi = false;
    [SerializeField] int vPeakCount = 3;
    [SerializeField] float vFallOff = 0.2f;
    [SerializeField] float vDropOff = 0.6f;
    [SerializeField] float vMinHeight = 0.25f;
    [SerializeField] float vMaxHeight = 0.4f;
    [SerializeField] VoronoiType voronoiType = VoronoiType.Linear;
    [Range(0, 20)]
    [SerializeField] int vSmoothIterations = 1;
    [SerializeField] bool createVoronoiSO = false;
    [SerializeField] string voronoiSOName = "SO_VoronoiValues_";

    [Header("Midpoint Displacement")]
    [SerializeField] bool testMPD = false;
    [SerializeField] float mpdHeightMin = -5;
    [SerializeField] float mpdHeightMax = 5;
    [SerializeField] int mpdHeightDampenerPower = 2;
    [SerializeField] int mpdRoughness = 2;
    [Range(0, 20)]
    [SerializeField] int mpdSmoothIterations = 1;
    [SerializeField] bool createMPDSO = false;
    [SerializeField] string MPDSOName = "SO_MPDValues_";

    [Header("Water")]
    [SerializeField] bool testWater = false;
    [SerializeField] GameObject waterGameObject = null;
    [SerializeField] float waterHeight = 0.5f;
    [SerializeField] bool createWaterSO = false;
    [SerializeField] string waterSOName = "SO_Water_";

    private void Start()
    {
        terrainData = GetComponent<Terrain>().terrainData;
    }

    private void Update()
    {
        // Terrain Generators
        Perlin();
        Voronoi();
        MPD();
        Water();
    }

    #region Perlin Noise
    void Perlin()
    {
        if (testPerlin)
        {
            SO_PerlinValues perlinValues = (SO_PerlinValues)ScriptableObject.CreateInstance("SO_PerlinValues");
            perlinValues.SetValues(perlinXScale, perlinYScale, perlinOctaves, perlinPersistance, perlinHeightScale, perlinSmoothIterations);
            perlinValues.GenerateTerrain(terrainData, GetHeightMap());
            testPerlin = false;
        }
        if (createPerlinSO)
        {
            SO_PerlinValues perlinValues = (SO_PerlinValues)ScriptableObject.CreateInstance("SO_PerlinValues");
            perlinValues.SetValues(perlinXScale, perlinYScale, perlinOctaves, perlinPersistance, perlinHeightScale, perlinSmoothIterations);
            AssetDatabase.CreateAsset(perlinValues, "Assets/Resources/ScriptableObjects/Perlin/" + perlinSOName + ".asset");
            AssetDatabase.SaveAssets();
            createPerlinSO = false;
        }
    }
    #endregion
    #region Voronoi
    void Voronoi()
    {
        if (testVoronoi)
        {
            SO_Voronoi voronoiValues = (SO_Voronoi)ScriptableObject.CreateInstance("SO_Voronoi");
            voronoiValues.SetValues(vPeakCount, vFallOff, vDropOff, vMinHeight, vMaxHeight, voronoiType, vSmoothIterations);
            voronoiValues.GenerateTerrain(terrainData, GetHeightMap());
            testVoronoi = false;
        }
        if (createVoronoiSO)
        {
            SO_Voronoi voronoiValues = (SO_Voronoi)ScriptableObject.CreateInstance("SO_Voronoi");
            voronoiValues.SetValues(vPeakCount, vFallOff, vDropOff, vMinHeight, vMaxHeight, voronoiType, vSmoothIterations);
            AssetDatabase.CreateAsset(voronoiValues, "Assets/Resources/ScriptableObjects/Voronoi/" + voronoiSOName + ".asset");
            AssetDatabase.SaveAssets();
            createVoronoiSO = false;
        }
    }
    #endregion
    #region MidpointDisplacement
    void MPD()
    {
        if (testMPD)
        {
            SO_MPD mpdValues = (SO_MPD)ScriptableObject.CreateInstance("SO_MPD");
            mpdValues.SetValues(mpdHeightMin, mpdHeightMax, mpdHeightDampenerPower, mpdRoughness, mpdSmoothIterations);
            mpdValues.GenerateTerrain(terrainData, GetHeightMap());
            testMPD = false;
        }
        if (createMPDSO)
        {
            SO_MPD mpdValues = (SO_MPD)ScriptableObject.CreateInstance("SO_MPD");
            mpdValues.SetValues(mpdHeightMin, mpdHeightMax, mpdHeightDampenerPower, mpdRoughness, mpdSmoothIterations);
            AssetDatabase.CreateAsset(mpdValues, "Assets/Resources/ScriptableObjects/MidpointDisplacement/" + MPDSOName + ".asset");
            AssetDatabase.SaveAssets();
            createMPDSO = false;
        }
    }
    #endregion
    #region Water
    void Water()
    {
        if (testWater)
        {
            SO_Water waterValues = (SO_Water)ScriptableObject.CreateInstance("SO_Water");
            waterValues.SetValues(waterGameObject, waterHeight);
            waterValues.Generate(terrainData, this.transform);
            testWater = false;
        }
        if (createWaterSO)
        {
            SO_Water waterValues = (SO_Water)ScriptableObject.CreateInstance("SO_Water");
            waterValues.SetValues(waterGameObject, waterHeight);
            AssetDatabase.CreateAsset(waterValues, "Assets/Resources/ScriptableObjects/Water/" + waterSOName + ".asset");
            AssetDatabase.SaveAssets();
            createWaterSO = false;
        }
    }
    #endregion


    #region Utility
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
    #endregion
}
