using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Poutre : MonoBehaviour
{
    [SerializeField] private Cadenas[] cadenas;
    [SerializeField] SwipeDetection swipeDetection;
    [SerializeField] private Grange grange;
    
    [SerializeField] private ParticleSystem touchGroundEffect;
    
    Vector3 Startpos;
    Quaternion startRotation;
    bool hasSwipe = false;

    private void Start()
    {
        Startpos = transform.position;
        startRotation = transform.rotation;
        GetComponent<Animator>().enabled = false;
    }

    private void Update()
    {
        if(canSwipe() && !hasSwipe)
        {
            GetComponent<Animator>().enabled = true;
            grange.hand.SetActive(true);
        }

    }

    public void ResetPoutre()
    {
        foreach (Cadenas cadena in cadenas)
        {
            cadena.transform.gameObject.SetActive(true);
            cadena.hp = cadena.maxHp[GameData.instance.dicoAmélioration[TypeAmelioration.Sortie].Item2];
            cadena.ResetCadenas();
        }

        if (gameObject.GetComponent<Rigidbody>())
        {
            Destroy(gameObject.GetComponent<Rigidbody>());
        }

        gameObject.transform.position = Startpos;
        gameObject.transform.rotation = startRotation;

        hasSwipe = false;
    }

    public async Task GetOffPoutre(SwipeType swipe)
    {
        if(swipe != SwipeType.Up) return;
        
        if (canSwipe())
        {
            hasSwipe = true;
            
            GetComponent<Animator>().enabled = false;
            
            await Task.Yield();
            
            gameObject.AddComponent<Rigidbody>();
            GetComponent<Rigidbody>().AddForce(Vector3.up * 500, ForceMode.Impulse);
            
            StartCoroutine(WaitALittle());
        }
    }

    IEnumerator WaitALittle()
    {
        yield return new WaitForSeconds(2f);
                    
        grange.OpenDoors();
        GameManager.instance.SheepGetOutGrange();
    }

    bool canSwipe()
    {
        foreach (Cadenas cadena in cadenas)
        {
            if (cadena.gameObject.activeSelf)
                return false;
        }
        return true;
    }
    
    void OnEnable()
    {
        if(SwipeDetection.instance != null)
            SwipeDetection.instance.OnSwipeDetected += OnSwipe;
    }
    void OnDisable() { SwipeDetection.instance.OnSwipeDetected -= OnSwipe; }
    
    void OnSwipe(SwipeType swipe)
    {
        _ = GetOffPoutre(swipe);
    }
    
    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            ContactPoint contact = other.contacts[0];
            Vector3 pointDeCollision = contact.point;
            
            touchGroundEffect.transform.position = pointDeCollision;
            
            touchGroundEffect.Play();
        }
    }
}
