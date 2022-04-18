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

	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;
	public TextureData textureData;

	public Material terrainMaterial;




	[Range(0, MeshSettings.numSupportedLODs-1)]
	public int editorLODPreview;

	public bool autoUpdate;

	Queue<MapThreadInfo<HeightMap>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<HeightMap>>();
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

	void Start()
	{
		textureData.ApplyToMaterial(terrainMaterial);
		textureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
	}

	public void DrawMapInEditor() {
		textureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
		HeightMap heightMap = HeightMapGenerator.GenerateHeightMap (meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, Vector2.zero);

		MapDisplay display = FindObjectOfType<MapDisplay> ();

		switch(drawMode) {
			case DrawMode.NoiseMap:
				display.DrawTexture (TextureGenerator.TextureFromHeightMap (heightMap.values));
				break;
			case DrawMode.Mesh:
				display.DrawMesh (MeshGenerator.GenerateTerrainMesh (heightMap.values, meshSettings, editorLODPreview));
				break;
			case DrawMode.FallOffMap:
				display.DrawTexture (TextureGenerator.TextureFromHeightMap(FallOffGenerator.GenerateFalloffMap(meshSettings.numVertsPerLine)));
				break;
			case DrawMode.HideAll:
				display.HideAll();
				break;
			default:
				Debug.LogError("DRAW MODE NOT IMPLEMENTED");
				break;
		}
	}

	public void RequestHeightMap(Vector2 center, Action<HeightMap> callback) {
		ThreadStart threadStart = delegate {
			HeightMapThread (center, callback);
		};

		new Thread (threadStart).Start ();
	}

	void HeightMapThread(Vector2 center, Action<HeightMap> callback) {
		HeightMap heightMap = HeightMapGenerator.GenerateHeightMap (meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, center);
		lock (mapDataThreadInfoQueue) {
			mapDataThreadInfoQueue.Enqueue (new MapThreadInfo<HeightMap> (callback, heightMap));
		}
	}

	public void RequestMeshData(HeightMap heightMap, int lod, Action<MeshData> callback) {
		ThreadStart threadStart = delegate {
			MeshDataThread (heightMap,  lod, callback);
		};

		new Thread (threadStart).Start ();
	}

	void MeshDataThread(HeightMap heightMap, int lod, Action<MeshData> callback) {
		MeshData meshData = MeshGenerator.GenerateTerrainMesh (heightMap.values, meshSettings, lod);
		lock (meshDataThreadInfoQueue) {
			meshDataThreadInfoQueue.Enqueue (new MapThreadInfo<MeshData> (callback, meshData));
		}
	}

	void Update() {
		if (mapDataThreadInfoQueue.Count > 0) {
			for (int i = 0; i < mapDataThreadInfoQueue.Count; i++) {
				MapThreadInfo<HeightMap> threadInfo = mapDataThreadInfoQueue.Dequeue ();
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

	void OnValidate() {

		if (meshSettings != null)
		{
			meshSettings.OnValuesUpdated -= OnValuesUpdated;
			meshSettings.OnValuesUpdated += OnValuesUpdated;
		}

		if(heightMapSettings != null)
		{
			heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
			heightMapSettings.OnValuesUpdated += OnValuesUpdated;
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