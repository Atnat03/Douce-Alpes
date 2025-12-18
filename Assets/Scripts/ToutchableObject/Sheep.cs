using System;
using System.Collections;
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
    [SerializeField] private GameObject puanteurVFX;
    
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
            transform.rotation = lockedRotation;
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
            puanteurVFX.SetActive(false);
        }
        else
        {
            curPuanteur = 100;
            puanteurVFX.SetActive(true);
        }
        
        nameText.text = sheepName;
        nameText.gameObject.SetActive(isFocusing);

        Bubble.transform.parent.GetComponent<CanvasGroup>().alpha = isOpen ? 0f : 1f;
    }

    private void ProcessWool()
    {
        processWool -= 2 * Time.deltaTime;
        
        if (processWool <= 0)
        {
            hasLaine = true;
            processWool = Random.Range(50, 100);
            
            GetComponent<Animator>().SetTrigger("WoolPop");
            
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

        isBeingCaressed = true;
        heartParticle.Play();
        GameManager.instance.Caresse(this);

        CancelInvoke(nameof(StopCaresse));
        Invoke(nameof(StopCaresse), 0.2f);
    }

    private void StopCaresse()
    {
        isBeingCaressed = false;
    }

    public override void TouchEvent()
    {
        if (GameManager.instance.currentCameraState != CamState.Default)
            return;
        
        if (GameManager.instance.shopOpen) return;

        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick <= doubleClickThreshold)
        {
            if (!isBeingCaressed && !sheepBoid.isAfraid)
            {
                if (GameManager.instance.currentCameraState != CamState.Default) return;
                
                WidowOpen();
            }

            lastClickTime = -1f;
            return;
        }

        lastClickTime = Time.time;

        if (GameManager.instance.getCurLockSheep() != this)
            StartCoroutine(LockCamWithDelay());
    }

    private IEnumerator LockCamWithDelay()
    {
        float startTime = Time.time;

        while (Time.time - startTime < doubleClickThreshold)
        {
            if (lastClickTime < 0f) yield break;
            yield return null;
        }

        if (GameManager.instance.getCurLockSheep() != this)
            GameManager.instance.LockCamOnSheep(this);

        lastClickTime = -1f;
    }

    public void WidowOpen()
    {
        isOpen = true;
        
        laine.GetComponent<Outline>().enabled = false;

        lockedPosition = transform.position;
        lockedRotation = Quaternion.Euler(0, 120, 0);

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
        SheepWindow.instance.Initialize(sheepName, currentSkinHat, currentSkinClothe, sheepId);
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
