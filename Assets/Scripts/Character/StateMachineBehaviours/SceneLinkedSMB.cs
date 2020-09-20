using UnityEngine;
using UnityEngine.Animations;

//호출한 클래스의 애니메이션을 배열로 저장하여 번호로 호출하기 위한 탬플릿 클래스
//
public class SceneLinkedSMB<TMonoBehaviour> : StateMachineBehaviour
    where TMonoBehaviour : MonoBehaviour
{
    protected TMonoBehaviour m_MonoBehaviour;

    public static void Initialise(Animator animator, TMonoBehaviour monoBehaviour)
    {
        SceneLinkedSMB<TMonoBehaviour>[] sceneLinkedSMBs = animator.GetBehaviours<SceneLinkedSMB<TMonoBehaviour>>();

        for(int i = 0; i < sceneLinkedSMBs.Length; i++)
        {
            sceneLinkedSMBs[i].InternalInitialise(animator, monoBehaviour);
        }
    }

    protected void InternalInitialise(Animator animator, TMonoBehaviour monoBehaviour)
    {
        m_MonoBehaviour = monoBehaviour;
    }
}
