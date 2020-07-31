using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleMovement : MonoBehaviour, IDamageable
{
    [SerializeField] List<Transform> waypoints = null;
    [SerializeField] float moveSpeed = 2f;
    SpriteRenderer myRenderer = null;
    Vector3 targetPosition = new Vector3(0f, 0f, 0f);
    AudioSource deathAudio = null;
    int waypointIndex = 0;
    bool isAlive = true;


    void Start()
    {
        myRenderer = GetComponent<SpriteRenderer>();
        deathAudio = GetComponent<AudioSource>();

        transform.position = waypoints[waypointIndex].transform.position;
    }

    void Update()
    {
        if (isAlive)
        {
            if (waypointIndex < waypoints.Count)
            {
                targetPosition = waypoints[waypointIndex].transform.position;

                // move right
                MoveToWaypoint(1, targetPosition);
            }
            else if (waypointIndex > 0)
            {
                // crappy bugfix for out of index issue
                if (waypointIndex == waypoints.Count) waypointIndex--;

                targetPosition = waypoints[waypointIndex].transform.position;

                // move left
                MoveToWaypoint(-1, targetPosition);
            }
        }
    }

    private void MoveToWaypoint(int movementDirection, Vector3 targetPosition)
    {
        var movementThisFrame = (movementDirection * moveSpeed) * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);

        if (transform.position == targetPosition)
        {
            transform.localScale = new Vector2(Mathf.Sign(movementDirection), 1f);
            waypointIndex += movementDirection;
        }
    }

    public void TakeDamage()
    {
        Animator enemyAnimator = GetComponent<Animator>();

        isAlive = false;
        deathAudio.Play();
        CapsuleCollider2D capsule = GetComponent<CapsuleCollider2D>();
        capsule.offset = new Vector2(capsule.offset.x, capsule.offset.y - 10f);

        // Destroy(GetComponent<CapsuleCollider2D>());

        enemyAnimator.SetTrigger("enemyDeath");
        Destroy(gameObject, enemyAnimator.GetCurrentAnimatorStateInfo(0).length);
    }
}
