using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationScript : MonoBehaviour
{
    public void DestroyMyself()
    {
        Destroy(transform.parent.gameObject);
    }
    public void ReleaseSparcle() {
        transform.parent.GetComponent<Mage>().Attack();
    }
}
