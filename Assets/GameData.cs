using System;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    [SerializeField] public List<SheepData> sheepDestroyData;
    public int nbSheep;
    public bool isSheepInside = false;
    [SerializeField] public GameObject sheepPrefab;
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        sheepDestroyData = new List<SheepData>();
    }

    private void Update()
    {
        isSheepInside = sheepDestroyData.Count == nbSheep;
    }
}
