using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class GenerateDetails : MonoBehaviour
{
    public Terrain terrain;
    public TerrainData terrainData;

    [SerializeField]
    int terrainLayer = 8;

    public LayerMask givenLayerMask;

    //Smooth ---------------------------------------
    public int smoothIterations = 1;

    //Vegetation ----------------------------------    
    [SerializeField] SO_Trees treeValues = null;

    //Details --------------------------------------
    [SerializeField] SO_Details detailValues = null;

    //Ground Textures -----------------------------
    [SerializeField] SO_GroundTextures groundTextureValues = null;

    //Water ----------------------------------------
    public float waterHeight = 0.5f;
    public GameObject waterGameObject;
    public Material shoreLineMaterial;
    public float shoreSize = 10.0f;

    //Erosion --------------------------------------
    public enum ErosionType
    {
        Rain, Thermal, Tidal, River, Wind, Canyon
    }
    public ErosionType erosionType = ErosionType.Rain;
    public float erosionStrength = 0.1f;
    public float erosionAmount = 0.01f;
    public int springsPerRiver = 5;
    public float solubility = 0.01f;
    public int droplets = 10;
    public int erosionSmoothAmount = 5;

    //Clouds ---------------------------------------
    public int numberOfClouds = 1;
    public int particlesPerCloud = 50;
    public float cloudParticleSize = 5.0f;
    public Vector3 cloudScaleMin = new Vector3(1.0f, 1.0f, 1.0f);
    public Vector3 cloudScaleMax = new Vector3(1.0f, 1.0f, 1.0f);
    public Material cloudMaterial = null;
    public Material cloudShadowMaterial = null;
    public Color cloudColour = Color.white;
    public Color cloudLining = Color.gray;
    public float cloudMinSpeed = 0.2f;
    public float cloudMaxSpeed = 0.5f;
    public int cloudDistanceTravelled = 500;

    private void OnEnable()
    {
        terrain = this.GetComponent<Terrain>();
        terrainData = terrain.terrainData;
    }

    //Vegetation -------------------------------------
    public void PlantVegetation()
    {
        if (!treeValues) return;

        TreePrototype[] newTreePrototypes;
        newTreePrototypes = new TreePrototype[treeValues.vegetation.Count];
        int tindex = 0;
        foreach (SO_Trees.Vegetation t in treeValues.vegetation)
        {
            newTreePrototypes[tindex] = new TreePrototype();
            newTreePrototypes[tindex].prefab = t.mesh;
            tindex++;
        }
        terrainData.treePrototypes = newTreePrototypes;

        if (treeValues.generationType == DetailGenerationTypes.Grid)
        {
            Debug.Log("Grid Tree Generation");
            List<TreeInstance> allVegetation = new List<TreeInstance>();
            for (int z = 0; z < terrainData.size.z; z += treeValues.treeSpacing)
            {
                for (int x = 0; x < terrainData.size.x; x += treeValues.treeSpacing)
                {
                    for (int tp = 0; tp < terrainData.treePrototypes.Length; tp++)
                    {
                        float thisHeight = terrainData.GetHeight(x, z) / terrainData.size.y;
                        float thisHeightStart = treeValues.vegetation[tp].minHeight;
                        float thisHeightEnd = treeValues.vegetation[tp].maxHeight;

                        float steepness = terrainData.GetSteepness(x / terrainData.size.x, z / terrainData.size.z);

                        if ((thisHeight >= thisHeightStart && thisHeight <= thisHeightEnd) &&
                            steepness >= treeValues.vegetation[tp].minSlope && steepness <= treeValues.vegetation[tp].maxSlope)
                        {
                            TreeInstance instance = new TreeInstance();
                            instance.position = new Vector3(
                                (x + Random.Range(-treeValues.vegetation[tp].scattering, treeValues.vegetation[tp].scattering)) / terrainData.size.x,
                                terrainData.GetHeight(x, z) / terrainData.size.y,
                                (z + Random.Range(-treeValues.vegetation[tp].scattering, treeValues.vegetation[tp].scattering)) / terrainData.size.z);

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
                                instance.color = Color.Lerp(treeValues.vegetation[tp].colour1, treeValues.vegetation[tp].colour2, Random.Range(0.0f, 1.0f));
                                instance.lightmapColor = treeValues.vegetation[tp].lightColour;
                                float scale = Random.Range(treeValues.vegetation[tp].minScale, treeValues.vegetation[tp].maxScale);
                                instance.heightScale = scale;
                                instance.widthScale = scale;
                                allVegetation.Add(instance);
                                if (allVegetation.Count >= treeValues.maximumTrees) goto TREESDONE;
                            }
                        }
                    }
                }
            }
        TREESDONE:
            terrainData.treeInstances = allVegetation.ToArray();
        }
        else if (treeValues.generationType == DetailGenerationTypes.Random)
        {
            List<TreeInstance> allVegetation = new List<TreeInstance>();
            for (int tp = 0; tp < terrainData.treePrototypes.Length; tp++)
            {
                int treesSpawned = 0;
                while (treesSpawned < treeValues.vegetation[tp].numberOfTrees)
                {
                    TreeInstance instance = new TreeInstance();
                    int x = Random.Range(0, (int)terrainData.size.x);
                    int z = Random.Range(0, (int)terrainData.size.z);

                    if (terrainData.GetHeight(x, z) / terrainData.size.y < treeValues.vegetation[tp].minHeight ||
                        terrainData.GetHeight(x, z) / terrainData.size.y > treeValues.vegetation[tp].maxHeight)
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
                    if (steepness <= treeValues.vegetation[tp].minSlope - 0.1f || steepness >= treeValues.vegetation[tp].maxSlope) { continue; }

                    instance.position = new Vector3(targetX, instance.position.y, targetZ);
                    instance.rotation = Random.Range(0, 360);
                    instance.prototypeIndex = tp;
                    instance.color = Color.Lerp(treeValues.vegetation[tp].colour1, treeValues.vegetation[tp].colour2, Random.Range(0.0f, 1.0f));
                    instance.lightmapColor = treeValues.vegetation[tp].lightColour;
                    float scale = Random.Range(treeValues.vegetation[tp].minScale, treeValues.vegetation[tp].maxScale);
                    instance.heightScale = scale;
                    instance.widthScale = scale;
                    allVegetation.Add(instance);
                    treesSpawned++;
                }              
            }
            terrainData.treeInstances = allVegetation.ToArray();
        }
    }

    //Details -------------------------------------------
    public void AddDetails()
    {
        if (!detailValues) return;

        DetailPrototype[] newDetailPrototypes;
        newDetailPrototypes = new DetailPrototype[detailValues.details.Count];
        int dindex = 0;
        foreach (SO_Details.Detail d in detailValues.details)
        {
            newDetailPrototypes[dindex] = new DetailPrototype();
            newDetailPrototypes[dindex].prototype = d.prototype;
            newDetailPrototypes[dindex].prototypeTexture = d.prototypeTexture;
            newDetailPrototypes[dindex].dryColor = d.dryColour;
            newDetailPrototypes[dindex].healthyColor = d.healthyColour;
            newDetailPrototypes[dindex].minHeight = d.heightRange.x;
            newDetailPrototypes[dindex].maxHeight = d.heightRange.y;
            newDetailPrototypes[dindex].minWidth = d.widthRange.x;
            newDetailPrototypes[dindex].maxWidth = d.widthRange.y;
            newDetailPrototypes[dindex].noiseSpread = d.noiseSpread;
            if (newDetailPrototypes[dindex].prototype)
            {
                newDetailPrototypes[dindex].usePrototypeMesh = true;
                newDetailPrototypes[dindex].renderMode = DetailRenderMode.VertexLit;
            }
            else
            {
                newDetailPrototypes[dindex].usePrototypeMesh = false;
                newDetailPrototypes[dindex].renderMode = DetailRenderMode.GrassBillboard;
            }
            dindex++;
        }
        terrainData.detailPrototypes = newDetailPrototypes;

        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        for (int i = 0; i < terrainData.detailPrototypes.Length; i++)
        {
            
            int[,] detailMap = new int[terrainData.detailWidth, terrainData.detailHeight];
            for (int y = 0; y < terrainData.detailResolution; y += detailValues.details[i].inDetailSpacing)
            {
                for (int x = 0; x < terrainData.detailResolution; x += detailValues.details[i].inDetailSpacing)
                {
                    if (UnityEngine.Random.Range(0.0f, 1.0f) > detailValues.details[i].density) continue;
                    int xHM = (int)(x / (float)terrainData.detailWidth * terrainData.heightmapResolution);
                    int yHM = (int)(y / (float)terrainData.detailHeight * terrainData.heightmapResolution);
                    float thisNoise = TerrainUtils.Map(Mathf.PerlinNoise(x * detailValues.details[i].feather,
                        y * detailValues.details[i].feather), 0, 1, 0.5f, 1);
                    float thisHeightStart = detailValues.details[i].minHeight * thisNoise - detailValues.details[i].overlap * thisNoise;
                    float nextHeightStart = detailValues.details[i].maxHeight * thisNoise + detailValues.details[i].overlap * thisNoise;
                    float thisHeight = heightMap[yHM, xHM];
                    float steepness = terrainData.GetSteepness(xHM / (float)terrainData.heightmapResolution, yHM / (float)terrainData.heightmapResolution);
                    if ((thisHeight >= thisHeightStart && thisHeight <= nextHeightStart) &&
                        (steepness >= detailValues.details[i].minSlope && steepness <= detailValues.details[i].maxSlope))
                    {
                        detailMap[y, x] = 1;
                    }
                }
            }
            terrainData.SetDetailLayer(0, 0, i, detailMap);
        }           
    }

    //Ground Textures -------------------------------
    public void GroundTextures()
    {
        if (!groundTextureValues) return;
        TerrainLayer[] newSplatPrototype;
        newSplatPrototype = new TerrainLayer[groundTextureValues.groundTextures.Count];
        int spindex = 0;
        foreach (SO_GroundTextures.GroundTexture sh in groundTextureValues.groundTextures)
        {
            newSplatPrototype[spindex] = new TerrainLayer();
            newSplatPrototype[spindex].diffuseTexture = sh.texture;
            newSplatPrototype[spindex].tileOffset = sh.tileOffset;
            newSplatPrototype[spindex].tileSize = sh.tileSize;
            newSplatPrototype[spindex].diffuseTexture.Apply(true);
            string path = "Assets/TerrainAssets/Layers/Layer_" + spindex + ".terrainlayer";
            AssetDatabase.CreateAsset(newSplatPrototype[spindex], path);
            spindex++;
            Selection.activeObject = this.gameObject;
        }
        terrainData.terrainLayers = newSplatPrototype;

        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        float[,,] splatMapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                float[] splat = new float[terrainData.alphamapLayers];
                for (int i = 0; i < groundTextureValues.groundTextures.Count; i++)
                {
                    float noise = Mathf.PerlinNoise(x * groundTextureValues.groundTextures[i].xNoise, y * groundTextureValues.groundTextures[i].yNoise)
                        * groundTextureValues.groundTextures[i].noiseScale;
                    float offset = groundTextureValues.groundTextures[i].offset + noise;
                    float thisHeightStart = groundTextureValues.groundTextures[i].minHeight - offset;
                    float thisHeightStop = groundTextureValues.groundTextures[i].maxHeight + offset;
                    float steepness = terrainData.GetSteepness(y / (float)terrainData.alphamapHeight,
                        x / (float)terrainData.alphamapWidth);
                    if (heightMap[x, y] >= thisHeightStart && heightMap[x, y] <= thisHeightStop &&
                        steepness >= groundTextureValues.groundTextures[i].minSlope && steepness <= groundTextureValues.groundTextures[i].maxSlope)
                    {
                        splat[i] = 1;
                    }
                }
                NormalizeVector(splat);
                for (int j = 0; j < groundTextureValues.groundTextures.Count; j++)
                {
                    splatMapData[x, y, j] = splat[j];
                }
            }
        }
        terrainData.SetAlphamaps(0, 0, splatMapData);
    }

    //Water -----------------------------------------
    public void AddWater()
    {
        GameObject water = GameObject.Find("water");
        if (!water)
        {
            water = Instantiate(waterGameObject, this.transform.position, this.transform.rotation);
            water.name = "water";
        }
        water.transform.position = this.transform.position + new Vector3(terrainData.size.x / 2,
                                                                        waterHeight * terrainData.size.y,
                                                                        terrainData.size.z / 2);
        water.transform.localScale = new Vector3(terrainData.size.x, 1, terrainData.size.z);
    }
    public void DrawShoreLine()
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        for (int y = 0; y < terrainData.heightmapResolution; y++)
        {
            for (int x = 0; x < terrainData.heightmapResolution; x++)
            {
                Vector2 thisLocation = new Vector2(x, y);
                List<Vector2> neighbours = GenerateNeighbours(thisLocation, terrainData.heightmapResolution, terrainData.heightmapResolution);
                foreach (Vector2 n in neighbours)
                {
                    if (heightMap[x, y] < waterHeight && heightMap[(int)n.x, (int)n.y] > waterHeight)
                    {
                        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                        go.GetComponent<MeshRenderer>().material = shoreLineMaterial;
                        go.transform.localScale *= shoreSize;

                        go.transform.position = this.transform.position +
                            new Vector3(y / (float)terrainData.heightmapResolution * terrainData.size.z,
                                        waterHeight * terrainData.size.y,
                                        x / (float)terrainData.heightmapResolution * terrainData.size.x);

                        go.transform.LookAt(new Vector3(n.y / (float)terrainData.heightmapResolution * terrainData.size.z,
                                                        waterHeight * terrainData.size.y,
                                                        n.x / (float)terrainData.heightmapResolution * terrainData.size.x));
                        go.transform.Rotate(90, 0, 0);
                        go.tag = "Shore";
                        go.layer = LayerMask.NameToLayer("Water");
                    }
                }
            }
        }

        GameObject[] shoreQuads = GameObject.FindGameObjectsWithTag("Shore");
        MeshFilter[] meshFilters = new MeshFilter[shoreQuads.Length];
        for (int m = 0; m < shoreQuads.Length; m++)
        {
            meshFilters[m] = shoreQuads[m].GetComponent<MeshFilter>();
        }
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }

        GameObject currentShoreLine = GameObject.Find("ShoreLine");
        if (currentShoreLine)
        {
            DestroyImmediate(currentShoreLine);
        }
        GameObject shoreLine = new GameObject();
        shoreLine.name = "ShoreLine";
        shoreLine.AddComponent<WaveAnimation>();
        shoreLine.transform.position = this.transform.position;
        shoreLine.transform.rotation = this.transform.rotation;
        MeshFilter thisMF = shoreLine.AddComponent<MeshFilter>();
        thisMF.mesh = new Mesh();
        shoreLine.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);

        MeshRenderer r = shoreLine.AddComponent<MeshRenderer>();
        r.sharedMaterial = shoreLineMaterial;

        for (int sQ = 0; sQ < shoreQuads.Length; sQ++)
        {
            DestroyImmediate(shoreQuads[sQ]);
        }
    }

    //Erosion ---------------------------------------
    public void Erosion()
    {
        switch (erosionType)
        {
            case ErosionType.Rain:
                Rain();
                break;
            case ErosionType.River:
                River();
                break;
            case ErosionType.Thermal:
                Thermal();
                break;
            case ErosionType.Tidal:
                Tidal();
                break;
            case ErosionType.Wind:
                Wind();
                break;
            case ErosionType.Canyon:
                Canyon();
                break;
            default:
                break;
        }
        smoothIterations = erosionSmoothAmount;
        Smooth();
    }
    void Rain()
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        for (int i = 0; i < droplets; i++)
        {
            heightMap[UnityEngine.Random.Range(0, terrainData.heightmapResolution), UnityEngine.Random.Range(0, terrainData.heightmapResolution)]
                -= erosionStrength;
        }
        terrainData.SetHeights(0, 0, heightMap);
    }
    void River()
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        float[,] erosionMap = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        for (int i = 0; i < droplets; i++)
        {
            Vector2 dropletPosition = new Vector2(UnityEngine.Random.Range(0, terrainData.heightmapResolution),
                UnityEngine.Random.Range(0, terrainData.heightmapResolution));
            erosionMap[(int)dropletPosition.x, (int)dropletPosition.y] = erosionStrength;
            for (int j = 0; j < springsPerRiver; j++)
            {
                erosionMap = RunRiver(dropletPosition, heightMap, erosionMap, terrainData.heightmapResolution);
            }
        }
        for (int y = 0; y < terrainData.heightmapResolution; y++)
        {
            for (int x = 0; x < terrainData.heightmapResolution; x++)
            {
                if (erosionMap[x, y] > 0)
                {
                    heightMap[x, y] -= erosionMap[x, y];
                }
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }
    float[,] RunRiver(Vector2 dropletPosition, float[,] heightMap, float[,] erosionMap, int heightMapResolution)
    {
        while (erosionMap[(int)dropletPosition.x, (int)dropletPosition.y] > 0)
        {
            List<Vector2> neighbours = GenerateNeighbours(dropletPosition, heightMapResolution, heightMapResolution);
            neighbours.Shuffle();
            bool foundLower = false;
            foreach (Vector2 n in neighbours)
            {
                if (heightMap[(int)n.x, (int)n.y] < heightMap[(int)dropletPosition.x, (int)dropletPosition.y])
                {
                    erosionMap[(int)n.x, (int)n.y] = erosionMap[(int)dropletPosition.x, (int)dropletPosition.y] - solubility;
                    dropletPosition = n;
                    foundLower = true;
                    break;
                }
            }
            if (!foundLower)
            {
                erosionMap[(int)dropletPosition.x, (int)dropletPosition.y] -= solubility;
            }
        }
        return erosionMap;
    }
    void Thermal()
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        for (int y = 0; y < terrainData.heightmapResolution; y++)
        {
            for (int x = 0; x < terrainData.heightmapResolution; x++)
            {
                Vector2 thisLocation = new Vector2(x, y);
                List<Vector2> neighbours = GenerateNeighbours(thisLocation, terrainData.heightmapResolution, terrainData.heightmapResolution);
                foreach (Vector2 n in neighbours)
                {
                    if (heightMap[x, y] > heightMap[(int)n.x, (int)n.y] + erosionStrength)
                    {
                        float currentHeight = heightMap[x, y];
                        heightMap[x, y] -= currentHeight * erosionAmount;
                        heightMap[(int)n.x, (int)n.y] += currentHeight * erosionAmount;
                    }
                }
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }
    void Tidal()
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        for (int y = 0; y < terrainData.heightmapResolution; y++)
        {
            for (int x = 0; x < terrainData.heightmapResolution; x++)
            {
                Vector2 thisLocation = new Vector2(x, y);
                List<Vector2> neighbours = GenerateNeighbours(thisLocation, terrainData.heightmapResolution, terrainData.heightmapResolution);
                foreach (Vector2 n in neighbours)
                {
                    if (heightMap[x, y] < waterHeight && heightMap[(int)n.x, (int)n.y] > waterHeight)
                    {
                        heightMap[x, y] = waterHeight;
                        heightMap[(int)n.x, (int)n.y] -= waterHeight * erosionAmount;
                    }
                }
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }
    void Wind()
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        int resolution = terrainData.heightmapResolution;

        float windDir = 30;
        float sinAngle = -Mathf.Sin(Mathf.Deg2Rad * windDir);
        float cosAngle = Mathf.Cos(Mathf.Deg2Rad * windDir);
        for (int y = -(resolution - 1) * 2; y <= resolution * 2; y += 10)
        {
            for (int x = -(resolution - 1) * 2; x <= resolution * 2; x += 1)
            {
                float thisNoise = (float)Mathf.PerlinNoise(x * 0.06f, y * 0.06f) * 100 * erosionStrength;
                int nx = (int)x;
                int digY = (int)y + (int)thisNoise;
                int ny = (int)y + 5 + (int)thisNoise;

                Vector2 digCoords = new Vector2(x * cosAngle - digY * sinAngle, digY * cosAngle + x * sinAngle);
                Vector2 pileCoords = new Vector2(nx * cosAngle - ny * sinAngle, ny * cosAngle + nx * sinAngle);

                if (!(pileCoords.x < 0 || pileCoords.x > (resolution - 1) || pileCoords.y < 0 || pileCoords.y > (resolution - 1) ||
                    (int)digCoords.x < 0 || (int)digCoords.x > (resolution - 1) || (int)digCoords.y < 0 || (int)digCoords.y > (resolution - 1)))
                {
                    heightMap[(int)digCoords.x, (int)digCoords.y] -= 0.001f;
                    heightMap[(int)pileCoords.x, (int)pileCoords.y] += 0.001f;
                }
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }
    float[,] tempHeightMap;
    void Canyon()
    {
        float digDepth = 0.05f;
        float bankSlope = 0.001f;
        float maxDepth = 0;
        tempHeightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        int cx = 1;
        int cy = UnityEngine.Random.Range(10, terrainData.heightmapResolution - 10);
        while (cy >= 0 && cy < terrainData.heightmapResolution && cx > 0 && cx < terrainData.heightmapResolution)
        {
            CanyonCrawler(cx, cy, tempHeightMap[cx, cy] - digDepth, bankSlope, maxDepth);
            cx = cx + UnityEngine.Random.Range(1, 3);
            cy = cy + UnityEngine.Random.Range(-2, 3);
        }
        terrainData.SetHeights(0, 0, tempHeightMap);
    }
    void CanyonCrawler(int x, int y, float height, float slope, float maxDepth)
    {
        if (x < 0 || x >= terrainData.heightmapResolution) return;
        if (y < 0 || y >= terrainData.heightmapResolution) return;
        if (height <= maxDepth) return;
        if (tempHeightMap[x, y] <= height) return;

        tempHeightMap[x, y] = height;

        CanyonCrawler(x + 1, y, height + UnityEngine.Random.Range(slope, slope + 0.01f), slope, maxDepth);
        CanyonCrawler(x - 1, y, height + UnityEngine.Random.Range(slope, slope + 0.01f), slope, maxDepth);
        CanyonCrawler(x + 1, y + 1, height + UnityEngine.Random.Range(slope, slope + 0.01f), slope, maxDepth);
        CanyonCrawler(x - 1, y + 1, height + UnityEngine.Random.Range(slope, slope + 0.01f), slope, maxDepth);
        CanyonCrawler(x, y - 1, height + UnityEngine.Random.Range(slope, slope + 0.01f), slope, maxDepth);
        CanyonCrawler(x, y + 1, height + UnityEngine.Random.Range(slope, slope + 0.01f), slope, maxDepth);
    }

    //Clouds ---------------------------------------
    public void GenerateClouds()
    {
        GameObject cloudManager = GameObject.Find("CloudManager");
        if (!cloudManager)
        {
            cloudManager = new GameObject();
            cloudManager.name = "CloudManager";
            cloudManager.AddComponent<CloudManager>();
            cloudManager.transform.position = this.transform.position;
        }

        GameObject[] allClouds = GameObject.FindGameObjectsWithTag("Cloud");
        for (int i = 0; i < allClouds.Length; i++)
        {
            DestroyImmediate(allClouds[i]);
        }
        for (int c = 0; c < numberOfClouds; c++)
        {
            GameObject cloudGO = new GameObject();
            cloudGO.name = "Cloud" + c;
            cloudGO.tag = "Cloud";
            cloudGO.transform.rotation = cloudManager.transform.rotation;
            cloudGO.transform.position = cloudManager.transform.position;

            CloudController cc = cloudGO.AddComponent<CloudController>();
            cc.lining = cloudLining;
            cc.colour = cloudColour;
            cc.numberOfParticles = particlesPerCloud;
            cc.minSpeed = cloudMinSpeed;
            cc.maxSpeed = cloudMaxSpeed;
            cc.distance = cloudDistanceTravelled;

            ParticleSystem cloudSystem = cloudGO.AddComponent<ParticleSystem>();
            Renderer cloudRend = cloudGO.GetComponent<Renderer>();
            cloudRend.material = cloudMaterial;

            cloudGO.layer = LayerMask.NameToLayer("Sky");
            GameObject cloudProjector = new GameObject();
            cloudProjector.name = "Shadow";
            cloudProjector.transform.position = cloudGO.transform.position;
            cloudProjector.transform.forward = Vector3.down;
            cloudProjector.transform.parent = cloudGO.transform;

            if (UnityEngine.Random.Range(0, 10) < 5)
            {
                Projector cp = cloudProjector.AddComponent<Projector>();
                cp.material = cloudShadowMaterial;
                cp.farClipPlane = terrainData.size.y;
                int skyLayerMask = 1 << LayerMask.NameToLayer("Sky");
                int waterLayerMask = 1 << LayerMask.NameToLayer("Water");
                cp.ignoreLayers = skyLayerMask | waterLayerMask;
                cp.fieldOfView = 20.0f;
            }

            cloudRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            cloudRend.receiveShadows = false;

            ParticleSystem.MainModule main = cloudSystem.main;
            main.loop = false;
            main.startLifetime = 10000000;
            main.startSpeed = 0;
            main.startSize = cloudParticleSize;
            main.startColor = Color.white;

            var emission = cloudSystem.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, (short)particlesPerCloud) });

            var shape = cloudSystem.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            Vector3 newScale = new Vector3(UnityEngine.Random.Range(cloudScaleMin.x, cloudScaleMax.x),
                UnityEngine.Random.Range(cloudScaleMin.y, cloudScaleMax.y),
                UnityEngine.Random.Range(cloudScaleMin.z, cloudScaleMax.z));
            shape.scale = newScale;

            cloudGO.transform.parent = cloudManager.transform;
            cloudGO.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }

    List<Vector2> GenerateNeighbours(Vector2 pos, int width, int height)
    {
        List<Vector2> neighbours = new List<Vector2>();
        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                if (!(x == 0 && y == 0))
                {
                    Vector2 nPos = new Vector2(Mathf.Clamp(pos.x + x, 0, width - 1), Mathf.Clamp(pos.y + y, 0, height - 1));
                    if (!neighbours.Contains(nPos))
                    {
                        neighbours.Add(nPos);
                    }
                }
            }
        }
        return neighbours;
    }

    public void Smooth()
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        float smoothProgress = 0;
        EditorUtility.DisplayProgressBar("Smoothing Terrain", "Progress", smoothProgress);

        for (int i = 0; i < smoothIterations; i++)
        {
            for (int y = 0; y < terrainData.heightmapResolution; y++)
            {
                for (int x = 0; x < terrainData.heightmapResolution; x++)
                {
                    float avgHeight = heightMap[x, y];
                    List<Vector2> neighbours = GenerateNeighbours(new Vector2(x, y), terrainData.heightmapResolution, terrainData.heightmapResolution);
                    foreach (Vector2 n in neighbours)
                    {
                        avgHeight += heightMap[(int)n.x, (int)n.y];
                    }
                    heightMap[x, y] = avgHeight / ((float)neighbours.Count + 1);
                }
            }
            smoothProgress++;
            EditorUtility.DisplayProgressBar("Smoothing Terrain", "Progress", smoothProgress / smoothIterations);
        }
        terrainData.SetHeights(0, 0, heightMap);
        EditorUtility.ClearProgressBar();
    }

    float[,] GetHeightMap()
    {
        return terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
    }

    void NormalizeVector(float[] v)
    {
        float total = 0;
        for (int i = 0; i < v.Length; i++)
        {
            total += v[i];
        }
        for (int i = 0; i < v.Length; i++)
        {
            v[i] /= total;
        }
    }
}
