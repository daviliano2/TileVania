using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemPickup : MonoBehaviour
{
    AudioSource gemAudio = null;

    private void Start()
    {
        gemAudio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        gemAudio.Play();
        Destroy(GetComponent<BoxCollider2D>());
        FindObjectOfType<GameSession>().AddGemsToScore();

        Animator gemAnimator = GetComponent<Animator>();
        gemAnimator.SetTrigger("pickup");

        Destroy(GetComponent<SpriteRenderer>(), 0.4f);
        Destroy(gameObject, gemAudio.clip.length);
    }
}
