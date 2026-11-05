using System.Collections;
using UnityEngine;

public class TimedPickups : MonoBehaviour
{
    [SerializeField] GameObject[] pickups;
    [SerializeField] float[] appearAt;

    void Start()
    {
        for (int i = 0; i < pickups.Length && i < appearAt.Length; i++)
            StartCoroutine(Appear(pickups[i], appearAt[i]));
    }

    IEnumerator Appear(GameObject pickup, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (pickup != null) pickup.SetActive(true);
    }
}