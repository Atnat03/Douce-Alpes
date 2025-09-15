using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class Sheep : TouchableObject
{
    [SerializeField] public int sheepId;
    [SerializeField] private string sheepName;

    [SerializeField] public int currentSkin;

    [SerializeField] private float currentCaressesValue;
    [SerializeField] private float maxCaressesValue = 100;
    
    [Header("Timer")]
    [SerializeField] private float holdingTimer;
    private float timer; 
    private bool startTimer = false;

    [Header("References")]
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private GameObject wheelTimerUI;
    [SerializeField] private Image valueWheelTimerImage;
    [SerializeField] private MeshRenderer model;
    [SerializeField] private ParticleSystem heartParticle;
    [SerializeField] private Transform spawnParticleCaresse;
    [SerializeField] private SkinListManager skinListManager;
    
    private SheepAI sheepAI;

    private void Start()
    {
        wheelTimerUI.SetActive(false);
        holdingTimer = TouchManager.instance.holdThreshold;
        sheepAI = GetComponent<SheepAI>();
    }

    private void Update()
    {
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
        
        SheepWindow.instance.GetInputField().onValueChanged.AddListener(ChangeName);
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

    public void AddCaresse()
    {
        currentCaressesValue += 1f;
        
        heartParticle.Play();
        
        GameManager.instance.Caresse(this);
    }

    public override void TouchEvent()
    {
        print(gameObject.name + " touched");
    }

    public void StartHolding()
    {
        if(GameManager.instance.currentCameraState != CamState.Default)
            return;
        
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
        GameManager.instance.ChangeCameraState(CamState.Sheep);
        GameManager.instance.ChangeCameraPos(
            cameraPosition.transform.position,
            cameraPosition.transform.rotation.eulerAngles
        );
        
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
