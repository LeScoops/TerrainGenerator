using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTerrainGeneration : ScriptableObject
{
    public virtual void GenerateTerrain(TerrainData terrainData, float[,] heightMap)
    {
        Debug.Log("ERROR: Type of Terrain not set");
    }
}
