# Spine

### 版本情况
- Unity版本：2022.3.6f1
- Spine版本：3.8.75Pro

### 基本使用
- 1，打开Spine动画编辑器，设置【纹理图集打包】，导出。最少三个文件。
  - json数据文件
  - Altas图集文件
  - png图集
- 2，将导出数据放到Unity资源文件夹Assets中
- 3, 将SpineLib拷贝到Assets中，或根据不同版本的spine和Unity
- 4, 修改资源属性
  - 修改texture 2d图集，纹理类型为 默认。
  - 修改texture 2d图集，压缩为 无。
  - 修改Altas文件后缀，改为XXx.altas.txt
  - 保存后，应当会自动生成三个新文件
    - Spine atlas asset文件
    - Material 文件
    - Skeleton data文件
- 5，创建游戏对象->Spine->SkeletonAnimation
  - 拖拽生成的Skeleton data文件到SkeletonAnimation脚本的SkeletonData asset中
  - 拖拽生成的Material文件到Gameobj上，添加 Meterial 属性
- 6，测试
  - 可通过调整Initial Skin调整皮肤
  - 可通过调整Animatiion name，loop, time scale 调整动作，循环和速度。

### 脚本常用函数

- 对象
  - SkeletonAnimation skeletonAnimation
    - Spine.AnimationState animationState = skeletonAnimation.AnimationState;
      - 播放动画：animationState.SetAnimation(0, animation.Name, bIsLoop);
      - 多对象动画(骑马)：animationState.SetAnimation(trackIndex, animation.Name, bIsLoop);
    - Skeleton skeleton = skeletonAnimation.Skeleton;
      - SkeletonData skeletonData = skeleton.Data;
        - ExposedList<Spine.Animation> animations = skeletonData.Animations;
          - 动画列表< 动画名：动画制作时间 >
- 