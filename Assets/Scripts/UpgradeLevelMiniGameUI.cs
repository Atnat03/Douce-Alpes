using System;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeLevelMiniGameUI : MonoBehaviour
{
    public Button[] buttons;

    private void Awake()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            TypeAmelioration t = (TypeAmelioration)i;
            
            buttons[i].onClick.AddListener(() => GameData.instance.AddLevelUpgrade(t));
        }
    }
}
