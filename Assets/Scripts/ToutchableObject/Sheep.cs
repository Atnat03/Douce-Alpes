using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class Sheep : TouchableObject
{
    [SerializeField] public int sheepId;
    [SerializeField] private string sheepName;

    [SerializeField] public int currentSkin;

    [SerializeField] private float maxCaressesValue = 100;

    [SerializeField] public bool hasLaine = true;
    
    [SerializeField] private bool isBeingCaressed = false;
    public bool IsBeingCaressed => isBeingCaressed;
    
    [Header("Timer")]
    [SerializeField] private float holdingTimer;
    private float timer; 
    private bool startTimer = false;

    [Header("References")]
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private GameObject wheelTimerUI;
    [SerializeField] private Image valueWheelTimerImage;
    [SerializeField] private ParticleSystem heartParticle;
    [SerializeField] private Transform spawnParticleCaresse;
    [SerializeField] private SkinListManager skinListManager;
    [SerializeField] public GameObject laine;
    
    private SheepAI sheepAI;

    private void Start()
    {
        wheelTimerUI.SetActive(false);
        holdingTimer = TouchManager.instance.holdThreshold;
        sheepAI = GetComponent<SheepAI>();
        
        laine.GetComponent<Outline>().enabled = false;
    }

    private void Update()
    {
        laine.SetActive(hasLaine);
        
        if (startTimer)
        {
            timer += Time.deltaTime;
            valueWheelTimerImage.fillAmount = timer / holdingTimer;

            if (timer >= holdingTimer)
            {
                wheelTimerUI.SetActive(false);
                startTimer = false;
                WidowOpen();
            }
        }
        
        //SheepWindow.instance.GetInputField().onValueChanged.AddListener(ChangeName);
    }

    void ChangeName(string newName)
    {
        sheepName = newName;
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
        GameManager.instance.LockCamOnSheep(this);
    }

    public void StartHolding()
    {
        if(GameManager.instance.currentCameraState != CamState.Default)
            return;
        
        if (isBeingCaressed) 
        {
            Debug.Log("Impossible d’ouvrir la fenêtre : le mouton est en train d’être caressé");
            return;
        }
        
        timer = 0f;
        startTimer = true;
        wheelTimerUI.SetActive(true);
        valueWheelTimerImage.fillAmount = 0f;
        StopAgentAndDesactivateScript(true);
    }

    public void CancelHolding()
    {
        startTimer = false;
        wheelTimerUI.SetActive(false);
        
        if(!SheepWindow.instance.isOpen)
            StopAgentAndDesactivateScript(false);
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

    public void OnTouchEnd()
    {
        CancelHolding();
    }

    public void SetCurrentSkin(int skinId)
    {
        currentSkin = skinId;
        skinListManager.UpdateSkinList(currentSkin);
    }
}
