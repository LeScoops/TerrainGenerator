using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_Perlin", menuName = "Scriptable Objects/Values/Perlin")]
public class SO_PerlinValues : BaseTerrainGeneration
{
    public float perlinXScale = 0.01f;
    public float perlinYScale = 0.01f;
    public int perlinOctaves = 3;
    public float perlinPersistance = 8;
    public float perlinHeightScale = 0.09f;
    public int smoothIterations = 1;

    public override void GenerateTerrain(TerrainData terrainData, float[,] heightMap, Transform givenTransform, Vector2 offset)
    {
        for (int x = 0; x < terrainData.heightmapResolution; x++)
        {
            for (int z = 0; z < terrainData.heightmapResolution; z++)
            {
                heightMap[x, z] += TerrainUtils.fBM(
                    (x + offset.x) * perlinXScale,
                    (z + offset.y) * perlinYScale, 
                    perlinOctaves,
                    perlinPersistance) * perlinHeightScale;
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
}
