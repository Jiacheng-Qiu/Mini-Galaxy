using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Make detailed terrain and sharp mountains
public class RigidNoiseFilter : INoiseFilter
{
    Noise noise = new Noise();
    NoiseSettings noiseSetting;
    public RigidNoiseFilter(NoiseSettings noiseSetting)
    {
        this.noiseSetting = noiseSetting;
    }

    public float Evaluate(Vector3 point)
    {
        // make value 0 to 1
        float eval = 0;
        float frequency = noiseSetting.baseRoughness;
        float amplitude = 1;
        float weight = 1;

        for (int i = 0; i < noiseSetting.numLayers; i++)
        {
            // To make the terrain much more varied with peaks, take absolute value, and invert the graphs
            float v = 1 - Mathf.Abs(noise.Evaluate(point * frequency + noiseSetting.center));
            // Taking squares will make the graph even sharper, like (1-sin)^2
            v *= v * weight;
            // Make regions low to be undetailed, while remaining in range 0-1
            weight = Mathf.Clamp01(v * noiseSetting.rigidWeightMultiplier);
            eval += v * amplitude;
            // Frequency increase if roughness > 1
            frequency *= noiseSetting.roughness;
            // Amplitude increase if persistence > 1
            amplitude *= noiseSetting.persistence;
        }
        eval = eval - noiseSetting.minVal; // Not taking 0 as min so sea levels will also be recorded
        return eval * noiseSetting.strength;
    }
}
