using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [Header("Tuto")]
    public bool isTutoInterior = true;
    public string[] messages;
    public GameObject papy;
    public TextMeshProUGUI message;
    private int idMessage = -1;
    public GameObject nextMessage;
    private bool isWritting = false;
    
    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        SwapSceneManager.instance.SwapingInteriorScene += Initialize;
        alreadyBubble = false;
        
        if (isTutoInterior)
        {
            NextMessage(true);
        }
        else
        {
            papy.SetActive(false);
        }
    }

    private void OnDisable()
    {
        DestroySheep();
        SwapSceneManager.instance.SwapingInteriorScene -= Initialize;
    }

    private void Update()
    {
        if (isTutoInterior)
            return;
        
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

    #region Tuto
    
    private void StopTuto()
    {
        isTutoInterior = false;
        papy.SetActive(false);
    }
    
    public void NextMessage(bool isFirst = false)
    {
        if (isWritting) return;
        
        nextMessage.SetActive(false);
        
        idMessage++;
        
        if (idMessage >= messages.Length)
        {
             StopTuto();
            
            return;
        }
        
        StopAllCoroutines();
        StartCoroutine(WriteSmooth(messages[idMessage], isFirst));
    }


    IEnumerator WriteSmooth(string fullMessage, bool isFirst = false, float charDelay = 0.025f)
    {
        isWritting = true;
        
        AudioManager.instance.PlaySound(32);
        
        if(!isFirst)
        {
            papy.GetComponent<Animator>().SetTrigger("NextMessage");

            yield return new WaitForSeconds(0.3f);
        }
        
        AudioManager.instance.PlaySound(Random.Range(43, 48));
        
        message.text = "";
        foreach (char c in fullMessage)
        {
            message.text += c;
            yield return new WaitForSeconds(charDelay);
        }
        
        nextMessage.SetActive(true);
        isWritting = false;
    }
    
    #endregion
}
