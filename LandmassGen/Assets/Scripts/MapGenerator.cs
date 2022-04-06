using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public bool autoUpdate;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawNoiseMap(noiseMap);

    }

    void OnValidate()
    {
        mapWidth = (mapWidth < 1) ? 1 : mapWidth;
        mapHeight = (mapHeight < 1) ? 1 : mapHeight;
        lacunarity = (lacunarity < 1) ? 1 : lacunarity;
        octaves = (octaves < 0) ? 0 : octaves;


    }
}
