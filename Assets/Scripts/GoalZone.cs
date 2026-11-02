using UnityEngine;

public class GoalZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) GameManager.Instance.ReachGoal();
    }
}