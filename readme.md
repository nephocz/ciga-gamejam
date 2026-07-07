# what reminds
- 一款由锚移动自主开发的2d横版解谜闯关游戏
1. 基本操作
    - A/D移动
    - 按下tab切换人物移动/地图旋转模式
    - 在地图模式下点击物体以固定

2. 关卡设计（感谢烤奶和明月对关卡设计的贡献）
3. 项目结构
    ```
    My project/
    ├── Assets/                         # Unity 项目主要资源
    │   ├── Scenes/                     # 场景文件
    │   │   ├── MainMenuScene.unity      # 主菜单
    │   │   ├── LevelSelectScene.unity   # 关卡选择
    │   │   ├── Level_001~006.unity      # 正式关卡
    │   │   ├── PauseMenuScene.unity     # 暂停菜单
    │   │   └── EndLevel.unity           # 结算/制作人员页面
    │   ├── scripts/                    # 游戏脚本
    │   │   ├── Core/                   # 游戏模式、分辨率、暂停、结束页等核心控制
    │   │   ├── Player/                 # 玩家移动、死亡、挤压检测、摩擦控制
    │   │   ├── UI/                     # 场景加载、转场、暂停、文字提示
    │   │   ├── audio/                  # BGM、音效类型与播放管理
    │   │   ├── input/                  # 鼠标点击与交互输入
    │   │   ├── map/                    # 地图旋转/变换控制
    │   │   ├── objects/                # 关卡物体、终点、可固定物体相关逻辑
    │   │   ├── physics/                # 2D 碰撞层、物理配置辅助
    │   │   └── view/                   # 视角/屏幕跟随相关逻辑
    │   ├── images/                     # 图片、美术资源
    │   │   ├── character/              # 角色图片
    │   │   ├── main menue/             # 主菜单与结尾页图片
    │   │   ├── objects/                # 关卡物体图片
    │   │   ├── pause menu/             # 暂停菜单图片
    │   │   └── 场地美术/               # 场景背景、关卡美术、锚点动画等
    │   ├── audios/                     # 背景音乐与版权声明
    │   ├── sound effects/              # 音效文件
    │   ├── fonts/                      # 字体与 TextMeshPro 字体资产
    │   ├── PhysicsMaterials/           # 2D 物理材质
    │   ├── Settings/                   # URP、Renderer2D、构建配置
    │   └── TextMesh Pro/               # TextMeshPro 依赖资源
    ├── Packages/                       # Unity 包管理配置
    ├── ProjectSettings/                # Unity 项目设置
    ├── readme.md                       # 项目说明
    └── codex.md                        # 协作/代码修改提示
    ```
4. 游玩方式
    1. clone本项目之后再unity里build游玩，编辑器版本为6000.4.10f1
    2. 期待后续的网络上传