using System;
using UnityEngine;

public class SwapSceneManager : MonoBehaviour
{
    public static SwapSceneManager instance;
    private void Awake() { instance = this; }

    [SerializeField] GameObject[] scenes;
    [SerializeField] private int startScene = 0;
    
    public event Action SwapingInteriorScene;
    public event Action SwapingDefaultScene;
    public event Action SwapingTonteScene;

    private void Start()
    {
        SwapScene(startScene);

    }

    public void SwapScene(int sceneID)
    {
        for (int i = 0; i < scenes.Length; i++)
        {
            if (sceneID == i)
            {
                scenes[i].SetActive(true);
            }
            else
            {
                scenes[i].SetActive(false);
            }
        }

        switch (sceneID)
        {
            case 0:
                SwapingDefaultScene?.Invoke();
                break;
            case 1:
                SwapingInteriorScene?.Invoke();
                break;
            case 2:
                SwapingTonteScene?.Invoke();
                break;
        }
        
    }
}
