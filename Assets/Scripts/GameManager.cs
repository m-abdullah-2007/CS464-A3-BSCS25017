using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] int targetScore = 8, lives = 3;
    [SerializeField] float timeLimit = 0f;          // 0 = no timer
    [SerializeField] float respawnDelay = 0.6f, spawnProtection = 1.5f;
    [SerializeField] Health player;
    [SerializeField] Transform respawnPoint;
    [SerializeField] TMP_Text scoreText, livesText, timerText;
    [SerializeField] GameObject winPanel, losePanel;

    int score, bonus;
    float timeLeft;
    bool ended;

    void Awake() { Instance = this; Time.timeScale = 1f; timeLeft = timeLimit; }

    void Start()
    {
        if (timerText != null) timerText.gameObject.SetActive(timeLimit > 0f);
        if (scoreText != null) scoreText.gameObject.SetActive(targetScore > 0);
        UpdateUI();
    }

    void Update()
    {
        if (ended || timeLimit <= 0f) return;
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f) { timeLeft = 0f; UpdateUI(); Lose(); return; }
        UpdateUI();
    }

    public void AddScore(int amount)
    {
        if (ended) return;
        score += amount; UpdateUI();
        if (targetScore > 0 && score >= targetScore) Win();
    }

    // Kept so the Collect-a-thon Coin.cs compiles unchanged.
    public void CollectRedCoin(Vector3 playerPosition) { AddScore(1); }
    public void AddBonus() { bonus++; }

    // Called by GoalZone when the player touches the goal pad.
    public void ReachGoal() { Win(); }

    public void UpdateUI()
    {
        if (scoreText != null && targetScore > 0)
            scoreText.text = $"Score: {score} / {targetScore}";
        livesText.text = $"Lives: {lives}   Health: {Mathf.Max(player.Current, 0)} / {player.Max}";
        if (timerText != null && timeLimit > 0f)
            timerText.text = $"Time: {Mathf.CeilToInt(timeLeft)}";
    }

    public void PlayerDied()
    {
        if (ended) return;
        lives--; UpdateUI();
        if (lives <= 0) Lose(); else StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        Rigidbody body = player.GetComponent<Rigidbody>();
        body.position = respawnPoint.position; body.linearVelocity = Vector3.zero;
        player.Restore(spawnProtection);
    }

    void Win()
    {
        if (ended) return;
        ended = true;
        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void Lose()
    {
        if (ended) return;
        ended = true;
        losePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(next < SceneManager.sceneCountInBuildSettings ? next : 0);
    }
}