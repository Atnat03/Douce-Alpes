using UnityEngine;
using UnityEngine.UI;

public class ArticleUnit : MonoBehaviour
{
    public Text titleTxt;
    public Image logoImage;
    public Text priceTxt;
    public Button buyBtn;
    public Button cancelBtn;

    public void ActivateButtons()
    {
        buyBtn.transform.parent.gameObject.SetActive(true);
    }

    public void DesactivateButtons()
    {
        buyBtn.transform.parent.gameObject.SetActive(false);
    }
}
