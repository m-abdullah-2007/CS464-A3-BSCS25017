using UnityEngine;

public class EnemyGuard : MonoBehaviour
{
    enum State { Patrol, Chase, Return }

    [SerializeField] Transform[] points;
    [SerializeField] PatrolMode mode = PatrolMode.Loop;
    [SerializeField] Transform player;
    [SerializeField] float patrolSpeed = 2.5f, chaseSpeed = 4.5f, turnSpeed = 360f;
    [SerializeField] float sightRange = 7f, loseRange = 9f, waitTime = 0f;
    [SerializeField] bool needLineOfSight = true;

    State state = State.Patrol;
    int index, step = 1;
    float waitUntil;
    Renderer rend;
    Color calmColor;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null) calmColor = rend.material.color;
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (points == null || points.Length == 0 || player == null) return;

        switch (state)
        {
            case State.Patrol:
                Patrol();
                if (CanSee(sightRange)) state = State.Chase;
                break;

            case State.Chase:
                MoveTo(new Vector3(player.position.x, transform.position.y, player.position.z), chaseSpeed);
                if (!CanSee(loseRange)) { index = NearestPoint(); state = State.Return; }
                break;

            case State.Return:
                if (CanSee(sightRange)) { state = State.Chase; break; }
                if (MoveTo(points[index].position, patrolSpeed)) state = State.Patrol;
                break;
        }

        if (rend != null) rend.material.color = state == State.Chase ? Color.red : calmColor;
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

    // Moves one step towards target, turns to face it, returns true once it has arrived.
    bool MoveTo(Vector3 target, float speed)
    {
        Vector3 toTarget = target - transform.position;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if (toTarget != Vector3.zero)
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, Quaternion.LookRotation(toTarget), turnSpeed * Time.deltaTime);
        return transform.position == target;
    }

    bool CanSee(float range)
    {
        Vector3 toPlayer = player.position - transform.position;
        if (toPlayer.magnitude > range) return false;
        if (!needLineOfSight) return true;
        // Walls block the view. Triggers (spikes, coins, other guards) are ignored.
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