using UnityEngine;

public class Build : TouchableObject
{
    [SerializeField] GameObject UI;

    void Start()
    {
        DesactivateUI();
    }
    
    public override void TouchEvent()
    {
        UI.SetActive(true);
        Invoke(nameof(DesactivateUI), 3f);
    }

    public void DesactivateUI()
    {
        UI.SetActive(false);
    }
}
