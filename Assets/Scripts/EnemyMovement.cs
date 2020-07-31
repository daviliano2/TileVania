using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour, IDamageable
{
    [SerializeField] float movementSpeed = 1f;
    Rigidbody2D myRigidBody = null;
    SpriteRenderer myRenderer = null;
    CapsuleCollider2D capsuleCollider = null;
    BoxCollider2D boxCollider = null;
    AudioSource deathAudio = null;

    bool isAlive = true;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        deathAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        bool isFacingRight = IsEnemyFacingRight();

        if (isAlive)
        {
            if (isFacingRight)
            {
                myRigidBody.velocity = new Vector2(movementSpeed, 0f);
            }
            else
            {
                myRigidBody.velocity = new Vector2(-movementSpeed, 0f);
            }
        }
        else
        {
            myRigidBody.velocity = new Vector2(0f, 0f);
        }
    }

    public void TakeDamage()
    {
        isAlive = false;
        deathAudio.Play();
        Animator enemyAnimator = GetComponent<Animator>();
        capsuleCollider.offset = new Vector2(capsuleCollider.offset.x, capsuleCollider.offset.y - 10f);
        boxCollider.offset = new Vector2(boxCollider.offset.x, boxCollider.offset.y - 10f);

        enemyAnimator.SetTrigger("enemyDeath");
        Destroy(gameObject, enemyAnimator.GetCurrentAnimatorStateInfo(0).length);
    }

    bool IsEnemyFacingRight()
    {
        return transform.localScale.x > 0;
    }

    private void OnTriggerExit2D(Collider2D otherCollider) => transform.localScale = new Vector2(-(Mathf.Sign(myRigidBody.velocity.x)), 1f);
}
