using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    public void AnimationEvents(string message)
    {
        if (message.Equals("DeathAnimationEnded"))
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
