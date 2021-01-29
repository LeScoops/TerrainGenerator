using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_Biome", menuName = "Scriptable Objects/Biomes/Biome")]
public class SO_Biome : ScriptableObject
{
    public BaseTerrainGeneration terrainGenerationValues = null;
    public List<BaseDetailsGeneration> listOfDetailsGeneration = null;

    public void Generate(TerrainData terrainData, float[,] heightMap, Transform givenTransform)
    {
        if (terrainGenerationValues)
        {
            terrainGenerationValues.GenerateTerrain(terrainData, heightMap);
        }
        if (listOfDetailsGeneration.Count != 0)
        {
            foreach (BaseDetailsGeneration detailValues in listOfDetailsGeneration)
            {
                detailValues.Generate(terrainData, givenTransform);
            }
        }
    }

    public void SetBaseTerrain(BaseTerrainGeneration terrainGenerationValues)
    {
        this.terrainGenerationValues = terrainGenerationValues;
    }
    public void SetDetailsGenerationList(List<BaseDetailsGeneration> listOfDetailsGeneration)
    {
        this.listOfDetailsGeneration = listOfDetailsGeneration;
    }
}
