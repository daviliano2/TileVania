using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogJump : MonoBehaviour, IDamageable
{
    const string IS_JUMPING = "isJumping";
    const string IS_FALLING = "isFalling";

    [SerializeField] int numberOfJumps = 1;
    [SerializeField] float movementSpeed = 1f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float timeBeforeJump = 1f;
    [SerializeField] float timeAfterJump = 1f;

    Rigidbody2D myRigidBody = null;
    SpriteRenderer myRenderer = null;
    CapsuleCollider2D capsuleCollider = null;
    BoxCollider2D feetCollider = null;
    Animator enemyAnimator = null;
    AudioSource deathAudio = null;


    LayerMask groundLayer;

    bool isAlive = true;
    bool isTouchingGround = true;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        feetCollider = GetComponent<BoxCollider2D>();
        enemyAnimator = GetComponent<Animator>();
        deathAudio = GetComponent<AudioSource>();

        groundLayer = LayerMask.GetMask("Ground");

        StartCoroutine(Jump());
    }

    IEnumerator Jump()
    {
        int jumpsMade = 0;
        int movementDirection = 1;

        if (numberOfJumps != 0)
        {
            while (true)
            {
                yield return new WaitForSeconds(timeBeforeJump);

                transform.localScale = new Vector2(Mathf.Sign(movementDirection), 1f);
                myRigidBody.velocity += new Vector2(movementSpeed * movementDirection, jumpSpeed);
                jumpsMade++;

                if (jumpsMade == numberOfJumps)
                {
                    movementDirection *= -1;
                    jumpsMade = 0;
                }

                yield return new WaitForSeconds(timeAfterJump);

                myRigidBody.velocity = new Vector2(0f, 0f);
            }
        }
    }

    void Update()
    {
        isTouchingGround = feetCollider.IsTouchingLayers(groundLayer);

        IsJumpingOrFalling(isTouchingGround);

        if (!isAlive)
        {
            myRigidBody.velocity = new Vector2(0f, 0f);
        }
    }

    private void IsJumpingOrFalling(bool isTouchingGround)
    {
        if (!isTouchingGround)
        {
            enemyAnimator.SetBool(IS_JUMPING, myRigidBody.velocity.y > 0);
            enemyAnimator.SetBool(IS_FALLING, myRigidBody.velocity.y < 0);
        }
        else
        {
            enemyAnimator.SetBool(IS_JUMPING, false);
            enemyAnimator.SetBool(IS_FALLING, false);
        }
    }

    public void TakeDamage()
    {
        isAlive = false;
        deathAudio.Play();
        capsuleCollider.offset = new Vector2(capsuleCollider.offset.x, capsuleCollider.offset.y - 10f);
        feetCollider.offset = new Vector2(feetCollider.offset.x, feetCollider.offset.y - 10f);

        // Destroy(GetComponent<CapsuleCollider2D>());

        enemyAnimator.SetTrigger("enemyDeath");
        Destroy(gameObject, enemyAnimator.GetCurrentAnimatorStateInfo(0).length);
    }
}
