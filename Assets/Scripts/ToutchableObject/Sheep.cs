using System;
using System.Collections;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Sheep : TouchableObject
{
    [SerializeField] public int sheepId;
    [SerializeField] public string sheepName;

    [SerializeField] public int currentSkinHat;
    [SerializeField] public int currentSkinClothe;
    [SerializeField] public int currentColorID;
    
    [SerializeField] public string birthDate;
    
    [SerializeField] public bool hasLaine = true;
    [SerializeField] public float processWool;

    [SerializeField] public float curPuanteur = 0;

    [SerializeField] private bool isBeingCaressed = false;
    public bool IsBeingCaressed => isBeingCaressed;

    [SerializeField] public bool isOpen = false;

    private Vector3 lockedPosition;
    private Quaternion lockedRotation;

    [Header("Double Click Settings")]
    [SerializeField] private float doubleClickThreshold = 0.3f; 
    private float lastClickTime = -1f;

    [Header("References")]
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private ParticleSystem heartParticle;
    [SerializeField] private SkinListManager skinListManager;
    [SerializeField] public GameObject laine;
    [SerializeField] public GameObject laineDessous;

    public SheepBoid sheepBoid;
    
    [SerializeField] private Sprite showerLogo;
    [SerializeField] private Sprite zzzzzzLogo;
    
    public Transform targetTransiPos;

    [SerializeField] private Text nameText;
    [SerializeField] public bool isFocusing = false;

    [SerializeField] private ColorSO colorData;
    [SerializeField] private GameObject larmes;
    
    [Header("Bulle")]
    [SerializeField] private GameObject Bubble;
    [SerializeField] private Image ImageInBubble;
    [SerializeField] private Sprite wantToGoIn;
    [SerializeField] private Sprite wantToDrink;
    
    [Header("Swipe Rotation")]
    [SerializeField] private float rotationStep = 30f;
    private int rotationIndex = 0;
    
    [Header("Swipe Zone (Screen %)")]
    [SerializeField, Range(0f, 1f)]
    private float swipeMinHeightPercent = 0.3f;

    [SerializeField, Range(0f, 1f)]
    private float swipeMaxHeightPercent = 0.6f;

    private bool swipeStartedInValidZone = false;

    [Header("Smooth Rotation")]
    [SerializeField] private float rotationDuration = 0.25f;

    private Coroutine rotationCoroutine;

    private float timerSound = 0;

    private void OnEnable()
    {
        if (SwipeDetection.instance != null)
        {
            SwipeDetection.instance.OnSwipeDetected += OnSwipeDetected;
            SwipeDetection.instance.OnFingerPositionUpdated += OnFingerPositionUpdated;
        }
    }

    private void OnDisable()
    {
        if (SwipeDetection.instance != null)
        {
            SwipeDetection.instance.OnSwipeDetected -= OnSwipeDetected;
            SwipeDetection.instance.OnFingerPositionUpdated -= OnFingerPositionUpdated;
        }
    }

    
    private void Start()
    {
        laine.GetComponent<Outline>().enabled = false;

        processWool = Random.Range(50, 100);
        curPuanteur = Random.Range(0, 50);
    }

    public void Initialize(int id, string name)
    {
        sheepId  = id;
        sheepName = name;

        skinListManager.Initialize();
        SetCurrentSkinClothe(10);
        
        if(name == "Seb")
            SetCurrentSkinHat(0);
        else if (name == "Dinnerbone" || name == "Grumm")
        {
            Transform model = laine.transform.parent;
            print(model);
            model.rotation = Quaternion.Euler(180, -90, 0);
            model.position += new Vector3(0, 1f, 0);
            GetComponent<Animator>().enabled = false;
        }else if (name == "Jeb_")
        {
            currentColorID = 6;
        }
        else
            SetCurrentSkinHat(13);
    }

    private void Update()
    {
        laine.SetActive(hasLaine);
        
        laine.GetComponent<MeshRenderer>().material = colorData.colorData[currentColorID].material;
        var mats = laineDessous.GetComponent<MeshRenderer>().materials;
        mats[1] = colorData.colorData[currentColorID].material;
        laineDessous.GetComponent<MeshRenderer>().materials = mats;

        larmes.SetActive(BonheurCalculator.instance.currentBonheur <= 10);
        
        if (isOpen)
        {
            sheepBoid.enabled = false;

            transform.position = lockedPosition;
        }
        else
        {
            sheepBoid.enabled = true;
        }

        if (!hasLaine)
        {
            ProcessWool();
        }

        if (curPuanteur < 100)
        {
            curPuanteur += 2 * Time.deltaTime;
        }
        else
        {
            curPuanteur = 100;
        }
        
        nameText.text = sheepName;
        nameText.gameObject.SetActive(isFocusing);

        Bubble.transform.parent.GetComponent<CanvasGroup>().alpha = isOpen ? 0f : 1f;

        if (timerSound <= 0)
        {
            AudioManager.instance.PlaySound(Random.Range(17,20), 1f, 0.1f);
            timerSound = Random.Range(5, 30);
        }
        else
        {
            timerSound -= Time.deltaTime;
        }
    }
    
    private void OnSwipeDetected(SwipeType swipe)
    {
        if (!isOpen)
            return;

        if (!swipeStartedInValidZone)
            return;

        if (GameManager.instance.getCurLockSheep() != this)
            return;

        if (swipe == SwipeType.Left)
            rotationIndex--;
        else if (swipe == SwipeType.Right)
            rotationIndex++;
        else
            return;

        rotationIndex = Mathf.Clamp(rotationIndex, -1, 1);

        float targetY = 120f + rotationIndex * rotationStep;
        lockedRotation = Quaternion.Euler(0, targetY, 0);

        if (rotationCoroutine != null)
            StopCoroutine(rotationCoroutine);

        rotationCoroutine = StartCoroutine(SmoothRotateTo(lockedRotation));
    }

    private IEnumerator SmoothRotateTo(Quaternion targetRotation)
    {
        Quaternion startRotation = transform.rotation;
        float elapsed = 0f;

        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / rotationDuration;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    
    private bool swipeZoneInitialized = false;

    private void OnFingerPositionUpdated(Vector2 pos)
    {
        if (!isOpen)
            return;

        if (pos == Vector2.zero)
        {
            swipeZoneInitialized = false;
            swipeStartedInValidZone = false;
            return;
        }

        if (swipeZoneInitialized)
            return;

        swipeZoneInitialized = true;

        float minY = Screen.height * swipeMinHeightPercent;
        float maxY = Screen.height * swipeMaxHeightPercent;

        swipeStartedInValidZone = pos.y >= minY && pos.y <= maxY;
    }

    private void ProcessWool()
    {
        processWool -= 2 * Time.deltaTime;
        
        if (processWool <= 0)
        {
            hasLaine = true;
            processWool = Random.Range(50, 100);
            
            GetComponent<Animator>().SetTrigger("WoolPop");
            
            AudioManager.instance.PlaySound(23);
            
            SetCurrentSkinClothe(currentSkinClothe);
            SetCurrentSkinHat(currentSkinHat);
            
            GameManager.instance.UpdateGrangeAvailability();
        }
    }

    public void ResetPuanteur()
    {
        curPuanteur = Random.Range(0, 50);
    }

    public void CutWhool()
    {
        hasLaine = false;
    }

    public void StopAgentAndDesactivateScript(bool state)
    {
        GetComponent<SheepBoid>().enabled = !state;
    }

    public Vector3 GetCameraPosition()
    {
        return cameraPosition.position;
    }

    public void ChangeOutlineState(bool state)
    {
        laine.GetComponent<Outline>().enabled = state;
    }

    public void AddCaresse()
    {
        if (GameManager.instance.shopOpen) return;
        if (isOpen) return;

        if (GameManager.instance.currentCameraState != CamState.Default)
            return;

        isBeingCaressed = true;
        heartParticle.Play();
        GameManager.instance.Caresse(this);
        
        AudioManager.instance.PlaySound(6, Random.Range(0.9f, 1.1f));

        CancelInvoke(nameof(StopCaresse));
        Invoke(nameof(StopCaresse), 0.2f);
    }

    private void StopCaresse()
    {
        isBeingCaressed = false;
    }

    private bool isSingleClickCoroutineRunning = false;

    public override void TouchEvent()
    {
        if (GameManager.instance.currentCameraState != CamState.Default) return;
        if (GameManager.instance.shopOpen) return;

        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick <= doubleClickThreshold)
        {
            // Double clic détecté
            lastClickTime = -1f;

            if (!isBeingCaressed && !sheepBoid.isAfraid)
            {
                WidowOpen();
            }
            
            isSingleClickCoroutineRunning = false;

            return;
        }

        // C'est le premier clic
        lastClickTime = Time.time;

        if (!isSingleClickCoroutineRunning)
            StartCoroutine(SingleClickDelay());
    }

    private IEnumerator SingleClickDelay()
    {
        float elapsed = 0f;
        while (elapsed < doubleClickThreshold)
        {
            elapsed += Time.deltaTime;

            // Si un deuxième clic est détecté, on annule le simple clic
            if (lastClickTime < 0f)
                yield break;

            yield return null;
        }

        // Simple clic : lock caméra sur le mouton
        if (GameManager.instance.getCurLockSheep() != this)
            GameManager.instance.LockCamOnSheep(this);

        lastClickTime = -1f;
        isSingleClickCoroutineRunning = false;
    }
    
    public void WidowOpen()
    {
        isOpen = true;
        
        laine.GetComponent<Outline>().enabled = false;
        
        rotationIndex = 0;
        lockedPosition = transform.position;
        lockedRotation = Quaternion.Euler(0, 120, 0);
        
        swipeStartedInValidZone = false;
        swipeZoneInitialized = false;

        transform.position = lockedPosition;
        transform.rotation = lockedRotation;

        if (GameManager.instance.getCurLockSheep() != this)
            GameManager.instance.LockCamOnSheep(this);

        GameManager.instance.ChangeCameraState(CamState.Sheep);
        GameManager.instance.ChangeCameraPos(
            cameraPosition.position,
            cameraPosition.rotation.eulerAngles,
            targetTransiPos
        );

        Camera.main.GetComponent<CameraControl>().ResetFOV();
        
        GameManager.instance.GetSheepWindow().SetActive(true);
        SheepWindow.instance.Initialize(sheepName, currentSkinHat, currentSkinClothe, sheepId, birthDate);
    }

    public void SetCurrentSkinHat(int skinId)
    {
        currentSkinHat = skinId;
        skinListManager.UpdateSkinListHat(currentSkinHat);
        UpdateCombo();
    }

    public void SetCurrentSkinClothe(int skinId)
    {
        currentSkinClothe = skinId;
        skinListManager.UpdateSkinListClothe(currentSkinClothe);
        UpdateCombo();
    }

    private void UpdateCombo()
    {
        CheckCombo(currentSkinHat, currentSkinClothe);
    }

    void CheckCombo(int hatId, int clothId)
    {
        if (skinListManager.HasCombo(hatId, clothId))
        {
            sheepBoid.SetNature(skinListManager.GetNatureFromCombo(hatId));
        }
        else
        {
            sheepBoid.SetNature(sheepBoid.natureBase);
        }
    }

    public void SetNewWoolColor(int idColor)
    {
        currentColorID = idColor;
    }

    public void DisableBubble()
    {
        if (!Bubble.activeSelf)
            return;
        
        Bubble.SetActive(false);
    }

    public void ActivatedBubble(bool isDrink)
    {
        Debug.Log("ActivatedBubble : " + isDrink);
        
        if (Bubble.activeSelf)
            return;
        
        Bubble.SetActive(true);

        Action a = isDrink
            ? Camera.main.GetComponent<CameraControl>().SetRootFocusAbreuvoir
            : Camera.main.GetComponent<CameraControl>().SetRootFocusGrange;

        Sprite s = isDrink ? wantToDrink : wantToGoIn;
        
        Bubble.GetComponent<Button>().onClick.AddListener(() => a());
        Bubble.GetComponent<Button>().onClick.AddListener(DisableBubble);
        ImageInBubble.sprite = s;
    }
    
    public bool HasActiveBubble()
    {
        return Bubble.activeSelf;
    }
}
