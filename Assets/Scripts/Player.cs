using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

	[Header("Dash info")]
	[SerializeField] private float dashSpeed;
	[SerializeField] private float dashDuration;
	private float dashTime;

	[SerializeField] private float dashCooldown;
	private float dashCooldownTimer;

	[Header("Collision info")]
	[SerializeField] private float groundCheckDistance;
	[SerializeField] private LayerMask whatisGround;
	private bool isGrounded;

	[Header("Attack info")]
	[SerializeField] private float comboTimer = .3f;
	private float comboTimeWindow;
	private bool isAttacking;
	private int comboCouter;

    private bool isMoving;
    private float xInput;
	private bool facingRight = true;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Update()
	{
		Movement();
		CheckInput();
		CollisionCheck();
		dashTime -= Time.deltaTime;
		dashCooldownTimer -= Time.deltaTime;
		comboTimeWindow -= Time.deltaTime;
		CheckFlip();
		AnimationController();
	}
	public void AttackOver()
	{
		isAttacking = false;
		comboCouter++;
		if (comboCouter > 2)	
			comboCouter = 0;

	}
	private void CollisionCheck()
	{
		isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatisGround);
	}

	private void AnimationController()
	{
		isMoving = rb.velocity.x != 0f;
		animator.SetFloat("yVelocity", rb.velocity.y);
		animator.SetBool("isMoving", isMoving);
		animator.SetBool("isGround", isGrounded);
		animator.SetBool("isDashing", dashTime > 0);
		animator.SetBool("isAttacking", isAttacking);
		animator.SetInteger("comboCouter", comboCouter);
	}

	private void CheckInput()
	{
		xInput = Input.GetAxisRaw("Horizontal");

		if (Input.GetButtonDown("Jump"))
		{
			Jumping();
		}
		StartAttackEvent();
		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			Dash();
		}
	}

	private void StartAttackEvent()
	{
		if (!isGrounded)
			return;

		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			if (comboTimeWindow < 0)
				comboCouter = 0;

			isAttacking = true;
			comboTimeWindow = comboTimer;
		}
	}

	private void Dash()
	{
		if(dashCooldownTimer < 0 && isAttacking)
		{
			dashCooldownTimer = dashCooldown;
			dashTime = dashDuration;
		}
	}
	private void Movement()
	{
		if (isAttacking)
		{
			rb.velocity = new Vector2(0, 0);
		}
		else if (dashTime > 0)
		{
			rb.velocity = new Vector2(xInput * dashSpeed, 0);
		}
		else
		{
			rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
		}
	}
	private void Jumping()
	{
		if(isGrounded)
			rb.velocity = new Vector2(rb.velocity.x, jumpForce);
	}
	private void CheckFlip()
	{
        if (rb.velocity.x < 0 && facingRight)
        {
            Flip();
        }
		if (rb.velocity.x > 0 && !facingRight)
		{
			Flip();
		}
    }
	private void Flip()
	{
		facingRight = !facingRight;
		transform.Rotate(0, 180, 0);
	}
	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckDistance));
	}
}
