using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_CompleteDetails", menuName = "Scriptable Objects/Values/CompleteDetails")]
public class SO_CompleteDetails : ScriptableObject
{
    public SO_Details details;
    public SO_Trees trees;
    public SO_GroundTextures groundTextures;

    public void GenerateDetails(TerrainData terrainData, GameObject gameObject, Transform transform)
    {
        if (details)
        {
            //details.Generate(terrainData);
        }
        if (trees)
        {
            trees.Generate(terrainData, transform);
        }
        if (groundTextures)
        {
            //groundTextures.Generate(terrainData, gameObject);
        }
    }
}
