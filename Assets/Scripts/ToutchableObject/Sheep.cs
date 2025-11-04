using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : TouchableObject
{
    [SerializeField] public int sheepId;
    [SerializeField] public string sheepName;

    [SerializeField] public int currentSkinHat;
    [SerializeField] public int currentSkinClothe;
    [SerializeField] public bool hasLaine = true;
    
    [SerializeField] private bool isBeingCaressed = false;
    public bool IsBeingCaressed => isBeingCaressed;
    
    [SerializeField] public bool isOpen = false;
    
    public float elapsedTime { get; set; }
    
    [Header("Double Click Settings")]
    [SerializeField] private float doubleClickThreshold = 0.3f; 
    private float lastClickTime = -1f;

    [Header("References")]
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private ParticleSystem heartParticle;
    [SerializeField] private Transform spawnParticleCaresse;
    [SerializeField] private SkinListManager skinListManager;
    [SerializeField] public GameObject laine;
    
    private SheepBoid sheepBoid;

    private void Start()
    {
        sheepBoid = GetComponent<SheepBoid>();
        laine.GetComponent<Outline>().enabled = false;
    }

    public void Initialize(int id, string name)
    {
        sheepId  = id;
        sheepName = name;
    }

    private void Update()
    {
        laine.SetActive(hasLaine);

        if (isOpen)
        {
            transform.position = transform.localPosition;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    void ChangeName(string newName)
    {
        sheepName = newName;
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
        return cameraPosition.transform.position;
    }

    public void ChangeOutlineState(bool state)
    {
        laine.gameObject.GetComponent<Outline>().enabled = state;
    }

    public void AddCaresse()
    {
        if (GameManager.instance.shopOpen) return;
        
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
        if (GameManager.instance.shopOpen) return;
        
        if (Time.time - lastClickTime <= doubleClickThreshold)
        {
            if (!isBeingCaressed && !sheepBoid.isAfraid)
            {
                WidowOpen();
            }
            else
            {
                Debug.Log("Impossible d’ouvrir la fenêtre : le mouton est en train d’être caressé");
            }
            
            lastClickTime = -1f;
        }
        else
        {
            lastClickTime = Time.time;
            GameManager.instance.LockCamOnSheep(this);
        }
    }

    public void WidowOpen()
    {
        isOpen = true;
        
        if(GameManager.instance.getCurLockSheep() != null)
            GameManager.instance.DelockSheep();

        transform.rotation = Quaternion.Euler(0, 180, 0);
        
        StopAgentAndDesactivateScript(true);
        
        GameManager.instance.ChangeCameraState(CamState.Sheep);
        GameManager.instance.ChangeCameraPos(
            cameraPosition.transform.position,
            cameraPosition.transform.rotation.eulerAngles,
            transform
        );
        
        Camera.main.gameObject.GetComponent<CameraControl>().ResetFOV();

        GameManager.instance.GetSheepWindow().SetActive(true);
        
        SheepWindow.instance.Initialize(sheepName, currentSkinHat, currentSkinClothe, sheepId);
    }

    public void SetCurrentSkinHat(int skinId)
    {
        currentSkinHat = skinId;
        skinListManager.UpdateSkinListHat(currentSkinHat);
    }
    
    public void SetCurrentSkinClothe(int skinId)
    {
        currentSkinClothe = skinId;
        skinListManager.UpdateSkinListClothe(currentSkinClothe);
    }
}

