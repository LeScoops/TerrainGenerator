﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GenerateRegion : MonoBehaviour
{
    [SerializeField] bool testRegion = false;
    [SerializeField] Vector2Int regionDimensions = new Vector2Int(2, 2);
    [SerializeField] SO_Biome[] biomes = null;
    [SerializeField] Material defaultMaterial = null;
    Dictionary<string, GameObject> terrainDictionary = new Dictionary<string, GameObject>();

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

        if (terrainDictionary.Count > 0)
        {
            ClearDictionary(regionDimensions);
        }

        for (int x = 0; x < regionDimensions.x; x++)
        {
            for (int z = 0; z < regionDimensions.y; z++)
            {
                GameObject terrainGameObject = new GameObject();
                terrainGameObject.name = x.ToString() + z.ToString();
                terrainGameObject.AddComponent<Terrain>();
                terrainGameObject.AddComponent<TerrainCollider>();
                terrainGameObject.AddComponent<PerlinValueHolder>();

                //int biomeIndex = Random.Range(0, biomes.Length);
                terrainGameObject.GetComponent<PerlinValueHolder>().SetPerlinValues(biomes[x].GetPerlinValues(), x);

                Terrain terrain = terrainGameObject.GetComponent<Terrain>();
                terrain.materialTemplate = defaultMaterial;

                TerrainData terrainData = new TerrainData();
                terrainData.name = x.ToString() + z.ToString();
                terrainData.heightmapResolution = 513;
                terrainData.size = terrainSize;
                terrainData.SetDetailResolution(1024, 32);

                terrainGameObject.GetComponent<Terrain>().terrainData = terrainData;
                terrainGameObject.GetComponent<TerrainCollider>().terrainData = terrainData;
                terrainGameObject.transform.SetParent(this.transform);
                terrainGameObject.transform.position = new Vector3(terrainData.size.x * x, 0, terrainData.size.z * z);

                SO_PerlinValues leftPerlinValues = null;
                Vector2 offset = new Vector2(seed + terrainData.size.z / 2 * z, seed + terrainData.size.x / 2 * x);

                if (x > 0)
                {
                    int leftNeighbourIndex = x - 1;
                    leftPerlinValues = terrainDictionary[leftNeighbourIndex.ToString() + z.ToString()].GetComponent<PerlinValueHolder>().GetPerlinValues();
                    biomes[x].Generate(terrainData, terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution),
                        this.transform, offset, leftPerlinValues);
                }
                else
                {
                    biomes[x].Generate(terrainData, terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution),
                        this.transform, offset);
                }

                terrainDictionary.Add(terrainData.name, terrainGameObject);
            }
        }
    }

    void ClearDictionary(Vector2Int regionDimensions)
    {
        for (int x = 0; x < regionDimensions.x; x++)
        {
            for (int z = 0; z < regionDimensions.y; z++)
            {
                if (terrainDictionary.ContainsKey(x.ToString() + z.ToString()))
                {
                    GameObject objectToDestroy = terrainDictionary[x.ToString() + z.ToString()];
                    terrainDictionary.Remove(x.ToString() + z.ToString());
                    DestroyImmediate(objectToDestroy);
                }
                else
                {
                    Debug.Log("Object to destroy not in dictionary");
                }
            }
        }
        terrainDictionary.Clear();
    }
}
