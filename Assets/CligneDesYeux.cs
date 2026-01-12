using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CligneDesYeux : MonoBehaviour
{
    public GameObject[] yeux;
    private float t;

    void ActivateClinDoeil(bool state)
    {
        yeux[0].SetActive(!state);
        yeux[1].SetActive(state);
    }
    
    private void Start()
    {
        ActivateClinDoeil(false);
        t = Random.Range(2f, 7f);
    }

    private void Update()
    {
        if (t <= 0)
        {
            StartCoroutine(Cligne());
            t = Random.Range(2f, 7f);
        }
        else
        {
            t -= Time.deltaTime;
        }
    }

    IEnumerator Cligne()
    {
        ActivateClinDoeil(true);
        yield return new WaitForSeconds(0.5f);
        ActivateClinDoeil(false);
    }
}
