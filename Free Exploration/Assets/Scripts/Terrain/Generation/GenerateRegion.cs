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
        int seed = Random.Range(0, 10000);
        Transform parentTransform = this.transform;
        Vector3 terrainSize = new Vector3(1024, 600, 1024);

        int value = biomes.Count;
        for (int x = 0; x < value; x++)
        {
            for (int z = 0; z < value; z++)
            {
                GameObject terrainGameObject = new GameObject();
                terrainGameObject.name = "Biomes-" + x + "-" + z;
                terrainGameObject.AddComponent<Terrain>();
                terrainGameObject.AddComponent<TerrainCollider>();

                Terrain terrain = terrainGameObject.GetComponent<Terrain>();
                terrain.materialTemplate = defaultMaterial;

                TerrainData terrainData = new TerrainData();
                terrainData.name = "Biomes-" + x + "-" + z;   
                terrainData.heightmapResolution = 513;
                terrainData.size = terrainSize;
                terrainData.SetDetailResolution(1024, 32);            

                terrainGameObject.GetComponent<Terrain>().terrainData = terrainData;
                terrainGameObject.GetComponent<TerrainCollider>().terrainData = terrainData;
                terrainGameObject.transform.SetParent(this.transform);

                terrainGameObject.transform.position = new Vector3(terrainData.size.x * x, 0, terrainData.size.z * z);

                Vector2 offset = new Vector2(seed + terrainData.size.z / 2 * z, seed + terrainData.size.x / 2 * x);                

                biomes[x].Generate(terrainData, terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution),
                    this.transform, offset);
            }
        }
    }
}
