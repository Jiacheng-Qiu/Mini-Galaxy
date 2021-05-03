using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorGenerator
{
    ColorSettings settings;
    Texture2D texture;
    const int textureResolution = 50;
    INoiseFilter biomeNoiseFilter;
    public void UpdateSettings(ColorSettings settings)
    {
        this.settings = settings;
        try {
            int height = settings.biomeColorSettings.biomes.Length;
            if (texture == null || texture.height != height)
                // Width: color change based on terrain height; Height: color change based on latitude
                // texture resolution * 2 makes first half the gradient of ocean, and second half the terrain
                texture = new Texture2D(textureResolution * 2, height, TextureFormat.RGBA32, false);
            biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(settings.biomeColorSettings.noise);
        }
        catch
        {
            Debug.LogError("The biome layer for current planet is of length 0!");
        }
    }

    public void UpdateElevation(MinMaxColor terrainElevation)
    {
        settings.planetMaterial.SetVector("_terrainElevation", new Vector4(terrainElevation.Min, terrainElevation.Max));
    }

    // Calculate the current biome the point is within
    public float BiomePercentFromPoint(Vector3 pointPos)
    {
        float heightPercent = (pointPos.y + 1) / 2f;
        heightPercent += (biomeNoiseFilter.Evaluate(pointPos) - settings.biomeColorSettings.noiseOffset) * settings.biomeColorSettings.noiseStrength;
        float biomeIndex = 0;
        int numBiomes = settings.biomeColorSettings.biomes.Length;
        float blendRange = settings.biomeColorSettings.blendAmount / 2f + 0.001f; // .001f is used to secure that blendRange won't be 0
        for(int i = 0; i < numBiomes; i++)
        {
            float dst = heightPercent - settings.biomeColorSettings.biomes[i].startHeight;
            float weight = Mathf.InverseLerp(-blendRange, blendRange, dst); // TODO: cause problem when height is too big
            biomeIndex *= (1 - weight); // reset biomeIndex to 0 when weight over bound
            biomeIndex += i * weight;
        }
        return biomeIndex / Mathf.Max(1, numBiomes - 1);
    }

    public void UpdateColors()
    {
        Color[] colors = new Color[texture.width * texture.height];
        int index = 0;
        foreach(var bio in settings.biomeColorSettings.biomes)
        {
            for (int i = 0; i < textureResolution * 2; i++)
            {
                Color calculated;
                if (i < textureResolution)
                {
                    calculated = settings.oceanColor.Evaluate(i / (textureResolution - 1f));
                }
                else
                {
                    calculated = bio.gradient.Evaluate((i - textureResolution) / (textureResolution - 1f));
                }
                    
                colors[index++] = calculated * (1 - bio.tintPercent) + bio.tint * bio.tintPercent;
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        settings.planetMaterial.SetTexture("_texture", texture);
    }
}
