using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_Trees", menuName = "Scriptable Objects/Values/Trees")]
public class SO_Trees : ScriptableObject
{
    //Vegetation ----------------------------------
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
}
