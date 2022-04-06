using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PerlinNoise))]
public class PerlinNoiseEditor : Editor
{

    string path = "Textures";
    string filename = "PerlinMap";



    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PerlinNoise noise = (PerlinNoise)target;

        if(DrawDefaultInspector()) {
            if(noise.autoUpdate)
            {
                noise.GenerateTexture();
            }
        }

        

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Path");
        path = GUILayout.TextField(path, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("FileName");
        filename = GUILayout.TextField(filename, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();    

        if(GUILayout.Button("Generate Noise"))
        {
            noise.GenerateTexture();
        }

        if(GUILayout.Button("Generate Random Noise"))
        {
            noise.RandomOffsets();
        }

        if(GUILayout.Button("Reset"))
        {
            noise.ResetTexture();
        }

        if(GUILayout.Button("Save texture"))
        {
            Debug.Log(Application.dataPath + "/" + path);
            noise.SaveTexture(path, filename);
        }
    }
}
