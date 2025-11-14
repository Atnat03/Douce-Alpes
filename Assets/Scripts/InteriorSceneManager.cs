using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class InteriorSceneManager : MonoBehaviour
{
    public List<SheepSkinManager> sheepInside;
    public List<Transform> randomSpawnPos;

    [SerializeField] public GameObject sheepPrefab;

    private void OnEnable()
    {
        Debug.Log("OnEnable");

        if (SwapSceneManager.instance != null)
        {
            SwapSceneManager.instance.SwapingInteriorScene += Initialize;
            SwapSceneManager.instance.SwapingDefaultScene += DestroySheep;
        }
    }

    private void OnDisable()
    {
        if (SwapSceneManager.instance != null)
        {
            SwapSceneManager.instance.SwapingInteriorScene -= Initialize;
            SwapSceneManager.instance.SwapingDefaultScene -= DestroySheep;
        }
    }

    private void Start()
    {
        sheepInside = new List<SheepSkinManager>();
    }

    public void Initialize()
    {
        Debug.Log("Initialize interior sheep");

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
            SheepSkinManager sheep = newSheep.GetComponent<SheepSkinManager>();

            sheep.Initialize(sheepData.id, sheepData.name);
            sheep.SetCurrentSkinHat(sheepData.skinHat);
            sheep.SetCurrentSkinClothe(sheepData.skinClothe);

            sheepInside.Add(sheep);
        }
    }

    public void DestroySheep()
    {
        Debug.Log("Destroy interior sheep");

        if (sheepInside.Count == 0)
            return;

        foreach (SheepSkinManager sheep in sheepInside)
        {
            if (sheep != null)
                Destroy(sheep.gameObject);
        }

        sheepInside.Clear();
    }

    private void OnDestroy()
    {
        if (SwapSceneManager.instance != null)
        {
            SwapSceneManager.instance.SwapingInteriorScene -= Initialize;
            SwapSceneManager.instance.SwapingDefaultScene -= DestroySheep;
        }
    }
}
