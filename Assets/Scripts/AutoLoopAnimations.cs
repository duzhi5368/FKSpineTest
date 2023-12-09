using System.Collections;
using UnityEngine;
using Spine.Unity;
using System.Collections.Generic;
using Spine;
//=====================================================================================
// Written by FreeKnight 2023/12/08
//=====================================================================================
// 自动播放spine动画
//=====================================================================================
public class AutoLoopAnimations : MonoBehaviour
{
    #region Inspector
    [SpineAnimation]
    public List<string> LoopAnimationNames = new List<string>()
    {
        "idle",
        "walk",
        "run",
    };

    #endregion

    SkeletonAnimation skeletonAnimation;
    Spine.AnimationState animationState;
    Skeleton skeleton;

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        animationState = skeletonAnimation.AnimationState;
        skeleton = skeletonAnimation.Skeleton;
        StartCoroutine(AutoChangeAnimation());
    }

    IEnumerator AutoChangeAnimation()
    {
        SkeletonData skeletonData = skeleton.Data;
        if (skeletonData == null)
            yield break;
        ExposedList<Spine.Animation> animations = skeletonData.Animations;
        if (animations == null)
            yield break;

        while (true)
        {
            foreach (var animation in animations)
            {
                animationState.SetAnimation(0, animation.Name, true);

                bool bIsLoop = LoopAnimationNames.Contains(animation.Name);
                if (bIsLoop)
                {
                    yield return new WaitForSeconds(3.0f);
                }
                else
                {
                    yield return new WaitForSeconds(animation.Duration);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
