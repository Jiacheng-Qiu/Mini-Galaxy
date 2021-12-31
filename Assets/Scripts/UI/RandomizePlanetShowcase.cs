using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizePlanetShowcase : MonoBehaviour
{
    public ShapeSettings shapeSetting;
    public ColorSettings colorSetting;
    public Material atmosphere;
    public Material water;
    public Planet planet;
    private Color[,] presetColors;

    private void Start()
    {
        presetColors = new Color[8,3];
        presetColors[0, 0] = new Color32(74, 111, 9, 255);
        presetColors[0, 1] = new Color32(170, 96, 45, 255);
        presetColors[0, 2] = new Color32(255, 255, 255, 255);

        presetColors[1, 0] = new Color32(75, 72, 69, 255);
        presetColors[1, 1] = new Color32(138, 130, 136, 255);
        presetColors[1, 2] = new Color32(255, 255, 255, 255);

        presetColors[2, 0] = new Color32(29, 127, 42, 255);
        presetColors[2, 1] = new Color32(135, 178, 55, 255);
        presetColors[2, 2] = new Color32(176, 107, 39, 255);

        presetColors[3, 0] = new Color32(26, 13, 9, 255);
        presetColors[3, 1] = new Color32(80, 10, 6, 255);
        presetColors[3, 2] = new Color32(147, 30, 29, 255);

        presetColors[4, 0] = new Color32(91, 60, 13, 255);
        presetColors[4, 1] = new Color32(140, 79, 34, 255);
        presetColors[4, 2] = new Color32(195, 135, 32, 255);

        presetColors[5, 0] = new Color32(40, 13, 58, 255);
        presetColors[5, 1] = new Color32(30, 16, 111, 255);
        presetColors[5, 2] = new Color32(49, 32, 130, 255);

    }

    public void Pressed()
    {
        // Shape
        shapeSetting.noiseLayers[0].noiseSettings.strength = Random.Range(0.05f, 0.15f);
        shapeSetting.noiseLayers[0].noiseSettings.roughness = Random.Range(2, 2.5f);
        shapeSetting.noiseLayers[0].noiseSettings.baseRoughness = Random.Range(1f, 2f);
        shapeSetting.noiseLayers[0].noiseSettings.center = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));

        // Color
        GradientColorKey[] pole = GetGradient(Random.Range(0, 6));
        GradientColorKey[] equator = GetGradient(Random.Range(0, 6));
        colorSetting.biomeColorSettings.biomes[0].gradient.colorKeys = pole;
        colorSetting.biomeColorSettings.biomes[1].gradient.colorKeys = equator;
        colorSetting.biomeColorSettings.biomes[1].startHeight = Random.Range(0.1f, 0.2f);
        colorSetting.biomeColorSettings.biomes[2].gradient.colorKeys = pole;
        colorSetting.biomeColorSettings.biomes[2].startHeight = Random.Range(0.8f, 9f);

        planet.Regenerate();

        // Water color
        int[] abc = { 30, 50, 165, 200, 220, 240, 260 };
        float randomBaseColor = abc[Random.Range(0, 7)];
        Color shallow = Color.HSVToRGB(randomBaseColor / 255, 0.91f, 0.95f);
        shallow.a = 0.73f;
        Color deep = Color.HSVToRGB((randomBaseColor + 12f) / 255, 0.91f, 0.53f);
        deep.a = 0.82f;
        Color deeper = Color.HSVToRGB((randomBaseColor + 12f) / 255, 0.91f, 0.28f);
        deeper.a = 0.94f;
        water.SetColor("ShallowWaterColor", shallow);
        water.SetColor("DeepWaterColor", deep);
        water.SetColor("SuperDeepWaterColor", deeper);

        // Atmosphere color
        Color atmosColor = Color.HSVToRGB(Random.Range(0f, 0.67f), 1, 0.8f);
        atmosColor.a = 0.8f;
        atmosphere.color = atmosColor;
    }

    private GradientColorKey[] GetGradient(int pos)
    {
        GradientColorKey[] keys = new GradientColorKey[3];
        for (int i = 0; i < 3; i++)
        {
            keys[i] = new GradientColorKey();
            keys[i].color = presetColors[pos, i];
            keys[i].time = i * 0.3f + Random.Range(0, 0.2f);
        }
        return keys;
    }
}
