using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CleaningTool { Shampoo, Shower, None }

public enum CleaningSide { Left, Front, Right }

public class CleanManager : MiniGameParent
{
    public static CleanManager instance;

    [Header("References")]
    public Camera camera;
    public Transform sheepTarget;

    [Header("Current Tool")]
    public CleaningTool currentTool;

    [Header("Tool Particles")]
    [SerializeField] private GameObject[] shampoos;
    [SerializeField] private GameObject shower;
    
    public List<GameObject> shampooList = new List<GameObject>();

    [Header("Clean Values")]
    private float cleanValue = 0;
    public float totalValueCleaned = 0;
    public int maxShampoo = 100;

    [Header("Anti-Spam Shampoo Settings")]
    [SerializeField] private float minDistanceBetweenShampoos = 0.05f;
    private Vector3 lastShampooPos = Vector3.zero;
    private bool hasLastPos = false;

    public int currentCleaningLayer = 0;
    public CleaningSide currentCleaningSide;

    [Header("Side Centers")]
    public Transform leftCenter;
    public Transform frontCenter;
    public Transform rightCenter;
    public float maxDistanceFromCenter = 1.0f;

    [HideInInspector] public bool allCleaned = false;
    [HideInInspector] public bool canAddShampoo = true;

    private int sheepIndex = 0;

    public float GetCleanValue() => cleanValue;

    [Header("Swipe Detection")]
    [SerializeField] private SwipeDetection swipeDetection;
    [SerializeField] private RectTransform fingerFollower;
    [SerializeField] private Image imageTool;
    [SerializeField] private Sprite logoShampoo;
    [SerializeField] private Sprite logoShower;

    private Vector2 currentFingerScreenPos = Vector2.zero;
    private bool isSwiping = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (swipeDetection == null) swipeDetection = SwipeDetection.instance;
        swipeDetection.OnFingerPositionUpdated += HandleFingerPositionUpdate;
        swipeDetection.OnSwipeEnded += HandleSwipeEnd;

        SetShampoo();
    }

    private void OnDestroy()
    {
        if (swipeDetection != null)
        {
            swipeDetection.OnFingerPositionUpdated -= HandleFingerPositionUpdate;
            swipeDetection.OnSwipeEnded -= HandleSwipeEnd;
        }
    }

    private void InitalizeSheep()
    {
        SheepData nextSheepData = GameData.instance.sheepDestroyData[sheepIndex];
        sheepTarget.GetComponent<SheepSkinManager>().Initialize(
            nextSheepData.id, nextSheepData.name, false, nextSheepData.colorID, nextSheepData.skinHat, nextSheepData.skinClothe);
        sheepIndex++;
    }
    
    public void ResetValueClean()
    {
        totalValueCleaned += cleanValue;
        cleanValue = 0;
        hasLastPos = false;
        lastShampooPos = Vector3.zero;
    }

    public void SetShampoo()
    {
        currentTool = CleaningTool.Shampoo;

        imageTool.sprite = logoShampoo;
    }

    public void SetShower()
    {
        currentTool = CleaningTool.Shower;
        
        imageTool.sprite = logoShower;
    }

    public void PerformClean(Vector3 pos)
    {
        ApplyClean(pos);
    }

    public void ApplyClean(Vector3 pos)
    {
        if (!canAddShampoo) return;

        if (totalValueCleaned >= maxShampoo && currentTool == CleaningTool.Shampoo) return;

        switch (currentTool)
        {
            case CleaningTool.Shampoo:
                TryAddShampoo(pos);
                break;
            case CleaningTool.Shower:
                CheckShampoo(pos);
                break;
        }
    }

    private void TryAddShampoo(Vector3 pos)
    {
        if (!hasLastPos || Vector3.Distance(lastShampooPos, pos) >= minDistanceBetweenShampoos)
        {
            GameObject prefab = shampoos[Random.Range(0, shampoos.Length)];
            float randomY = Random.Range(0f, 360f);
            Quaternion randomRotation = Quaternion.Euler(0f, randomY, 0f);

            GameObject s = Instantiate(prefab, pos, randomRotation);
            s.layer = currentCleaningLayer;

            float randomScale = Random.Range(0.75f, 1f);
            s.transform.localScale = Vector3.one * randomScale;

            shampooList.Add(s);
            cleanValue += 1f;

            lastShampooPos = pos;
            hasLastPos = true;
        }
    }

    private void CheckShampoo(Vector3 pos)
    {
        GameObject d = Instantiate(shower, pos, Quaternion.identity);
        Destroy(d, 0.3f);

        float radius = 0.1f;
        for (int i = shampooList.Count - 1; i >= 0; i--)
        {
            GameObject s = shampooList[i];
            if (s.layer == currentCleaningLayer && Vector3.Distance(s.transform.position, pos) <= radius)
            {
                Destroy(s);
                shampooList.RemoveAt(i);
            }
        }

        if (shampooList.Count == 0)
        {
            allCleaned = true;
            OnAllCleaned();
        }
    }

    public void OnAllCleaned()
    {
        Debug.Log("✅ Tout est propre !");
        ResetValueClean();
        hasLastPos = false;
        EndMiniGame(TypeAmelioration.Nettoyage);
        GameData.instance.timer.canButtonG = true;
        GameData.instance.timer.canButtonC = false;
        GameData.instance.timer.UpdateAllButton();
    }

    public void ResetCleanSystem()
    {
        Debug.Log("🔄 Reset du système de nettoyage...");
        foreach (GameObject s in shampooList) Destroy(s);
        shampooList.Clear();
        allCleaned = false;
        totalValueCleaned = 0;
        cleanValue = 0;
        hasLastPos = false;
        lastShampooPos = Vector3.zero;
        SetShampoo();
    }

    private void HandleFingerPositionUpdate(Vector2 screenPos)
    {
        if (screenPos == Vector2.zero)
        {
            isSwiping = false;
            if (fingerFollower != null) fingerFollower.gameObject.SetActive(false);
            return;
        }

        currentFingerScreenPos = screenPos;
        isSwiping = true;

        // Update follower position et active-le
        UpdateFollowerPosition(screenPos);

        if (currentTool != CleaningTool.None)
        {
            Debug.Log($"Doigt à screen pos: {screenPos} | Outil: {currentTool}");

            Vector3 worldPos = camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));
            if (Vector3.Distance(worldPos, sheepTarget.position) > maxDistanceFromCenter * 2)
            {
                Debug.Log("⚠️ Swipe hors mouton !");
            }

            // Option : Force un clean si tu veux bypass SwipeDetection (décommente si besoin)
            // PerformClean(worldPos);
        }
    }

    private void UpdateFollowerPosition(Vector2 screenPosition)
    {
        if (fingerFollower != null)
        {
            fingerFollower.position = screenPosition;
            fingerFollower.gameObject.SetActive(true);  // Active pendant swipe
        }
    }

    private void HandleSwipeEnd()
    {
        isSwiping = false;
        currentFingerScreenPos = Vector2.zero;
        if (fingerFollower != null) fingerFollower.gameObject.SetActive(false);
    }

    public Vector2 GetCurrentFingerScreenPosition() => currentFingerScreenPos;

    private void OnDisable()
    {
        if (swipeDetection != null)
        {
            swipeDetection.OnFingerPositionUpdated -= HandleFingerPositionUpdate;
            swipeDetection.OnSwipeEnded -= HandleSwipeEnd;
        }

        foreach (GameObject shampoo in shampooList)
        {
            Destroy(shampoo);
        }
    }

    void OnEnable()
    {
        //InitalizeSheep(); 
        SetShampoo();

        if (swipeDetection == null) swipeDetection = SwipeDetection.instance;
        swipeDetection.OnFingerPositionUpdated += HandleFingerPositionUpdate;
        swipeDetection.OnSwipeEnded += HandleSwipeEnd;

        if (TutoManager.instance != null) TutoManager.instance.MiniJeuClean();
    }
}