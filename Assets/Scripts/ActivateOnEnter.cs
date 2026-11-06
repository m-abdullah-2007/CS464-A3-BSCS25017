using UnityEngine;

public class ActivateOnEnter : MonoBehaviour
{
    [SerializeField] GameObject target;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && target != null) target.SetActive(true);
        if (target == null) Debug.LogWarning("Target Not Set");
    }
}