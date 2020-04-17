using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sparkle : MonoBehaviour
{
    public float _sparkleSpeed = 3f; 
    public float _timeToDestroy = 5f;
    private bool _fly;
    private Vector2 _direction;
    void Start() {
        GameObject _player = GameObject.FindGameObjectWithTag("Player");
        if (_player!=null) {
            _direction = Vector3.Normalize( _player.transform.position - transform.position);
            Destroy(this.gameObject, _timeToDestroy);
            _fly = true;
        } else {
            transform.GetComponent<Animator>().SetTrigger("hit");
        }
        

    }
    void Update() {
        if (_fly) {
            transform.Translate(Vector2.right * _sparkleSpeed * Time.deltaTime);
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            IDamagable hit = other.GetComponent<IDamagable>();
            if (hit != null) {
                hit.Damage();
                _fly = false;
                transform.GetComponent<BoxCollider2D>().enabled = false;
                transform.GetComponent<Animator>().SetTrigger("hit");
            }
        }
    }

    private void destroyMyself() {
        Destroy(this.gameObject);
    }
}
