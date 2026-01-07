using System;
using System.Collections;
using UnityEngine;

public class Cadenas : TouchableObject
{
    public int hp;
    public int[] maxHp;

    [SerializeField] private ParticleSystem touchEffect;
    [SerializeField] private ParticleSystem unlockEffect;
    
    Vector3 pos;
    private Quaternion rot;
    
    [SerializeField] Animator animator;
    [SerializeField] Collider colModel;
    [SerializeField] Collider colButton;
    
    bool fallCadenas = false;

    private void Start()
    {
        hp = maxHp[0];
        
        pos = transform.position;
        rot = transform.rotation;
        
        colModel.enabled = false;
        colButton.enabled = true;
    }

    private void Update()
    {
        if(!fallCadenas)
            animator.enabled = GameManager.instance.currentCameraState == CamState.MiniGame;
        else
        {
            animator.enabled = false;
        }
    }

    public override void TouchEvent()
    {
        if (GameManager.instance.currentCameraState != CamState.MiniGame) return;
        
        Debug.Log("Perte d'hp");
        
        touchEffect.Play();
        AudioManager.instance.PlaySound(4, 1f, 0.3f);
        
        hp--;

        if (hp == 0)
        {
            unlockEffect.Play();

            gameObject.AddComponent<Rigidbody>();
            colModel.enabled = true;
            colButton.enabled = false;
            fallCadenas = true;
            
            StartCoroutine(WaitBeforeDesactivate());
        }
    }

    IEnumerator WaitBeforeDesactivate()
    {
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
        Destroy(GetComponent<Rigidbody>());
        
        colModel.enabled = false;
        colButton.enabled = true;
        fallCadenas = false;
    }

    public void ResetCadenas()
    {
        transform.rotation = rot;
        transform.position = new Vector3(pos.x, -0.3f, pos.z);
    }
}
