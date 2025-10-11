using System;
using UnityEngine;

public class AddSkins : MonoBehaviour
{
    [SerializeField] private SkinScriptable skinData;
    [SerializeField] private GameObject skinPrefab;
    [SerializeField] private SimpleScrollSnapBridge scrollSnapBridge;

    private void Awake()
    {
        foreach (SkinSkelete skin in skinData.skins)
        {
            GameObject skinGO = Instantiate(skinPrefab);

            SkinUnit s = skinGO.GetComponent<SkinUnit>();
            s.id = skin.id;
            s.name = skin.name;
            s.GetComponent<UnityEngine.UI.Image>().sprite = skin.logo;

            scrollSnapBridge.AddExistingPanel(skinGO);
        }
    }
    
    public GameObject GetSelectedPanel()
    {
        if (scrollSnapBridge == null || scrollSnapBridge.scrollSnap == null)
            return null;

        int selectedIndex = scrollSnapBridge.scrollSnap.CenteredPanel;
        return scrollSnapBridge.scrollSnap.Panels[selectedIndex].gameObject;
    }

    private void Update()
    {
        SheepWindow.instance.SetNewCurrentSkin(GetSelectedPanel().gameObject.GetComponent<SkinUnit>().id);
    }
}