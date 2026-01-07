using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class FrogClignement : MonoBehaviour
{
    [SerializeField] Material materialWhite;
    [SerializeField] private Material materialBlack;
    [SerializeField] private MeshRenderer meshRenderer;
    private float t = 0;
    
    void Update()
    {
        if (t <= 0)
        {
            StartCoroutine(Clignement());
            t = Random.Range(0.75f, 4f);
        }
        else
        {
            t -= Time.deltaTime;
        }
    }
    IEnumerator Clignement()
    {
        Material[] mats = meshRenderer.materials;

        mats[1] = materialWhite;
        meshRenderer.materials = mats;

        yield return new WaitForSeconds(0.15f);

        mats[1] = materialBlack;
        meshRenderer.materials = mats;
    }

}
