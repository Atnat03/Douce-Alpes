using System;
using UnityEngine;

public class InteriorTouchMiniGame : TouchableObject
{
    [SerializeField] private int sceneToGo;
    [SerializeField] private MiniGames minijeu;
    [SerializeField] private bool isTonte;
    [SerializeField] private InteriorSceneManager sceneManager;
    [SerializeField] private GameObject exclamation;

    private void Update()
    {
        exclamation.SetActive(GameData.instance.timer.currentMiniJeuToDo == minijeu);
    }

    public override void TouchEvent()
    {
        if(GameData.instance.timer.currentMiniJeuToDo == minijeu)
        {
            if(isTonte)
                sceneManager.DisableTonteBubble();
            else
                sceneManager.DisableCleanBubble();
            
            SwapSceneManager.instance.SwapScene(sceneToGo);
        }
    }
}
