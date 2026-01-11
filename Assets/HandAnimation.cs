using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HandAnimation : MonoBehaviour
{
    [SerializeField] private Image handImage;

    [SerializeField] private Vector2[] targets;
    private Coroutine animationCoroutine;

	public bool isAlphaDecrese = true;
    public float duration = 1.5f;
    
    void OnEnable()
    {
        TouchManager.instance.OnStartEvent += OnFingerPressed;
        animationCoroutine = StartCoroutine(PlayAnimation());
    }

    private void OnDisable()
    {
        TouchManager.instance.OnStartEvent -= OnFingerPressed;
        handImage.gameObject.SetActive(false);
    }

    private void OnFingerPressed(Vector2 position, float timer)
    {
        if (handImage.gameObject.activeSelf)
        {
            handImage.gameObject.SetActive(false);
        }
    }

    IEnumerator PlayAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        
        handImage.gameObject.SetActive(true);

        while (true)
        {
            for (int n = 0; n < targets.Length - 1; n++)
            {
                float t = 0f;
                
                handImage.rectTransform.anchoredPosition = targets[n];

				if(isAlphaDecrese)
                	handImage.GetComponent<CanvasGroup>().alpha = 1f;

                while (t < duration)
                {
                    float normalizedT = t / duration;

                    handImage.rectTransform.anchoredPosition =
                        Vector2.Lerp(targets[n], targets[n + 1], normalizedT);

                    handImage.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f, 0f, normalizedT);

                    t += Time.deltaTime;
                    yield return null;
                    
                    print(targets.Length + " / " + t + " / " + handImage.rectTransform.anchoredPosition);
                }

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
    
    
}
