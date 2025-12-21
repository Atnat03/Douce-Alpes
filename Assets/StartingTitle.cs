using System;
using System.Collections;
using UnityEngine;
    
public class StartingTitle : MonoBehaviour
{
    public static StartingTitle instance;

    private void Awake()
    {
        instance = this;
    }

    public Camera titleCamera;
    public Camera mainCamera;

    public bool isTesting = false;
    bool isAnimating = false;
    
    private void Start()
    {
        if (!isTesting)
        {
            titleCamera.gameObject.SetActive(true);
            mainCamera.gameObject.SetActive(false);
            GameData.instance.IsStatGame = false;
        }
        else
        {
            titleCamera.gameObject.SetActive(false);
            mainCamera.gameObject.SetActive(true);
            GameData.instance.StartGame();
        }
    }

    public void StartAnimation()
    {
        if(!isAnimating)
            StartCoroutine(AnimationCamera());
    }

    IEnumerator AnimationCamera()
    {
        isAnimating = true;
        
        float temps = 0f;
        float duree = 3f;
        Vector3 from = titleCamera.transform.position;
        Vector3 to = mainCamera.transform.position;

        while (temps < duree)
        {
            titleCamera.transform.position = Vector3.Lerp(from, to, temps / duree);
            temps += Time.deltaTime;
            yield return null;
        }

        titleCamera.transform.position = to;
        
        mainCamera.gameObject.SetActive(true);
        titleCamera.gameObject.SetActive(false);
        GameData.instance.StartGame();

        Destroy(gameObject);
        
        isAnimating = false;
    }
}
