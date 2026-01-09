using System;
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

    public void SelectAndUnSelect()
    {
        isSelect = !isSelect;
        selectButton.color = isSelect  ? Color.white : Color.grey;
        Settings.instance.SetPlayaSound(isSelect);
        ChangeSelect?.Invoke();
    }

    public void BuyButton()
    {
        if (PlayerMoney.instance.isEnoughtMoney(price))
        {
            PlayerMoney.instance.RemoveMoney(price);

            SelectAndUnSelect();
            
            Settings.instance.ActivatePlayaToggle();
            
            buyButton.SetActive(false);
        }
    }

    public void ActivateDLC(bool state)
    {
        pageDLC.SetActive(state);
    }
}
