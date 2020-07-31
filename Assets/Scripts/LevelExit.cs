using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] float slowmoTime = 0.2f;

    AudioSource exitAudio = null;

    void Start()
    {
        exitAudio = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        StartCoroutine(LoadNextLevel());
    }

    IEnumerator LoadNextLevel()
    {
        Time.timeScale = slowmoTime;
        exitAudio.Play();
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        Time.timeScale = 1f;

        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);

        // shitty workaround to avoid bug when a new scene loads. The singleton pattern
        // keeps the previous ScenePersist object meaning that there are no gems to pickup
        // also the score is not reseted to 0, that's why we do it manually.
        Destroy(FindObjectOfType<ScenePersist>().gameObject);
        FindObjectOfType<GameSession>().resetScore();
    }
}
