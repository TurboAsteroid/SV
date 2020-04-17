using UnityEngine;
using System.Collections;
using Prime31;
using System.Collections.Generic;
using System;

public class Monster : MonoBehaviour, IDamagable
{
	// movement config
	public float runSpeed = 2f;
	public List<GameObject> wayPoints = new List<GameObject>();
	
	[HideInInspector]
	protected Animator _animator;
	protected Vector2 _velocity;
	public int Health {get; set;}
	protected int _currentWayPoint = 0;

	public float _lookingRange = 4f;
	public float _battleRange = 7f;
	protected bool _combatMode = false;
	protected GameObject _player;
	protected Vector2 _lastTargetPosition;
	protected float _attackRange = 7f;

	void Start() {
		_animator = GetComponentInChildren<Animator>();
		_player = GameObject.FindGameObjectWithTag("Player");
		Health = 3;
	}

	// the Update loop contains a very simple example of moving the character around and controlling the animation
	void Update()
	{
		Combat();
		AllMovement();
		
	}


	protected void Combat () {
		CheckTarget();
	}
	protected void CheckTarget() {
		if (_player == null) {
			_combatMode = false;
		}
		if (!_combatMode) {
			RaycastHit2D hit;
			if (_velocity.x >= 0) {
				hit = Physics2D.Raycast(transform.position, Vector2.right, _lookingRange, 1 << 8);
				if (hit.collider != null) {
					_combatMode = true;
					_lastTargetPosition = _player.transform.position;
				}
			}
			if (_velocity.x <= 0 && !_combatMode){
				hit = Physics2D.Raycast(transform.position, Vector2.left, _lookingRange, 1 << 8);
				if (hit.collider != null) {
					_combatMode = true;
					_lastTargetPosition = _player.transform.position;
				}
			}
		}
		if (_combatMode) {
			if (Vector3.SqrMagnitude(transform.position - _player.transform.position) <= Math.Pow(_battleRange, 2)) {
				_lastTargetPosition = _player.transform.position;
				goToAttak();
			} else {
				_combatMode = false;
			}
		}
	}
	protected void goToAttak() {
		if (Vector3.SqrMagnitude(transform.position - _player.transform.position) <= Math.Pow(_attackRange, 2)) {
			_animator.SetTrigger("attack");
			//Debug.Log("IN RANGE!");
		}
	}
	public virtual void Attack() {
		//Debug.Log("Attack!!!!!!!!");
	}
	protected void AllMovement(){
		HorisontalMovement();

		transform.Translate(_velocity);
	}
	protected void HorisontalMovement(){
		if (
			!_animator.GetCurrentAnimatorStateInfo(0).IsName("idle") &&
			!_animator.GetCurrentAnimatorStateInfo(0).IsName("hit") &&
			!_animator.GetCurrentAnimatorStateInfo(0).IsName("death") &&
			!_animator.GetCurrentAnimatorStateInfo(0).IsName("attack")
		) {
			Vector2 myPosition = transform.position;
			Vector2 wayPointPosition = _combatMode ? _lastTargetPosition : (Vector2)wayPoints[_currentWayPoint].transform.position;

			bool isIn = true;

			for( int i = 0 ; i < 2 && isIn ; ++i )
			{
				if( myPosition[i] < wayPointPosition[i] - 0.4 || myPosition[i] > wayPointPosition[i] + 0.4 ) isIn = false ;
			}
			
			if (isIn && !_combatMode) {
				_animator.SetTrigger("Idle");
				_currentWayPoint = (_currentWayPoint + 1) % wayPoints.Count;
			} else if (isIn && !_combatMode) {
				
			}

			_velocity.x = Vector3.Normalize(wayPointPosition - myPosition).x * runSpeed / 60;
			
		} else {
			_velocity.x = 0;
		}
		FlipModel();
		_animator.SetFloat ("velocityX", Mathf.Abs (_velocity.x));
	}
	protected void FlipModel() {
		bool flipSprite;
		Vector2 lTemp = transform.localScale;
		float pos1 = _velocity.x;
		float pos2 = 0f;
		if (_combatMode) {
			pos1 = _player.transform.position.x;
			pos2 = transform.position.x;
		}
		flipSprite = (lTemp.x < 0 ? (pos1 > pos2) : (pos1 < pos2));

        if (flipSprite) {   
            lTemp.x *= -1;
            transform.localScale = lTemp;
        }
    }
	
    public void Damage () {
        Health--;
		Debug.Log(Health);
        _animator.SetTrigger("hit");
		_combatMode = true;

        if (Health < 1) {
            Death();
        }
    }
    public void Death() {
        _animator.SetTrigger("death");
    }
}
