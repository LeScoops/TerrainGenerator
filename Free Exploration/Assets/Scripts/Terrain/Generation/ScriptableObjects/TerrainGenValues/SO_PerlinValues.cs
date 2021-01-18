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

    public override void GenerateTerrain(TerrainData terrainData, float[,] heightMap)
    {
        float offset = Random.Range(0, 10000);
        for (int y = 0; y < terrainData.heightmapResolution; y++)
        {
            for (int x = 0; x < terrainData.heightmapResolution; x++)
            {
                heightMap[x, y] += TerrainUtils.fBM((x + offset) * perlinXScale, (y + offset) * perlinYScale, perlinOctaves,
                    perlinPersistance) * perlinHeightScale;
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void SetValues(float perlinXScale = 0.01f, float perlinYScale = 0.01f, int perlinOctaves = 3,
        float perlinPersistance = 8, float perlinHeightScale = 0.09f)
    {
        this.perlinXScale = perlinXScale;
        this.perlinYScale = perlinYScale;
        this.perlinOctaves = perlinOctaves;
        this.perlinPersistance = perlinPersistance;
        this.perlinHeightScale = perlinHeightScale;
    }
}
