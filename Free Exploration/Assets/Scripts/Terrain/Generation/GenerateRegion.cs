using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GenerateRegion : MonoBehaviour
{
    [SerializeField] bool testRegion = false;
    [SerializeField] List<SO_Biome> biomes = null;
    [SerializeField] Material defaultMaterial = null;

    private void Update()
    {
        if (testRegion)
        {
            Generate();
            testRegion = false;
        }
    }

    public void Generate()
    {
        for (int i = 0; i < biomes.Count; i++)
        {
            GameObject terrainGameObject = new GameObject();
            terrainGameObject.name = "Biomes-" + i;
            terrainGameObject.AddComponent<Terrain>();
            terrainGameObject.AddComponent<TerrainCollider>();

            Terrain terrain = terrainGameObject.GetComponent<Terrain>();
            terrain.materialTemplate = defaultMaterial;

            TerrainData terrainData = new TerrainData();
            terrainData.name = "Biomes-" + i;   
            terrainData.heightmapResolution = 513;
            terrainData.size = new Vector3(1000, 600, 1000);
            terrainData.SetDetailResolution(1024, 32);            

            terrainGameObject.GetComponent<Terrain>().terrainData = terrainData;
            terrainGameObject.GetComponent<TerrainCollider>().terrainData = terrainData;
            terrainGameObject.transform.SetParent(this.transform);

            terrainGameObject.transform.Translate(Vector3.right * terrainData.size.x * i);
            biomes[i].Generate(terrainData, terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution), this.transform);
        }
    }
}
