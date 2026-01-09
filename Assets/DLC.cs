using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DLC : MonoBehaviour
{
    public GameObject pageDLC;
    public bool isSelect = false;
    public int price;
    public GameObject buyButton;
    public Image selectButton;

    public static Action ChangeSelect;
    
    public Animator animatorPlage;
    public Animator animatorMontagne;
    
    [SerializeField] protected GameObject buyInfo;
    [SerializeField] protected GameObject cantBuyInfo;
    protected bool isShowingCantBuy = false;

    private void Start()
    {
        animatorPlage.SetBool("Sortis", false);
        animatorMontagne.SetBool("Sortis", true);
    }

    public void SelectAndUnSelect()
    {
        isSelect = !isSelect;
        selectButton.color = isSelect  ? Color.gray : Color.white;

        if (isSelect)
        {
            animatorPlage.SetBool("Sortis", true);
            animatorMontagne.SetBool("Sortis", false);
        }
        else
        {
            animatorPlage.SetBool("Sortis", false);
            animatorMontagne.SetBool("Sortis", true);
        }
        
        Settings.instance.SetPlayaSound(isSelect);
        ChangeSelect?.Invoke();
    }

    public void BuyButton()
    {
        if (PlayerMoney.instance.isEnoughtMoney(price))
        {
            PlayerMoney.instance.RemoveMoney(price);
            AudioManager.instance.PlaySound(3, 1f, 0.25f); 

            SelectAndUnSelect();
            
            Settings.instance.ActivatePlayaToggle();
            
            buyButton.SetActive(false);
        }
        else
        {
            AudioManager.instance.PlaySound(5);
            StartCoroutine(CantBuyIt());
        }
    }
    
    IEnumerator CantBuyIt()
    {
        isShowingCantBuy = true;
    
        cantBuyInfo.SetActive(true);
    
        yield return new WaitForSeconds(1.5f);
    
        cantBuyInfo.SetActive(false);
    
        isShowingCantBuy = false;
    }   

    public void ActivateDLC(bool state)
    {
        pageDLC.SetActive(state);
    }
}
