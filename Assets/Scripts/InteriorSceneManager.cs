using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class InteriorSceneManager : MonoBehaviour
{
    public static InteriorSceneManager instance;
    
    public List<GameObject> sheepInside = new();
    public List<Transform> randomSpawnPos;

    [SerializeField] public GameObject sheepPrefab;

    public bool alreadyBubble = false;
    
    public Transform centerGrange;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        alreadyBubble = false;
        SwapSceneManager.instance.SwapingInteriorScene += Initialize;
    }

    private void OnDisable()
    {
        DestroySheep();
        SwapSceneManager.instance.SwapingInteriorScene -= Initialize;
    }

    private void Update()
    {
        if (GameData.instance.timer.currentMiniJeuToDo == MiniGames.Tonte && !alreadyBubble)
        {
            alreadyBubble = true;
            CheckBubble(true);
        }
        if (GameData.instance.timer.currentMiniJeuToDo == MiniGames.Nettoyage && !alreadyBubble)
        {
            alreadyBubble = true;
            CheckBubble(false);
        }
        if (GameData.instance.timer.currentMiniJeuToDo == MiniGames.Sortie && !alreadyBubble)
        {
            alreadyBubble = true;
            CheckBubble(false, true);
        }
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

            sheep.Initialize(sheepData.id, sheepData.name, sheepData.hasWhool, sheepData.colorID, sheepData.skinHat, sheepData.skinClothe);
            sheep.SetCurrentSkinHat(sheepData.skinHat);
            sheep.SetCurrentSkinClothe(sheepData.skinClothe);

            newSheep.GetComponent<SheepMovingAnimation>().center = centerGrange;
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
    
        #region Bubble

    public SheepSkinManager currentSheepNettoyage = null;
    public SheepSkinManager currentSheepTonte = null;
    public SheepSkinManager currentSheepGetOut = null;

    public void CheckBubble(bool isTonte, bool isGetOut = false)
    {
        if(!isGetOut)
        {
            if (isTonte)
            {
                Debug.Log("drink");
                if (currentSheepTonte == null)
                {
                    Debug.Log("drink pas null");

                    List<GameObject> availableSheep = sheepInside.FindAll(sheep =>
                        !sheep.GetComponent<SheepSkinManager>().HasActiveBubble());

                    if (availableSheep.Count > 0)
                    {
                        currentSheepTonte = availableSheep[Random.Range(0, availableSheep.Count)]
                            .GetComponent<SheepSkinManager>();
                        currentSheepTonte.ActivatedBubble(true);
                    }
                    else
                    {
                        Debug.LogWarning("Aucun mouton disponible pour la grange");
                    }
                }
            }
            else
            {
                Debug.Log("pas drink");
                if (currentSheepTonte == null)
                {
                    Debug.Log("pas drink pas null");

                    List<GameObject> availableSheep = sheepInside.FindAll(sheep =>
                        !sheep.GetComponent<SheepSkinManager>().HasActiveBubble());

                    if (availableSheep.Count > 0)
                    {
                        currentSheepTonte = availableSheep[Random.Range(0, availableSheep.Count)]
                            .GetComponent<SheepSkinManager>();
                        currentSheepTonte.ActivatedBubble(false);
                    }
                    else
                    {
                        Debug.LogWarning("Aucun mouton disponible pour la grange");
                    }
                }
            }
        }else
        {
            if (currentSheepGetOut == null)
            {
                Debug.Log("pas drink pas null");

                List<GameObject> availableSheep = sheepInside.FindAll(sheep =>
                    !sheep.GetComponent<SheepSkinManager>().HasActiveBubble());

                if (availableSheep.Count > 0)
                {
                    currentSheepGetOut = availableSheep[Random.Range(0, availableSheep.Count)]
                        .GetComponent<SheepSkinManager>();
                    currentSheepGetOut.ActivatedBubble(false, true);
                }
                else
                {
                    Debug.LogWarning("Aucun mouton disponible pour la grange");
                }
            }
        }
    }
    public void DisableTonteBubble()
    {
        currentSheepTonte.DisableBubble();
        currentSheepTonte = null;
    }

    public void DisableCleanBubble()
    {
        if(currentSheepNettoyage == null)
            return;
        
        currentSheepNettoyage.DisableBubble();
        currentSheepNettoyage = null;
    }
    
    public void DisableSortieBubble()
    {
        currentSheepGetOut.DisableBubble();
        currentSheepGetOut = null;
    }

    #endregion

}
