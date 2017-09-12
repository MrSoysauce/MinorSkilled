using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : StateMachineBehaviour {

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Transform player = GameManager.Instance.player.transform;
        Quaternion targetRotation = Quaternion.LookRotation(player.position - animator.transform.position);
        targetRotation = Quaternion.Euler(new Vector3(0, targetRotation.eulerAngles.y, 0));
        animator.rootRotation = targetRotation;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Transform player = GameManager.Instance.player.transform;
        Quaternion targetRotation = Quaternion.LookRotation(player.position - animator.transform.position);
        targetRotation = Quaternion.Euler(new Vector3(0, targetRotation.eulerAngles.y, 0));
        animator.rootRotation = targetRotation;
    }
}
