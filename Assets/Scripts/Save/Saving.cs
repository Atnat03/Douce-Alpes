using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-2)]
public class Saving : MonoBehaviour
{
    public static Saving instance;

    public event Action savingEvent;
    public event Action<PlayerData> loadingEvent;

    public bool hasToLoadSave = true;
    
    [Header("Data")]
    [SerializeField] public double curTime;
    [SerializeField] public float savingDuration = 5f;
    [SerializeField] public GameObject savingUI;
    [SerializeField] public Dropdown dropDurations;

    private const string FIRST_LAUNCH_KEY = "FirstLaunch";
    
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (!hasToLoadSave) return;
        
        print("loading in Saving script");
        
        if (!PlayerPrefs.HasKey(FIRST_LAUNCH_KEY))
        {
            Debug.Log("Premier lancement du jeu");

            ResetSaving();

            PlayerPrefs.SetInt(FIRST_LAUNCH_KEY, 1);
            PlayerPrefs.Save();
        }
        else
        {
            LoadData();
        }
        
        StartWaitSaving();
    }

    public void SetSavingDuration()
    {
        int selectedIndex = dropDurations.value;
        string text = dropDurations.options[selectedIndex].text;

        string numberOnly = Regex.Match(text, @"\d+").Value;

        savingDuration = int.Parse(numberOnly);
    }
    
    [ContextMenu("Save Game")]
    public void StartWaitSaving()
    {
        StartCoroutine(WaitSaving());
    }

    IEnumerator WaitSaving()
    {
        yield return new WaitForSeconds(savingDuration-2);
        
        Debug.Log("Saving...");
        savingUI.SetActive(true);
        yield return new WaitForSeconds(2f);
        savingUI.SetActive(false);
        
        savingEvent?.Invoke();
        SaveManager.SavePlayer(this);

        StartWaitSaving();
    }
    
    private void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveManager.SavePlayer(this);
    }

    private void OnApplicationQuit()
    {
        SaveManager.SavePlayer(this);
    }

    public void LoadData()
    {
        print("load data");
        PlayerData data = SaveManager.LoadPlayer();
        loadingEvent?.Invoke(data);
    }

    [ContextMenu("ResetSave")]
    public void ResetSaving()
    {
        SaveManager.ResetSave();
        SceneManager.LoadScene(0);
    }
}
