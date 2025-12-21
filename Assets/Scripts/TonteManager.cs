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

    [Header("Particule")]
    [SerializeField] private ParticleSystem particleTonte;
    [SerializeField] private ParticleSystem particleTonteLevel2;
    [SerializeField] private Transform[] listPoints;

    [Header("Tonte Settings")]
    [SerializeField] private float touchRadius = 0.2f;
    [SerializeField] private float touchRadiusLevel2 = 0.4f;

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
    
    [Header("Finger Offset")]
    [SerializeField] private Vector2 screenOffset = new Vector2(40f, 60f); 

    [Header("Radial Offset Correction")]
    [SerializeField] private float offsetStartDistance = 0.4f; 
    [SerializeField] private float maxOffset = 0.15f;          
    [SerializeField] private float offsetStrength = 1.0f;      
    
    private void Awake()
    {
        instance = this;
        SwapSceneManager.instance.SwapingTonteScene += Initialize;
    }

    private void OnEnable()
    {
        if (GameData.instance.dicoAmélioration[TypeAmelioration.Tonte].Item2 > 1)
        {
            particleTonte = particleTonteLevel2;
            touchRadius = touchRadiusLevel2;
        }
        
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

    private bool isFingerDown = false;
    private void OnFingerPressed(Vector2 screenPos, float timer)
    {
        if (currentSheep == null || !canTonte)
            return;
        
        isFingerDown = true;

        Vector2 offsetScreenPos = screenPos + screenOffset;
        
        Ray ray = Camera.main.ScreenPointToRay(offsetScreenPos);
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
    

    private Plane tontePlane;

    private void OnFingerMoved(Vector3 fingerWorldPos)
    {
        if (currentSheep == null || !canTonte)
        {
            particleTonte.Stop();
            return;
        }

        if (!particleTonte.isPlaying)
            return;

        // 🔹 Position UI de l'outil (inchangée)
        Vector3 uiPos = Camera.main.WorldToScreenPoint(fingerWorldPos);
        toolUI.position = uiPos;

        // 🔹 Position écran + offset doigt
        Vector3 screenPos = uiPos;
        screenPos.x += screenOffset.x;
        screenPos.y += screenOffset.y;

        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 hitPos = hit.point;

            // 🔹 Centre du mouton (ou de la laine)
            Vector3 center = tontePoint.position;

            // 🔹 Direction radiale
            Vector3 dirFromCenter = (hitPos - center);
            float distanceFromCenter = dirFromCenter.magnitude;

            if (distanceFromCenter > offsetStartDistance)
            {
                float t = Mathf.InverseLerp(
                    offsetStartDistance,
                    offsetStartDistance + 0.5f,
                    distanceFromCenter
                );

                float offsetAmount = Mathf.Lerp(0f, maxOffset, t) * offsetStrength;

                hitPos += dirFromCenter.normalized * offsetAmount;
            }

            particleTonte.transform.position = hitPos;
        }


        // 🔹 Feedback haptique
        int r = UnityEngine.Random.Range(0, 50);
        if (r == 0 && isFingerDown)
            Handheld.Vibrate();

        // 🔹 Détection logique
        DetectTouchedPoint(fingerWorldPos);
    }



    private void OnFingerReleased(Vector2 screenPos, float timer)
    {
        isFingerDown = false;
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
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(tontePoint.position, particleTonte.transform.position);
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
