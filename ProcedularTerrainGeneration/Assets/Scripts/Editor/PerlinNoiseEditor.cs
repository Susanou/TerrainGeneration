using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PerlinNoise))]
public class PerlinNoiseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PerlinNoise noise = (PerlinNoise)target;

        if(GUILayout.Button("Generate Noise"))
        {
            noise.GenerateTexture();
        }

        if(GUILayout.Button("Reset"))
        {
            noise.ResetTexture();
        }
    }
}
