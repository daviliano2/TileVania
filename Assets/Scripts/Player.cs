using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    const string IS_RUNNING = "isRunning";
    const string IS_JUMPING = "isJumping";
    const string IS_FALLING = "isFalling";
    const string IS_CLIMBING = "isClimbing";
    const string IS_CROUCHING = "isCrouching";
    const string DEATH_TRIGGER = "death";

    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(0, 10f);
    [SerializeField] AudioClip jumpAudio = null;
    [SerializeField] AudioClip deathAudio = null;
    Rigidbody2D myRigidBody = null;
    Animator myAnimator = null;
    SpriteRenderer myRenderer = null;
    LayerMask groundLayer;
    LayerMask climbingLayer;
    LayerMask enemyLayer;
    LayerMask hazardsLayer;
    CapsuleCollider2D bodyCollider = null;
    BoxCollider2D feetCollider = null;

    bool isAlive = true;
    bool isTouchingGround = true;
    bool isTouchingLadder = false;
    bool isCrouching = false;
    float startingGravityScale = 0f;
    float originalBodyColliderOffset = 0f;
    float originalBodyColliderSize = 0f;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myRenderer = GetComponent<SpriteRenderer>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        feetCollider = GetComponent<BoxCollider2D>();
        groundLayer = LayerMask.GetMask("Ground");
        climbingLayer = LayerMask.GetMask("Climbing");
        enemyLayer = LayerMask.GetMask("Enemy");
        hazardsLayer = LayerMask.GetMask("Hazard");
        startingGravityScale = myRigidBody.gravityScale;
        originalBodyColliderOffset = bodyCollider.offset.y;
        originalBodyColliderSize = bodyCollider.size.y;
    }

    void Update()
    {
        isTouchingGround = feetCollider.IsTouchingLayers(groundLayer);
        isTouchingLadder = feetCollider.IsTouchingLayers(climbingLayer);
        bool isTouchingHazard =
            feetCollider.IsTouchingLayers(hazardsLayer) || bodyCollider.IsTouchingLayers(hazardsLayer);
        if (isAlive)
        {
            Run();
            Jump(isTouchingGround, isTouchingLadder);
            Crouch(isTouchingGround);
            IsJumpingOrFalling(isTouchingGround, isTouchingLadder);
            ClimbLadder(isTouchingLadder);
            Die(isTouchingHazard);
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        // Kill enemy functionality
        IDamageable dmgObject = otherCollider.GetComponent<IDamageable>();
        if (dmgObject != null)
        {
            // crappy bug fix for high velocity collisions
            otherCollider.offset = new Vector2(otherCollider.offset.x, otherCollider.offset.y - 10f);
            myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, jumpSpeed);

            if (!otherCollider.IsTouching(feetCollider))
            {
                Die(false, otherCollider.IsTouching(bodyCollider));
            }
            dmgObject?.TakeDamage();
        }
    }

    private void ClimbLadder(bool isTouchingLadder)
    {
        if (isTouchingLadder)
        {
            myRigidBody.gravityScale = 0f;
            float controlInput = Input.GetAxis("Vertical");
            bool isPlayerClimbing = controlInput != 0;
            Vector2 playerVelocity = new Vector2(myRigidBody.velocity.x, controlInput * runSpeed);
            myRigidBody.velocity = playerVelocity;
            myAnimator.SetBool(IS_CLIMBING, isPlayerClimbing);
        }
        else
        {
            myRigidBody.gravityScale = startingGravityScale;
            myAnimator.SetBool(IS_CLIMBING, false);
        }
    }

    private void Run()
    {
        if (!isCrouching)
        {
            float controlInput = Input.GetAxis("Horizontal");
            bool isPlayerRunning = controlInput != 0; // or Mathf.Abs(myRigidBody.velocity.x) > 0
            Vector2 playerVelocity = new Vector2(controlInput * runSpeed, myRigidBody.velocity.y);
            myRigidBody.velocity = playerVelocity;
            myAnimator.SetBool(IS_RUNNING, isPlayerRunning);
            FlipX(isPlayerRunning);
        }
        else
        {
            myAnimator.SetBool(IS_RUNNING, false);
            myRigidBody.velocity = new Vector2(0f, 0f);
        }
    }

    private void Jump(bool isTouchingGround, bool isTouchingLadder)
    {
        if (!isCrouching && ((isTouchingGround || isTouchingLadder) && Input.GetButtonDown("Jump")))
        {
            AudioSource.PlayClipAtPoint(jumpAudio, gameObject.transform.position, 0.5f);
            Vector2 addJumpVelocity = new Vector2(0f, jumpSpeed);
            myRigidBody.velocity += addJumpVelocity;
        }
    }

    private void Crouch(bool isTouchingGround)
    {
        if (isTouchingGround && Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (Input.GetKey(KeyCode.DownArrow))
            {
                myAnimator.SetBool(IS_CROUCHING, true);
                isCrouching = true;
                UpdateBodyCollider(-0.5f, 0.3f);
            }
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            myAnimator.SetBool(IS_CROUCHING, false);
            isCrouching = false;
            UpdateBodyCollider(originalBodyColliderOffset, originalBodyColliderSize);
        }
    }

    private void UpdateBodyCollider(float offsetY, float sizeY)
    {
        bodyCollider.offset = new Vector2(bodyCollider.offset.x, offsetY);
        bodyCollider.size = new Vector2(bodyCollider.size.x, sizeY);
    }

    private void Die(bool isTouchingHazard, bool isTouchingBody = false)
    {
        if (isTouchingHazard || isTouchingBody)
        {
            AudioSource.PlayClipAtPoint(deathAudio, gameObject.transform.position, 0.5f);
            isAlive = false;
            myAnimator.SetTrigger(DEATH_TRIGGER);
            myRigidBody.velocity = deathKick;

            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }

    private void FlipX(bool isMovingHorizontally)
    {
        // Math.Sign(f) returns 1 when "f" is positive or 0, returns -1 if it's negative
        if (isMovingHorizontally) myRenderer.flipX = Mathf.Sign(myRigidBody.velocity.x) == -1;
    }

    private void IsJumpingOrFalling(bool isTouchingGround, bool isTouchingLadder)
    {
        if (!isTouchingGround && !isTouchingLadder)
        {
            myAnimator.SetBool(IS_JUMPING, myRigidBody.velocity.y > 0);
            myAnimator.SetBool(IS_FALLING, myRigidBody.velocity.y < 0);
        }
        else
        {
            myAnimator.SetBool(IS_JUMPING, false);
            myAnimator.SetBool(IS_FALLING, false);
        }
    }
}
