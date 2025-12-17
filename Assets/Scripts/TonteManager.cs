using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TonteManager : MiniGameParent
{
    public static TonteManager instance;

    [Header("Mouton")]
    [SerializeField] private GameObject sheepModel;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform tontePoint;
    [SerializeField] private Transform destroyPoint;

    [Header("UI")]
    [SerializeField] private Text nameText;
    [SerializeField] private Text nbToCutText;
    [SerializeField] private Button backButton;

    [Header("Particule")]
    [SerializeField] private ParticleSystem particleTonte;
    [SerializeField] private Transform[] listPoints;

    [Header("Tonte Settings")]
    [SerializeField] private float touchRadius = 0.15f;

    private GameObject currentSheep;
    private List<Transform> curList = new List<Transform>();
    private int sheepIndex = 0;
    private bool canTonte = false;
    private float currentValueTonte;
    [SerializeField] private float miniValueToEnd;
    public int RawSheepWhoolDrop = 0;
    
    [SerializeField] private RectTransform spawnLaineSprite;
    private SheepData currentSheepData;
    [SerializeField] RectTransform toolUI; 

    private void Awake()
    {
        instance = this;
        SwapSceneManager.instance.SwapingTonteScene += Initialize;
    }

    private void OnEnable()
    {
        if (TouchManager.instance != null)
        {
            TouchManager.instance.OnGetFingerPosition += OnFingerMoved;
            TouchManager.instance.OnEndEvent += OnFingerReleased;
            TouchManager.instance.OnStartEvent += OnFingerPressed;
        }

        if (TutoManager.instance != null)
            TutoManager.instance.MiniJeuTonte();
    }

    private void OnDisable()
    {
        if (TouchManager.instance != null)
        {
            TouchManager.instance.OnGetFingerPosition -= OnFingerMoved;
            TouchManager.instance.OnEndEvent -= OnFingerReleased;
            TouchManager.instance.OnStartEvent -= OnFingerPressed;
        }
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

            EndMiniGame(TypeAmelioration.Tonte);

            GameData.instance.timer.canButtonT = false;
            GameData.instance.timer.canButtonC = true;
            GameData.instance.timer.UpdateAllButton();

            StartCoroutine(WaitBeforeSwapScene());

            return;
        }

        curList.Clear();
        foreach (Transform t in listPoints)
            curList.Add(t);

        particleTonte.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particleTonte.transform.position = tontePoint.position;

        SheepData nextSheepData = GameData.instance.sheepDestroyData[sheepIndex];
        currentSheepData = nextSheepData;
        
        currentSheep = Instantiate(sheepModel, spawnPoint.position, spawnPoint.rotation, transform);
        
        nameText.text = nextSheepData.name;
        currentSheep.GetComponent<SheepSkinManager>().Initialize(
            nextSheepData.id,
            nextSheepData.name,
            true,
            nextSheepData.colorID,
            nextSheepData.skinHat,
            nextSheepData.skinClothe, true);
        
        nbToCutText.text = $"{sheepIndex + 1}/{GameData.instance.sheepDestroyData.Count}";

        StartCoroutine(MoveOverTime(currentSheep.transform, tontePoint.position, 2f, true));

        sheepIndex++;
    }

    IEnumerator WaitBeforeSwapScene()
    {
        yield return new WaitForSeconds(2f);
        SwapSceneManager.instance.SwapScene(1);
    }
    
    private void OnFingerPressed(Vector2 screenPos, float timer)
    {
        if (currentSheep == null || !canTonte)
            return;

        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Déplacer la particule sur le doigt
            particleTonte.transform.position = hit.point;

            // Puis l’activer
            particleTonte.Play();
        }
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
            particleTonte.Stop();
    }
    

    private void OnFingerMoved(Vector3 fingerWorldPos)
    {
        if (currentSheep == null || !canTonte)
        {
            particleTonte.Stop();
            return;
        }

        if (!particleTonte.isPlaying)
            return;

        Vector3 fromCenter = fingerWorldPos - currentSheep.transform.position;
        float distance = fromCenter.magnitude;
        float maxDistance = 0.85f;

        Vector3 direction = fromCenter.normalized;
        float t = Mathf.Clamp01(distance / maxDistance);
        t = Mathf.Pow(t, 1.5f);
        Vector3 offset = direction * Mathf.Lerp(0f, 1.5f, t);

        particleTonte.transform.position = currentSheep.transform.position + offset;
        
        toolUI.position = Camera.main.WorldToScreenPoint(fingerWorldPos);

        int r = Random.Range(0, 50);
        if (r == 0)
            Handheld.Vibrate();

        DetectTouchedPoint(fingerWorldPos);
    }

    private void OnFingerReleased(Vector2 screenPos, float timer)
    {
        if (particleTonte != null)
            particleTonte.Stop();
        
        if (curList.Count <= miniValueToEnd && canTonte)
        {
            EndTonte();
        }
    }

    private void DetectTouchedPoint(Vector3 fingerWorldPos)
    {
        posFinger = fingerWorldPos;
        
        for (int i = curList.Count - 1; i >= 0; i--)
        {
            float dist = Vector3.Distance(curList[i].position, fingerWorldPos);
            
            if (dist <= touchRadius)
            {
                curList.Remove(curList[i]);
                break;
            }
        }

        UpdateProgress();
    }

    private Vector3 posFinger;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(posFinger, touchRadius);
    }

    private void UpdateProgress()
    {
        currentValueTonte = 1f - (float)curList.Count / listPoints.Length;
    }

    public void EndTonte()
    {
        if (currentSheep != null)
        {
            currentSheepData.hasWhool = false;
            PlayerMoney.instance.AddWhool(GameData.instance.GetLevelUpgrade(TypeAmelioration.Tonte), spawnLaineSprite.position);
            StartCoroutine(SendToDestroy(currentSheep));
        }
    }

    private IEnumerator SendToDestroy(GameObject sheep)
    {
        if (currentSheep == sheep)
            currentSheep = null;

        if (sheep != null)
            yield return MoveOverTime(sheep.transform, destroyPoint.position, 1f, false);

        if (sheep != null)
            Destroy(sheep);

        yield return new WaitForSeconds(0.25f);
        NextSheep();
    }

    private void OnDestroy()
    {
        SwapSceneManager.instance.SwapingTonteScene -= Initialize;
    }
}
