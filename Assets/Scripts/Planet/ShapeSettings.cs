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
        public bool useMask = false;
        public NoiseSettings noiseSettings;
        public void easyInput(NoiseSettings.FilterType filterType, Vector3 center)
        {
            noiseSettings = new NoiseSettings();
            noiseSettings.filterType = filterType;
            noiseSettings.center = center;
        }
    }

    public void InitNoiseArr(int size)
    {
        noiseLayers = new NoiseLayer[size];
        for (int i = 0; i < size; i++)
        {
            noiseLayers[i] = new NoiseLayer();
        }
    }
}
