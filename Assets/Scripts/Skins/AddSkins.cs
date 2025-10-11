using System;
using UnityEngine;

public class AddSkins : MonoBehaviour
{
    [SerializeField] private SkinScriptable skinData;
    [SerializeField] private GameObject skinPrefab;
    [SerializeField] private SimpleScrollSnapBridge scrollSnapBridge;

    private void Awake()
    {
        var snap = scrollSnapBridge.ScrollSnap;

        // 1️⃣ Ajouter les panels
        foreach (SkinSkelete skin in skinData.skins)
        {
            GameObject skinGO = Instantiate(skinPrefab);
            SkinUnit s = skinGO.GetComponent<SkinUnit>();
            s.id = skin.id;
            s.name = skin.name;
            s.GetComponent<UnityEngine.UI.Image>().sprite = skin.logo;

            scrollSnapBridge.AddExistingPanel(skinGO);
        }

        RectTransform prefabRect = skinPrefab.GetComponent<RectTransform>();
        float panelWidth = snap.Viewport.rect.width / 5f;
        float panelHeight = prefabRect.rect.height / prefabRect.rect.width * panelWidth;

        snap.Size = new Vector2(panelWidth, panelHeight);


        snap.Setup();
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