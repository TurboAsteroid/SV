using UnityEngine;
using System.Collections;
using Prime31;
using System.Collections.Generic;

public class Monster : MonoBehaviour
{
	// movement config
	public float gravity = -25f;
	public float runSpeed = 8f;
	public List<GameObject> wayPoints = new List<GameObject>();
	
	[HideInInspector]
	protected CharacterController2D _controller;
	protected Animator _animator;
	protected RaycastHit2D _lastControllerColliderHit;
	protected Vector2 _velocity;
	protected int Health {get; set;}
	protected int currentWayPoint = 0;

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
	}
	protected void AllMovement(){
		VerticalMovement();
		HorisontalMovement();

		_controller.move( _velocity * Time.deltaTime );

		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;
	}
	protected void HorisontalMovement(){
		if (wayPoints.Count > 0) {
			if (wayPoints[currentWayPoint]) {
				Vector2 enemyPosition = transform.position;
				Vector2 wayPointPosition = wayPoints[currentWayPoint].transform.position;

				bool isIn = true;
 
				for( int i = 0 ; i < 2 && isIn ; ++i )
				{
					if( enemyPosition[i] < wayPointPosition[i] - 0.5 || enemyPosition[i] > wayPointPosition[i] + 0.5 ) isIn = false ;
				}
				
				if (isIn) {
					currentWayPoint = (currentWayPoint + 1) % wayPoints.Count;
				}

				_velocity.x = Mathf.Lerp( _velocity.x, Vector3.Normalize(wayPointPosition - enemyPosition).x * runSpeed, Time.deltaTime * 20 );
			}
		} else {
			_velocity.x = 0;
		}

		FlipModel(_velocity.x);
		_animator.SetFloat ("velocityX", Mathf.Abs (_velocity.x));
	}
	protected void VerticalMovement(){
		_velocity.y = 0;
		
		// apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;

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
