using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DialogueClip : PlayableAsset, ITimelineClipAsset
{
    //PlayableAsset의 Inspector에서 데이터 값을 PlaybleBehaviour로 복사하는 것은 실수 유발가능성이 있다.
    //Asset Data의 PlayableBehaviour에 대한 참조
    public DialogueBehaviour template = new DialogueBehaviour();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DialogueBehaviour>.Create(graph, template);


        return playable;
    }
}
