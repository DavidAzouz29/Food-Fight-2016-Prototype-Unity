﻿using UnityEngine;
using System.Collections;

public class AttackExit : StateMachineBehaviour {

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("AttackTrigger", false);
        animator.SetBool("Attacking1Left", false);
        animator.SetBool("Attacking1Right", false);
        animator.SetBool("Attacking2", false);
        animator.SetBool("Blocking", false);
        PlayerCollision[] temp;
        temp = animator.gameObject.GetComponentsInChildren<PlayerCollision>();
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i].weaponIsActive = false;
            temp[i].isHeavyAttack = false;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
