using System;
using System.Collections;
using UnityEngine;

public class Saving : MonoBehaviour
{
    public static Saving instance;
    private void Awake() { instance = this; }

    public event Action savingEvent;
    public event Action<PlayerData> loadingEvent;

    [Header("Data")]
    [SerializeField] public double curTime;

    private void Start()
    {
        StartWaitSaving();
        LoadData(SaveManager.LoadPlayer());
    }

    void StartWaitSaving()
    {
        StartCoroutine(WaitSaving());
    }

    IEnumerator WaitSaving()
    {
        yield return new WaitForSeconds(5);
        savingEvent?.Invoke();
        SaveManager.SavePlayer(this);

        StartWaitSaving();
    }

    public void LoadData(PlayerData data)
    {
        loadingEvent?.Invoke(data);
    }
}
