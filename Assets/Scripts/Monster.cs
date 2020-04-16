using UnityEngine;
using System.Collections;
using Prime31;
using System.Collections.Generic;

public class Monster : MonoBehaviour, IDamagable
{
	// movement config
	public float runSpeed = 8f;
	public List<GameObject> wayPoints = new List<GameObject>();
	
	[HideInInspector]
	protected Animator _animator;
	protected Vector2 _velocity;
	public int Health {get; set;}
	protected int currentWayPoint = 0;

	void Start() {
		_animator = GetComponentInChildren<Animator>();
		Health = 3;
	}

	// the Update loop contains a very simple example of moving the character around and controlling the animation
	void Update()
	{
		AllMovement();
	}

	protected void AllMovement(){
		HorisontalMovement();

		transform.Translate(_velocity);
	}
	protected void HorisontalMovement(){
		if (wayPoints.Count > 0 && !(
			_animator.GetCurrentAnimatorStateInfo(0).IsName("idle") ||
			_animator.GetCurrentAnimatorStateInfo(0).IsName("hit") ||
			_animator.GetCurrentAnimatorStateInfo(0).IsName("death")
		)
		) {
			FlipModel(_velocity.x);
			
			Vector2 enemyPosition = transform.position;
			Vector2 wayPointPosition = wayPoints[currentWayPoint].transform.position;

			bool isIn = true;

			for( int i = 0 ; i < 2 && isIn ; ++i )
			{
				if( enemyPosition[i] < wayPointPosition[i] - 0.4 || enemyPosition[i] > wayPointPosition[i] + 0.4 ) isIn = false ;
			}
			
			if (isIn) {
				_animator.SetTrigger("Idle");
				currentWayPoint = (currentWayPoint + 1) % wayPoints.Count;
			}

			_velocity.x = Vector3.Normalize(wayPointPosition - enemyPosition).x * runSpeed / 60;
			
		} else {
			_velocity.x = 0;
		}
		
		_animator.SetFloat ("velocityX", Mathf.Abs (_velocity.x));
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
        _animator.SetTrigger("hit");

        if (Health < 1) {
            Death();
        }
    }
    public void Death() {
        _animator.SetTrigger("death");
    }
}
