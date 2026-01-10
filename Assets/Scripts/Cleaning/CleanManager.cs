using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    [SerializeField] private Sprite[] logoShower;

    private Vector2 currentFingerScreenPos = Vector2.zero;
    private bool isSwiping = false;

    [HideInInspector] public bool canRotateCamera = false;
    [HideInInspector] public bool sheepIsMoving = false;
    
    private int currentCycle = 0;            
    public int randomShakeValue = 0;
    public bool alreadyShaken = false;
    
    [Header("Head Detection")]
    public float headDetectionMultiplier = 0.6f;
    public Vector3 headDetectionOffset = new Vector3(0f, 0.1f, 0.05f);
    public Transform focusCam;
    
    [SerializeField] private ParticleSystem cleanVFX;
    [SerializeField] private AudioSource audioClean;
    [SerializeField] private AudioClip bubbleClean;
    [SerializeField] private AudioClip shampooClean;
    [SerializeField] private AudioClip showerSound;

    [Header("Finger Offset")]
    [SerializeField] private Vector2 screenOffset = new Vector2(40f, 60f);

    [Header("Radial Offset Correction")]
    [SerializeField] private float offsetStartDistance = 0.4f;
    [SerializeField] private float maxOffset = 0.15f;
    [SerializeField] private float offsetStrength = 1.0f;

    
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

    private void OnEnable()
    {
        TouchManager.instance.OnEndEvent += OnFingerReleased;
    }

    private void OnDestroy()
    {
        if (swipeDetection != null)
        {
            swipeDetection.OnFingerPositionUpdated -= HandleFingerPositionUpdate;
            swipeDetection.OnSwipeEnded -= HandleSwipeEnd;
        }
        
        TouchManager.instance.OnEndEvent -= OnFingerReleased;

        SwapSceneManager.instance.SwapingCleanScene -= Initialize;
    }

    private void OnFingerReleased(Vector2 screenPos, float timer)
    {
        audioClean.Stop();
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
        {
            NextSheep();
        }
    }

    private void NextSheep()
    {
        canRotateCamera = false;

        currentCycle = 0;
        alreadyShaken = false;

        if (sheepIndex >= GameData.instance.sheepDestroyData.Count)
        {
            StartCoroutine(WaitBeforeChange());
            
            ResetCleanSystem();
            
            nameText.text = "Tous les moutons sont finis !";
            nbToCleanText.text = "";
            backButton.gameObject.SetActive(true);
            EndMiniGame(TypeAmelioration.Nettoyage);
            
            AudioManager.instance.PlaySound(11);

            GameData.instance.timer.canButtonC = false;
            GameData.instance.timer.canButtonG = true;
            
            GameData.instance.timer.UpdateAllButton();
            SwapSceneManager.instance.SwapScene(1);
            return;
        }

        SheepData nextSheepData = GameData.instance.sheepDestroyData[sheepIndex];

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
        
        currentSheep.GetComponent<SheepSkinManager>().animator.SetBool("Walk", true);

        StartCoroutine(InitializeSheep(currentSheep.transform));
    }

    IEnumerator WaitBeforeChange()
    {
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator InitializeSheep(Transform sheep)
    {
        yield return StartCoroutine(MoveOverTime(sheep, cleanPoint.position, 2f));
    
        sheepIndex++;
    
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
        target.GetComponent<SheepSkinManager>().animator.SetBool("Walk", false);
    }
    public void StartNewCycle()
    {
        currentCycle++;
        alreadyShaken = false;
        randomShakeValue = Random.Range(0, maxShampoo);
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
        audioClean.clip = shampooClean;
        currentTool = CleaningTool.Shampoo;
        imageTool.sprite = logoShampoo;
    }

    public void SetShower()
    {
        audioClean.clip = showerSound;
        currentTool = CleaningTool.Shower;
        imageTool.sprite = logoShower[GameData.instance.GetLevel(TypeAmelioration.Nettoyage)];
    }

    public void ApplyClean(Vector3 pos, bool isHead = false)
    {
        if (!canAddShampoo) return;
        if (totalValueCleaned >= maxShampoo && currentTool == CleaningTool.Shampoo) return;

        if(!audioClean.isPlaying)
            audioClean.Play();
        
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
            float size = Random.Range(1f, 1.5f);
            s.transform.localScale *= size;

            s.layer = currentCleaningLayer;

            shampooList.Add(s);
            cleanValue += 1f;

            lastShampooPos = pos;
            hasLastPos = true;
            
            int r = Random.Range(0, 33);
            if (r == 0)
            {
                TriggerShake();
            }
        }
    }

    public void TriggerShake()
    {
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
    }

    public void OnAllCleaned()
    {
        if (currentSheep == null)
        {
            return;
        }

        StartCoroutine(CleanedSequence());
    }
    
    private IEnumerator CleanedSequence()
    {
        canAddShampoo = false;
        canRotateCamera = false;
        
        cleanVFX.Play();
        AudioManager.instance.PlaySound(28);

        yield return StartCoroutine(
            RotateCameraAroundSheep(
                camera.transform.position,
                camera.transform.position + new Vector3(0f, 0.3f, -0.6f),
                1.2f
            )
        );

        yield return new WaitForSeconds(0.3f);
        TriggerShake();
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(SendToDestroy(currentSheep));
    }

    
    private IEnumerator RotateCameraAroundSheep(
        Vector3 startPos,
        Vector3 endPos,
        float duration)
    {
        Transform cam = camera.transform;
        Transform focus = focusCam != null ? focusCam : sheepTarget;

        if (cam == null || focus == null)
            yield break;

        Vector3 center = focus.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            Vector3 dirStart = startPos - center;
            Vector3 dirEnd = endPos - center;

            Vector3 dir = Vector3.Slerp(dirStart, dirEnd, t);
            cam.position = center + dir;

            cam.rotation = Quaternion.LookRotation(center - cam.position);

            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.position = endPos;
        cam.rotation = Quaternion.LookRotation(center - cam.position);
    }
    
    private IEnumerator SendToDestroy(GameObject sheep)
    {
        if (sheep == null)
            yield break;

        sheep.GetComponent<SheepSkinManager>().PlayJumpAnimation();
        yield return new WaitForSeconds(0.3f);

        yield return StartCoroutine(
            MoveOverTime(sheep.transform, destroyPoint.position, 1f)
        );

        Destroy(sheep);
        currentSheep = null;
        sheepTarget = null;

        yield return new WaitForSeconds(0.25f);

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
    
    private void HandleFingerPositionUpdate(Vector2 screenPos)
    {
        currentFingerScreenPos = screenPos;

        if (screenPos == Vector2.zero)
            return;

        imageTool.transform.parent.GetComponent<Animator>().SetBool("Using", true);

        Vector2 offsetPos = screenPos + screenOffset;
        imageTool.transform.parent.position = offsetPos;

        Ray ray = camera.ScreenPointToRay(offsetPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 hitPos = hit.point;

            Vector3 center = cleanPoint.position;
            Vector3 dirFromCenter = hitPos - center;
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
        }
    }


    private void HandleSwipeEnd()
    {
        currentFingerScreenPos = Vector2.zero;
        imageTool.transform.parent.GetComponent<Animator>().SetBool("Using", false);
        audioClean.Stop();
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
