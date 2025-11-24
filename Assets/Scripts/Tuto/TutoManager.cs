using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum TutoState
{
    Start,
    AddMouton,
    ExplicationBonheur,
    Abreuvoir,
    MiniJeuGrange,
    MiniJeuTonte,
    MiniJeuCleanning,
    MiniJeuSortis,
    Shop,
}

public class TutoManager : MonoBehaviour
{
    public static TutoManager instance; private void Awake(){ instance = this; }

    public bool isTuto = true;
    [SerializeField] public DataDialogue dataDialogue;
    [SerializeField] public TutoState tutoState;
    [SerializeField] public Image dialogueImage;
    [SerializeField] public TutoPointerArrow arrow;
    
    [Header("Start")]
    [SerializeField] public GameObject[] startStateGameObjects;
    
    [Header("AddMouton")]
    [SerializeField] public Button addMoutonButton;

    [Header("Scripts a enable")]
    [SerializeField] private Collider grange;
    [SerializeField] private Collider shop;
    [SerializeField] private Collider abreuvoir;
    
    void Start()
    {
        arrow.gameObject.SetActive(false);
        
        if (!isTuto)
        {
            enabled = false;
            return;
        }

        StartCoroutine(StartTuto());
    }

    public void EnableDialogueBox(string dialogue)
    {
        if (!isTuto)
            return;
        
        dialogueImage.gameObject.SetActive(true);
        dialogueImage.GetComponentInChildren<Text>().text = dialogue;
    }

    public void DisableDialogueBox()
    {
        dialogueImage.gameObject.SetActive(false);
    }
    
    public IEnumerator StartTuto()
    {
        yield return null;
        
        tutoState = TutoState.Start;
        DisableDialogueBox();
        
        foreach (GameObject go in startStateGameObjects)
        {
            go.SetActive(false);
        }
        
        grange.enabled = false;
        abreuvoir.enabled = false;
        shop.enabled = false;
    }

    public void AddMouton()
    {
        foreach (GameObject go in startStateGameObjects)
        {
            go.SetActive(true);
        }
        
        tutoState = TutoState.AddMouton;
        addMoutonButton.gameObject.SetActive(true);
        EnableDialogueBox(dataDialogue.dialogues[0]);
    }

    public void ExplicationBonheur()
    {
        tutoState = TutoState.ExplicationBonheur;
        EnableDialogueBox(dataDialogue.dialogues[1] + SheepBoidManager.instance.nameInputField.text + dataDialogue.dialogues[2]);
    }

    public void GoToShop()
    {
        arrow.gameObject.SetActive(true);
        arrow.targetPos = shop.transform;
        tutoState = TutoState.Shop;
        shop.enabled = true;
        EnableDialogueBox(dataDialogue.dialogues[3]);
    }
    
    public void Shop()
    {
        arrow.gameObject.SetActive(false);
        EnableDialogueBox(dataDialogue.dialogues[4]);
    }

    public void GoToAbreuvoir()
    {
        arrow.gameObject.SetActive(true);
        arrow.targetPos = abreuvoir.transform;
        tutoState = TutoState.Abreuvoir;
        EnableDialogueBox(dataDialogue.dialogues[5]);
        abreuvoir.enabled = true;
    }

    public void Abreuvoir()
    {
        arrow.gameObject.SetActive(false);
        EnableDialogueBox(dataDialogue.dialogues[6]);
    }

    public void GoToGrangeEnter()
    {        
        arrow.gameObject.SetActive(true);
        arrow.targetPos = grange.transform;
        tutoState = TutoState.MiniJeuGrange;
        EnableDialogueBox(dataDialogue.dialogues[7]);
        grange.enabled = true;
    }

    public void MiniJeuGrange()
    {
        arrow.gameObject.SetActive(false);
        EnableDialogueBox(dataDialogue.dialogues[8]);
    }

    public void MiniJeuTonte()
    {
        if(!isTuto)
            return;
        tutoState = TutoState.MiniJeuTonte;
        EnableDialogueBox(dataDialogue.dialogues[9]);
    }

    public void MiniJeuClean()
    {
        if(!isTuto)
            return;
        tutoState = TutoState.MiniJeuCleanning;
        EnableDialogueBox(dataDialogue.dialogues[10]);
    }
    
    public void GoToGrangeOut()
    {        
        arrow.gameObject.SetActive(true);
        arrow.targetPos = grange.transform;
        EnableDialogueBox(dataDialogue.dialogues[11]);
    }

    public void MiniJeuSortis()
    {
        arrow.gameObject.SetActive(false);
        tutoState = TutoState.MiniJeuSortis;
    }
}
