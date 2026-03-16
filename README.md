# 2D Action RPG（Unity）

这是一个使用 **Unity 6 + URP** 开发的 **2D 横版动作 RPG 项目**。

项目实现了完整的 **角色战斗系统、技能树系统、敌人 AI、背包装备系统、存档系统以及 UI 系统**，并采用 **模块化架构 + 数据驱动设计** 实现游戏功能。

在 Boss 战斗中实现了 **对象池（Object Pooling）优化**，用于管理 Boss 技能（火球与冲击波）的生成与回收，减少频繁的 Instantiate / Destroy 带来的 **GC 和性能开销**。

项目的目标是构建一个 **可扩展的 RPG 游戏框架**，便于后续扩展新的技能、敌人、装备以及玩法。

### demo: https://www.bilibili.com/video/BV1zuc6zaEN4/?spm_id_from=333.1387.homepage.video_card.click&vd_source=d319ae0b424260d71c40698aacb9cfcb

---

# 项目玩法

游戏主要包含以下核心玩法循环：

* 玩家移动与战斗
* 技能树解锁与升级
* 敌人战斗与 Boss 战
* 背包与装备系统
* 敌人掉落系统
* 场景探索与传送
* 存档与读档

玩家可以在关卡中探索、击败敌人、获取装备、解锁技能，并挑战 Boss。

---

# 技术栈

**引擎**

* Unity 6
* URP（Universal Render Pipeline）

**Unity 系统**

* Unity New Input System
* Rigidbody2D 物理系统
* Animator 动画状态机
* Cinemachine 相机系统
* ScriptableObject 数据配置

**AI**

* 有限状态机（FSM）
* Behavior Designer 行为树

**架构设计**

* 组件化架构（Component-based）
* ScriptableObject 数据驱动
* 接口化存档系统
* 模块化战斗系统
* 对象池（Object Pooling）性能优化

---

# 项目结构

```
Assets
 ├── Scripts
 │   ├── Player            # 玩家逻辑
 │   ├── Enemy             # 敌人与Boss
 │   ├── SkillSystem       # 技能系统
 │   ├── InventorySystem   # 背包系统
 │   ├── StatSystem        # 属性系统
 │   ├── SaveSystem        # 存档系统
 │   ├── UI                # UI系统
 │   ├── StateMachine      # 状态机框架
 │   ├── Entity            # 实体基础类
 │   └── Data              # ScriptableObject数据
 │
 ├── Prefabs
 ├── Animations
 ├── Scenes
 └── ScriptableObjects
```

---

# 玩家战斗系统

玩家角色使用 **状态机（State Machine）架构**实现。

玩家状态包括：

* Idle（待机）
* Move（移动）
* Jump（跳跃）
* Dash（冲刺）
* Attack（攻击）
* Counter（反击）
* Skill（技能）

核心脚本：

```
Player.cs
PlayerState.cs
Player_AttackState.cs
Player_DashState.cs
Player_JumpState.cs
StateMachine.cs
EntityState.cs
```

职责包括：

* 玩家输入处理
* 动画控制
* 状态切换
* 战斗逻辑

这种架构使角色逻辑 **清晰且易扩展**。

---

# 敌人 AI 系统

敌人 AI 使用两种架构：

## 普通敌人 FSM

普通敌人使用 **有限状态机（FSM）**。

状态包括：

* Idle
* Move
* Battle
* Attack
* Stunned
* Dead

核心脚本：

```
Enemy.cs
Enemy_IdleState.cs
Enemy_MoveState.cs
Enemy_AttackState.cs
Enemy_StunnedState.cs
```

---

## Boss 行为树 AI

Boss 使用 **Behavior Designer 行为树**。

行为树负责：

* 技能选择
* 距离判断
* 冷却判断
* 攻击逻辑

相关脚本：

```
BossAction.cs
Shoot.cs
ShootWave.cs
Skill.cs
```

行为树负责 **AI 决策**，技能脚本负责 **技能实现**。

---

# Boss 技能对象池优化

在 Boss 战斗中，Boss 会频繁释放：

* 火球技能（Fireball）
* 冲击波技能（Shockwave）

如果每次都使用：

```
Instantiate()
Destroy()
```

会产生大量 **GC 和性能开销**。

因此项目实现了 **对象池（Object Pooling）** 来管理 Boss 技能对象。

对象池流程：

1. 游戏开始时预生成一定数量的技能对象
2. Boss 释放技能时从对象池中取出对象
3. 技能结束后回收到对象池
4. 下次技能释放时复用对象

优点：

* 减少内存分配
* 降低 GC 频率
* 提高战斗性能稳定性

---

# 技能系统

项目实现了一个 **模块化技能系统**。

技能包括：

* Dash（冲刺）
* Sword Throw（飞剑）
* Time Echo（时间回响）

技能统一继承：

```
Skill_Base.cs
```

具体技能：

```
Skill_Dash.cs
Skill_SwordThrow.cs
Skill_TimeEcho.cs
SkillObject_TimeEcho.cs
```

功能包括：

* 技能冷却
* 技能升级
* 战斗触发
* 技能特效

技能数据通过 **ScriptableObject** 配置。

---

# 技能树系统

玩家可以通过 **技能树 UI** 解锁技能。

功能：

* 节点式升级
* 技能依赖关系
* 分支技能路线
* UI 可视化

核心脚本：

```
UI_SkillTree.cs
UI_TreeNode.cs
```

---

# 背包与装备系统

项目实现了完整的背包系统：

* 拾取物品
* 装备系统
* 快捷物品栏
* 物品效果

核心脚本：

```
Inventory_Player.cs
Inventory_Item.cs
Inventory_Storage.cs
```

物品通过 ScriptableObject 配置：

```
ItemDataSO.cs
ConsumableItemDataSO.cs
ItemEffect_DataSO.cs
```

---

# 掉落系统

敌人死亡后会触发 **掉落系统**生成随机物品。

核心脚本：

```
Entity_DropManager.cs
ItemListDataSO.cs
ItemDataSO.cs
```

---

## 掉落流程

当敌人死亡时：

1. 调用 `DropItems()`
2. 从 `ItemListDataSO` 中随机选择物品
3. 根据稀有度和最大数量限制生成掉落
4. 在敌人位置生成掉落物体

核心逻辑：

```
DropItems()
 ├─ RollDrops()
 ├─ 限制最大掉落数量
 └─ CreateItemDrop()
```

---

## 掉落规则

掉落系统包含两个限制：

### 稀有度限制

```
maxRarityAmount
```

用于控制掉落物品的稀有度上限。

---

### 最大掉落数量

```
maxItemsToDrop
```

限制敌人一次死亡最多掉落的物品数量。

---

## 掉落物生成

掉落物通过 prefab 实例化：

```
Instantiate(itemDropPrefab, transform.position, Quaternion.identity)
```

拾取后会加入玩家背包。

---

## 数据驱动配置

掉落物通过 `ItemListDataSO` 管理。

该 ScriptableObject 可以自动收集所有 `ItemDataSO`：

```
Auto-fill with all ItemDataSO
```

这样新增物品时无需修改代码。

---

# 角色属性系统

属性系统管理：

* 生命值
* 法力值
* 攻击力
* 暴击率
* 护甲
* 元素伤害
* 元素抗性

核心脚本：

```
Stat.cs
Entity_Stats.cs
Entity_Combat.cs
```

伤害计算包含：

* 物理伤害
* 暴击
* 护甲减伤
* 元素伤害
* 状态效果

---

# 状态效果系统

状态系统负责管理：

* 燃烧
* 闪电
* 减速

核心脚本：

```
Entity_StatusHandler.cs
```

---

# 存档系统

项目实现了 **模块化存档系统**。

所有需要存档的系统实现接口：

```
ISaveable
```

SaveManager 会自动收集这些组件并保存数据。

核心脚本：

```
SaveManager.cs
GameData.cs
FileDataHandler.cs
SerializableDictionary.cs
```

功能：

* JSON 存档
* XOR 简单加密
* 自动注册存档对象
* 跨场景数据保存

---

# 相机系统

相机系统使用 **Cinemachine**。

功能：

* 玩家跟随
* 平滑移动
* 屏幕震动

脚本：

```
CameraController.cs
```

---

# 场景管理

场景切换由 **GameManager** 管理。

功能：

* 场景淡入淡出
* 复活点
* 传送点
* 场景切换

核心脚本：

```
GameManager.cs
```

GameManager 使用 `DontDestroyOnLoad` 跨场景存在。

---

# 技术亮点

### 数据驱动设计

大量配置通过 **ScriptableObject** 管理。

---

### 模块化战斗系统

通过接口解耦：

```
IDamageable
AttackData
Entity_Combat
```

---

### Boss 技能对象池优化

Boss 技能（火球 / 冲击波）使用对象池管理：

* 减少 GC
* 提高性能稳定性

---

### 可扩展 AI 架构

普通敌人使用 FSM
Boss 使用 Behavior Tree

---

### 解耦存档系统

实现 `ISaveable` 即可加入存档。

