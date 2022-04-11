using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateableData : ScriptableObject
{

    public event System.Action OnValuesUpdated;
    public bool autoUpdate;

    protected virtual void OnValidate()
    {
        if(autoUpdate)
        {
            UnityEditor.EditorApplication.update += NotifyUpdate;
        }
    }

    public void NotifyUpdate()
    {
        UnityEditor.EditorApplication.update -= NotifyUpdate;
        if (OnValuesUpdated != null)
        {
            OnValuesUpdated();
        }
    }
}
