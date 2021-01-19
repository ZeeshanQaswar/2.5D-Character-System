using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleBool : StateMachineBehaviour
{
    
    public string boolName;

    [Header("== ANIM STATE HANDLER ==")]
    [Range(0, 1)] public float effectiveEndPoint;

    [Header("== BOOL STATE ==")]
    public bool stateOnStart;
    public bool stateOnExit;

    [Header("== HANDLE ROOT MOTION ==")]
    public bool onStartRootMotion;
    public bool onEndRootMotion;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.applyRootMotion = onStartRootMotion;
        animator.SetBool(boolName, stateOnStart);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!(stateInfo.normalizedTime < effectiveEndPoint))
        {
            Debug.Log("End Reached");
            animator.applyRootMotion = onEndRootMotion;
            animator.SetBool(boolName, stateOnExit);
        }
    }

}
