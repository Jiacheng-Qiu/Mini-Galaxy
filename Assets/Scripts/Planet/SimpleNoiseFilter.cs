using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNoiseFilter : INoiseFilter
{
    Noise noise = new Noise();
    NoiseSettings noiseSetting;
    public SimpleNoiseFilter(NoiseSettings noiseSetting)
    {
        this.noiseSetting = noiseSetting;
    }

    public float Evaluate(Vector3 point)
    {
        // make value 0 to 1
        float eval = 0;
        float frequency = noiseSetting.baseRoughness;
        float amplitude = 1;
        for (int i = 0; i < noiseSetting.numLayers; i++)
        {
            float v = noise.Evaluate(point * frequency + noiseSetting.center);
            eval += (v + 1) * 0.5f * amplitude;
            // Frequency increase if roughness > 1
            frequency *= noiseSetting.roughness;
            // Amplitude increase if persistence > 1
            amplitude *= noiseSetting.persistence;
        }
        eval = eval - noiseSetting.minVal; // Not taking 0 as min so sea levels will also be recorded
        return eval * noiseSetting.strength;
    }
}
