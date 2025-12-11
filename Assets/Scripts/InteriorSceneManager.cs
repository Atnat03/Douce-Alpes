using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class InteriorSceneManager : MonoBehaviour
{
    public List<GameObject> sheepInside = new();
    public List<Transform> randomSpawnPos;

    [SerializeField] public GameObject sheepPrefab;

    private void OnEnable()
    {
        SwapSceneManager.instance.SwapingInteriorScene += Initialize;
    }

    private void OnDisable()
    {
        DestroySheep();
        SwapSceneManager.instance.SwapingInteriorScene -= Initialize;
    }
    
    public void Initialize()
    {
        if (GameData.instance.nbSheep <= 0) 
            return;

        List<Transform> spawnPool = new List<Transform>(randomSpawnPos);

        foreach (SheepData sheepData in GameData.instance.sheepDestroyData)
        {
            if (spawnPool.Count == 0)
            {
                Debug.LogWarning("Not enough spawn positions for sheep!");
                break;
            }

            Transform pos = spawnPool[Random.Range(0, spawnPool.Count)];
            spawnPool.Remove(pos);

            Vector3 rot = new Vector3(0, Random.Range(0, 360), 0);

            GameObject newSheep = Instantiate(sheepPrefab, pos.position, Quaternion.Euler(rot), transform);
            sheepInside.Add(newSheep);
            
            SheepSkinManager sheep = newSheep.GetComponent<SheepSkinManager>();

            sheep.Initialize(sheepData.id, sheepData.name, sheepData.hasWhool, sheepData.colorID);
            sheep.SetCurrentSkinHat(sheepData.skinHat);
            sheep.SetCurrentSkinClothe(sheepData.skinClothe);
        }
    }

    public void DestroySheep()
    {
        if (sheepInside.Count == 0)
            return;

        foreach (GameObject sheep in sheepInside)
        {
            if (sheep != null)
                Destroy(sheep.gameObject);
        }

        sheepInside.Clear();
    }
}
