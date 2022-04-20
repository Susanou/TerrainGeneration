using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapPreview))]
public class MapPreviewEditor : Editor
{

    public override void OnInspectorGUI()
    {
        MapPreview mapGen = (MapPreview)target;

        if(DrawDefaultInspector())
        {
            if(mapGen.autoUpdate)
            {
                mapGen.DrawMapInEditor();
            }
        }

        if(GUILayout.Button("Generate Noise Map"))
        {
            mapGen.DrawMapInEditor();
        }
    }

}
