using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TricotModelFactory : EditorWindow
{
    public string modelName = "";
    public Sprite logo = null;
    public int woolToUse = 0;
    public List<ModelDraw> pattern = new List<ModelDraw>();
    public ModelDraw currentModelPattern =  new ModelDraw();
    public int sellPrice = 0;
    public int unlockPrice = 0;
    
    [MenuItem("Tools/TricotFactory")]
    public static void ShowWindow()
    {
        GetWindow(typeof(TricotModelFactory));
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect((Screen.width / 2) - 32, 8, 64, 64), (Texture)Resources.Load("tricot"), ScaleMode.ScaleToFit, true);

        GUILayout.Space(84);
        
        GUILayout.Label("Tricot Factory", EditorStyles.boldLabel);
        
        GUILayout.Space(16);

        modelName = EditorGUILayout.TextField("Model name", modelName);
        logo = EditorGUILayout.ObjectField("Logo", logo, typeof(Sprite), true) as Sprite;
        woolToUse = EditorGUILayout.IntField("Wool to use", woolToUse);
        
        GUILayout.Space(16);
        
        sellPrice = EditorGUILayout.IntField("Sell price", sellPrice);
        unlockPrice = EditorGUILayout.IntField("Unlock price", unlockPrice);

        GUILayout.Space(16);
        
        CreateGrid();
        
        GUILayout.BeginVertical();
        
        GUI.enabled = currentModelPattern.pointsList.Count > 0;
        
        if (GUILayout.Button("Create Shape",GUILayout.Width(130), GUILayout.Height(40)))
        {
            AddCurrentModelToPattern();
        }
        GUILayout.EndVertical();

        ShowCurrentList();
        
        GUILayout.BeginHorizontal();

        GUI.enabled = currentModelPattern.pointsList.Count > 0;
                
        if (GUILayout.Button("Clear Pattern"))
        {
            pattern.Clear();
        }
        
        GUI.enabled = modelName != "" && logo != null;
        
        if (GUILayout.Button("Create Model"))
        {
            GenerateModel();
        }
        
        GUILayout.EndHorizontal();
    }

    private void CreateGrid()
    {
        int gridSize = 3;
        GUILayout.Label("Pattern", EditorStyles.boldLabel);
        int n = 1;

        for (int y = 0; y < gridSize; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < gridSize; x++)
            {
                bool isSelected = currentModelPattern.pointsList.Contains(n);

                Color oldColor = GUI.backgroundColor;

                if (isSelected)
                    GUI.backgroundColor = Color.blue;

                GUI.enabled = !isSelected;

                if (GUILayout.Button($"{n}", GUILayout.Width(40), GUILayout.Height(40)))
                {
                    currentModelPattern.pointsList.Add(n);
                }

                GUI.backgroundColor = oldColor;
                GUI.enabled = true;

                n++;
            }
            GUILayout.EndHorizontal();
        }
    }


    public void AddCurrentModelToPattern()
    {
        Debug.Log($"Ajout de la nouvelle force au pattern de création");
        pattern.Add(currentModelPattern);
        currentModelPattern = new ModelDraw();
    }

    private void ShowCurrentList()
    {
        GUILayout.Space(10);
        GUILayout.Label("All Model Patterns", EditorStyles.boldLabel);

        if (pattern.Count == 0 && (currentModelPattern.pointsList == null || currentModelPattern.pointsList.Count == 0))
        {
            GUILayout.Label("No patterns added yet.");
            return;
        }

        for (int i = 0; i < pattern.Count; i++)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Label($"Pattern {i + 1}: ", GUILayout.Width(80));
            foreach (int point in pattern[i].pointsList)
            {
                GUILayout.Label(point.ToString(), GUILayout.Width(30));
            }
            GUILayout.EndHorizontal();
        }

        if (currentModelPattern.pointsList != null && currentModelPattern.pointsList.Count > 0)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Label($"Current: ", GUILayout.Width(80));
            foreach (int point in currentModelPattern.pointsList)
            {
                GUILayout.Label(point.ToString(), GUILayout.Width(30));
            }
            GUILayout.EndHorizontal();
        }
    }

    private void GenerateModel()
    {
        ModelListeSO liste = Resources.Load<ModelListeSO>("TricotModelList");
        
        ModelDrawSO newSo = CreateInstance<ModelDrawSO>();
        newSo.name = modelName;
        newSo.image = logo;
        newSo.whoolToUse = woolToUse;
        newSo.pattern = pattern;
        newSo.sellPrice = sellPrice;
        newSo.unlockPrice = unlockPrice;
        
        string path = $"Assets/Resources/Models/{modelName}.asset";
        AssetDatabase.CreateAsset(newSo, path);
        AssetDatabase.SaveAssets();

        liste.listeModel.Add(newSo);
        EditorUtility.SetDirty(liste);

        Debug.Log($"Model '{modelName}' created at {path}");
    }
}
