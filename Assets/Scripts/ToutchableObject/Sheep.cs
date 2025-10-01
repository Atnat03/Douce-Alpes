using System.Collections;
using UnityEngine;

public class Sheep : TouchableObject
{
    [SerializeField] public int sheepId;
    [SerializeField] public string sheepName;

    [SerializeField] public int currentSkin;
    [SerializeField] public bool hasLaine = true;
    
    [SerializeField] private bool isBeingCaressed = false;
    public bool IsBeingCaressed => isBeingCaressed;
    
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
    
    private SheepAI sheepAI;

    private void Start()
    {
        sheepAI = GetComponent<SheepAI>();
        laine.GetComponent<Outline>().enabled = false;
    }

    private void Update()
    {
        laine.SetActive(hasLaine);
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
        sheepAI.StopAgent(state);
        sheepAI.enabled = !state;
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
            if (!isBeingCaressed) 
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
        if(GameManager.instance.getCurLockSheep() != null)
            GameManager.instance.DelockSheep();
        
        GameManager.instance.ChangeCameraState(CamState.Sheep);
        GameManager.instance.ChangeCameraPos(
            cameraPosition.transform.position,
            cameraPosition.transform.rotation.eulerAngles
        );
        
        Camera.main.gameObject.GetComponent<CameraControl>().ResetFOV();
        
        GameManager.instance.GetSheepWindow().SetActive(true);
        StopAgentAndDesactivateScript(true);
        
        SheepWindow.instance.Initialize(sheepName, currentSkin, sheepId);
    }

    public void SetCurrentSkin(int skinId)
    {
        currentSkin = skinId;
        skinListManager.UpdateSkinList(currentSkin);
    }
}

