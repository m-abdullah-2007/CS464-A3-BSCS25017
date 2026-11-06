using UnityEngine;

public class Chaser : MonoBehaviour
{
    [SerializeField] float speed = 3.5f, sightRange = 9f, lifetime = 10f;
    [SerializeField] int damage = 1;
    [SerializeField] float knockback = 6f;
    Transform player;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        Destroy(gameObject, lifetime);   // so every wave ends and the next one can start
    }

    void Update()
    {
        if (player == null || !CanSee()) return;
        Vector3 target = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    bool CanSee()
    {
        Vector3 toPlayer = player.position - transform.position;
        if (toPlayer.magnitude > sightRange) return false;
        if (Physics.Raycast(transform.position, toPlayer.normalized, out RaycastHit hit,
                            sightRange, ~0, QueryTriggerInteraction.Ignore))
            return hit.collider.CompareTag("Player");
        return false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Health health) || !health.TakeDamage(damage)) return;
        Rigidbody body = other.attachedRigidbody;
        Hazard.Knock(other, transform.position, knockback, 0.25f);
        Destroy(gameObject);
    }
}