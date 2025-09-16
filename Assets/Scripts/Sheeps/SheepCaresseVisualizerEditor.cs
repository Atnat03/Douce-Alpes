using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class SheepCaresseVisualizerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameManager gm = (GameManager)target;

        GUILayout.Space(10);
        GUILayout.Label("Courbe de caresses", EditorStyles.boldLabel);

        if (gm.caresseCurveValues != null && gm.caresseCurveValues.Length > 0)
        {
            Rect rect = GUILayoutUtility.GetRect(300, 150);
            GUI.Box(rect, "");

            // Labels axes
            GUI.Label(new Rect(rect.x + 10, rect.y, 60, 20), "Bonheur");
            GUI.Label(new Rect(rect.x + rect.width - 30, rect.y + rect.height + 5, 60, 20), "Swipe");

            // Grille horizontale
            int gridLines = 5;
            float max = Mathf.Max(gm.caresseCurveValues);
            float min = Mathf.Min(gm.caresseCurveValues);

            for (int i = 0; i <= gridLines; i++)
            {
                float y = rect.y + rect.height - (i / (float)gridLines) * rect.height;
                Handles.color = Color.gray;
                Handles.DrawLine(new Vector3(rect.x, y), new Vector3(rect.x + rect.width, y));
                GUI.Label(new Rect(rect.x - 40, y - 10, 40, 20), Mathf.Lerp(min, max, i / (float)gridLines).ToString("F1"));
            }

            // Dessin de la courbe
            Handles.color = Color.cyan;
            for (int i = 1; i < gm.caresseCurveValues.Length; i++)
            {
                float x0 = rect.x + ((i - 1f) / (gm.caresseCurveValues.Length - 1)) * rect.width;
                float y0 = rect.y + rect.height - ((gm.caresseCurveValues[i - 1] - min) / (max - min)) * rect.height;

                float x1 = rect.x + (i / (float)(gm.caresseCurveValues.Length - 1)) * rect.width;
                float y1 = rect.y + rect.height - ((gm.caresseCurveValues[i] - min) / (max - min)) * rect.height;

                Handles.DrawLine(new Vector3(x0, y0), new Vector3(x1, y1));
            }
        }

        GUILayout.Space(10);
        GUILayout.Label("Valeurs de bonheur par swipe", EditorStyles.boldLabel);

        if (gm.caresseCurveValues != null && gm.caresseCurveValues.Length > 0)
        {
            EditorGUILayout.BeginVertical("box");
            for (int i = 0; i < gm.caresseCurveValues.Length / 2; i++)
            {
                EditorGUILayout.LabelField($"Swipe {i + 1}", gm.caresseCurveValues[i].ToString("F2"));
            }
            EditorGUILayout.EndVertical();
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Recalculer Courbe de Caresse"))
        {
            RecalculateCaresseCurve(gm);
        }
    }

    void RecalculateCaresseCurve(GameManager gm)
    {
        int sampleSwipes = 20;
        gm.caresseCurveValues = new float[sampleSwipes];
    
        for (int i = 0; i < sampleSwipes; i++)
        {
            float bonus = gm.caresseBaseValue * Mathf.Exp(-i / gm.saturationCarrese);
            gm.caresseCurveValues[i] = bonus;
        }
    }

}
