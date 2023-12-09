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
- 