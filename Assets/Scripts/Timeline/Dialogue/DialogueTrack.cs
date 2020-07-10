using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// 트랙없이 클립만 추가하면 모든 클립마다 구성요소를 할당해야하기 때문에
/// 사용자 정의 트랙을 정의하여 객체 또는 컴포넌트를 바인딩하여 트랙에서 관리한다.
/// </summary>
/// <typeparam name="TrackClipType">트랙에서 사용할 PlayableAsset 유형</typeparam>
/// <typeparam name="TrackBindingType">트랙에 바인딩할 유형 지정<value>The Type value of GameObject, Component, Asset</value></typeparam>

[TrackColor(0.7794118f, 0.4002983f, 0.1547362f)]
[TrackClipType(typeof(DialogueClip))]
[TrackBindingType(typeof(DialogueCanvasController))]
public class DialogueTrack : TrackAsset
{
}
