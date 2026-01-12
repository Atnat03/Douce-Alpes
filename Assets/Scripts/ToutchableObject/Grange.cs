using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Grange : Build
{
    [SerializeField] private GameObject sheepDestroyer;

    private int nbSheepInGrange = 0;
    
    [Header("Gates")]
    public bool gateState = false;
    
    [SerializeField] public GameObject gate1;
    
    [SerializeField] public GameObject gate2;

    [SerializeField] private GameObject keyCloseGate;

    [SerializeField] private Poutre poutre;

    public Transform spawnGetOffTransform;
    public Transform endSpawnGetOffTransform;

    public bool AllSheepAreOutside = true;

    public Transform targetTransiPos;
    public Animator doorAnimator;
    
    public GameObject handZommed;
    public GameObject handNotZommed;
    public bool isGrangeZoomed = false;

    [SerializeField] private Text textGrange;
    [SerializeField] private BoxCollider boxCollider;
    
    [SerializeField] private GameObject interiorButton;
    [SerializeField] private GameObject interiorImageButton;
    [SerializeField] private InteriorSceneManager interiorScene;
    [SerializeField] private GameObject zzzzParticle;
    
    void Start()
    {
        keyCloseGate.SetActive(false); 
        gateState = true;
    }

    private void OnEnable()
    {
        if (gateState)
        {
            doorAnimator.SetTrigger("Open");
        }
        else
        {
            doorAnimator.SetTrigger("Close");
        }
        
                
        if (GameData.instance.timer.currentMiniJeuToDo == MiniGames.Rentree || GameData.instance.timer.currentMiniJeuToDo == MiniGames.Sortie)
        {
            exclamation.SetActive(true);
        }
    }

    public void LaunchMiniGame()
    {
        GameManager.instance.ChangeCameraState(CamState.MiniGame);
        UpdateCameraZoom();
    }

    public void OpenDoors()
    {
        gateState = true;
        keyCloseGate.SetActive(false);
        doorAnimator.ResetTrigger("Close");
        
        doorAnimator.SetTrigger("Open");
        
        if(TutoManager.instance != null)
            TutoManager.instance.MiniJeuGrange();
    }
    
    public void CloseDoors()
    {
        doorAnimator.ResetTrigger("Open");
        
        gateState = false;
        
        doorAnimator.SetTrigger("Close");

        StartCoroutine(CloseDoorsDesactivatePoutre());
        
        if(TutoManager.instance != null)
            TutoManager.instance.MiniJeuSortis();
    }

    IEnumerator CloseDoorsDesactivatePoutre()
    {
        yield return new WaitForSeconds(1f);
        keyCloseGate.SetActive(true);
    }

    public void AddSheepInGrange()
    {
        nbSheepInGrange++;

        if (nbSheepInGrange >= GameData.instance.nbSheep && GameData.instance.nbSheep >0)
        {
            //Camera.main.gameObject.GetComponent<ChangingCamera>().ResetPosition();
        }
    }
    
    private void UpdateCameraZoom()
    {
        if (GameData.instance.isSheepInside)
        {
            isGrangeZoomed = true;
            CloseDoors();
            ZoomCamera();
        }
        else
        {
            handNotZommed.SetActive(true);
            OpenDoors();
            MiniGameCamera();
        }
    }

    private void ZoomCamera()
    {
        GameManager.instance.ChangeCameraPos(
            GameManager.instance.GetMiniGameZoomCamPos().position,
            GameManager.instance.GetMiniGameZoomCamPos().rotation.eulerAngles,
            targetTransiPos,true
        );
    }

    private void MiniGameCamera()
    {
        GameManager.instance.ChangeCameraPos(
            GameManager.instance.GetMiniGameCamPos().position,
            GameManager.instance.GetMiniGameCamPos().rotation.eulerAngles,
            targetTransiPos
        );
    }

    public void DesactivateHand()
    {
        handNotZommed.SetActive(false);
    }

    private void Update()
    {
        if (interiorScene.isTutoInterior)
        {
            interiorImageButton.SetActive(false);
            interiorButton.SetActive(false);
        }
        else
        {
            interiorImageButton.SetActive(true);
            interiorButton.SetActive(true);
        }
        
        bool canShowHandZoomed =
            isGrangeZoomed &&
            poutre.CanSwipe() &&
            !poutre.hasSwipe;

        handZommed.SetActive(canShowHandZoomed);

        
        zzzzParticle.SetActive(GameData.instance.dayMoment.GetCurrentDayMoment() == WeatherManager.DayMoment.Night);
        
        sheepDestroyer.SetActive(GameManager.instance.currentCameraState == CamState.MiniGame && AllSheepAreOutside);
        boxCollider.enabled = GameManager.instance.currentCameraState == CamState.Default;

        if (GameData.instance.timer.currentMiniJeuToDo == MiniGames.Rentree)
            textGrange.text = "Rentrer";
        
        if (GameData.instance.timer.currentMiniJeuToDo == MiniGames.Sortie)
            textGrange.text = "Sortir";
    }
    
    public Poutre GetPoutre() { return poutre; }
    public Transform GetSheepDestroyer() { return sheepDestroyer.transform; }
    
    //Sortie animé
    public void AnimSheepGetOffGrange(GameObject sheep)
    {
        float travelTime = 4f;
        StartCoroutine(SmoothTravelSortie(sheep, endSpawnGetOffTransform.position, travelTime));
    }

    private IEnumerator SmoothTravelSortie(GameObject sheep, Vector3 targetPos, float duration)
    {
        if (sheep == null) yield break;

        Vector3 startPos = sheep.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            t = t * t * (3f - 2f * t);

            sheep.transform.position = Vector3.Lerp(startPos, targetPos, t);
            sheep.transform.LookAt(targetPos); 

            yield return null;
        }

        sheep.transform.position = targetPos;

        SheepBoid boid = sheep.GetComponent<SheepBoid>();
        if (boid != null)
            boid.enabled = true;
    }

    public new void CloseUI()
    {
        UI.GetComponent<Animator>().SetTrigger("Close");
        StartCoroutine(CloseDelay());
    }

    IEnumerator CloseDelay()
    {
        yield return new WaitForSeconds(1f);
        UI.SetActive(false);
    }
}
