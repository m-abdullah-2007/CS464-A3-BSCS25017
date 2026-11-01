using UnityEngine;

public enum CoinType { Normal, Red, Blue }

public class Coin : MonoBehaviour
{
    [SerializeField] int value = 1;
    [SerializeField] float spinSpeed = 90f;

    CoinType type = CoinType.Normal;

    void Awake() => ApplyColor();

    // Called by the Spawner right after Instantiate
    public void Init(CoinType newType)
    {
        type = newType;
        ApplyColor();
    }

    void ApplyColor()
    {
        Renderer r = GetComponent<Renderer>();
        if (r == null) return;
        switch (type)
        {
            case CoinType.Red: r.material.color = Color.red; break;
            case CoinType.Blue: r.material.color = Color.blue; break;
            default: r.material.color = Color.yellow; break;
        }
    }

    void Update() => transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        switch (type)
        {
            case CoinType.Normal:
                GameManager.Instance.AddScore(value);
                break;
            case CoinType.Red:
                GameManager.Instance.CollectRedCoin(other.transform.position);
                break;
            case CoinType.Blue:
                GameManager.Instance.AddBonus();
                break;
        }
        Destroy(gameObject);
    }
}