using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NicheManager : TouchableObject
{
    [Header("Références")]
    [SerializeField] private Chien chien;
    [SerializeField] private NavMeshAgent agentChien;
    [SerializeField] private Transform nichePos;
    [SerializeField] private Transform cameraZoomPos;
    [SerializeField] private GameObject buttonQuit;
    [SerializeField] private InputField dogNameInput;
    [SerializeField] private GameObject createSheepUI;
    [SerializeField] private GameObject colorChange;

    [Header("État du chien")]
    [SerializeField] private bool isInNiche = false;
    public string dogName = "";

    private void Start()
    {
        GameManager.instance.startMiniGame += SortirLeChien;
        GameManager.instance.endMiniGame += RentrerLeChien;

        dogNameInput.text = dogName;
    }

    private void OnEnable()
    {
        RentrerLeChien();
    }

    private void OnDisable()
    {
        GameManager.instance.startMiniGame -= SortirLeChien;
        GameManager.instance.endMiniGame -= RentrerLeChien;
    }

    public override void TouchEvent()
    {
        if (createSheepUI.activeInHierarchy) return;
        
        if (GameManager.instance.currentCameraState != CamState.Default)
            return;

        GameManager.instance.ChangeCameraState(CamState.Dog);
        GameManager.instance.ChangeCameraPos(
            cameraZoomPos.position,
            cameraZoomPos.rotation.eulerAngles,
            transform
        );

        buttonQuit.SetActive(true);
    }

    private void SortirLeChien()
    {
        if (SheepBoidManager.instance.nbInstantSheep == 0)
            return;

        chien.SetMiniGameActive(true);
        isInNiche = false;
    }

    private void RentrerLeChien()
    {
        chien.SetMiniGameActive(false);
        agentChien.isStopped = false;
        agentChien.SetDestination(nichePos.position);
        isInNiche = false;
    }

    private void Update()
    {
        dogNameInput.enabled = GameManager.instance.currentCameraState == CamState.Dog;
        colorChange.SetActive(GameManager.instance.currentCameraState == CamState.Dog);
        
        float distanceToNiche = Vector3.Distance(agentChien.transform.position, nichePos.position);

        if (distanceToNiche < 1f && !isInNiche)
        {
            isInNiche = true;
            StartCoroutine(RotateSmoothInNiche());
        }
    }

    private IEnumerator RotateSmoothInNiche()
    {
        Quaternion startRotation = agentChien.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0f, 116f, 0f);
        float duration = 0.8f;
        float elapsedTime = 0f;

        agentChien.isStopped = true;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            agentChien.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        agentChien.transform.rotation = targetRotation;
        agentChien.isStopped = false;
    }

    public void ChangeDogName()
    {
        dogName = dogNameInput.text;
    }
}
