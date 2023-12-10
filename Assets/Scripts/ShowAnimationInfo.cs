using Spine.Unity;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//=====================================================================================
// Written by FreeKnight 2023/12/08
//=====================================================================================
// 自动显示当前spine动作名
//=====================================================================================
public class ShowAnimationInfo : MonoBehaviour
{
    private Text textComponent;
    private SkeletonAnimation skeletonAnimation;
    private Spine.AnimationState animationState;
    private Skeleton skeleton;
    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        animationState = skeletonAnimation.AnimationState;
        skeleton = skeletonAnimation.Skeleton;

        CreateTextObject();
    }

    void CreateTextObject()
    {
        GameObject textObject = new GameObject("AnimationNameText");
        textComponent = textObject.AddComponent<Text>();
        textComponent.text = "当前动画";
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.color = Color.white;
        textComponent.fontSize = 24;
        textComponent.alignment = TextAnchor.MiddleCenter;

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            textObject.transform.SetParent(canvas.transform, false);

            // 设置 Text 对象的位置为屏幕中央的上方
            RectTransform rectTransform = textObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0f, -50f);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (animationState != null && animationState.Tracks.Count > 0)
        {
            TrackEntry track = animationState.GetCurrent(0);
            if (track != null && track.Animation != null)
            {
                string currentAnimationName = track.Animation.Name;
                if (textComponent != null)
                {
                    textComponent.text = "当前动画: " + currentAnimationName;
                }
            }
        }
    }
}
