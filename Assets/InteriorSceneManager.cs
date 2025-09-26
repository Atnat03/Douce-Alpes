using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class InteriorSceneManager : MonoBehaviour
{
    public List<Sheep> sheepInside;

    public List<Transform> randomSpawnPos;
    
    [SerializeField] public GameObject sheepPrefab;

    private void Awake()
    {
        SwapSceneManager.instance.SwapingInteriorScene += Initialize;
        SwapSceneManager.instance.SwapingDefaultScene += DestroySheep;
    }

    void Start()
    {
        sheepInside = new List<Sheep>();
    }
    
    public void Initialize()
    {
        Debug.Log("Initialize interior sheep");
        
        if (GameData.instance.nbSheep != 0)
        {
            List<Transform> randomSpawn = randomSpawnPos;
            
            foreach (SheepData sheepData in GameData.instance.sheepDestroyData)
            {
                Transform spawnPos = randomSpawn[Random.Range(0, randomSpawn.Count)];
                randomSpawn.Remove(spawnPos);
                
                GameObject newSheep = Instantiate(sheepPrefab, spawnPos.position, spawnPos.rotation, transform.parent);
                Sheep sheep = newSheep.GetComponent<Sheep>();
        
                sheep.sheepId = sheepData.id;
                sheep.name = sheepData.name;
                sheep.currentSkin = sheepData.skin;
        
                sheepInside.Add(sheep);
            }
        }

    }

    public void DestroySheep()
    {
        Debug.Log("Destroy interior sheep");
        
        if(sheepInside.Count != 0)
        {
            foreach (Sheep sheepData in sheepInside)
            {
                Destroy(sheepData.gameObject);
            }
            
            sheepInside.Clear();
        }
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
