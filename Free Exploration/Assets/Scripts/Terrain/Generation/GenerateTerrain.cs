using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class GenerateTerrain : MonoBehaviour
{
    public bool resetTerrain = true;
    public Terrain terrain;
    public TerrainData terrainData;

    // Perlin Noise ----------------
    /*public*/ float perlinXScale = 0.01f;
    /*public*/ float perlinYScale = 0.01f;
    /*public*/ int perlinOctaves = 3;
    /*public*/ float perlinPersistance = 8;
    /*public*/ float perlinHeightScale = 0.09f;
    [SerializeField] SO_PerlinValues perlinValues;

    // Voronoi -----------------------
    /*public*/ int vPeakCount = 3;
    /*public*/ float vFallOff = 0.2f;
    /*public*/ float vDropOff = 0.6f;
    /*public*/ float vMinHeight = 0.25f;
    /*public*/ float vMaxHeight = 0.4f;
    /*public*/ VoronoiType voronoiType = VoronoiType.Linear;
    [SerializeField] SO_Voronoi voronoiValues;

    // Midpoint Displacement ------------------------
    /*public*/ int mpdHeightMin = -5;
    /*public*/ int mpdHeightMax = 5;
    /*public*/ int mpdHeightDampenerPower = 2;
    /*public*/ int mpdRoughness = 2;
    [SerializeField] SO_MPD MPDValues;

    // Smooth ---------------------------------------
    public int smoothIterations = 1;


    private void OnEnable()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
    }

    // Single Perlin --------------------------------
    public void Perlin()
    {
        if (perlinValues)
        {
            perlinXScale = perlinValues.perlinXScale;
            perlinYScale = perlinValues.perlinYScale;
            perlinOctaves = perlinValues.perlinOctaves;
            perlinPersistance = perlinValues.perlinPersistance;
            perlinHeightScale = perlinValues.perlinHeightScale;
        }

        float[,] heightMap = GetHeightMap();

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

    // Voronoi ----------------------------------------
    public void Voronoi()
    {
        if (voronoiValues)
        {
            vPeakCount = voronoiValues.vPeakCount;
            vFallOff = voronoiValues.vFallOff;
            vDropOff = voronoiValues.vDropOff;
            vMinHeight = voronoiValues.vMinHeight;
            vMaxHeight = voronoiValues.vMaxHeight;
            voronoiType = voronoiValues.voronoiType;
        }

        float[,] heightMap = GetHeightMap();
        for (int p = 0; p < vPeakCount; p++)
        {
            Vector3 peak = new Vector3(UnityEngine.Random.Range(0, terrainData.heightmapResolution),
                                       UnityEngine.Random.Range(vMinHeight, vMaxHeight),
                                       UnityEngine.Random.Range(0, terrainData.heightmapResolution));

            if (heightMap[(int)peak.x, (int)peak.z] < peak.y)
            {
                heightMap[(int)peak.x, (int)peak.z] = peak.y;
            }
            else
            {
                continue;
            }

            Vector2 peakLocation = new Vector2(peak.x, peak.z);
            float maxDistance = Vector2.Distance(new Vector2(0, 0), new Vector2(terrainData.heightmapResolution, terrainData.heightmapResolution));

            for (int y = 0; y < terrainData.heightmapResolution; y++)
            {
                for (int x = 0; x < terrainData.heightmapResolution; x++)
                {
                    if (!(x == peak.x && y == peak.z))
                    {
                        float distanceToPeak = Vector2.Distance(peakLocation, new Vector2(x, y)) / maxDistance;
                        float height;
                        if (voronoiType == VoronoiType.Combined)
                        {
                            height = peak.y - distanceToPeak * vFallOff - Mathf.Pow(distanceToPeak, vDropOff);
                        }
                        else if (voronoiType == VoronoiType.Power)
                        {
                            height = peak.y - Mathf.Pow(distanceToPeak, vDropOff) * vFallOff;
                        }
                        else if (voronoiType == VoronoiType.SinPow)
                        {
                            height = peak.y - Mathf.Pow(distanceToPeak * 3, vFallOff) -
                                Mathf.Sin(distanceToPeak * 2 * Mathf.PI) / vDropOff;
                        }
                        else
                        {
                            height = peak.y - distanceToPeak * vFallOff;
                        }

                        if (heightMap[x, y] < height)
                        {
                            heightMap[x, y] = height;
                        }
                    }
                }
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    // Midpoint Displacement --------------------------
    public void MidPointDisplacement()
    {
        if (MPDValues)
        {
            mpdHeightMin = MPDValues.mpdHeightMin;
            mpdHeightMax = MPDValues.mpdHeightMax;
            mpdHeightDampenerPower = MPDValues.mpdHeightDampenerPower;
            mpdRoughness = MPDValues.mpdRoughness;
        }

        float[,] heightMap = GetHeightMap();
        int width = terrainData.heightmapResolution - 1;
        int squareSize = width;
        float heightMin = mpdHeightMin;
        float heightMax = mpdHeightMax;
        float heightDampener = (float)Mathf.Pow(mpdHeightDampenerPower, -1 * mpdRoughness);

        int cornerX, cornerY;
        int midX, midY;
        int pmidXL, pmidXR, pmidYU, pmidYD;

        while (squareSize > 0)
        {
            for (int x = 0; x < width; x += squareSize)
            {
                for (int y = 0; y < width; y += squareSize)
                {
                    cornerX = (x + squareSize);
                    cornerY = (y + squareSize);

                    midX = (int)(x + squareSize / 2.0f);
                    midY = (int)(y + squareSize / 2.0f);

                    heightMap[midX, midY] = (float)((heightMap[x, y] +
                                                     heightMap[cornerX, y] +
                                                     heightMap[x, cornerY] +
                                                     heightMap[cornerX, cornerY]) / 4.0f +
                                                     UnityEngine.Random.Range(heightMin, heightMax));
                }
            }

            for (int x = 0; x < width; x += squareSize)
            {
                for (int y = 0; y < width; y += squareSize)
                {
                    cornerX = (x + squareSize);
                    cornerY = (y + squareSize);
                    midX = (int)(x + squareSize / 2.0f);
                    midY = (int)(y + squareSize / 2.0f);
                    pmidXR = (int)(midX + squareSize);
                    pmidYU = (int)(midY + squareSize);
                    pmidXL = (int)(midX - squareSize);
                    pmidYD = (int)(midY - squareSize);

                    if (pmidXL <= 0 || pmidYD <= 0 || pmidXR >= width - 1 || pmidYU >= width - 1)
                        continue;

                    heightMap[midX, y] = (float)((heightMap[midX, midY] +
                                                  heightMap[x, y] +
                                                  heightMap[midX, pmidYD] +
                                                  heightMap[cornerX, y]) / 4.0f + UnityEngine.Random.Range(heightMin, heightMax));

                    heightMap[midX, cornerY] = (float)((heightMap[x, cornerY] +
                                                        heightMap[midX, midY] +
                                                        heightMap[cornerX, cornerY] +
                                                        heightMap[midX, pmidYU]) / 4.0f + UnityEngine.Random.Range(heightMin, heightMax));

                    heightMap[x, midY] = (float)((heightMap[x, y] +
                                                  heightMap[pmidXL, midY] +
                                                  heightMap[x, cornerY] +
                                                  heightMap[midX, midY]) / 4.0f + UnityEngine.Random.Range(heightMin, heightMax));

                    heightMap[cornerX, midY] = (float)((heightMap[cornerX, y] +
                                                        heightMap[midX, midY] +
                                                        heightMap[cornerX, cornerY] +
                                                        heightMap[pmidXR, midY]) / 4.0f + UnityEngine.Random.Range(heightMin, heightMax));
                }
            }
            squareSize = (int)(squareSize / 2.0f);
            heightMin *= heightDampener;
            heightMax *= heightDampener;
        }

        terrainData.SetHeights(0, 0, heightMap);
    }

    // Smooth -----------------------------------------
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
        if (!resetTerrain)
        {
            return terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        }
        else
        {
            return new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        }
    }
    public void ResetTerrain()
    {
        float[,] heightMap = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        for (int x = 0; x < terrainData.heightmapResolution; x++)
        {
            for (int z = 0; z < terrainData.heightmapResolution; z++)
            {
                heightMap[x, z] = 0;
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }
}
