using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : Monster
{
    public GameObject sparklePrefab;
    public override void Attack() {
        Instantiate(sparklePrefab, transform.position, Quaternion.identity);
    }
}
