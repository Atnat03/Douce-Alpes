using System;
using UnityEngine;
using UnityEngine.AI;

public class NicheManager : TouchableObject
{
    [SerializeField] private Chien chien;
    [SerializeField] private Transform nichePos;
    [SerializeField] private bool isInNiche = false;
    [SerializeField] private Transform cameraZoomPos;
    [SerializeField] private GameObject buttonQuit;
    Vector3 startPos;
    Vector3 startRot;

    public string dogName = "";
    
    private void Start()
    {
        GameManager.instance.startMiniGame += SortirLeChien;
        GameManager.instance.endMiniGame += RentrerLeChien;
        
        startPos = chien.transform.position;
        startRot = chien.transform.rotation.eulerAngles;

        chien.enabled = false;
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
        chien.gameObject.GetComponent<NavMeshAgent>().SetDestination(startPos);
    }

    private void Update()
    {
        isInNiche = Vector3.Distance(chien.transform.position, startPos) < 0.2f;

        if (isInNiche)
            chien.transform.rotation = Quaternion.Euler(startRot);
    }
}
