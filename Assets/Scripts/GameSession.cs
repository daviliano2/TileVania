using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{
    [SerializeField] int playerLives = 3;
    [SerializeField] int score = 0;
    [SerializeField] int maxScore = 9;
    [SerializeField] Text livesText = null;
    [SerializeField] Text scoreText = null;

    AudioSource lifeUpAudio = null;

    private void Awake()
    {
        int numberGameSessions = FindObjectsOfType<GameSession>().Length;

        if (numberGameSessions > 1) Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        livesText.text = playerLives.ToString();
        scoreText.text = $"{score}/{maxScore}";
        lifeUpAudio = GetComponent<AudioSource>();
    }

    public void ProcessPlayerDeath()
    {
        if (playerLives > 1) StartCoroutine(TakeOneLife());
        else StartCoroutine(ResetGameSession());
    }

    public void AddGemsToScore()
    {
        score++;
        scoreText.text = $"{score}/{maxScore}";
        if (score == maxScore)
        {
            playerLives++;
            livesText.text = playerLives.ToString();
            lifeUpAudio.Play();
        }
    }

    IEnumerator TakeOneLife()
    {
        yield return new WaitForSecondsRealtime(1f);

        playerLives--;
        livesText.text = playerLives.ToString();
        // resetScore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator ResetGameSession()
    {
        yield return new WaitForSecondsRealtime(1f);

        resetScore();
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }

    public void resetScore()
    {
        score = 0;
        scoreText.text = $"{score}/{maxScore}";
    }
}
