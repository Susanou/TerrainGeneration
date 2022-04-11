using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TerrainData : UpdateableData
{
    public bool useFlatShading;
    public bool useFalloff;

    public float uniformScale = 2.5f;

	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;
}
