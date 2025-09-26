using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Poutre : MonoBehaviour
{
    [SerializeField] private Cadenas[] cadenas;
    [SerializeField] SwipeDetection swipeDetection;
    [SerializeField] private Grange grange;
    
    Vector3 Startpos;

    private void Start()
    {
        Startpos = transform.position;
    }

    public void ResetPoutre()
    {
        foreach (Cadenas cadena in cadenas)
        {
            cadena.transform.gameObject.SetActive(true);
            cadena.hp = cadena.maxHp;
        }

        if (gameObject.GetComponent<Rigidbody>())
        {
            Destroy(gameObject.GetComponent<Rigidbody>());
        }

        gameObject.transform.position = Startpos;
    }

    public void GetOffPoutre()
    {
        if (canSwipeUp())
        {
            gameObject.AddComponent<Rigidbody>();
            GetComponent<Rigidbody>().AddForce(Vector3.up * Time.deltaTime * 1000, ForceMode.Impulse);

            StartCoroutine(WaitALittle());
        }
    }

    IEnumerator WaitALittle()
    {
        yield return new WaitForSeconds(1f);
                    
        grange.OpenDoors();
        GameManager.instance.SheepGetOutGrange();
    }

    bool canSwipeUp()
    {
        foreach (Cadenas cadena in cadenas)
        {
            if (cadena.gameObject.activeSelf)
                return false;
        }
        return true;
    }
}
