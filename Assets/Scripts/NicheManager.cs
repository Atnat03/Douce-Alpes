using System;
using UnityEngine;
using UnityEngine.AI;

public class NicheManager : TouchableObject
{
    [SerializeField] private Chien chien;
    [SerializeField] private Transform nichePos;
    [SerializeField] private bool isInNiche = false;
    [SerializeField] private Transform cameraZoomPos;

    private void Start()
    {
        GameManager.instance.startMiniGame += SortirLeChien;
        GameManager.instance.endMiniGame += RentrerLeChien;
        
        RentrerLeChien();
    }
    
    private void OnDisable()
    {
        GameManager.instance.startMiniGame -= SortirLeChien;
        GameManager.instance.endMiniGame -= RentrerLeChien;
    }

    private void SortirLeChien()
    {
        chien.enabled = true;
        isInNiche = false;
    }

    private void RentrerLeChien()
    {
        chien.enabled = false;
        chien.gameObject.GetComponent<NavMeshAgent>().SetDestination(nichePos.position);
    }

    private void Update()
    {
        isInNiche = Vector3.Distance(chien.transform.position, nichePos.position) < 0.2f;
        
        if(isInNiche)
            chien.transform.LookAt(new Vector3(0, -90, 0));
    }

    public override void TouchEvent()
    {
        GameManager.instance.ChangeCameraPos(cameraZoomPos.position, cameraZoomPos.rotation.eulerAngles, transform);
        GameManager.instance.ChangeCameraState(CamState.Dog);
    }
}
