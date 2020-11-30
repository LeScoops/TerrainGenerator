using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_GroundTexture", menuName = "Scriptable Objects/Values/GroundTexture")]
public class SO_GroundTextures : ScriptableObject
{
    [System.Serializable]
    public class GroundTexture
    {
        public Texture2D texture = null;
        public float minHeight = 0.1f;
        public float maxHeight = 0.2f;
        public float minSlope = 0.0f;
        public float maxSlope = 90.0f;
        public float offset = 0.1f;
        public float xNoise = 0.1f;
        public float yNoise = 0.1f;
        public float noiseScale = 0.05f;
        public Vector2 tileOffset = new Vector2(0, 0);
        public Vector2 tileSize = new Vector2(50, 50);
    }
    public List<GroundTexture> groundTextures = new List<GroundTexture>()
    {
        new GroundTexture()
    };
}
