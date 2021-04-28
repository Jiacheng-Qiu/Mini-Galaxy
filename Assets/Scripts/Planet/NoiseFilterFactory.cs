using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseFilterFactory
{
    // Create a noise filter based on given settings
    public static INoiseFilter CreateNoiseFilter(NoiseSettings settings)
    {
        // After new definition of filters, return here to add them
        switch (settings.filterType)
        {
            case NoiseSettings.FilterType.Simple:
                return new SimpleNoiseFilter(settings);
            case NoiseSettings.FilterType.Rigid:
                return new RigidNoiseFilter(settings);
        }
        return null;
    }
}
