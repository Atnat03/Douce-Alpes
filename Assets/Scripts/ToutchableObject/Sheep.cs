using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class Sheep : TouchableObject
{
    [SerializeField] public int sheepId;
    [SerializeField] private string sheepName;

    [SerializeField] private int currentSkin;

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
        
        GameManager.instance.Caresse();
    }

    public override void TouchEvent()
    {
        print(gameObject.name + " touched");
    }

    public void StartHolding()
    {
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

    public void SetCurrentSkin(int skinId, Material newObj)
    {
        currentSkin = skinId;
        model.material = newObj;
    }
}
