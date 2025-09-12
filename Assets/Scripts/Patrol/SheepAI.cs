using Unity.VisualScripting;
using UnityEngine;

public class SheepAI : PatrolObjets
{
    private int luckToEat = 4;

    public override void WaitingAction()
    {
        int luckNB = Random.Range(0, luckToEat);
        if (luckNB == 1)
        {
            Debug.Log("Le mouton broute");
        }
    }
}
