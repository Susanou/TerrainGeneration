using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PerlinNoise))]
public class PerlinNoiseEditor : Editor
{

    string path;
    string name;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PerlinNoise noise = (PerlinNoise)target;

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Path");
        path = GUILayout.TextField(path, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("FileName");
        name = GUILayout.TextField(name, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();    

        if(GUILayout.Button("Generate Noise"))
        {
            noise.GenerateTexture();
        }

        if(GUILayout.Button("Reset"))
        {
            noise.ResetTexture();
        }

        if(GUILayout.Button("Save texture"))
        {
            Debug.Log(Application.dataPath + "/" + path);
            noise.SaveTexture(path, name);
        }
    }
}
