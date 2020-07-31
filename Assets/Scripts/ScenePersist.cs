using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePersist : MonoBehaviour
{
    int startingSceneIndex = 0;
    private void Awake()
    {
        int numberScenePersist = FindObjectsOfType<ScenePersist>().Length;
        
        if (numberScenePersist > 1) Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        startingSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    void Update()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        if (currentSceneIndex != startingSceneIndex) Destroy(gameObject);
    }
}
