using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SheepBoidManager : MonoBehaviour
{
    public static Action<SheepBoid> OnListChanged;
    
    public static SheepBoidManager instance;

    [Header("Réglages généraux")] 
    public Vector3 bounds;
    public GameObject prefab;
    public int countStart = 50;
    public Vector3 spawnPosition = Vector3.zero;
    public float spawnDelay = 0.2f;

    [Header("Mouvement Boids")]
    public float neighborRadius = 3f;
    public float separationRadius = 1f;
    public float minSpeed = 1f;
    public float maxSpeed = 3f;
    public float separationWeight = 1.5f;
    public float alignmentWeight = 1f;
    public float cohesionWeight = 1f;
    public float noise = 0.3f;

    [Header("Limites de la zone")]
    public float boundMargin = 3f;
    public float boundaryWeight = 2f;

    [Header("Pauses")]
    public Vector2 minTimeBetweenPauses = new Vector2(5, 10);
    public Vector2 pauseDuration = new Vector2(1, 3);

    private int nbInstantSheep = 0;

    [Header("Ui")] 
    [SerializeField] private InputField nameInputField;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        prefab = GameData.instance.sheepPrefab;
        StartCoroutine(SpawnSheepRoutine());
    }

    private IEnumerator SpawnSheepRoutine()
    {
        for (int i = 0; i < countStart; i++)
        {
            SpawnNewSheep("Antoine");

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public void CreateSheep()
    {
        if (nameInputField.text != "")
        {
            SpawnNewSheep(nameInputField.text);
        }
        else
        {
            Debug.LogError("Entré de nom incorrect");
        }
    }

    public void SpawnNewSheep(string name)
    {
        GameObject go = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
        SheepBoid sheep = go.GetComponent<SheepBoid>();
        sheep.manager = this;

        NatureType randomNature = GetRandomNature();
        sheep.SetNature(randomNature);

        Sheep sheepScript = sheep.GetComponent<Sheep>();
        sheepScript.sheepId = nbInstantSheep;
        
        GameManager.instance.sheepList.Add(sheepScript);

        nbInstantSheep++;
        GameData.instance.nbSheep++;
        
        sheepScript.Initialize(nbInstantSheep, name);
        
        Debug.Log("Created new sheep");
        
        OnListChanged?.Invoke(sheep);
    }

    private int nbDominant = 0;
    private int nbPeureux = 0;
    private int nbSolitaire = 0;
    
    NatureType GetRandomNature()
    {
        float P_Dominant = 30 - (9 * nbDominant);
        float P_Peureux = 20 - (3.5f * nbPeureux);
        float P_Solitaire = 45 - (11 * nbSolitaire);

        P_Dominant = Mathf.Max(P_Dominant, 0f);
        P_Peureux = Mathf.Max(P_Peureux, 0f);
        P_Solitaire = Mathf.Max(P_Solitaire, 0f);

        float P_Standard = 100f - (P_Dominant + P_Peureux + P_Solitaire);
        P_Standard = Mathf.Max(P_Standard, 0f);

        // Total
        float total = P_Dominant + P_Peureux + P_Solitaire + P_Standard;
        if (total <= 0f)
            return NatureType.Standard;

        float randomValue = Random.Range(0f, total);

        if (randomValue < P_Dominant)
            return NatureType.Dominant;
        else if (randomValue < P_Dominant + P_Peureux)
            return NatureType.Peureux;
        else if (randomValue < P_Dominant + P_Peureux + P_Solitaire)
            return NatureType.Solitaire;
        else
            return NatureType.Standard;
    }

    public GameObject SheepGetOffAndRecreate(SheepData data, Vector3 spawnP)
    {
        GameObject go = Instantiate(prefab, spawnP, Quaternion.identity, transform);
        SheepBoid sheep = go.GetComponent<SheepBoid>();
        sheep.manager = this;

        Sheep sheepScript = sheep.GetComponent<Sheep>();
        sheepScript.sheepId = data.id;
        sheepScript.currentSkin = data.skin;
        sheepScript.hasLaine = data.hasWhool;
        sheepScript.sheepName = data.name;
        sheep.natureType = data.nature;
        
        GameManager.instance.sheepList.Add(sheepScript);

        sheep.enabled = false;

        nbInstantSheep++;
        GameData.instance.nbSheep++;
        
        sheepScript.Initialize(nbInstantSheep, name);
        
        Debug.Log("Load sheep : " +sheepScript.sheepId);
        
        OnListChanged?.Invoke(sheep);

        return go;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnPosition, new Vector3(1f, 0.1f, 1f));
        
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(bounds.x * 2, 0.1f, bounds.z * 2));
    }
}
