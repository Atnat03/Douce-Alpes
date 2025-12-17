using UnityEngine;

public class MiniGameParent : MonoBehaviour
{
    [SerializeField] protected RectTransform rectTransform;
    
    public void EndMiniGame(TypeAmelioration type)
    {
        BonheurCalculator.instance.AddBonheur(rectTransform.position, GameData.instance.GetLevelUpgrade(type));
    
        GameData.instance.StartMiniGameCooldown(type);
    }

    public static bool CheckIfCanUpgrade(TypeAmelioration type)
    {
        (AmeliorationValueSO, int) curUpgrade = GameData.instance.GetSOUpgrade(type);

        if (GameData.instance.nbSheep >= curUpgrade.Item1.levelsValue[curUpgrade.Item2].miniSheep && 
            GameData.instance.nbSheep <= curUpgrade.Item1.levelsValue[curUpgrade.Item2].maxiSheep)
        {
            return true;
        }
        return false;
    }
}
