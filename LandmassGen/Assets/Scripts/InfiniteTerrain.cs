using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteTerrain : MonoBehaviour
{
    public const float maxViewDist = 450;
    public Transform viewer;

    public static Vector2 viewerPosition;
    int chunkSize;
    int chunksInViewDist;

    Dictionary<Vector2, TerrainChunk> terrainChunkDict = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> chunksVisiblePrevUpdate = new List<TerrainChunk>();

    void Start()
    {
        chunkSize = MapGenerator.mapChunkSize- 1;
        chunksInViewDist = Mathf.RoundToInt(maxViewDist / chunkSize);
    }

    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks()
    {

        foreach(TerrainChunk chunk in chunksVisiblePrevUpdate)
        {
            chunk.SetVisible(false);
        }
        chunksVisiblePrevUpdate.Clear();


        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunksInViewDist; yOffset <= chunksInViewDist; yOffset++)
        {
            for (int xOffset = -chunksInViewDist; xOffset <= chunksInViewDist; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunkDict.ContainsKey(viewedChunkCoord)) {
                    terrainChunkDict[viewedChunkCoord].UpdateTerrainChunk();

                    if(terrainChunkDict[viewedChunkCoord].isVisible())
                    {
                        chunksVisiblePrevUpdate.Add(terrainChunkDict[viewedChunkCoord]);
                    }

                }
                else{
                    terrainChunkDict.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize,    transform));
                }

            }
        }
    }


    public class TerrainChunk 
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        public TerrainChunk(Vector2 coords, int size, Transform parent)
        {
            position = coords * size;
            Vector3 position3D = new Vector3(position.x, 0, position.y);

            bounds = new Bounds(position, Vector2.one * size);

            meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObject.transform.position = position3D;
            meshObject.transform.localScale = Vector3.one * size / 10f; // default scale is 10 so just divide by that to get back into our own
            meshObject.transform.parent = parent;
            SetVisible(false);
        }

        public void UpdateTerrainChunk()
        {
            float viewerDistFromChunk = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

            bool visible = viewerDistFromChunk <= maxViewDist;
            SetVisible(visible);
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool isVisible()
        {
            return meshObject.activeSelf;
        }

    }
}