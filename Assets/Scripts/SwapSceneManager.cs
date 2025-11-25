using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SwapSceneManager : MonoBehaviour
{
    public static SwapSceneManager instance;
    private void Awake() { instance = this; }

    [SerializeField] GameObject[] scenes;
    [SerializeField] private int startScene = 0;
    [SerializeField] private bool isTesting = true;
    public int currentSceneId;
    
    public event Action SwapingInteriorScene;
    public event Action SwapingDefaultScene;
    public event Action SwapingTonteScene;
    public event Action SwapingCleanScene;
    public event Action SwapingTricotScene;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration;

    private void Start()
    {
        if(isTesting)
            SwapScene(startScene);
        
        fadeImage.gameObject.SetActive(false);
    }

    public void SwapScene(int sceneID)
    {
        currentSceneId = sceneID;
        
        for (int i = 0; i < scenes.Length; i++)
        {
            if (sceneID == i)
            {
                AnimateSwapingScene(scenes[i], i);
            }
            else
            {
                scenes[i].SetActive(false);
            }
        }
    }

    void AnimateSwapingScene(GameObject scene, int i)
    {
        StartCoroutine(FadeTransition(scene, i));
    }

    IEnumerator FadeTransition(GameObject scene, int i)
    {
        yield return StartCoroutine(FadeOut()); 
        scene.SetActive(true);
        TriggerInitialiseScene(i);

        yield return StartCoroutine(FadeIn());
    }


    public IEnumerator FadeIn()
    {
        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            color.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            fadeImage.color = color;
            timer += Time.deltaTime;
            yield return null;
        }
        
        fadeImage.gameObject.SetActive(false);

        color.a = 0f;
        fadeImage.color = color;
    }

    public IEnumerator FadeOut()
    {
        float timer = 0f;
        Color color = fadeImage.color;
        
        fadeImage.gameObject.SetActive(true);

        while (timer < fadeDuration)
        {
            color.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            fadeImage.color = color;
            timer += Time.deltaTime;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;
    }
    
    void TriggerInitialiseScene(int sceneID)
    {
        switch (sceneID)
        {
            case 0:
                SwapingDefaultScene?.Invoke();
                break;
            case 1:
                SwapingInteriorScene?.Invoke();
                break;
            case 2:
                SwapingTonteScene?.Invoke();
                break;
            case 3: 
                SwapingCleanScene?.Invoke();
                break;
            case 4:
                SwapingTricotScene?.Invoke();
                break;
        }
    }
}
