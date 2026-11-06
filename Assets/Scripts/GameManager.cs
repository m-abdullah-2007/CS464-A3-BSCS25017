using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] int targetScore = 8, lives = 3;
    [SerializeField] bool scoreWins = true;          // false = coins are bonus only
    [SerializeField] bool goalNeedsScore = false;    // true = exit locked until targetScore coins
    [SerializeField] Renderer goalPad;               // optional: turns red/green with the lock
    [SerializeField] float timeLimit = 0f;           // 0 = no timer
    [SerializeField] bool surviveToWin = false;      // true = timer reaching 0 is a win
    [SerializeField] string timerLabel = "Time";
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
        SetGoalColor();
        UpdateUI();
    }

    void Update()
    {
        if (ended || timeLimit <= 0f) return;
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f; UpdateUI();
            if (surviveToWin) Win(); else Lose();
            return;
        }
        UpdateUI();
    }

    public void AddScore(int amount)
    {
        if (ended) return;
        score += amount;
        SetGoalColor();
        UpdateUI();
        if (scoreWins && targetScore > 0 && score >= targetScore) Win();
    }

    // Kept so the Collect-a-thon Coin.cs compiles unchanged.
    public void CollectRedCoin(Vector3 playerPosition) { AddScore(1); }
    public void AddBonus() { bonus++; }

    // Called by GoalZone. Ignored while the exit is still locked.
    public void ReachGoal()
    {
        if (goalNeedsScore && score < targetScore) return;
        Win();
    }

    void SetGoalColor()
    {
        if (goalPad == null || !goalNeedsScore) return;
        goalPad.material.color = score >= targetScore ? Color.green : Color.red;
    }

    public void UpdateUI()
    {
        if (scoreText != null && targetScore > 0)
        {
            if (goalNeedsScore)
                scoreText.text = $"Coins: {score} / {targetScore}   " +
                                 (score >= targetScore ? "EXIT OPEN" : "exit locked");
            else
                scoreText.text = scoreWins ? $"Score: {score} / {targetScore}"
                                           : $"Bonus coins: {score} / {targetScore}";
        }
        livesText.text = $"Lives: {lives}   Health: {Mathf.Max(player.Current, 0)} / {player.Max}";
        if (timerText != null && timeLimit > 0f)
            timerText.text = $"{timerLabel}: {Mathf.CeilToInt(timeLeft)}";
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
        if (IsLastLevel()) HideNextButton();
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

    // Loads the next scene in Build Settings. Does nothing on the last level,
    // so it never wraps back to Level01.
    public void NextLevel()
    {
        if (IsLastLevel()) return;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    bool IsLastLevel()
    {
        return SceneManager.GetActiveScene().buildIndex + 1 >= SceneManager.sceneCountInBuildSettings;
    }

    // Finds the NextLevelButton by name, including inactive objects inside the
    // win panel, and hides it. If the name doesn't match, nothing happens.
    void HideNextButton()
    {
        Transform[] all = Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Transform t in all)
            if (t.name == "NextLevelButton") t.gameObject.SetActive(false);
    }
}