using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class SwapSceneManager : MonoBehaviour
{
    public static SwapSceneManager instance;
    private void Awake() { instance = this; }

    [SerializeField] GameObject[] scenes;
    [SerializeField] private int startScene = 0;
    public int currentSceneId;
    
    public event Action SwapingInteriorScene;
    public event Action SwapingDefaultScene;
    public event Action SwapingTonteScene;
    public event Action SwapingCleanScene;
    public event Action SwapingTricotScene;

    [SerializeField] private CanvasGroup fadeCanva;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration;

    [SerializeField] private MusicManager musicManager;
    
    private void Start()
    {
        SwapScene(startScene);
        
        fadeCanva.alpha = 0f;
        fadeImage.gameObject.SetActive(false);
    }

    public void SwapScene(int sceneID)
    {
        print(sceneID);
        
        currentSceneId = sceneID;
        
        for (int i = 0; i < scenes.Length; i++)
        {
            if (sceneID == i)
            {
                print("ANimatted");
                
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
    
    public void SwapSceneInteriorExterior(int sceneID)
    {
        currentSceneId = sceneID;
        
        for (int i = 0; i < scenes.Length; i++)
        {
            if (sceneID == i)
            {
                AnimateSwapingSceneInteriorExterior(scenes[i], i);
            }
            else
            {
                scenes[i].SetActive(false);
            }
        }
    }

    void AnimateSwapingSceneInteriorExterior(GameObject scene, int i)
    {
        StartCoroutine(FadeTransitionInteriorExterior(scene, i));
    }
    
    IEnumerator FadeTransitionInteriorExterior(GameObject scene, int i)
    {
        fadeCanva.alpha = 1f;
        fadeCanva.GetComponent<Animator>().SetTrigger("Fade");
        
        musicManager.ChangeMusique();
        
        yield return new WaitForSeconds(1f);
        AudioManager.instance.PlaySound(1);
        
        scene.SetActive(true);
        TriggerInitialiseScene(i);

        yield return null;
        fadeCanva.GetComponent<Animator>().SetTrigger("Fade");
        
        yield return new WaitForSeconds(1f);
        fadeCanva.alpha = 0f;
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
        Debug.Log(sceneID);
        
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
