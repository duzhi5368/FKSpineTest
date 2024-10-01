using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
//=====================================================================================
// Written by FreeKnight 2023/12/10
//=====================================================================================
// Animation handler ��
// ��ʹ��MacAnim��Ϊ״̬��ʱ��ͨ���ַ�����Ϊ״̬��
// �粻ʹ��MacAnim��Ϊ״̬������ʹ���Զ���ScriptableObject��Ϊ�洢 ״̬��Spine���� �Ĺ�����
//=====================================================================================
public class SkeletonAnimationHandle : MonoBehaviour
{
    // ״̬ �� Spine���� ������
    [System.Serializable]
    public class StateNameToAnimationReference
    {
        public string stateName;
        public AnimationReferenceAsset animation;
    }

    // ��������֮��Ĺ���
    [System.Serializable]
    public class AnimationTransition
    {
        public AnimationReferenceAsset from;
        public AnimationReferenceAsset to;
        public AnimationReferenceAsset transition;
    }

    public SkeletonAnimation skeletonAnimation;
    public List<StateNameToAnimationReference> statesAndAnimations = new List<StateNameToAnimationReference>();
    public List<AnimationTransition> transitions = new List<AnimationTransition>();
    public Spine.Animation TargetAnimation { get; private set; }

    void Awake()
    {

        foreach (var entry in statesAndAnimations)
        {
            entry.animation.Initialize();
        }
        foreach (var entry in transitions)
        {
            entry.from.Initialize();
            entry.to.Initialize();
            entry.transition.Initialize();
        }
    }

    // Y�ᷴת
    public void SetFlip(float horizontal)
    {
        if (horizontal != 0)
        {
            skeletonAnimation.Skeleton.ScaleX = horizontal > 0 ? 1f : -1f;
        }
    }

    // ���Ŷ������������������㣩
    public void PlayAnimationForState(string stateShortName, int layerIndex)
    {
        PlayAnimationForState(StringToHash(stateShortName), layerIndex);
    }

    public void PlayOneShot(Spine.Animation oneShot, int layerIndex)
    {
        var state = skeletonAnimation.AnimationState;
        state.SetAnimation(0, oneShot, false);

        var transition = TryGetTransition(oneShot, TargetAnimation);
        if (transition != null)
            state.AddAnimation(0, transition, false, 0f);

        state.AddAnimation(0, this.TargetAnimation, true, 0f);
    }

    #region ��������
    public void PlayAnimationForState(int shortNameHash, int layerIndex)
    {
        var foundAnimation = GetAnimationForState(shortNameHash);
        if (foundAnimation == null)
            return;

        PlayNewAnimation(foundAnimation, layerIndex);
    }

    public Spine.Animation GetAnimationForState(string stateShortName)
    {
        return GetAnimationForState(StringToHash(stateShortName));
    }

    public Spine.Animation GetAnimationForState(int shortNameHash)
    {
        var foundState = statesAndAnimations.Find(entry => StringToHash(entry.stateName) == shortNameHash);
        return (foundState == null) ? null : foundState.animation;
    }

    int StringToHash(string s)
    {
        return Animator.StringToHash(s);
    }

    public void PlayNewAnimation(Spine.Animation target, int layerIndex)
    {
        Spine.Animation transition = null;
        Spine.Animation current = null;

        current = GetCurrentAnimation(layerIndex);
        if (current != null)
            transition = TryGetTransition(current, target);

        if (transition != null)
        {
            skeletonAnimation.AnimationState.SetAnimation(layerIndex, transition, false);
            skeletonAnimation.AnimationState.AddAnimation(layerIndex, target, true, 0f);
        }
        else
        {
            skeletonAnimation.AnimationState.SetAnimation(layerIndex, target, true);
        }

        this.TargetAnimation = target;
    }

    Spine.Animation TryGetTransition(Spine.Animation from, Spine.Animation to)
    {
        foreach (var transition in transitions)
        {
            if (transition.from.Animation == from && transition.to.Animation == to)
            {
                return transition.transition.Animation;
            }
        }
        return null;
    }

    Spine.Animation GetCurrentAnimation(int layerIndex)
    {
        var currentTrackEntry = skeletonAnimation.AnimationState.GetCurrent(layerIndex);
        return (currentTrackEntry != null) ? currentTrackEntry.Animation : null;
    }

    #endregion
}
