using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//=====================================================================================
// Written by FreeKnight 2023/12/10
//=====================================================================================
// ²¥·ÅÒÆ¶¯½Å²½Éù
//=====================================================================================
public class FootAudioHandler : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;

    [SpineEvent(dataField: "skeletonAnimation", fallbackToTextField: true)]
    public string eventName;
    public AudioSource audioSource;
    public AudioClip audioClip;

    private Spine.EventData eventData;
    void Start()
    {
        if (skeletonAnimation == null)
            skeletonAnimation = GetComponent<SkeletonAnimation>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (audioSource == null) 
            return;
        if (skeletonAnimation == null) 
            return;
        skeletonAnimation.Initialize(false);
        if (!skeletonAnimation.valid)
            return;

        eventData = skeletonAnimation.Skeleton.Data.FindEvent(eventName);
        skeletonAnimation.AnimationState.Event += HandleAnimationStateEvent;
    }

    private void HandleAnimationStateEvent(TrackEntry trackEntry, Spine.Event e)
    {
        bool eventMatch = (eventData == e.Data);
        if (eventMatch)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}
