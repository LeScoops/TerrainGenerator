using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_CompleteBiome", menuName = "Scriptable Objects/Biomes/Complete")]
public class SO_CompleteBiome : ScriptableObject
{
    public BaseTerrainGeneration baseTerrainGeneration;
    public SO_Smooth smooth;
    public SO_Water water;
    public SO_CompleteDetails completeDetails;

    public void Generate(TerrainData terrainData, float[,] heightMap, GameObject givenGameObject, Transform givenTransform)
    {
        if (baseTerrainGeneration)
        {
            baseTerrainGeneration.GenerateTerrain(terrainData, heightMap);
        }
        if (smooth)
        {
            smooth.Smooth(terrainData);
        }
        if (water)
        {
            water.Generate(terrainData, givenTransform);
        }
        if (completeDetails)
        {
            completeDetails.GenerateDetails(terrainData, givenGameObject, givenTransform);
        }
    }
}
