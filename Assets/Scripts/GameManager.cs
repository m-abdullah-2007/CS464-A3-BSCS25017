using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] int targetScore = 8, lives = 3;
    [SerializeField] float respawnDelay = 0.6f, spawnProtection = 1.5f;
    [SerializeField] Health player;
    [SerializeField] Transform respawnPoint;
    [SerializeField] TMP_Text scoreText, livesText;
    [SerializeField] GameObject winPanel, losePanel;

    int score, bonus;

    void Awake() { Instance = this; Time.timeScale = 1f; }
    void Start() => UpdateUI();

    public void AddScore(int amount)
    {
        score += amount; UpdateUI();
        if (score >= targetScore) Win();
    }

    // Kept so the Collect-a-thon Coin.cs compiles unchanged (not used in Level 01).
    public void CollectRedCoin(Vector3 playerPosition) { AddScore(1); }
    public void AddBonus() { bonus++; }

    public void UpdateUI()
    {
        scoreText.text = $"Score: {score} / {targetScore}";
        livesText.text = $"Lives: {lives}   Health: {Mathf.Max(player.Current, 0)} / {player.Max}";
    }

    public void PlayerDied()
    {
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

    void Win() { winPanel.SetActive(true); Time.timeScale = 0f; }
    void Lose() { losePanel.SetActive(true); Time.timeScale = 0f; }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // For the Next Level button. Loads the next scene in Build Settings, or scene 0 after the last.
    public void NextLevel()
    {
        Time.timeScale = 1f;
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(next < SceneManager.sceneCountInBuildSettings ? next : 0);
    }
}