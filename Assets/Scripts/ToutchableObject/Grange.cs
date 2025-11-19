using System;
using System.Collections;
using UnityEngine;

public class Grange : Build
{
    [SerializeField] private GameObject sheepDestroyer;

    private int nbSheepInGrange = 0;
    
    [Header("Gates")]
    public bool gateState = false;
    
    [SerializeField] private Vector3 gate1_Close, gate1_Open;
    [SerializeField] private GameObject gate1;
    
    [SerializeField] private Vector3 gate2_Close, gate2_Open;
    [SerializeField] private GameObject gate2;

    [SerializeField] private GameObject keyCloseGate;

    [SerializeField] private Poutre poutre;

    public Transform spawnGetOffTransform;
    public Transform endSpawnGetOffTransform;

    public bool AllSheepAreOutside = true;

    public GameObject uiFlecheClick;

    void Start()
    {
        OpenDoors();
    }

    public override void TouchEvent()
    {
        base.TouchEvent();
        uiFlecheClick.SetActive(false);
    }

    public void LaunchMiniGame()
    {
        GameManager.instance.ChangeCameraState(CamState.MiniGame);
        DesactivateUI();
        OpenDoors();
        UpdateCameraZoom();
    }

    public void OpenDoors()
    {
        gate1.transform.rotation = Quaternion.Euler(gate1_Open);
        gate2.transform.rotation = Quaternion.Euler(gate2_Open);
        gateState = true;
        keyCloseGate.SetActive(false);
    }
    
    public void CloseDoors()
    {
        gate1.transform.rotation = Quaternion.Euler(gate1_Close);
        gate2.transform.rotation = Quaternion.Euler(gate2_Close);
        gateState = false;
        keyCloseGate.SetActive(true);
    }

    public void AddSheepInGrange()
    {
        nbSheepInGrange++;

        if (nbSheepInGrange >= GameData.instance.nbSheep && GameData.instance.nbSheep >0)
        {
            Camera.main.gameObject.GetComponent<ChangingCamera>().ResetPosition();
            CloseDoors();
        }
    }
    
    private void UpdateCameraZoom()
    {
        int totalSheep = GameData.instance.nbSheep;
        Debug.Log("Total sheep: " + totalSheep);

        if (nbSheepInGrange >= totalSheep && totalSheep > 0)
        {
            CloseDoors();
            ZoomCamera();
        }
        else
        {
            OpenDoors();
            MiniGameCamera();
        }
    }

    private void ZoomCamera()
    {
        GameManager.instance.ChangeCameraPos(
            GameManager.instance.GetMiniGameZoomCamPos().position,
            GameManager.instance.GetMiniGameZoomCamPos().rotation.eulerAngles,
            transform,true
        );
    }

    private void MiniGameCamera()
    {
        GameManager.instance.ChangeCameraPos(
            GameManager.instance.GetMiniGameCamPos().position,
            GameManager.instance.GetMiniGameCamPos().rotation.eulerAngles,
            transform
        );
    }

    private void Update()
    {
        sheepDestroyer.SetActive(GameManager.instance.currentCameraState == CamState.MiniGame && AllSheepAreOutside);
    }
    
    public Poutre GetPoutre() { return poutre; }
    public Transform GetSheepDestroyer() { return sheepDestroyer.transform; }
    
    //Sortie animé
    public void AnimSheepGetOffGrange(GameObject sheep)
    {
        float travelTime = 2f;
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


}
