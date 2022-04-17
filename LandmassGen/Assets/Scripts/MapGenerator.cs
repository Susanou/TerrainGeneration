using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {
	
	public enum DrawMode {
        HideAll,
        NoiseMap, 
        Mesh,
		FallOffMap,
		};
	public DrawMode drawMode;

	public TerrainData terrainData;
	public NoiseData noiseData;
	public TextureData textureData;

	public Material terrainMaterial;

    [Range(0, MeshGenerator.numSupportedChunkSizes-1)]
    public int chunkSizeIndex;
    [Range(0, MeshGenerator.numSupportedFlatshadedChunkSizes-1)]
    public int flatshadedChunkSizeIndex;


	[Range(0, MeshGenerator.numSupportedLODs-1)]
	public int editorLODPreview;

	public bool autoUpdate;

	Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
	Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

	float[,] fallOffMap;

	void OnValuesUpdated()
	{
		if(!Application.isPlaying)
		{
			DrawMapInEditor();
		}
	}

	void OnTextureValuesUpdated()
	{
		textureData.ApplyToMaterial(terrainMaterial);
	}

	public int mapChunkSize
	{
		get { 

			if(terrainData.useFlatShading)
			{
				return MeshGenerator.supportedFlatshadedChunkSizes[flatshadedChunkSizeIndex] - 1;
			} else
			{
				return MeshGenerator.supportedChunkSizes[chunkSizeIndex] - 1;
			}
		}
	}

	void Awake()
	{
		textureData.ApplyToMaterial(terrainMaterial);
		textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);
	}

	public void DrawMapInEditor() {
		textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);
		MapData mapData = GenerateMapData (Vector2.zero);

		MapDisplay display = FindObjectOfType<MapDisplay> ();

		switch(drawMode) {
			case DrawMode.NoiseMap:
				display.DrawTexture (TextureGenerator.TextureFromHeightMap (mapData.heightMap));
				break;
			case DrawMode.Mesh:
				display.DrawMesh (MeshGenerator.GenerateTerrainMesh (mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, editorLODPreview, terrainData.useFlatShading));
				break;
			case DrawMode.FallOffMap:
				display.DrawTexture (TextureGenerator.TextureFromHeightMap(FallOffGenerator.GenerateFalloffMap(mapChunkSize)));
				break;
			case DrawMode.HideAll:
				display.HideAll();
				break;
			default:
				Debug.LogError("DRAW MODE NOT IMPLEMENTED");
				break;
		}
	}

	public void RequestMapData(Vector2 center, Action<MapData> callback) {
		ThreadStart threadStart = delegate {
			MapDataThread (center, callback);
		};

		new Thread (threadStart).Start ();
	}

	void MapDataThread(Vector2 center, Action<MapData> callback) {
		MapData mapData = GenerateMapData (center);
		lock (mapDataThreadInfoQueue) {
			mapDataThreadInfoQueue.Enqueue (new MapThreadInfo<MapData> (callback, mapData));
		}
	}

	public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback) {
		ThreadStart threadStart = delegate {
			MeshDataThread (mapData,  lod, callback);
		};

		new Thread (threadStart).Start ();
	}

	void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback) {
		MeshData meshData = MeshGenerator.GenerateTerrainMesh (mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, lod, terrainData.useFlatShading);
		lock (meshDataThreadInfoQueue) {
			meshDataThreadInfoQueue.Enqueue (new MapThreadInfo<MeshData> (callback, meshData));
		}
	}

	void Update() {
		if (mapDataThreadInfoQueue.Count > 0) {
			for (int i = 0; i < mapDataThreadInfoQueue.Count; i++) {
				MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue ();
				threadInfo.callback (threadInfo.parameter);
			}
		}

		if (meshDataThreadInfoQueue.Count > 0) {
			for (int i = 0; i < meshDataThreadInfoQueue.Count; i++) {
				MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue ();
				threadInfo.callback (threadInfo.parameter);
			}
		}
	}

	MapData GenerateMapData(Vector2 center) {
		// +2 is to compensate for the border
		float[,] noiseMap = Noise.GenerateNoiseMap (mapChunkSize + 2, mapChunkSize + 2, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, center + noiseData.offset, noiseData.normalizeMode);

		if(terrainData.useFalloff)
		{
			if(fallOffMap == null)
			{
				fallOffMap = FallOffGenerator.GenerateFalloffMap(mapChunkSize + 2);
			}

			for (int y = 0; y < mapChunkSize+2; y++) {
				for (int x = 0; x < mapChunkSize+2; x++) {
					if(terrainData.useFalloff)
					{
						noiseMap[x, y] = Mathf.Clamp (noiseMap [x, y] - fallOffMap[x, y], 0, 1);
					}
				}
			}
		}
	
		return new MapData (noiseMap);
	}

	void OnValidate() {

		if (terrainData != null)
		{
			terrainData.OnValuesUpdated -= OnValuesUpdated;
			terrainData.OnValuesUpdated += OnValuesUpdated;
		}

		if(noiseData != null)
		{
			noiseData.OnValuesUpdated -= OnValuesUpdated;
			noiseData.OnValuesUpdated += OnValuesUpdated;
		}

		if(textureData != null)
		{
			textureData.OnValuesUpdated -= OnTextureValuesUpdated;
			textureData.OnValuesUpdated += OnTextureValuesUpdated;
		}
	}

	struct MapThreadInfo<T> {
		public readonly Action<T> callback;
		public readonly T parameter;

		public MapThreadInfo (Action<T> callback, T parameter)
		{
			this.callback = callback;
			this.parameter = parameter;
		}
		
	}

}

public struct MapData {
	public readonly float[,] heightMap;

	public MapData (float[,] heightMap)
	{
		this.heightMap = heightMap;
	}
}