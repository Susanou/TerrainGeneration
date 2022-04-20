using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPreview : MonoBehaviour
{
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

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
        textureData.ApplyToMaterial(terrainMaterial);
		textureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
		HeightMap heightMap = HeightMapGenerator.GenerateHeightMap (meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, Vector2.zero);

		switch(drawMode) {
			case DrawMode.NoiseMap:
				DrawTexture (TextureGenerator.TextureFromHeightMap (heightMap));
				break;
			case DrawMode.Mesh:
				DrawMesh (MeshGenerator.GenerateTerrainMesh (heightMap.values, meshSettings, editorLODPreview));
				break;
			case DrawMode.FallOffMap:
				DrawTexture (TextureGenerator.TextureFromHeightMap(new HeightMap(FallOffGenerator.GenerateFalloffMap(meshSettings.numVertsPerLine), 0, 1)));
				break;
			case DrawMode.HideAll:
				HideAll();
				break;
			default:
				Debug.LogError("DRAW MODE NOT IMPLEMENTED");
				break;
		}
	}


    public void DrawTexture(Texture2D texture)
    {
        meshFilter.gameObject.SetActive(false);
        textureRender.gameObject.SetActive(true);

        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;
    }

    public void DrawMesh(MeshData meshData)
    {
        meshFilter.gameObject.SetActive(true);
        textureRender.gameObject.SetActive(false);

        meshFilter.sharedMesh = meshData.CreateMesh();
    }

    public void HideAll()
    {
        meshFilter.gameObject.SetActive(false);
        textureRender.gameObject.SetActive(false);
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
