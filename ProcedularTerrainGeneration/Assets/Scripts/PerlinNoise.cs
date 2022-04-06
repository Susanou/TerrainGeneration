using UnityEngine;
using System.IO;

public class PerlinNoise : MonoBehaviour
{
    public int width = 256;
    public int height = 256;

    public float offsetX = 100f;
    public float offsetY = 100f;

    public float scale = 20.0f;

    public bool autoUpdate;

    private Texture2D originalTexture;
    private Texture2D currentTexture;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        offsetX = Random.Range(0f, 99999f);
        offsetY = Random.Range(0f, 99999f);

        originalTexture = (Texture2D)renderer.sharedMaterial.mainTexture;
    }

    void Update()
    {   
        GenerateTexture();
    }

    public void GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        
        //Generate Perlin Noise map for texture

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Color color = CalculateColor(x, y);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();

        GetComponent<Renderer>().sharedMaterial.mainTexture = texture;
        currentTexture = texture;
    }

    public void RandomOffsets()
    {
        offsetX = Random.Range(0f, 99999f);
        offsetY = Random.Range(0f, 99999f);
        GenerateTexture();
    }

    public void ResetTexture()
    {
        GetComponent<Renderer>().sharedMaterial.mainTexture = originalTexture;
        offsetX = 100f;
        offsetY = 100f;
        scale = 20.0f;
    }

    public void SaveTexture(string path, string name)
    {

        byte[] bytes = ImageConversion.EncodeToPNG(currentTexture);
        var dirPath = Application.dataPath + "/" + path + "/";
        if(!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + name + ".png", bytes);
    }

    Color CalculateColor(int x, int y)
    {
        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;

        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        return new Color(sample, sample, sample);
    }
}
