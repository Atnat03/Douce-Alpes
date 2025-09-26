using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TonteManager : MonoBehaviour
{
    [SerializeField] private GameObject sheepModel;
    [SerializeField] private Text nameText;
    [SerializeField] private Text nbToCutText;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform tontePoint;
    [SerializeField] private Transform destroyPoint;

    [SerializeField] private Button tonteButton;

    private GameObject currentSheep;
    private int sheepIndex = 0;

    [SerializeField] private Button backButton;

    private void Awake()
    {
        SwapSceneManager.instance.SwapingTonteScene += Initialize;
        
        tonteButton.onClick.AddListener(EndTonte);
        backButton.onClick.AddListener(ExitScene);
    }

    public void Initialize()
    {
        backButton.gameObject.SetActive(false);
        
        sheepIndex = 0;

        if (currentSheep != null)
        {
            Destroy(currentSheep);
            currentSheep = null;
        }

        if (GameData.instance.sheepDestroyData.Count > 0)
            NextSheep();
    }

    private void NextSheep()
    {
        if (sheepIndex >= GameData.instance.sheepDestroyData.Count)
        {
            nameText.text = "Tous les moutons sont finis !";
            nbToCutText.text = "";
            
            backButton.gameObject.SetActive(true);
            
            return;
        }

        SheepData nextSheepData = GameData.instance.sheepDestroyData[sheepIndex];

        currentSheep = Instantiate(sheepModel, spawnPoint.position, spawnPoint.rotation, transform);

        nameText.text = nextSheepData.name;
        nbToCutText.text = $"{sheepIndex + 1}/{GameData.instance.sheepDestroyData.Count}";

        StartCoroutine(MoveOverTime(currentSheep.transform, tontePoint.position, 2f));

        sheepIndex++;
    }
    
    public void EndTonte()
    {
        if (currentSheep != null)
        {
            StartCoroutine(SendToDestroy(currentSheep));
        }
    }

    private IEnumerator SendToDestroy(GameObject sheep)
    {
        yield return MoveOverTime(sheep.transform, destroyPoint.position, 1f);

        Destroy(sheep);

        currentSheep = null;

        yield return new WaitForSeconds(0.25f);
        NextSheep();
    }

    private IEnumerator MoveOverTime(Transform target, Vector3 destination, float duration)
    {
        Vector3 start = target.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            target.position = Vector3.Lerp(start, destination, t);
            yield return null;
        }

        target.position = destination;
    }

    void ExitScene()
    {
        SwapSceneManager.instance.SwapScene(0);
    }

    private void OnDestroy()
    {
        if (SwapSceneManager.instance != null)
            SwapSceneManager.instance.SwapingTonteScene -= Initialize;

        tonteButton.onClick.RemoveListener(EndTonte);
    }
}
