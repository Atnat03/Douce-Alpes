using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public enum CamState
{
    StatingGame,
    Default,
    Sheep,
    LockSheep,
    MiniGame,
    Drink,
    Dog,
    Shop,
    CreateSheep,
    Settings
}

[System.Serializable]
public class SheepData
{
    public int id;
    public string name;
    public int skinHat;
    public int skinClothe;
	public bool hasWhool;
    public NatureType nature;
    public int colorID;
    public Vector3 position;
    public string birthDate;

    public SheepData(int id, string name, int skinHat,int skinClothe, bool hasWhool, NatureType nature, int colorID, string birthDate)
    {
        this.id = id;
        this.name = name;
        this.skinHat = skinHat;
        this.skinClothe = skinClothe;
		this.hasWhool = hasWhool;
        this.nature = nature;
        this.colorID = colorID;
        this.birthDate = birthDate;
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public event Action<Vector3, Vector3, Transform> SheepHold;
    public event Action<Sheep> SheepClicked;
    public event Action<Vector3, Vector3, Transform, bool> GrangeClicked;
    public event Action<Vector3, Vector3, Transform> AbreuvoirClicked;
    public event Action<Vector3, Vector3, Transform> NicheClicked;
    public event Action<Vector3, Vector3, Transform> OnClickOnShop;

    public event Action<GameObject> SheepEnter;

    public Action startMiniGame;
    public Action endMiniGame;
    
    public CamState currentCameraState = CamState.Default;
    
    public CameraControl cameraFollow;

    [SerializeField] private GameObject[] UItoDisableWhenSheepIsOn;

    [SerializeField] public List<Sheep> sheepList;
    
    [Header("Starting Game")]
    [SerializeField] GameObject[] elementsToDisable;

    [SerializeField] private GameObject uiStart;
    
    [Header("Sheep")]
    [SerializeField] private int SheepCount;
    [SerializeField] private GameObject sheepWidow;
    [SerializeField] private Sheep curLockSheep = null;
    [SerializeField] public bool isLock = false;
    
    [Header("Grange Mini Game")]
    [SerializeField] private Transform miniGameCamPos;
    [SerializeField] private Transform miniGameZoomCamPos;
    [SerializeField] public Grange grange;
    [SerializeField] public GameObject chien;
    
    [SerializeField] private Transform sheepSpawn;
    [SerializeField] private GameObject uiMiniGame;

    [Header("Abreuvoir")]
    [SerializeField] private Abreuvoir abreuvoir;
    [SerializeField] private Transform cameraPosAbreuvoir;

    [Header("Shop")]
    public bool shopOpen = false;
    
    [Header("Caresse Config")]
    [SerializeField] public float caresseBaseValue = 1f;
    [SerializeField] public float saturationCarrese = 2f;    
    [SerializeField] private float recoveryTime = 3f;        

    private Dictionary<int, int> sheepSwipeCount = new Dictionary<int, int>();
    private Dictionary<int, float> sheepLastSwipeTime = new Dictionary<int, float>();

    [SerializeField] public Button buttonForTonte;
    [SerializeField] public GameObject friendsUI;
    
    [Header("Caresse Visualizer")]
    public float[] caresseCurveValues;
    
    int RealTime = System.DateTime.Now.Hour;
    
    [SerializeField] public GameObject poufParticle;
    [SerializeField] public Transform particleSpawn;
    [SerializeField] public GameObject sheepCreatorButton;
    [SerializeField] public PanneauShop panneauShop;
    
    private void Awake()
    {
        instance = this;
        
        sheepWidow.SetActive(false);
    }

    private void Start()
    {
        currentCameraState = CamState.Default;
        
        GameData.instance.nbSheep = sheepList.Count;
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
        
        if(newState == CamState.MiniGame)
            startMiniGame?.Invoke();
    }

    public void ResetCamera()
    {
        cameraFollow.enabled = false;
        
        ChangePlayerEnvironnement(true);

        if (currentCameraState == CamState.Sheep)
        {
            Sheep sheep = GetSheep(SheepWindow.instance.GetCurrentSheepID());
            if(sheep != null) sheep.StopAgentAndDesactivateScript(false);
                    
            sheepWidow.SetActive(false);
            sheep.isOpen = false;
            SheepWindow.instance.ResetValue();
        }
        
        if(currentCameraState == CamState.MiniGame)
            endMiniGame?.Invoke();
        
        if(isLock)
            DelockSheep();
        
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

    public void ChangeCameraPos(Vector3 pos, Vector3 rot, Transform target, bool isZoomGrange = false)
    {
        switch (currentCameraState)
        {
            case CamState.Sheep:
                SheepHold?.Invoke(pos, rot, target);
                break; 
            case CamState.MiniGame:
                GrangeClicked?.Invoke(pos, rot, target, isZoomGrange);
                break;
            case CamState.Drink:
                AbreuvoirClicked?.Invoke(pos, rot, target);
                break;
            case CamState.Dog:
                NicheClicked?.Invoke(pos, rot, target);
                break;
            case CamState.Shop:
                OnClickOnShop?.Invoke(pos,rot, target);
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
            sheep.isFocusing = true;
        }
    }

    public void ActivatedDog()
    {
        chien.SetActive(true);
    }


    public void DelockSheep()
    {
        if (curLockSheep.isOpen) return;
        
        cameraFollow.gameObject.GetComponent<ChangingCamera>().ResetCameraLock(curLockSheep);
        curLockSheep.isFocusing = false;
        
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
            Debug.Log("Carresse " + bonus);

            BonheurCalculator.instance.AddBonheur(Camera.main.WorldToScreenPoint(sheep.gameObject.transform.position), bonus);

            sheepFatigue[id] += 1f;
        }

        sheepLastSwipeTime[id] = Time.time;
    }
    
    private void Update()
    {
        uiMiniGame.SetActive(CamState.MiniGame == currentCameraState);
        
        friendsUI.gameObject.SetActive(currentCameraState == CamState.Default);
        sheepCreatorButton.SetActive(currentCameraState == CamState.Default 
        && !GameData.instance.isSheepInside); 

        if (CheckAllSheepHasWool() && sheepList.Count > 0)
        {
            if (currentSheepGrange == null)
            {
                CheckBubble(false);
            }
        }
    }

    public void AddAllSheep() 
    {
        for (int i = 0; i < sheepList.Count; i++)
        {
            SheepEnterGrange(sheepList[i]);
        }
    }
    
    //GrangeMiniGame
    public void SheepEnterGrange(Sheep sheep)
    {
        if (!sheepList.Contains(sheep)) Debug.LogError("Le mouton n'existe pas");

        SheepData newDataSheep = new SheepData(sheep.sheepId, sheep.sheepName, sheep.currentSkinHat,  sheep.currentSkinClothe, sheep.hasLaine, sheep.GetComponent<SheepBoid>().natureType, sheep.currentColorID, sheep.birthDate);
        GameData.instance.sheepDestroyData.Add(newDataSheep);

        sheepList.Remove(sheep);
        SheepEnter?.Invoke(sheep.gameObject);
        
        Destroy(sheep.gameObject);
        
        grange.AddSheepInGrange();

        if (sheepList.Count == 0)
        {
            sheepList = new List<Sheep>();
            
            StartCoroutine(NextFrameChangeScene());
        }
        
    }

    IEnumerator NextFrameChangeScene()
    {
        grange.CloseDoors();
        
        AudioManager.instance.PlaySound(11);
        
        yield return new WaitForSeconds(1f);
        
        if (GameData.instance.timer.currentMiniJeuToDo == MiniGames.Rentree)
        {
            GameData.instance.isSheepInside = true;
            
            GameData.instance.timer.canButtonG = false;
            GameData.instance.timer.canButtonT = true;

            GameData.instance.StartMiniGameCooldown(TypeAmelioration.Rentree);
            
            BonheurCalculator.instance.AddBonheur(Vector2.zero, GameData.instance.GetLevelUpgrade(TypeAmelioration.Rentree));
            
            grange.CloseUI();
            
            GameData.instance.timer.UpdateAllButton();
            SwapSceneManager.instance.SwapSceneInteriorExterior(1);
        }
    }

    
    public void SheepGetOutGrange()
    {
        StartCoroutine(SpawnSheepOneByOne());
    }

    private IEnumerator SpawnSheepOneByOne()
    {
        List<SheepData> toRemove = new List<SheepData>();

        BonheurCalculator.instance.AddBonheur(Vector2.zero, GameData.instance.GetLevelUpgrade(TypeAmelioration.Sortie));

        grange.OpenDoors();
        grange.GetPoutre().ResetPoutre();
        grange.AllSheepAreOutside = false;

        SheepBoidManager.instance.nbInstantSheep = 0;
        
        yield return new WaitForSeconds(1f);

        float delayBetweenSheep = 0.75f; 

        foreach (SheepData sheepData in GameData.instance.sheepDestroyData)
        {
            GameObject newSheep = SheepBoidManager.instance.SheepGetOffAndRecreate(sheepData, grange.spawnGetOffTransform.position);
            Sheep sheep = newSheep.GetComponent<Sheep>();

            Instantiate(poufParticle, particleSpawn.position + Vector3.right, Quaternion.identity);
            
            if (sheepData.hasWhool == false)
                sheep.CutWhool();
            else
                sheep.hasLaine = true;
            
            sheep.ResetPuanteur();

            newSheep.GetComponent<SheepBoid>().enabled = false;

            grange.AnimSheepGetOffGrange(newSheep);

            toRemove.Add(sheepData);
            
            yield return new WaitForSeconds(delayBetweenSheep);
        }

        if(TutoManager.instance != null)
            TutoManager.instance.GoToShop();
        
        AudioManager.instance.PlaySound(9, 1f, 0.2f);
        
        grange.AllSheepAreOutside = true;
        GameData.instance.sheepDestroyData.Clear();
        
        GameData.instance.timer.canButtonG = false;
        GameData.instance.timer.UpdateAllButton();
        
        StartCoroutine(GetOffGrange());
        
        GameData.instance.isSheepInside = false;
    }

    IEnumerator GetOffGrange()
    {
        Camera.main.GetComponent<ChangingCamera>().ResetPosition();
        
        yield return new WaitForSeconds(1f);
        GameData.instance.RecapOfTheDay();
    }
    
    //Abreuvoir
    public void ActivateAbreuvoir()
    {
        ChangeCameraState(CamState.Drink);
        ChangeCameraPos(cameraPosAbreuvoir.position, cameraPosAbreuvoir.rotation.eulerAngles, abreuvoir.transform);
    }
    
    public bool CheckAllSheepHasWool()
    {
        if (sheepList.Count == 0)
            return false;

        foreach (Sheep s in sheepList)
        {
            if (!s.hasLaine)
                return false;
        }

        return true;
    }
    
    public void UpdateGrangeAvailability()
    {
        bool canEnterGrange = CheckAllSheepHasWool();

        GameData.instance.timer.canButtonG = canEnterGrange;
        GameData.instance.timer.grangeButton.interactable = canEnterGrange;
    }

    
    void ResetTheScene()
    {
        Debug.Log("Reset the scene");
        
        cameraFollow.enabled = true;
        cameraFollow.ResetCameraPoseDefault();
        cameraFollow.GetComponent<ChangingCamera>().StopAll();
    }

    void OnEnable()
    {
        SwapSceneManager.instance.SwapingDefaultScene += ResetTheScene;
    }
    
    void OnDisable()
    {
        SwapSceneManager.instance.SwapingDefaultScene -= ResetTheScene;
    }

    public void AnimatedBackFlip()
    {
        foreach (Sheep sheep in sheepList)
        {
            Debug.Log(sheep.sheepName);
            sheep.GetComponent<Animator>().SetTrigger("Flip");
        }
    }

    #region Bubble

    public Sheep currentSheepGrange = null;
    public Sheep currentSheepAbreuvoir = null;

    public void CheckBubble(bool isDrink)
    {
        Debug.Log("Checking Bubble");
        if (isDrink)
        {
            Debug.Log("drink");
            if (currentSheepAbreuvoir == null)
            {
                Debug.Log("drink pas null");
            
                List<Sheep> availableSheep = sheepList.FindAll(sheep => !sheep.HasActiveBubble());
            
                if (availableSheep.Count > 0)
                {
                    currentSheepAbreuvoir = availableSheep[Random.Range(0, availableSheep.Count)];
                    currentSheepAbreuvoir.ActivatedBubble(true);
                }
                else
                {
                    Debug.LogWarning("Aucun mouton disponible pour l'abreuvoir");
                }
            }
        }
        else
        {
            Debug.Log("pas drink");
            if (currentSheepGrange == null)
            {
                Debug.Log("pas drink pas null");
            
                List<Sheep> availableSheep = sheepList.FindAll(sheep => !sheep.HasActiveBubble());
            
                if (availableSheep.Count > 0)
                {
                    currentSheepGrange = availableSheep[Random.Range(0, availableSheep.Count)];
                    currentSheepGrange.ActivatedBubble(false);
                }
                else
                {
                    Debug.LogWarning("Aucun mouton disponible pour la grange");
                }
            }
        }
    }
    public void DisableDinkBubble()
    {
        currentSheepAbreuvoir.DisableBubble();
        currentSheepAbreuvoir = null;
        abreuvoir.alreadyBubble = false;
    }
    
    public void DisableGrangeBubble() => currentSheepAbreuvoir.DisableBubble();

    #endregion

    public void CloseWindowShopAndGoToShop()
    {
        StartCoroutine(GoToShop());
    }

    IEnumerator GoToShop()
    {
        ChangingCamera change = cameraFollow.GetComponent<ChangingCamera>();
        
        change.ResetPosition();
        
        yield return new WaitForSeconds(1f);
        
        panneauShop.OpenUI();
        
    }
}
