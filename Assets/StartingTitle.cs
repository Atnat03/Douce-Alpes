using System;
using System.Collections;
using TMPro;
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
    public GameObject titleUI;

    public bool isTesting = false;
    bool isAnimating = false;

    public string[] messages;
    public int currentMessage;
    public GameObject papy;
    public TextMeshProUGUI message;
    private int idMessage = -1;
    public GameObject nextMessage;
    
    private void Start()
    {
        papy.SetActive(false);
        

        if (isTesting)
        {
            titleCamera.gameObject.SetActive(true);
            titleUI.SetActive(true);
            mainCamera.gameObject.SetActive(false);
            GameData.instance.IsStatGame = false;
        }
        else
        {

            PlayGame();
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
        
        papy.SetActive(true);

        NextMessage(true);
    }

    public void NextMessage(bool isFirst = false)
    {
        nextMessage.SetActive(false);
        
        idMessage++;

        if (idMessage >= messages.Length)
        {
            PlayGame();
            return;
        }
        
        StopAllCoroutines();
        StartCoroutine(WriteSmooth(messages[idMessage], isFirst));
    }

    IEnumerator WriteSmooth(string fullMessage, bool isFirst = false, float charDelay = 0.025f)
    {
        if(!isFirst)
        {
            papy.GetComponent<Animator>().SetTrigger("NextMessage");

            yield return new WaitForSeconds(0.3f);
        }
        
        message.text = "";
        foreach (char c in fullMessage)
        {
            message.text += c;
            yield return new WaitForSeconds(charDelay);
        }
        
        nextMessage.SetActive(true);
    }

    void PlayGame()
    {
        papy.SetActive(false);
        mainCamera.gameObject.SetActive(true);
        titleUI.SetActive(false);
        titleCamera.gameObject.SetActive(false);
        GameData.instance.StartGame();

        Destroy(gameObject);
        
        isAnimating = false;
    }
}
