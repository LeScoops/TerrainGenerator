using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_Trees", menuName = "Scriptable Objects/Values/Trees")]
public class SO_Trees : ScriptableObject
{
    [System.Serializable]
    public class Vegetation
    {
        public GameObject mesh;
        public float minHeight = 0.1f;
        public float maxHeight = 0.2f;
        public float scattering = 5.0f;
        public float minSlope = 0;
        public float maxSlope = 90;
        public float minScale = 0.5f;
        public float maxScale = 1.0f;
        public Color colour1 = Color.white;
        public Color colour2 = Color.white;
        public Color lightColour = Color.white;
        public int numberOfTrees = 100;
    }
    public List<Vegetation> vegetation = new List<Vegetation>()
    {
        new Vegetation()
    };
    public DetailGenerationTypes generationType = DetailGenerationTypes.Grid;
    public int maximumTrees = 1000;
    public int treeSpacing = 5;
    [SerializeField] int terrainLayer = 8;

    public void Generate(TerrainData terrainData, Transform transform)
    {
        TreePrototype[] newTreePrototypes;
        newTreePrototypes = new TreePrototype[vegetation.Count];
        int tindex = 0;
        foreach (SO_Trees.Vegetation t in vegetation)
        {
            newTreePrototypes[tindex] = new TreePrototype();
            newTreePrototypes[tindex].prefab = t.mesh;
            tindex++;
        }
        terrainData.treePrototypes = newTreePrototypes;

        if (generationType == DetailGenerationTypes.Grid)
        {
            Debug.Log("Grid Tree Generation");
            List<TreeInstance> allVegetation = new List<TreeInstance>();
            for (int z = 0; z < terrainData.size.z; z += treeSpacing)
            {
                for (int x = 0; x < terrainData.size.x; x += treeSpacing)
                {
                    for (int tp = 0; tp < terrainData.treePrototypes.Length; tp++)
                    {
                        float thisHeight = terrainData.GetHeight(x, z) / terrainData.size.y;
                        float thisHeightStart = vegetation[tp].minHeight;
                        float thisHeightEnd = vegetation[tp].maxHeight;

                        float steepness = terrainData.GetSteepness(x / terrainData.size.x, z / terrainData.size.z);

                        if ((thisHeight >= thisHeightStart && thisHeight <= thisHeightEnd) &&
                            steepness >= vegetation[tp].minSlope && steepness <= vegetation[tp].maxSlope)
                        {
                            TreeInstance instance = new TreeInstance();
                            instance.position = new Vector3(
                                (x + Random.Range(-vegetation[tp].scattering, vegetation[tp].scattering)) / terrainData.size.x,
                                terrainData.GetHeight(x, z) / terrainData.size.y,
                                (z + Random.Range(-vegetation[tp].scattering, vegetation[tp].scattering)) / terrainData.size.z);

                            Vector3 treeWorldPos = new Vector3(instance.position.x * terrainData.size.x,
                                instance.position.y * terrainData.size.y,
                                instance.position.z * terrainData.size.z) + transform.position;

                            RaycastHit hit;
                            int layerMask = terrainLayer;
                            if (Physics.Raycast(treeWorldPos, -Vector3.up, out hit, 100, layerMask) ||
                                Physics.Raycast(treeWorldPos, Vector3.up, out hit, 100, layerMask))
                            {
                                float treeHeight = (hit.point.y - transform.position.y) / terrainData.size.y;
                                instance.position = new Vector3(instance.position.x, treeHeight, instance.position.z);
                                instance.rotation = Random.Range(0, 360);
                                instance.prototypeIndex = tp;
                                instance.color = Color.Lerp(vegetation[tp].colour1, vegetation[tp].colour2, Random.Range(0.0f, 1.0f));
                                instance.lightmapColor = vegetation[tp].lightColour;
                                float scale = Random.Range(vegetation[tp].minScale, vegetation[tp].maxScale);
                                instance.heightScale = scale;
                                instance.widthScale = scale;
                                allVegetation.Add(instance);
                                if (allVegetation.Count >= maximumTrees) goto TREESDONE;
                            }
                        }
                    }
                }
            }
        TREESDONE:
            terrainData.treeInstances = allVegetation.ToArray();
        }
        else if (generationType == DetailGenerationTypes.Random)
        {
            List<TreeInstance> allVegetation = new List<TreeInstance>();
            for (int tp = 0; tp < terrainData.treePrototypes.Length; tp++)
            {
                int treesSpawned = 0;
                while (treesSpawned < vegetation[tp].numberOfTrees)
                {
                    TreeInstance instance = new TreeInstance();
                    int x = Random.Range(0, (int)terrainData.size.x);
                    int z = Random.Range(0, (int)terrainData.size.z);

                    if (terrainData.GetHeight(x, z) / terrainData.size.y < vegetation[tp].minHeight ||
                        terrainData.GetHeight(x, z) / terrainData.size.y > vegetation[tp].maxHeight)
                    {
                        continue;
                    }
                    instance.position = new Vector3(x / terrainData.size.x,
                                                    terrainData.GetHeight(x, z) / terrainData.size.y,
                                                    z / terrainData.size.z);

                    float targetX = instance.position.x * terrainData.size.x / terrainData.alphamapWidth;
                    float targetZ = instance.position.z * terrainData.size.z / terrainData.alphamapHeight;
                    if (targetX > 1f || targetZ > 1f) { continue; }

                    float steepness = terrainData.GetSteepness(targetX, targetZ);
                    if (steepness <= vegetation[tp].minSlope - 0.1f || steepness >= vegetation[tp].maxSlope) { continue; }

                    instance.position = new Vector3(targetX, instance.position.y, targetZ);
                    instance.rotation = Random.Range(0, 360);
                    instance.prototypeIndex = tp;
                    instance.color = Color.Lerp(vegetation[tp].colour1, vegetation[tp].colour2, Random.Range(0.0f, 1.0f));
                    instance.lightmapColor = vegetation[tp].lightColour;
                    float scale = Random.Range(vegetation[tp].minScale, vegetation[tp].maxScale);
                    instance.heightScale = scale;
                    instance.widthScale = scale;
                    allVegetation.Add(instance);
                    treesSpawned++;
                }
            }
            terrainData.treeInstances = allVegetation.ToArray();
        }
    }
}
