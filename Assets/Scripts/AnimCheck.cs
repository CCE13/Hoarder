using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimCheck : StateMachineBehaviour
{
    // Start is called before the first frame update

    public string paramToCheck;
    public bool falseMe;
    public bool needEnter;
    public bool needExit;
    public bool needUpdate;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (needEnter)
        {
            if (falseMe)
            {
                animator.SetBool(paramToCheck, false);
            }
            else
            {
                animator.SetBool(paramToCheck, true);
            }
            
        }
        
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!needUpdate) { return; }
        if (stateInfo.normalizedTime > 1 && !animator.IsInTransition(0))
        {
            animator.SetBool(paramToCheck, false);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (needExit)
        {
            if (falseMe)
            {
                animator.SetBool(paramToCheck, true);
            }
            else
            {
                animator.SetBool(paramToCheck, false);
            }
            
        }
        
    }
}
