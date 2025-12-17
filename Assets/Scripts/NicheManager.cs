using System;
using System.Collections;
using Unity.Splines.Examples;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NicheManager : TouchableObject
{
    [SerializeField] private Chien chien;
    [SerializeField] private NavMeshAgent agentChien;
    [SerializeField] private Transform nichePos;
    [SerializeField] private bool isInNiche = false;
    [SerializeField] private Transform cameraZoomPos;
    [SerializeField] private GameObject buttonQuit;
    [SerializeField] private InputField dogNameTxt;
    

    public string dogName = "";
    
    private void Start()
    {
        GameManager.instance.startMiniGame += SortirLeChien;
        GameManager.instance.endMiniGame += RentrerLeChien;
        
    }

    public override void TouchEvent()
    {
        base.TouchEvent();
        
        GameManager.instance.ChangeCameraState(CamState.Dog);
        GameManager.instance.ChangeCameraPos(cameraZoomPos.position, cameraZoomPos.rotation.eulerAngles, transform);
        buttonQuit.SetActive(true);
    }

    void OnEnable()
    {
        RentrerLeChien();
    }

    public void InitializeDog(string name)
    {
        dogName = name;
    }
    
    private void OnDisable()
    {
        GameManager.instance.startMiniGame -= SortirLeChien;
        GameManager.instance.endMiniGame -= RentrerLeChien;
    }

    private void SortirLeChien()
    {
        if (SheepBoidManager.instance.nbInstantSheep == 0)
            return;
        
        chien.enabled = true;
        isInNiche = false;
    }

    private void RentrerLeChien()
    {
        chien.enabled = false;
        agentChien.SetDestination(nichePos.position);
        
    }

    private void Update()
    {
        dogNameTxt.text = dogName;
        
        float distance = Vector3.Distance(transform.position, nichePos.position);
        Debug.Log(distance);

        if (distance < 1f && !isInNiche)
        {
            StartCoroutine(RotateSmoothInNiche());
        }
    }

    IEnumerator RotateSmoothInNiche()
    {
        Quaternion startRotation = agentChien.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(
            0f,
            -90,
            0f
        );

        float duration = 0.8f;
        float elapsedTime = 0f;

        agentChien.isStopped = true;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            agentChien.transform.rotation = Quaternion.Slerp(
                startRotation,
                targetRotation,
                t
            );

            yield return null;
        }

        agentChien.transform.rotation = targetRotation;
        isInNiche = true;
    }


    public void ChangeDogName()
    {
        dogName = dogNameTxt.text;
    }
}
