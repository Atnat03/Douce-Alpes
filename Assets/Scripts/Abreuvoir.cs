using UnityEngine;

public class Abreuvoir : MonoBehaviour
{
    public static Abreuvoir instance;

    [Header("Water")]
    [SerializeField] private float maximumWater = 100f;
    [SerializeField] private float currentWater = 100f;
    [SerializeField] private float waterDecreaseRate = 0.5f;
    [SerializeField] private float waterAddValue = 5f;
    [SerializeField] private Animator animatorPompe;
    [SerializeField] private UnityEngine.UI.Image curDrinkImage;
    [SerializeField] private GameObject ui;

    [Header("Drink Places")]
    [SerializeField] private Transform drinkPlace1;
    [SerializeField] private Transform drinkPlace2;
    private bool isOccupied1 = false;
    private bool isOccupied2 = false;

    private void Awake() => instance = this;

    private void Update()
    {
        // UI
        curDrinkImage.fillAmount = currentWater / maximumWater;
        ui.SetActive(GameManager.instance.currentCameraState == CamState.Drink);

        // Water logic
        if (GameData.instance.isSheepInside) return;
        currentWater = Mathf.Max(0, currentWater - Time.deltaTime * waterDecreaseRate);
    }

    public void AddWater()
    {
        if (GameManager.instance.currentCameraState != CamState.Drink) return;
        currentWater = Mathf.Min(currentWater + waterAddValue, maximumWater);
        animatorPompe?.SetTrigger("Pompe");
    }

    // --- 🐑 Gestion des places à l’abreuvoir ---
    public bool TryReservePlace(out Transform place)
    {
        place = null;

        if (!isOccupied1)
        {
            isOccupied1 = true;
            place = drinkPlace1;
            return true;
        }

        if (!isOccupied2)
        {
            isOccupied2 = true;
            place = drinkPlace2;
            return true;
        }

        return false;
    }

    public void FreePlace(Transform place)
    {
        if (place == drinkPlace1) isOccupied1 = false;
        else if (place == drinkPlace2) isOccupied2 = false;
    }
}