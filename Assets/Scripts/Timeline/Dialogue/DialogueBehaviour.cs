using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// PlayableBehaviour 에서 재정의 가능한 메서드 목록 https://docs.unity3d.com/ScriptReference/Playables.PlayableBehaviour.html?_ga=2.228919004.1328280129.1594297415-43934125.1592278097
/// </summary>

[Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
    public string phraseKey;
    public int textIndex;
    public bool pause = false;

    private DialogueCanvasController m_DialogueCanvas;
    private PlayableDirector m_Director;
    private TrackAsset m_TrackAsset;

    private bool m_IsClipPlayed = false;
    private bool m_CanPause = false;

    public override void OnPlayableCreate(Playable playable)
    {
        m_Director = (playable.GetGraph().GetResolver() as PlayableDirector);

        TimelineAsset timelineAsset = m_Director.playableAsset as TimelineAsset;
        var trackAssets = timelineAsset.GetOutputTracks();

        foreach (var track in trackAssets)
        {
            if (track is DialogueTrack)
            {
                m_TrackAsset = track;
                var binding = m_Director.GetGenericBinding(track) as DialogueCanvasController;
                m_DialogueCanvas = binding;
                break;
            }
        }
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (!m_IsClipPlayed && 0 < info.weight)
        {
            m_DialogueCanvas.Next(phraseKey, textIndex);

            if (Application.isPlaying)
            {

                if (pause)
                {
                    m_CanPause = true;
                }
            }

            m_IsClipPlayed = true;
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if(m_CanPause)
        {
            m_CanPause = false;
            m_Director.playableGraph.GetRootPlayable(0).SetSpeed(0d);
            m_DialogueCanvas.SendResumeAction(ResumeTimeline);
        }

        //Dialogue Track is Done
        if (info.evaluationType == FrameData.EvaluationType.Playback)
        {
            if (m_TrackAsset.end <= m_Director.time)
            {
                m_DialogueCanvas.SendResumeAction(null);
                m_DialogueCanvas.DeactivateCanvasWithDelay(1f);
                UIManager.Instance.ToggleHUDCanvas(true);
            }
        }

        m_IsClipPlayed = false;
    }

    void ResumeTimeline()
    {
        m_Director.playableGraph.GetRootPlayable(0).SetSpeed(1d);
    }
}
