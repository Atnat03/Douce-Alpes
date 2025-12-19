using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CloseCatalogue : MonoBehaviour
{
    public Animator animator;

    public void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => CloseUI());
    }

    private void CloseUI()
    {
        StartCoroutine(CloseEUI());
    }

    IEnumerator CloseEUI()
    {
        animator.SetTrigger("Close");
        
        yield return new WaitForSeconds(0.6f);
        animator.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
