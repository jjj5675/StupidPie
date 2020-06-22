using UnityEngine;

public class GrabbingSMB : SceneLinkedSMB<PlayerBehaviour>
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.TeleportToColliderSide();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.UpdateFacing();
        m_MonoBehaviour.GrabbingWallHorizontalMovement();
        m_MonoBehaviour.SlidingMovement();
        m_MonoBehaviour.CheckForGrounded();
        m_MonoBehaviour.CheckForJumpPadCollisionEnter();
        if (m_MonoBehaviour.CheckForJumpInput())
        {
            m_MonoBehaviour.WallLeapMovement();
        }
        m_MonoBehaviour.CheckForGrabbingWall();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
