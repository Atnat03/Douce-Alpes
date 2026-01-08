using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HappyExplosion : MonoBehaviour
{
    [SerializeField] private GameObject prefabHeart;
    [SerializeField] private int numberHeart;

    public void PerformParticle()
    {
        StartCoroutine(AnimatedExplosion());
    }

    IEnumerator AnimatedExplosion()
    {
        List<GameObject> heartsList = new List<GameObject>();
        List<Vector3> directions = new List<Vector3>();

        float elapsedTime = 0f;
        float duration = 1f;
        float explosionDistance = 100f;
        GetComponent<CanvasGroup>().alpha = 1f;

        for (int i = 0; i < numberHeart; i++)
        {
            GameObject newHeart = Instantiate(prefabHeart, transform);

            float randomZ = Random.Range(0f, 360f);
            newHeart.transform.rotation = Quaternion.Euler(0f, 0f, randomZ);

            float angle = Random.Range(0f, 360f);
            Vector3 dir = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad),
                0f
            );

            heartsList.Add(newHeart);
            directions.Add(dir.normalized);
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);

            for (int i = 0; i < heartsList.Count; i++)
            {
                heartsList[i].transform.localPosition =
                    directions[i] * Mathf.Lerp(0f, explosionDistance, t);
                
                GetComponent<CanvasGroup>().alpha = 1f - t;
            }

            yield return null;
        }
    }

}
