using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_Perlin", menuName = "Scriptable Objects/Values/Perlin")]
public class SO_PerlinValues : BaseTerrainGeneration
{
    //[Range(0.1f, 0.0001f)]
    public float perlinXScale = 0.01f;
    //[Range(0.1f, 0.0001f)]
    public float perlinYScale = 0.01f;
    public int perlinOctaves = 3;
    public float perlinPersistance = 8;
    public float perlinHeightScale = 0.09f;
    [Range(0, 10)]
    public int smoothIterations = 1;

    BaseTerrainGeneration leftNeighbour;
    BaseTerrainGeneration upNeighbour;
    BaseTerrainGeneration rightNeighbour;
    BaseTerrainGeneration downNeighbour;

    public override void GenerateTerrain(TerrainData terrainData, float[,] heightMap, Transform givenTransform, Vector2 offset,
        SO_PerlinValues leftPerlinValues = null, TerrainData upNeighbour = null)
    {        
        float[,] leftHeightMap = new float[1, 1];
        float[,] tempHeightMap = new float[1, 1];

        //if (leftPerlinValues)
        //{
        //    Debug.Log("left - " + leftPerlinValues.perlinXScale);
        //    Debug.Log("right - " + perlinXScale);
        //}

        for (int x = 0; x < terrainData.heightmapResolution; x++)
        {
            for (int z = 0; z < terrainData.heightmapResolution; z++)
            {
                //modifiedPerlinYScale = (leftPerlinValues.perlinYScale * (1 - (z / terrainData.heightmapResolution))) +
                //   perlinYScale * (z / terrainData.heightmapResolution);
                    
                if (leftPerlinValues)
                {

                    tempHeightMap[0, 0] = TerrainUtils.fBM(
                        (x + offset.x) * perlinXScale,
                        (z + offset.y) * perlinYScale,
                        perlinOctaves,
                        perlinPersistance) * perlinHeightScale;

                    leftHeightMap[0, 0] = TerrainUtils.fBM(
                        (x + offset.x) * leftPerlinValues.perlinXScale,
                        (z + offset.y) * leftPerlinValues.perlinYScale,
                        leftPerlinValues.perlinOctaves,
                        leftPerlinValues.perlinPersistance) * leftPerlinValues.perlinHeightScale;

                    float finalHeight = tempHeightMap[0,0];

                    if (z < terrainData.heightmapResolution / 6)
                        finalHeight = tempHeightMap[0,0] * (z * 0.01f) + leftHeightMap[0,0] * (1 - z * 0.01f);

                    //Debug.Log("1 Temp Height - " + tempHeightMap[0, 0]);
                    //Debug.Log("2 Left Height - " + leftHeightMap[0, 0]);
                    //Debug.Log("3 Final Height - " + finalHeight);

                    heightMap[x, z] += finalHeight;
                }
                else
                {
                    heightMap[x, z] += TerrainUtils.fBM(
                        (x + offset.x ) * perlinXScale,
                        (z + offset.y ) * perlinYScale, 
                        perlinOctaves,
                        perlinPersistance) * perlinHeightScale;
                }

            }
        }
        Smooth(terrainData, smoothIterations);
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void SetValues(float perlinXScale = 0.01f, float perlinYScale = 0.01f, int perlinOctaves = 3,
        float perlinPersistance = 8, float perlinHeightScale = 0.09f, int smoothIterations = 1)
    {
        this.perlinXScale = perlinXScale;
        this.perlinYScale = perlinYScale;
        this.perlinOctaves = perlinOctaves;
        this.perlinPersistance = perlinPersistance;
        this.perlinHeightScale = perlinHeightScale;
        this.smoothIterations = smoothIterations;
    }

    public SO_PerlinValues GetValues()
    {
        return this;
    }

    public override void SetNeighbours(BaseTerrainGeneration leftNeighbour = null, BaseTerrainGeneration upNeighbour = null,
        BaseTerrainGeneration rightNeighbour = null, BaseTerrainGeneration downNeighbour = null)
    {
        this.leftNeighbour = leftNeighbour;
        this.upNeighbour = upNeighbour;
        this.rightNeighbour = rightNeighbour;
        this.downNeighbour = downNeighbour;
    }
}
