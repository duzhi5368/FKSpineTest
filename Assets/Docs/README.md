# Spine

### 版本情况
- Unity版本：2022.3.6f1
- Spine版本：3.8.75Pro
- 文档参考：https://zh.esotericsoftware.com/spine-unity#Spine-Examples-%2F-Getting-Started

### 基本使用
- 1，打开Spine动画编辑器，设置【纹理图集打包】，导出JSON。最少三个文件。（切记不要选中【动画清理】）
  - json数据文件
  - Altas.txt图集文件【在导出时，可选择（打包设置->图集扩展名，修改.altas为.altas.txt）】
  - png图集
- 2，导出时，也可导出二进制格式。此时需要将后缀名从 .skel 改为 .skel.bytes.
  - 导出时，尽量选择纹理打包。性能比较好，而且在UI上使用的Spine动画SkeletonGraphic仅支持单纹理图。
- 3，将导出数据放到Unity资源文件夹Assets中
- 4, 将SpineLib拷贝到Assets中，或根据不同版本的spine和Unity
- 5, 修改资源属性
  - 修改texture 2d图集，纹理类型为 默认。
  - 修改texture 2d图集，压缩为 无。
  - 修改Altas文件后缀，改为XXx.altas.txt【这步可以在导出时设置而避免】
  - 保存为 altas.txt 后，应当会自动生成三个新文件
    - Spine atlas asset文件
    - Material 文件
    - Skeleton data文件
- 6，创建游戏对象->Spine->SkeletonAnimation
  - 拖拽生成的Skeleton data文件到SkeletonAnimation脚本的SkeletonData asset中
  - 拖拽生成的Material文件到Gameobj上，添加 Meterial 属性
- 7，测试
  - 可通过调整Initial Skin调整皮肤
  - 可通过调整Animatiion name，loop, time scale 调整动作，循环和速度。


### 增强使用

#### 图片纹理打包细则

- 方式1：在spine导出时，如果在 【打包设置】->【输出】->【预乘Alpha/ premultiply alpha】选中了，那么：
  - 在导出的png的Unity Texture设置中，需要设置【sRPG(Color texture)】为【启用/选中】状态。
  - 在导出的png的Unity Texture设置中，在【Alpha is Transparency】此处要为【禁用/未选中】状态。
  - 在生成的 XXX_meterial 设置中，【Straright Alpha Texture】要为【禁用/未选中】状态
- 方式2：在spine导出时，如果在 【打包设置】->【输出】->【溢出/bleed】选中了，那么：
  - 在导出的png的Unity Texture设置中，需要设置【sRPG(Color texture)】为【启用/选中】状态。
  - 在导出的png的Unity Texture设置中，在【Alpha is Transparency】此处要为【启用/选中】状态。
  - 在生成的 XXX_meterial 设置中，【Straright Alpha Texture】要为【启用/选中】状态

默认情况下，我们都使用方式1；只有在自定义特殊shader时，可能需要方式2.
**上述配置不正确的话，可能会在透明区域周围有深色的边框，或图片周围有有彩色条带。**

#### 自定义Altas素材打包

- 可使用Unity的SpriteAtlas替代Spine的Atlas，此时可使用【 Windows-> Spine -> SpriteAtlas inport】进行访问。

#### Skeleton 烘培

- **不推荐使用。** 它会丢失大量数据信息，不能支持spine的任何对象，只能支持最基本的animation clips。如果一定要用，可以打开 XXX_skeletonData文件的【Inspector】的右上角的三个点【:】，选择 【Skeleton baking】

#### 其他设置

- 在Unity中，【编辑 -> 首选项 -> spine 】中有各种设置，一般情况并不需要额外处理。

#### 动画信息预览

- 在生成的 XXX_SkeletonData 文件【Inspector】中，可在【Preview】下点击【Setup Pose】，在界面最下方有个【Preview】项，将其显示，然后选择 动作左侧的播放按钮，即可预览动作。

### 脚本常用函数

- 对象
  - SkeletonAnimation skeletonAnimation
    - Spine.AnimationState animationState = skeletonAnimation.AnimationState;
    - Skeleton skeleton = skeletonAnimation.Skeleton;
      - SkeletonData skeletonData = skeleton.Data;
        - Spine.EventData eventData = skeletonData.FindEvent(eventName);
        - ExposedList<Spine.Animation> animations = skeletonData.Animations;
  - SkeletonAnimation

- 获取动画信息：动画列表< 动画名：动画制作时间 >
```C#
  ExposedList<Spine.Animation> animations = skeletonData.Animations;
```

- 播放动画
```C#
  // 可以通过动画名字符串播放
  TrackEntry entry = animationState.SetAnimation(0, animation.Name, bIsLoop);
  skeletonAnimation.timeScale = 0.5f; // 播放速度比例

  // 也可以通过 AnimationReferenceAsset 作为参数播放
  public AnimationReferenceAsset animationReferenceAsset;
  TrackEntry entry = skeletonAnimation.AnimationState.SetAnimation(trackIndex,
animationReferenceAsset, true);
```

- 播放动画队列
```C#
// 延迟2秒后播放 run
  TrackEntry entry = skeletonAnimation.AnimationState.AddAnimation(trackIndex,
"run", true, 2);
```

- 多对象动画（例如骑马-自身一套，马一套）
```C#
  animationState.SetAnimation(0, animation.Name, bIsLoop);
  animationState.SetAnimation(1, animation.Name, bIsLoop);
```

- Spine编辑事件回调处理（例如：脚步声）
```C#
  void Awake(){
    Spine.EventData eventData = skeletonAnimation.Skeleton.Data.FindEvent(eventName);
    skeletonAnimation.AnimationState.Event += HandleAnimationStateEvent;
  }

  void HandleAnimationStateEvent(TrackEntry trackEntry, Spine.Event e) { 
    if(eventData == e.Data){ 
      doSth();
    } 
  }
```

- 普通事件回调
```C#
void Awake()
{
    skeletonAnimation = GetComponent<SkeletonAnimation>();
    animationState = skeletonAnimation.AnimationState;
    
     // registering for events raised by any animation
    animationState.Start += OnSpineAnimationStart;
    animationState.Interrupt += OnSpineAnimationInterrupt;
    animationState.End += OnSpineAnimationEnd;
    animationState.Dispose += OnSpineAnimationDispose;
    animationState.Complete += OnSpineAnimationComplete;
    
    animationState.Event += OnUserDefinedEvent; // 编辑器注册事件

    // registering for events raised by a single animation track entry
    Spine.TrackEntry trackEntry = animationState.SetAnimation(trackIndex, "wa
lk", true);
    trackEntry.Start += OnSpineAnimationStart;
    trackEntry.Interrupt += OnSpineAnimationInterrupt;
    trackEntry.End += OnSpineAnimationEnd;
    trackEntry.Dispose += OnSpineAnimationDispose;
    trackEntry.Complete += OnSpineAnimationComplete;
    trackEntry.Event += OnUserDefinedEvent;
}

public void OnSpineAnimationStart(TrackEntry trackEntry) {}
public void OnSpineAnimationComplete(TrackEntry trackEntry) {}
```

- Spine额外提供的yield指令

  - WaitForSpineAnimation 等待, 直到 Spine.TrackEntry 引发一个具体事件。
  - WaitForSpineAnimationComplete. 等待, 直到 Spine.TrackEntry 引发一
个 Complete 事件.
  - WaitForSpineAnimationEnd. 等待, 直到 Spine.TrackEntry 引发一个
End 事件.
  - WaitForSpineEvent. 等待, 直到 Spine.AnimationState 引发了一个自定
义的 Spine.Event 事件（编辑器注册事件）
```C#

var track = skeletonAnimation.state.SetAnimation(0, "interruptible"
, false);
var completeOrEnd = WaitForSpineAnimation.AnimationEventTypes.Compl
ete | WaitForSpineAnimation.AnimationEventTypes.End;
yield return new WaitForSpineAnimation(track, completeOrEnd);

```

- Spine的Unity属性关键字支持
```C#

[SpineBone] public string bone;
[SpineSlot] public string slot;
[SpineAttachment] public string attachment;
[SpineSkin] public string skin;
[SpineAnimation] public string animation;
[SpineEvent] public string event;
[SpineIkConstraint] public string ikConstraint;
[SpineTransformConstraint] public string transformConstraint;
[SpinePathConstraint] public string pathConstraint;

```

- 动作混合
```C#
        animationState.SetAnimation(0, firstAnimationName, true);
        // 添加第二个动画到混合队列（用于混合两个动画）
        TrackEntry secondAnimation = animationState.SetAnimation(1, secondAnimationName, true);
        secondAnimation.mixDuration = 0.5f; // 设置混合时间
        secondAnimation.mixBlend = MixBlend.Additive; // 混合模式
```

- 动画UI操作
```C#
        skeletonGraphic.AnimationState.SetAnimation(0, "yourAnimationName", true);
        // 设置动画速度
        skeletonGraphic.timeScale = 1.5f;
        // 添加事件监听器
        skeletonGraphic.AnimationState.Event += HandleEvent;
```

- 挂接附件

挂接附件需要一个 槽位  和 一个附件名。
```C#
[SpineSlot] public string slotProperty = "slotName";
[SpineAttachment] public string attachmentProperty = "attachmentName";

bool success = skeletonAnimation.Skeleton.SetAttachment("slotName", "attachmentName");
```

- 重置Setup pose
```C#
skeleton.SetToSetupPose();
skeleton.SetBonesToSetupPose();
skeleton.SetSlotsToSetupPose();
```

- 设置皮肤
```C#
[SpineSkin] public string skinProperty = "skinName";

bool success = skeletonAnimation.Skeleton.SetSkin("skinName");
```

- 组合皮肤
  
将多个皮肤，组装成为一个完整的角色皮肤。

```C#
var skeleton = skeletonAnimation.Skeleton;
var skeletonData = skeleton.data;
var mixAndMatchSkin = new Skin("custom-girl");
mixAndMatchSkin.AddSkin(skeletonData.FindSkin("skin-base"));
mixAndMatchSkin.AddSkin(skeletonData.FindSkin("nose/short"));
mixAndMatchSkin.AddSkin(skeletonData.FindSkin("eyelids/girly"));
skeleton.SetSkin(mixAndMatchSkin);
skeleton.SetSlotsToSetupPose();
```

- Skeleton Animation回调
```C#
void AfterUpdateComplete (ISkeletonAnimation anim) {
// this is called after animation updates have been completed.
}

void Start() {
  skeletonAnimation.UpdateComplete -= AfterUpdateComplete;
  skeletonAnimation.UpdateComplete += AfterUpdateComplete;
}
```
还有其他几个函数：
  - SkeletonAnimation.BeforeApply 本帧动画之前触发。要修改skeleton状态时用它。
  - SkeletonAnimation.UpdateLocal 本帧动画本地坐标更新完毕。要修改skeleton本地坐标时用它。
  - SkeletonAnimation.UpdateComplete 本帧动画完全完毕后触发。要取值时用它。
  - SkeletonAnimation.UpdateWorld 如果要根据世界坐标来修改骨骼本地坐标，使用它。


### 使用Unity动画器

- 选择导入的资源之一：Skeleton Data,点击【create Animation reference assests】，生成一系列 AnimationReferenceAssert，每一个AnimationReferenceAssert 相当于一个引用了 Spine.Animation的Unity Asset。
- 代码中使用下面方式来获取 状态名：动画片段 之间的关联
```C#
    // 状态 和 Spine动画 关联。
    [System.Serializable]
    public class StateNameToAnimationReference
    {
        public string stateName;
        public AnimationReferenceAsset animation;
    }

    // 两个动画之间的过渡
    [System.Serializable]
    public class AnimationTransition
    {
        public AnimationReferenceAsset from;
        public AnimationReferenceAsset to;
        public AnimationReferenceAsset transition;
    }
```
- 如想使用Unity自带的MecAnim，可在Skeleton Data-> Skots点击【Generate Mecanim Controller】，生成一个Animator controller
- 编辑生成Animator controller，编辑好状态图，和设置好切换条件。

### 辅助属性接口

##### 使用 BoneFollower（例如：跑步粒子，开火粒子）

- 注意：SkeletonGraphics有专用的 BoneFollowerGraphics
- 该组件引用一个SkeletonAnimation的骨骼，每次update时，自动transform为该骨骼的transform
- 它是一个单独的GameObject，不挂骨骼

##### 使用 PointFollower

- 类似BoneFollower，但挂接的是一个 PointAttackment  挂接点，而非骨骼。

##### 使用 SkeletonUtilityBone

- 可以编码修改骨骼位置。


### 着色器

在生成的 XXX_material 中可以修改shader，Spine提供了数十种shader支持。

编码修改 material 
```C#
// 修改整个骨骼的材质
skeletonAnimation.CustomMaterialOverride[originalMaterial] = newMaterial; 
skeletonAnimation.CustomMaterialOverride.Remove(originalMaterial); 

// 修改单一槽位上的材质
skeletonAnimation.CustomSlotMaterial[slot] = newMaterial;
skeletonAnimation.CustomSlotMaterial.Remove(slot);
```