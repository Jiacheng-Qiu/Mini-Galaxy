using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ColorSettings : ScriptableObject
{
    public Material planetMaterial;
    public BiomeColorSettings biomeColorSettings = new BiomeColorSettings();
    public Gradient oceanColor;

    [System.Serializable]
    public class BiomeColorSettings
    {
        public Biome[] biomes;
        // Noise to make dividers of biomes less sharp
        public NoiseSettings noise;
        public float noiseOffset;
        public float noiseStrength;
        public float blendAmount; // Make noise applied onto biome also be adjusted based on distance from borders

        [System.Serializable]
        public class Biome
        {
            public Gradient gradient;
            public Color tint;
            public float startHeight = 0; // Range from 0-1
            public float tintPercent = 0; // Range from 0-1
        }

        public void InitBiomeArr(int size, Gradient gradient)
        {
            biomes = new Biome[size];
            for (int i = 0; i < size; i++)
            {
                biomes[i] = new Biome();
                biomes[i].gradient = gradient;
            }

            // Also init noise layer, only one is needed
            noise = new NoiseSettings();
            noise.filterType = NoiseSettings.FilterType.Simple;
        }
    }
}
