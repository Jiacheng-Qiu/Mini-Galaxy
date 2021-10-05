using UnityEngine;

// Provide control over planet noises
[System.Serializable]
public class NoiseSettings
{
    public enum FilterType
    {
        Simple, Rigid
    }
    public FilterType filterType;
    // Control height
    public float strength = 0.1f;
    // Control frequence
    public float roughness = 2.2f;
    // Lower bound of roughness
    public float baseRoughness = 1;
    public Vector3 center = new Vector3(0, 0, 0);

    // Multi-layer noise filter with increasing amplitude (A good way to control clearness for distant view)
    public int numLayers = 4;
    // Effect of single layer on output
    public float persistence = 0.5f;
    // So that the terrain wont have too much crashing in (create sea level)
    public float minVal = 0.9f;

    // Only applied for rigid noise
    public float rigidWeightMultiplier = 0.8f;
}
