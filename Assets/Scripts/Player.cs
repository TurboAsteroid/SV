﻿using UnityEngine;
using System.Collections;
using Prime31;


public class Player : MonoBehaviour, IDamagable
{
	// movement config
	public float gravity = -25f;
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;

	[HideInInspector]
	protected CharacterController2D _controller;
	protected Animator _animator;
	protected RaycastHit2D _lastControllerColliderHit;
	protected Vector2 _velocity;
	public int Health {get; set;}
	void Awake()
	{
		_animator = GetComponentInChildren<Animator>();
		_controller = GetComponent<CharacterController2D>();

		// listen to some events for illustration purposes
		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;
	}

	void Start() {
		Health = 3;
	}


	#region Event Listeners

	void onControllerCollider( RaycastHit2D hit )
	{
		// bail out on plain old ground hits cause they arent very interesting
		if( hit.normal.y == 1f )
			return;

		// logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
		//Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
	}


	void onTriggerEnterEvent( Collider2D col )
	{
		Debug.Log( "onTriggerEnterEvent: " + col.gameObject.name );
	}


	void onTriggerExitEvent( Collider2D col )
	{
		Debug.Log( "onTriggerExitEvent: " + col.gameObject.name );
	}

	#endregion


	// the Update loop contains a very simple example of moving the character around and controlling the animation
	void Update()
	{
		AllMovement();
		Attac();
	}

	protected void Attac(){
		if (Input.GetButtonDown("Fire1") && _controller.isGrounded) {
            _animator.SetTrigger("Attack");
        }
	}
	protected void AllMovement(){
		FlipModel(Input.GetAxis ("Horizontal"));
		VerticalMovement();
		HorisontalMovement();

		_controller.move( _velocity * Time.deltaTime );

		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;
	}
	protected void HorisontalMovement(){
		// apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
		var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp( _velocity.x, Input.GetAxis ("Horizontal") * runSpeed, Time.deltaTime * smoothedMovementFactor );
		_animator.SetFloat ("velocityX", Mathf.Abs (_velocity.x));
	}
	protected void VerticalMovement(){
		_animator.SetBool ("grounded", _controller.isGrounded);
		if (_controller.isGrounded) {
			_velocity.y = 0;
            _controller.doubleJump = false;
        }
		// we can only jump whilst grounded
		if( _controller.isGrounded && Input.GetButtonDown( "Jump" ) )
		{
			_velocity.y = Mathf.Sqrt( 2f * jumpHeight * -gravity );
		}
		if (Input.GetButtonDown("Jump") && !_controller.isGrounded && !_controller.doubleJump) {
            _velocity.y = Mathf.Sqrt( 2f * jumpHeight * -gravity );
            _animator.SetTrigger("DoubleJump");
            _controller.doubleJump = true;
        }
		
		_animator.SetFloat("velocityY", _velocity.y);
		
		// apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;

		// if holding down bump up our movement amount and turn off one way platform detection for a frame.
		// this lets us jump down through one way platforms
		if( _controller.isGrounded && Input.GetKey( KeyCode.DownArrow ) )
		{
			_velocity.y *= 3f; //из-за этого можно делать паурджамп, без этого не проваливается сквозь односторонние платформы
			_controller.ignoreOneWayPlatformsThisFrame = true;
		}
	}
	protected void FlipModel(float moveX) {
        Vector2 lTemp = transform.localScale;
        bool flipSprite = (lTemp.x < 0 ? (moveX > 0f) : (moveX < 0f));

        if (flipSprite) 
        {   
            lTemp.x *= -1;
            transform.localScale = lTemp;
        }
    }
	
    public void Damage () {
        Health--;
        Debug.Log(Health);

        _animator.SetTrigger("attacked");

        if (Health < 1) {
            Death();
        }
    }
    public void Death() {
        _animator.SetTrigger("death");
    }
}
