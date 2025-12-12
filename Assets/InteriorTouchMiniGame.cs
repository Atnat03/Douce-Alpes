using UnityEngine;

public class InteriorTouchMiniGame : TouchableObject
{
    [SerializeField] private int sceneToGo;
    [SerializeField] private MiniGames minijeu;

    public override void TouchEvent()
    {
        if(GameData.instance.timer.currentMiniJeuToDo == minijeu)
            SwapSceneManager.instance.SwapScene(sceneToGo);
    }
}
