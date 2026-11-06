using System.Collections;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] float knockback = 6f;
    [SerializeField] float knockTime = 0.25f;
    [SerializeField] float restTime = 0f, warnTime = 0.7f, strikeTime = 0.4f;
    [SerializeField] float startDelay = 0f;
    [SerializeField] GameObject warning;

    IEnumerator Start()
    {
        if (restTime <= 0f) yield break;          // spikes: always on
        Collider zone = GetComponent<Collider>();
        zone.enabled = false;
        if (startDelay > 0f) yield return new WaitForSeconds(startDelay);
        while (true)                              // crusher: rest, warn, strike
        {
            zone.enabled = false;
            yield return new WaitForSeconds(restTime);
            if (warning != null) warning.SetActive(true);
            yield return new WaitForSeconds(Mathf.Max(warnTime, 0.5f));
            if (warning != null) warning.SetActive(false);
            zone.enabled = true;
            yield return new WaitForSeconds(strikeTime);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent(out Health health) || !health.TakeDamage(damage)) return;
        Knock(other, transform.position, knockback, knockTime);
    }

    // Shared by Hazard, EnemyGuard and Chaser: away and flat, steering waits.
    public static void Knock(Collider other, Vector3 from, float force, float seconds)
    {
        Rigidbody body = other.attachedRigidbody;
        if (body == null) return;
        Vector3 away = body.position - from;
        away.y = 0f;
        Vector3 push = away.normalized * force;
        if (other.TryGetComponent(out PlayerController p)) p.Knockback(push, seconds);
        else body.AddForce(push, ForceMode.Impulse);
    }
}