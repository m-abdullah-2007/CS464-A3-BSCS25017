using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [SerializeField] float bounceSpeed = 7f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Rigidbody body = other.attachedRigidbody;
        if (body == null) return;
        body.AddForce(Vector3.up * bounceSpeed, ForceMode.VelocityChange);
    }
}