using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        
        Debug.Log("hit" + other.name);
        IDamagable hit = other.GetComponent<IDamagable>();
        if (hit != null) {
            hit.Damage();
        }
    }
}
