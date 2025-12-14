using System.Collections;
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
    [SerializeField] private GameObject sheepModel;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform cleanPoint;
    [SerializeField] private Transform destroyPoint;

    [Header("UI")]
    [SerializeField] private Text nameText;
    [SerializeField] private Text nbToCleanText;
    [SerializeField] private Button backButton;

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
    public GameObject currentSheep;

    public float GetCleanValue() => cleanValue;

    [Header("Swipe Detection")]
    [SerializeField] private SwipeDetection swipeDetection;
    [SerializeField] private RectTransform fingerFollower;
    [SerializeField] private Image imageTool;
    [SerializeField] private Sprite logoShampoo;
    [SerializeField] private Sprite logoShower;

    private Vector2 currentFingerScreenPos = Vector2.zero;
    private bool isSwiping = false;

    [HideInInspector] public bool canRotateCamera = false;
    [HideInInspector] public bool sheepIsMoving = false;

    // ==========================
    // 🔀 SHAKE SYSTEM
    // ==========================
    private int currentCycle = 0;            // 2 cycles par mouton
    public int randomShakeValue = 0;
    public bool alreadyShaken = false;
    
    [Header("Head Detection")]
    public float headDetectionMultiplier = 0.6f;
    public Vector3 headDetectionOffset = new Vector3(0f, 0.1f, 0.05f);

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        SwapSceneManager.instance.SwapingCleanScene += Initialize;
        backButton.onClick.AddListener(ExitScene);
    }

    private void Start()
    {
        if (swipeDetection == null)
            swipeDetection = SwipeDetection.instance;

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

        SwapSceneManager.instance.SwapingCleanScene -= Initialize;
    }

    public void Initialize()
    {
        Debug.Log("🔄 Initialize appelé - Réinitialisation du jeu");
    
        backButton.gameObject.SetActive(false);
        sheepIndex = 0;  // ✅ Bien à 0

        if (currentSheep != null)
        {
            Destroy(currentSheep);
            currentSheep = null;
        }

        if (GameData.instance.sheepDestroyData.Count > 0)
        {
            Debug.Log($"📋 {GameData.instance.sheepDestroyData.Count} mouton(s) à nettoyer");
            NextSheep();
        }
    }

    private void NextSheep()
    {
        Debug.Log($"🐑 NextSheep appelé | sheepIndex = {sheepIndex} | Total moutons = {GameData.instance.sheepDestroyData.Count}");

        canRotateCamera = false;

        currentCycle = 0;
        alreadyShaken = false;

        if (sheepIndex >= GameData.instance.sheepDestroyData.Count)
        {
            Debug.Log("❌ Fin du jeu : tous les moutons sont finis");
            nameText.text = "Tous les moutons sont finis !";
            nbToCleanText.text = "";
            backButton.gameObject.SetActive(true);
            EndMiniGame(TypeAmelioration.Nettoyage);
            SwapSceneManager.instance.SwapScene(1);
            return;
        }

        SheepData nextSheepData = GameData.instance.sheepDestroyData[sheepIndex];
        Debug.Log($"✅ Chargement du mouton {nextSheepData.name} (index {sheepIndex})");

        currentSheep = Instantiate(sheepModel, spawnPoint.position, spawnPoint.rotation, transform);
        sheepTarget = currentSheep.transform;

        nameText.text = nextSheepData.name;
        currentSheep.GetComponent<SheepSkinManager>().Initialize(
            nextSheepData.id,
            nextSheepData.name,
            false,
            nextSheepData.colorID,
            nextSheepData.skinHat,
            nextSheepData.skinClothe
        );

        nbToCleanText.text = $"{sheepIndex + 1}/{GameData.instance.sheepDestroyData.Count}";

        // ✅ Lancer le mouvement et incrémenter APRÈS
        StartCoroutine(InitializeSheep(currentSheep.transform));
    }

// ✅ Nouvelle coroutine pour gérer l'arrivée du mouton
    private IEnumerator InitializeSheep(Transform sheep)
    {
        // Attendre que le mouton arrive
        yield return StartCoroutine(MoveOverTime(sheep, cleanPoint.position, 2f));
    
        // ✅ Incrémenter UNIQUEMENT quand le mouton est arrivé
        sheepIndex++;
        Debug.Log($"📈 sheepIndex incrémenté à {sheepIndex}");
    
        // Initialiser le système de nettoyage
        ResetCleanSystem();
        FindObjectOfType<StateMachineClean>().InitializedStates();
    }
    
    
    public float GetHeadDetectionRadius()
    {
        var skin = currentSheep.GetComponentInChildren<SkinnedMeshRenderer>();
        return skin.bounds.extents.y * headDetectionMultiplier;
    }

    private IEnumerator MoveOverTime(Transform target, Vector3 destination, float duration)
    {
        if (target == null)
        {
            Debug.LogWarning("⚠️ MoveOverTime : target est null");
            yield break;
        }
    
        sheepIsMoving = true;
        Vector3 start = target.position;
        float elapsed = 0f;

        canAddShampoo = false;
        canRotateCamera = false;

        while (elapsed < duration)
        {
            // ✅ Vérifier que le target existe toujours
            if (target == null)
            {
                Debug.LogWarning("⚠️ MoveOverTime interrompu : target détruit");
                sheepIsMoving = false;
                yield break;
            }
        
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            target.position = Vector3.Lerp(start, destination, t);
            yield return null;
        }

        if (target != null)
        {
            target.position = destination;
        }
    
        canAddShampoo = true;
        sheepIsMoving = false;
    
        Debug.Log("✅ MoveOverTime terminé");
    }
    public void StartNewCycle()
    {
        currentCycle++;
        alreadyShaken = false;
        randomShakeValue = Random.Range(0, maxShampoo);

        Debug.Log($"🔀 Cycle {currentCycle} | Shake à {randomShakeValue}");
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

    public void ApplyClean(Vector3 pos, bool isHead = false)
    {
        if (!canAddShampoo) return;
        if (totalValueCleaned >= maxShampoo && currentTool == CleaningTool.Shampoo) return;

        switch (currentTool)
        {
            case CleaningTool.Shampoo:
                TryAddShampoo(pos, isHead);
                break;
            case CleaningTool.Shower:
                CheckShampoo(pos);
                break;
        }
    }

    private void TryAddShampoo(Vector3 pos, bool isHead = false)
    {
        Transform parent = isHead
            ? currentSheep.GetComponent<SheepCleanningModel>().head
            : currentSheep.GetComponent<SheepCleanningModel>().body;
        
        if (!hasLastPos || Vector3.Distance(lastShampooPos, pos) >= minDistanceBetweenShampoos)
        {
            GameObject prefab = shampoos[Random.Range(0, shampoos.Length)];
            Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            GameObject s = Instantiate(prefab, pos, rot);
            s.transform.SetParent(parent, true);;
            float size = Random.Range(0.75f, 1f);
            s.transform.localScale *= size;

            s.layer = currentCleaningLayer;

            shampooList.Add(s);
            cleanValue += 1f;

            lastShampooPos = pos;
            hasLastPos = true;
            
            int r = Random.Range(0, 10);
            if (r == 0)
            {
                TriggerShake();
            }
        }
    }

    public void TriggerShake()
    {
        Debug.Log("🐑💥 LE MOUTON SE SECOUE !");
        currentSheep.GetComponent<SheepSkinManager>().PlayShakeAnimation();
    }

    private void CheckShampoo(Vector3 pos)
    {
        GameObject d = Instantiate(shower, pos, Quaternion.identity);
        Destroy(d, 0.3f);

        float radius = 0.1f;
        for (int i = shampooList.Count - 1; i >= 0; i--)
        {
            GameObject s = shampooList[i];
            if (s.layer == currentCleaningLayer &&
                Vector3.Distance(s.transform.position, pos) <= radius)
            {
                Destroy(s);
                shampooList.RemoveAt(i);
            }
        }

        // ✅ NE PAS déclencher allCleaned ici, juste vérifier la liste globale
        // La vérification se fait dans RightPosState après avoir rincé les 3 côtés
    }

    public void OnAllCleaned()
    {
        Debug.Log("🎉 OnAllCleaned appelé");
    
        // ✅ Protection : éviter les appels multiples
        if (currentSheep == null)
        {
            Debug.LogWarning("⚠️ OnAllCleaned ignoré : currentSheep est null");
            return;
        }
    
        ResetValueClean();
        StartCoroutine(SendToDestroy(currentSheep));
    }

    private IEnumerator SendToDestroy(GameObject sheep)
    {
        Debug.Log("🚀 SendToDestroy démarré");
    
        if (sheep == null || currentSheep == null)
        {
            Debug.LogWarning("⚠️ SendToDestroy annulé : mouton déjà détruit");
            yield break;
        }
    
        // ✅ Bloquer les interactions
        canAddShampoo = false;
    
        currentSheep.GetComponent<SheepSkinManager>().PlayJumpAnimation();
        yield return new WaitForSeconds(0.3f);
    
        // ✅ Déplacer le mouton vers la sortie
        yield return StartCoroutine(MoveOverTime(sheep.transform, destroyPoint.position, 1f));
    
        Debug.Log("🗑️ Destruction du mouton");
        Destroy(sheep);
        currentSheep = null;
        sheepTarget = null;
    
        yield return new WaitForSeconds(0.25f);
    
        Debug.Log("📞 SendToDestroy appelle NextSheep()");
        NextSheep();
    }  
    
    public void ResetCleanSystem()
    {
        foreach (GameObject s in shampooList) Destroy(s);
        shampooList.Clear();

        allCleaned = false;
        totalValueCleaned = 0;
        cleanValue = 0;
        hasLastPos = false;
        lastShampooPos = Vector3.zero;

        SetShampoo();
    }

    // ==========================
    // 👆 INPUT
    // ==========================
    private void HandleFingerPositionUpdate(Vector2 screenPos)
    {
        currentFingerScreenPos = screenPos;

        if (screenPos != Vector2.zero)
            imageTool.transform.position = screenPos;
    }

    private void HandleSwipeEnd()
    {
        currentFingerScreenPos = Vector2.zero;
    }

    private void ExitScene()
    {
        SwapSceneManager.instance.SwapScene(1);
    }
    
    void OnDrawGizmos()
    {
        if (currentSheep == null) return;

        Transform head = currentSheep
            .GetComponent<SheepCleanningModel>().head;

        Vector3 headCenter = head.TransformPoint(headDetectionOffset);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(headCenter, headDetectionMultiplier);
    }

}
