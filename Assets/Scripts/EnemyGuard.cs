using UnityEngine;

public class EnemyGuard : MonoBehaviour
{
    enum State { Patrol, Chase, Return }

    [SerializeField] Transform[] points;
    [SerializeField] PatrolMode mode = PatrolMode.Loop;
    [SerializeField] Transform player;
    [SerializeField] float patrolSpeed = 2.5f, chaseSpeed = 4.5f, retreatSpeed = 4f, turnSpeed = 360f;
    [SerializeField] float sightRange = 7f, loseRange = 9f, waitTime = 0f;
    [SerializeField] float arriveDistance = 0.1f;
    [SerializeField] bool needLineOfSight = true;
    [SerializeField] int damage = 1;
    [SerializeField] float knockback = 6f, knockTime = 0.25f;
    [SerializeField] float hitPause = 2f;

    State state = State.Patrol;
    int index, step = 1;
    float waitUntil, ignorePlayerUntil;
    bool retreating;
    Renderer rend;
    Color calmColor;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null) calmColor = rend.material.color;

        Hazard oldHazard = GetComponent<Hazard>();
        if (oldHazard != null) { oldHazard.enabled = false; Destroy(oldHazard); }

        // Give-up range must be longer than chase range, or it flickers at the edge.
        if (loseRange <= sightRange) loseRange = sightRange + 2f;
    }

    void Start()
    {
        if (player == null)
        {
            PlayerController pc = FindAnyObjectByType<PlayerController>();
            if (pc != null) player = pc.transform;
        }
    }


    void Update()
    {
        if (points == null || points.Length == 0 || player == null) return;

        switch (state)
        {
            case State.Patrol:
                Patrol();
                if (Time.time >= ignorePlayerUntil && CanSee(sightRange)) state = State.Chase;
                break;

            case State.Chase:
                MoveTo(new Vector3(player.position.x, transform.position.y, player.position.z), chaseSpeed);
                if (!CanSee(loseRange)) { index = NearestPoint(); state = State.Return; }
                break;

            case State.Return:
                if (!retreating && CanSee(sightRange)) { state = State.Chase; break; }
                if (MoveTo(points[index].position, retreating ? retreatSpeed : patrolSpeed))
                {
                    if (retreating)
                    {
                        retreating = false;
                        waitUntil = Time.time + hitPause;
                        ignorePlayerUntil = waitUntil;
                    }
                    state = State.Patrol;
                }
                break;
        }

        bool resting = retreating || Time.time < ignorePlayerUntil;
        if (rend != null)
            rend.material.color = state == State.Chase ? Color.red : resting ? Color.gray : calmColor;
    }

    void OnTriggerStay(Collider other)
    {
        if (retreating || Time.time < ignorePlayerUntil) return;
        if (!other.TryGetComponent(out Health health)) return;

        if (health.TakeDamage(damage))
            Hazard.Knock(other, transform.position, knockback, knockTime);
        StartRetreat();
    }

    void StartRetreat()
    {
        retreating = true;
        index = NearestPoint();
        state = State.Return;
    }

    void Patrol()
    {
        if (Time.time < waitUntil) return;
        if (MoveTo(points[index].position, patrolSpeed))
        {
            index = NextIndex();
            waitUntil = Time.time + waitTime;
        }
    }

    // Moves one step, then reports arrival by distance (not ==).
    bool MoveTo(Vector3 target, float speed)
    {
        Vector3 toTarget = target - transform.position;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if (toTarget != Vector3.zero)
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, Quaternion.LookRotation(toTarget), turnSpeed * Time.deltaTime);
        return Vector3.Distance(transform.position, target) < arriveDistance;
    }

    bool CanSee(float range)
    {
        Vector3 toPlayer = player.position - transform.position;
        if (toPlayer.magnitude > range) return false;
        if (!needLineOfSight) return true;
        if (Physics.Raycast(transform.position, toPlayer.normalized, out RaycastHit hit,
                            range, ~0, QueryTriggerInteraction.Ignore))
            return hit.collider.CompareTag("Player");
        return false;
    }

    int NextIndex()
    {
        if (mode == PatrolMode.Loop) return (index + 1) % points.Length;
        if (mode == PatrolMode.PingPong)
        {
            if (index + step < 0 || index + step >= points.Length) step = -step;
            return index + step;
        }
        int next = Random.Range(0, points.Length);
        return next == index ? (next + 1) % points.Length : next;
    }

    int NearestPoint()
    {
        int best = 0; float bestDist = float.MaxValue;
        for (int i = 0; i < points.Length; i++)
        {
            float d = Vector3.Distance(transform.position, points[i].position);
            if (d < bestDist) { bestDist = d; best = i; }
        }
        return best;
    }
}