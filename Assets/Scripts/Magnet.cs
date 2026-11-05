using UnityEngine;

public class Magnet : MonoBehaviour
{
    [SerializeField] float radius = 6f, pull = 12f;
    float activeUntil;

    public void Activate(float seconds) { activeUntil = Time.time + seconds; }

    void FixedUpdate()
    {
        if (Time.time >= activeUntil) return;
        Collider[] near = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider c in near)
        {
            if (!c.TryGetComponent<Coin>(out _)) continue;
            c.transform.position = Vector3.MoveTowards(
                c.transform.position, transform.position, pull * Time.fixedDeltaTime);
        }
    }
}