using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public enum CamState
{
    StatingGame,
    Default,
    Sheep,
    LockSheep,
    MiniGame,
    Drink,
}

public class SheepData
{
    public int id;
    public string name;
    public int skin;

    public SheepData(int id, string name, int skin)
    {
        this.id = id;
        this.name = name;
        this.skin = skin;
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public event Action<Vector3, Vector3> SheepHold;
    public event Action<Sheep> SheepClicked;
    public event Action<Vector3, Vector3> GrangeClicked;
    public event Action<Vector3, Vector3> AbreuvoirClicked;
    
    public CamState currentCameraState = CamState.StatingGame;
    
    CameraControl cameraFollow;

    [SerializeField] private GameObject[] UItoDisableWhenSheepIsOn;

    [SerializeField] public List<Sheep> sheepList;
    [SerializeField] private List<SheepData> sheepDestroyData;
    public bool isSheepOutside = true;
    
    [Header("Starting Game")]
    [SerializeField] GameObject[] elementsToDisable;

    [SerializeField] private GameObject uiStart;
    
    [Header("Bonheur")] 
    [SerializeField] private float currentBonheur;
    [SerializeField] private float maxBonheur;
    [SerializeField] Text txtBonheur;
    
    [Header("Money")]
    [SerializeField] private int currentMoney;
    [SerializeField] Text txtMoney;
    
    [Header("Sheep")]
    [SerializeField] private int SheepCount;
    [SerializeField] private GameObject sheepWidow;
    [SerializeField] private Sheep curLockSheep = null;
    [SerializeField] public bool isLock = false;
    
    [Header("Grange Mini Game")]
    [SerializeField] private Transform miniGameCamPos;
    [SerializeField] private Transform miniGameZoomCamPos;
    [SerializeField] public Grange grange;
    [SerializeField] private GameObject sheepPrefab;
    [SerializeField] private Transform sheepSpawn;
    [SerializeField] private GameObject uiMiniGame;

    [Header("Abreuvoir")]
    [SerializeField] private Abreuvoir abreuvoir;
    [SerializeField] private Transform cameraPosAbreuvoir;
        
    [Header("Shop")]
    [SerializeField] private GameObject shopUI;

    public bool shopOpen = false;
    
    [Header("Caresse Config")]
    [SerializeField] public float caresseBaseValue = 1f;
    [SerializeField] public float saturationCarrese = 2f;    
    [SerializeField] private float recoveryTime = 3f;        

    private Dictionary<int, int> sheepSwipeCount = new Dictionary<int, int>();
    private Dictionary<int, float> sheepLastSwipeTime = new Dictionary<int, float>();
    
    [Header("Caresse Visualizer")]
    public float[] caresseCurveValues;
    
    int RealTime = System.DateTime.Now.Hour;
    
    private void Awake()
    {
        instance = this;
        
        cameraFollow = Camera.main.GetComponent<CameraControl>();
        sheepWidow.SetActive(false);
    }

    private void Start()
    {
        sheepDestroyData = new List<SheepData>();

        currentCameraState = CamState.StatingGame;
        
        shopUI.SetActive(false);

        ActivateElementsBeforeStart(false);
    }

    public void StartGameCameraTravelling()
    {
        StartCoroutine(WaitBeoreStart());
    }

    IEnumerator WaitBeoreStart()
    {
        Camera.main.GetComponent<Animator>().SetBool("Starting", true);
        Camera.main.GetComponent<CameraControl>().enabled = false;
        
        yield return new WaitForSeconds(3f);
        
        Debug.Log("Game start !");
        
        Camera.main.GetComponent<CameraControl>().enabled = true;
        Camera.main.GetComponent<Animator>().enabled = false;
        
        uiStart.SetActive(false);
        currentCameraState = CamState.Default;
        
        ActivateElementsBeforeStart(true);
    }
    
    void ActivateElementsBeforeStart(bool state)
    {
        foreach (GameObject go in elementsToDisable)
        {
            go.SetActive(state);
        }
    }

    public Sheep GetSheep(int idSheep)
    {
        return sheepList.FirstOrDefault(s => s.sheepId == idSheep);
    }

    public Sheep getCurLockSheep() { return curLockSheep;}
    
    public GameObject GetSheepWindow(){return  sheepWidow;}
    
    public void ChangeCameraState(CamState newState)
    {
        currentCameraState = newState;
        cameraFollow.enabled = false;
        ChangePlayerEnvironnement(false);
    }

    public void ResetCamera()
    {
        cameraFollow.enabled = true;
        
        ChangePlayerEnvironnement(true);

        if (currentCameraState == CamState.Sheep)
        {
            Sheep sheep = GetSheep(SheepWindow.instance.GetCurrentSheepID());
            if(sheep != null) sheep.StopAgentAndDesactivateScript(false);
                    
            sheepWidow.SetActive(false);
            SheepWindow.instance.ResetValue();
        }
        
        currentCameraState = CamState.Default;
    }

    public Transform GetMiniGameCamPos() { return miniGameCamPos; }
    public Transform GetMiniGameZoomCamPos() { return miniGameZoomCamPos; }
    public Transform GetAbreuvoirCameraPosition() { return cameraPosAbreuvoir;}

    public void ChangePlayerEnvironnement(bool state)
    {
        foreach (GameObject obj in UItoDisableWhenSheepIsOn)
        {
            obj.SetActive(state);
        }
    }

    public void ChangeCameraPos(Vector3 pos, Vector3 rot)
    {
        switch (currentCameraState)
        {
            case CamState.Sheep:
                SheepHold?.Invoke(pos, rot);
                break; 
            case CamState.MiniGame:
                GrangeClicked?.Invoke(pos, rot);
                break;
            case CamState.Drink:
                AbreuvoirClicked?.Invoke(pos, rot);
                break;
            case CamState.Default:
                break;
        }
    }

    public void LockCamOnSheep(Sheep sheep)
    {
        if(curLockSheep == null)
        {
            isLock = true;

            SheepClicked?.Invoke(sheep);

            curLockSheep = sheep;
        }
    }

    public void DelockSheep()
    {
        Debug.Log("DelockSheep");
        
        cameraFollow.gameObject.GetComponent<ChangingCamera>().ResetCameraLock(curLockSheep);
        
        curLockSheep = null;
        
        isLock = false;
    }

    private Dictionary<int, float> sheepFatigue = new Dictionary<int, float>();

    public void Caresse(Sheep sheep)
    {
        if (sheep == null) return;

        int id = sheep.sheepId;

        if (!sheepFatigue.ContainsKey(id)) sheepFatigue[id] = 0f;
        if (!sheepLastSwipeTime.ContainsKey(id)) sheepLastSwipeTime[id] = Time.time;

        float timeSinceLastSwipe = Time.time - sheepLastSwipeTime[id];

        sheepFatigue[id] = Mathf.Max(0f, sheepFatigue[id] - (timeSinceLastSwipe / recoveryTime));

        float bonus = caresseBaseValue * Mathf.Exp(-sheepFatigue[id] / saturationCarrese);

        if (bonus > 0.01f) 
        {
            currentBonheur = Mathf.Min(maxBonheur, currentBonheur + bonus);
            sheepFatigue[id] += 1f; 
        }

        sheepLastSwipeTime[id] = Time.time;

        Debug.Log($"{sheep.name} : +{bonus:F2} bonheur (total {currentBonheur:F2}) | Fatigue: {sheepFatigue[id]:F2}");
    }

    private void Update()
    {
        int bonheur = (int)((currentBonheur / maxBonheur) * 100);
        txtBonheur.text = bonheur + " %";
        txtMoney.text = currentMoney.ToString();
        
        uiMiniGame.SetActive(CamState.MiniGame == currentCameraState);

        isSheepOutside = sheepList.Count != 0;
    }
    
    //GrangeMiniGame
    public void SheepEnterGrange(Sheep sheep)
    {
        if (!sheepList.Contains(sheep)) Debug.LogError("Le mouton n'existe pas");

        SheepData newDataSheep = new SheepData(sheep.sheepId, sheep.name, sheep.currentSkin);
        sheepDestroyData.Add(newDataSheep);

        sheepList.Remove(sheep);
        Destroy(sheep.gameObject);
        
        grange.AddSheepInGrange();

        if (sheepList.Count == 0)
            sheepList = new List<Sheep>();
    }
    
    public void SheepGetOutGrange()
    {
        List<SheepData> toRemove = new List<SheepData>();

        foreach (SheepData sheepData in sheepDestroyData)
        {
            GameObject newSheep = Instantiate(sheepPrefab, sheepSpawn.position, sheepSpawn.rotation, null);
            Sheep sheep = newSheep.GetComponent<Sheep>();
        
            sheep.sheepId = sheepData.id;
            sheep.name = sheepData.name;
            sheep.currentSkin = sheepData.skin;
        
            sheepList.Add(sheep);
            toRemove.Add(sheepData);
        }

        sheepDestroyData = new List<SheepData>();
    }
    
    //Shop
    public void ActivateShop()
    {
        shopOpen = true;
        shopUI.SetActive(true);
        ChangePlayerEnvironnement(false);
    }

    public void DeactivateShop()
    {
        shopOpen = false;
        shopUI.SetActive(false);
        ChangePlayerEnvironnement(true);
    }
    
    //Abreuvoir
    public void ActivateAbreuvoir()
    {
        ChangeCameraState(CamState.Drink);
        ChangeCameraPos(cameraPosAbreuvoir.position, cameraPosAbreuvoir.rotation.eulerAngles);
    }
}
