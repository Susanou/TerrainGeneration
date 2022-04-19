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

}