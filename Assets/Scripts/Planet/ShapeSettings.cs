using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ShapeSettings : ScriptableObject
{
    public float planetRadius = 1;
    public NoiseLayer[] noiseLayers;

    [System.Serializable]
    public class NoiseLayer
    {
        public bool enabled = true;
        // This bool marks FIRST layer as base, so THIS layer will ONLY affect terrain on top of base
        public bool useMask;
        public NoiseSettings noiseSettings;
    }
}
