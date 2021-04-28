using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;

// Used for generating the points on sphere based on radius and noise layer
public class ShapeGenerator
{
    ShapeSettings setting;
    INoiseFilter[] noiseFilters;
    public MinMaxColor terrainElevation;
    public void UpdateSettings(ShapeSettings setting)
    {
        this.setting = setting;
        this.noiseFilters = new INoiseFilter[setting.noiseLayers.Length];
        for (int i = 0; i < noiseFilters.Length; i++)
        {
            noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(setting.noiseLayers[i].noiseSettings);
        }
        terrainElevation = new MinMaxColor();
    }

    // Calculate unscaled point on planet based on point on sphere
    public float UnscaledElevation(Vector3 pointSphere)
    {
        // Noise evaluation and reflect output terrain on sphere, all layers are cumulated
        float elevation = 0;
        float baseValue = 0;
        if (noiseFilters.Length > 0) {
            baseValue = noiseFilters[0].Evaluate(pointSphere);
            if (setting.noiseLayers[0].enabled)
            {
                elevation = baseValue;
            }
        }
        for (int i = 1; i < noiseFilters.Length; i++)
        {
            // If base layer is used, then mask would be other multiples
            float mask = (setting.noiseLayers[i].useMask) ? baseValue : 1;
            elevation += noiseFilters[i].Evaluate(pointSphere) * mask;
        }

        terrainElevation.AddValue(elevation);
        return elevation;
    }

    public float ScaledElevation(float unscaled)
    {
        float elevation = Mathf.Max(0, unscaled); // Works for terrain instead of sea
        elevation = setting.planetRadius * (1 + elevation);
        return elevation;
    }
}
