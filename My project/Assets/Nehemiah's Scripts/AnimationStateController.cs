using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    public Animator Character;

    void Update()
    {
        // these keys are just for manual testing in Editor
        if (Input.GetKeyDown(KeyCode.M)) stopDoingStuff();
        if (Input.GetKeyDown(KeyCode.N)) Attack();
        if (Input.GetKeyDown(KeyCode.B)) Die();
        if (Input.GetKeyDown(KeyCode.V)) Move();
    }

    public void stopDoingStuff()
    {
        // reset all
        Character.SetBool("isAttacking", false);
        Character.SetBool("isMoving", false);
        Character.SetBool("isDying", false);
        Character.SetBool("isDoingNothing", false);

        // set what they're doing
        Character.SetBool("isDoingNothing", true);
        Debug.Log("[Anim] Character " + Character.name + " is doing nothing.");
    }

    public void Attack()
    {
        // reset all
        Character.SetBool("isAttacking", false);
        Character.SetBool("isMoving", false);
        Character.SetBool("isDying", false);
        Character.SetBool("isDoingNothing", false);

        // set what they're doing
        Character.SetBool("isAttacking", true);
        Debug.Log("[Anim] Character " + Character.name + " is ATTACKING.");
    }

    public void Die()
    {
        // reset all
        Character.SetBool("isAttacking", false);
        Character.SetBool("isMoving", false);
        Character.SetBool("isDying", false);
        Character.SetBool("isDoingNothing", false);

        // set what they're doing
        Character.SetBool("isDying", true);
        Debug.Log("[Anim] Character " + Character.name + " is DYING.");
    }

    public void Move()
    {
        // reset all
        Character.SetBool("isAttacking", false);
        Character.SetBool("isMoving", false);
        Character.SetBool("isDying", false);
        Character.SetBool("isDoingNothing", false);

        // set what they're doing
        Character.SetBool("isMoving", true);
        Debug.Log("[Anim] Character " + Character.name + " is moving.");
    }

}
