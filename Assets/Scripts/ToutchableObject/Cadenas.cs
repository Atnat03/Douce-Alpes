using System;
using System.Collections;
using UnityEngine;

public class Cadenas : TouchableObject
{
    public int hp = 2;
    [HideInInspector]public int maxHp;

    [SerializeField] private ParticleSystem touchEffect;
    [SerializeField] private ParticleSystem unlockEffect;
    [SerializeField] private ParticleSystem touchGroundEffect;
    
    Vector3 pos;
    private Vector3 rot;

    private void Start()
    {
        maxHp = hp;
        
        pos = transform.position;
        rot = transform.rotation.eulerAngles;
    }

    public override void TouchEvent()
    {
        if (GameManager.instance.currentCameraState != CamState.MiniGame) return;
        
        Debug.Log("Perte d'hp");
        
        touchEffect.Play();
        
        hp--;

        if (hp == 0)
        {
            unlockEffect.Play();

            gameObject.AddComponent<Rigidbody>();
            
            StartCoroutine(WaitBeforeDesactivate());
        }
    }

    IEnumerator WaitBeforeDesactivate()
    {
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
        Destroy(GetComponent<Rigidbody>());

        transform.position = pos;
        rot = transform.rotation.eulerAngles;
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
