using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleBool : StateMachineBehaviour
{
    [Header("== PROPERTIES ==")]
    public string boolName;
    public bool stateOnStart;
    public bool stateOnExit;
    public bool startRootMotion;
    public bool endRootMotion;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.applyRootMotion = startRootMotion;
        animator.SetBool(boolName, stateOnStart);
    }
    

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.applyRootMotion = endRootMotion;
        animator.SetBool(boolName, stateOnExit);
    }

}
