using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_MPD", menuName = "Scriptable Objects/Values/MidpointDisplacement")]
public class SO_MPD : ScriptableObject
{
    public int mpdHeightMin = -5;
    public int mpdHeightMax = 5;
    public int mpdHeightDampenerPower = 2;
    public int mpdRoughness = 2;
}
