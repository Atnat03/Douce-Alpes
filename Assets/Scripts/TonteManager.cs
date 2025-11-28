using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TonteManager : MiniGameParent
{
    public static TonteManager instance;

    [SerializeField] private GameObject sheepModel;
    [SerializeField] private Text nameText;
    [SerializeField] private Text nbToCutText;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform tontePoint;
    [SerializeField] private Transform destroyPoint;

    [SerializeField] private Button tonteButton;
    private bool canTonte = false;

    private GameObject currentSheep;
    private int sheepIndex = 0;

    [SerializeField] private Button backButton;

    [SerializeField] private ParticleSystem particleTonte;
    [SerializeField] private Transform[] listPoints;
    private List<Transform> curList = new List<Transform>();

    [SerializeField] private float currentValueTonte;
    [SerializeField] private float touchRadius = 0.15f;

    private void Awake()
    {
        instance = this;

        SwapSceneManager.instance.SwapingTonteScene += Initialize;

        tonteButton.onClick.AddListener(EndTonte);
        backButton.onClick.AddListener(ExitScene);
    }

    private void Update()
    {
        UpdateProgress();
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

    public void Tondre()
    {
        SheepData sheepData = GameData.instance.sheepDestroyData[sheepIndex - 1];
        sheepData.hasWhool = false;
    }

    private void NextSheep()
    {
        if (sheepIndex >= GameData.instance.sheepDestroyData.Count)
        {
            nameText.text = "Tous les moutons sont finis !";
            nbToCutText.text = "";

            backButton.gameObject.SetActive(true);

            EndMiniGame(TypeAmelioration.Tonte);

            GameData.instance.timer.canButtonT = false;
            GameData.instance.timer.canButtonC = true;
            GameData.instance.timer.UpdateAllButton();

            return;
        }

        curList.Clear();
        foreach (Transform t in listPoints)
            curList.Add(t);

        particleTonte.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particleTonte.transform.position = tontePoint.position;

        SheepData nextSheepData = GameData.instance.sheepDestroyData[sheepIndex];

        currentSheep = Instantiate(sheepModel, spawnPoint.position, spawnPoint.rotation, transform);

        nameText.text = nextSheepData.name;
        nbToCutText.text = $"{sheepIndex + 1}/{GameData.instance.sheepDestroyData.Count}";

        StartCoroutine(MoveOverTime(currentSheep.transform, tontePoint.position, 2f, true));

        sheepIndex++;
    }

    public void EndTonte()
    {
        if (currentSheep != null)
        {
            PlayerMoney.instance.AddWhool(100);
            StartCoroutine(SendToDestroy(currentSheep));
        }
    }

    private IEnumerator SendToDestroy(GameObject sheep)
    {
        yield return MoveOverTime(sheep.transform, destroyPoint.position, 1f, false);

        Destroy(sheep);
        currentSheep = null;

        yield return new WaitForSeconds(0.25f);
        NextSheep();
    }

    private IEnumerator MoveOverTime(Transform target, Vector3 destination, float duration, bool isPosTonte)
    {
        Vector3 start = target.position;
        float elapsed = 0f;
        canTonte = false;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            target.position = Vector3.Lerp(start, destination, t);
            yield return null;
        }

        target.position = destination;

        canTonte = true;
        
        if (isPosTonte)
            particleTonte.Play();
    }

    private void SetEffectPositionToFingerPosition(Vector3 pos)
    {
        if (currentSheep == null)
            return;

        Vector3 fromCenter = pos - currentSheep.transform.position;
        float distance = fromCenter.magnitude;
        float maxDistance = 0.85f;

        Vector3 direction = fromCenter.normalized;

        float t = Mathf.Clamp01(distance / maxDistance);
        t = Mathf.Pow(t, 1.5f);

        float offsetStrength = Mathf.Lerp(0f, 1.5f, t);
        Vector3 offset = direction * offsetStrength;

        particleTonte.transform.position = currentSheep.transform.position + offset;

        DetectTouchedPoint(pos);
    }

    void DetectTouchedPoint(Vector3 fingerWorldPos)
    {
        for (int i = curList.Count - 1; i >= 0; i--)
        {
            float dist = Vector3.Distance(curList[i].position, fingerWorldPos);

            if (dist <= touchRadius)
            {
                RemovePoint(curList[i]);
                break;
            }
        }
    }

    void RemovePoint(Transform t)
    {
        curList.Remove(t);

        if (curList.Count == 0)
            EndTonte();
    }

    void UpdateProgress()
    {
        currentValueTonte = (float)curList.Count / listPoints.Length;

        if (currentValueTonte <= 0.1 && canTonte)
        {
            EndTonte();
        }
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

    private void OnEnable()
    {
        TouchManager.instance.OnGetFingerPosition += SetEffectPositionToFingerPosition;

        if (TutoManager.instance != null)
            TutoManager.instance.MiniJeuTonte();
    }

    private void OnDisable()
    {
        TouchManager.instance.OnGetFingerPosition -= SetEffectPositionToFingerPosition;
    }
}
