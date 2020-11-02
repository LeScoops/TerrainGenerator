using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_Voronoi", menuName = "Scriptable Objects/Values/Voronoi")]
public class SO_Voronoi : ScriptableObject
{
    public int vPeakCount = 3;
    public float vFallOff = 0.2f;
    public float vDropOff = 0.6f;
    public float vMinHeight = 0.25f;
    public float vMaxHeight = 0.4f;
    public VoronoiType voronoiType = VoronoiType.Linear;
}
