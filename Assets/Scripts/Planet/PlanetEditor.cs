using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
// Used to customize planet shape in Inspector, not usable in scripting
public class PlanetEditor : Editor
{
    Planet planet;
    Editor shapeEditor;
    Editor colorEditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed)
            {
                planet.GeneratePlanet();
            }
        }

        DrawSettingsEditor(planet.shapeSetting, planet.OnShapeSettingUpdated, ref shapeEditor);
        DrawSettingsEditor(planet.colorSetting, planet.OnColorSettingUpdated, ref colorEditor);
    }

    void DrawSettingsEditor(Object settings, System.Action onSettingUpdated, ref Editor editor)
    {
        if (settings != null)
        {
            EditorGUILayout.InspectorTitlebar(true, settings);
            // Auto refresh when update
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                // Create new editor when have to
                CreateCachedEditor(settings, null, ref editor);
                editor.OnInspectorGUI();
                if (check.changed)
                {
                    if (onSettingUpdated != null)
                    {
                        onSettingUpdated();
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        planet = (Planet)target;
    }
}
