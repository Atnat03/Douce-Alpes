using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public Button buttonSheepCreate;
    public RectTransform ScreenCenter;
    public RectTransform EndPosSheepButton;

    private bool isWritting = false;
    
    private void Start()
    {
        papy.SetActive(false);
        
        titleCamera.gameObject.SetActive(true);
        titleUI.SetActive(true);
        mainCamera.gameObject.SetActive(false);
        GameData.instance.IsStatGame = false;
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
        
        if(GameData.instance.isTuto)
        {
            papy.SetActive(true);

            NextMessage(true);
        }
        else
        {
            PlayGame();
        }
    }

    public void NextMessage(bool isFirst = false)
    {
        if (isWritting) return;
        
        nextMessage.SetActive(false);
        
        idMessage++;

        if (idMessage >= messages.Length)
        {
            StartCoroutine(ButtonSheepAnimation());
            
            return;
        }
        
        StopAllCoroutines();
        StartCoroutine(WriteSmooth(messages[idMessage], isFirst));
    }

    IEnumerator WriteSmooth(string fullMessage, bool isFirst = false, float charDelay = 0.025f)
    {
        isWritting = true;
        
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
        isWritting = false;
    }

    IEnumerator ButtonSheepAnimation()
    { 
        float t = 0f;
        
        papy.SetActive(false);
        buttonSheepCreate.transform.localScale = Vector3.one * 0;
        buttonSheepCreate.transform.position = ScreenCenter.position;
        
        buttonSheepCreate.transform.parent.gameObject.SetActive(true);
        
        while (t <= 0.3f)
        {
            buttonSheepCreate.transform.localScale = Vector3.Lerp(buttonSheepCreate.transform.localScale, Vector3.one*2, t);
            
            t += Time.deltaTime;
            yield return null;
        }

        t = 0f;
        
        buttonSheepCreate.interactable = false;
        
        yield return new WaitForSeconds(1.5f);

        while (t <= 1)
        {
            buttonSheepCreate.transform.localScale = Vector3.Lerp(buttonSheepCreate.transform.localScale, Vector3.one, t);
            buttonSheepCreate.transform.position = Vector2.Lerp(ScreenCenter.position, EndPosSheepButton.position, t);
            
            t += Time.deltaTime;
            yield return null;
        }
        
        
        buttonSheepCreate.transform.position = EndPosSheepButton.position;
        buttonSheepCreate.transform.localScale = Vector3.one;
        
        buttonSheepCreate.interactable = true;
        
        PlayGame();
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
