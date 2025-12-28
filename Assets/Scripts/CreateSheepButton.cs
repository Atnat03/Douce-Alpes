using System;
using UnityEngine;
using UnityEngine.UI;

public class CreateSheepButton : MonoBehaviour
{
    [SerializeField] private GameObject sheepcreator;
    [SerializeField] private Text price;

    public void Update()
    {
        GetComponent<Button>().interactable =
            PlayerMoney.instance.isEnoughtMoney(PlayerMoney.instance.GetCurrentSheepPrice());
        
        price.text = PlayerMoney.instance.GetCurrentSheepPrice().ToString();
    }

    public void Click()
    {
        PlayerMoney.instance.RemoveMoney(PlayerMoney.instance.GetCurrentSheepPrice());
        sheepcreator.SetActive(true);
        gameObject.SetActive(false);

        GameManager.instance.ChangeCameraState(CamState.CreateSheep);
    }

    public void Exit()
    {
        GameManager.instance.ChangeCameraState(CamState.Default);
    }
}
