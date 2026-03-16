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
![image](https://github.com/peinanlin/2D-RPG/blob/master/img/%E6%95%8C%E4%BA%BA%E7%8A%B6%E6%80%81%E6%9C%BA.png)
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
### 如何进入攻击状态
![image](https://github.com/peinanlin/2D-RPG/blob/master/img/%E6%95%8C%E4%BA%BA%E7%8A%B6%E6%80%81%E6%9C%BA%E6%A3%80%E6%B5%8B.png)
* 黄色线：玩家检测距离（playerCheckDistance）
表示敌人朝当前朝向发出的玩家搜索射线范围。
敌人会通过 Physics2D.Raycast() 从 playerCheck.position 出发，沿 facingDir 方向检测玩家；如果检测到玩家，就可以进入战斗相关逻辑。

* 蓝色线：攻击距离（attackDistance）
表示敌人进入攻击状态的有效距离范围。
当玩家进入这个范围后，敌人状态机会从 battleState 切换到 attackState。

* 绿色线：最小后撤距离（minRetreatDistance）
表示敌人与玩家距离过近时的安全距离阈值。
当玩家贴得太近时，敌人不会一味前压，而是可以根据战斗状态逻辑进行后撤，从而避免敌人和玩家完全重叠，提升战斗表现与可读性。

---

## Boss 行为树 AI

Boss 使用**Behavior Designer 行为树**实现战斗 AI。
相比普通敌人使用的 有限状态机（FSM），Boss 的战斗逻辑更复杂，因此使用行为树来实现 条件驱动的技能选择与战斗阶段控制。

![image](https://github.com/peinanlin/2D-RPG/blob/master/img/boss%E8%A1%8C%E4%B8%BA%E6%A0%91.png)

Boss 战斗主要包含三个行为：

* 追击玩家

* 释放冲击波攻击

* 释放火球技能

相关脚本：

```
BossAction.cs
Shoot.cs
ShootWave.cs
Skill.cs
```

行为树负责 **AI 决策**，技能脚本负责 **技能实现**。

---

### Boss 技能对象池优化

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

伤害计算由以下三个核心模块共同完成：

```text
Entity_Combat.cs   负责攻击行为与命中检测
Entity_Stats.cs    负责角色属性计算
Entity_Health.cs   负责伤害结算与生命值变化
```

系统将 **攻击逻辑、属性计算、伤害结算** 三部分进行解耦，使战斗系统更易扩展和维护。

---

## 战斗系统架构

整体战斗流程如下：

```text
攻击触发
      │
      ▼
Entity_Combat.PerformAttack()
      │
      ▼
读取攻击者属性 (Entity_Stats)
      │
      ▼
计算攻击数据 (AttackData)
      │
      ▼
目标 TakeDamage()
      │
      ▼
闪避判定
      │
      ▼
护甲减伤计算
      │
      ▼
元素抗性计算
      │
      ▼
最终伤害结算
      │
      ▼
扣除生命值 (Entity_Health)
```

这种设计将 **攻击行为** 与 **伤害计算** 分离，使不同攻击技能可以复用同一套战斗系统。

---

## 伤害计算公式

最终伤害由 **物理伤害 + 元素伤害** 组成。

```text
FinalDamage =
PhysicalDamageTaken
+
ElementalDamageTaken
```

---

## 物理伤害计算

### 1. 基础攻击伤害

基础物理伤害由攻击力与力量属性共同决定：

```
BaseDamage = Damage + Strength
```

对应代码：

```csharp
GetBaseDamage() => offense.damage + major.strength
```

---

### 2. 暴击计算

暴击率由 **基础暴击率 + 敏捷加成** 决定：

```
CritChance = CritChanceStat + (Agility × 0.3)
```

暴击伤害倍率由 **暴击伤害 + 力量加成** 决定：

```
CritMultiplier = (CritPower + Strength × 0.5) / 100
```

如果触发暴击：

```
PhysicalDamage = BaseDamage × CritMultiplier
```

否则：

```
PhysicalDamage = BaseDamage
```

---

## 护甲减伤计算

目标的护甲会减少物理伤害。

### 1. 基础护甲

```
Armor = ArmorStat + Vitality
```

对应代码：

```csharp
GetBaseArmor() => defense.armor + major.vitality
```

---

### 2. 护甲穿透

攻击者可以拥有 **Armor Reduction** 来降低目标护甲：

```
EffectiveArmor = Armor × (1 - ArmorReduction)
```

---

### 3. 护甲减伤公式

```
Mitigation = EffectiveArmor / (EffectiveArmor + 100)
```

减伤上限：

```
MaxMitigation = 85%
```

最终物理伤害：

```
PhysicalDamageTaken = PhysicalDamage × (1 - Mitigation)
```

---

## 元素伤害系统

攻击可能附带三种元素伤害：

```
Fire
Ice
Lightning
```

系统会选择 **最高元素伤害作为主元素**。

---

### 元素伤害计算

```
ElementDamage =
HighestElementDamage
+ (OtherElementDamage × 0.5)
+ Intelligence
```

其中：

* Intelligence 每点提供 **+1 元素伤害**

---

## 元素抗性系统

目标角色拥有对应元素抗性：

```
FireResistance
IceResistance
LightningResistance
```

抗性还会受到 **Intelligence 加成**：

```
Resistance = BaseResistance + (Intelligence × 0.5)
```

抗性上限：

```
75%
```

最终元素伤害：

```
ElementalDamageTaken = ElementDamage × (1 - Resistance)
```

---

## 闪避

目标在受到攻击前会进行闪避判定：

```
Evasion = BaseEvasion + (Agility × 0.5)
```

闪避上限：

```
85%
```

如果触发闪避：

```
攻击完全无效
```

---

## 最终伤害结算

当所有计算完成后：

```
FinalDamage =
PhysicalDamageTaken
+
ElementalDamageTaken
```

最终生命值变化：

```
Health -= FinalDamage
```

并触发：

* 受击特效
* 击退效果
* 状态效果


---

# 状态效果系统

状态系统负责管理：

* 燃烧

 ![gif](https://github.com/peinanlin/2D-RPG/blob/master/img/%E7%87%83%E7%83%A7.gif)
  
* 闪电

 ![gif](https://github.com/peinanlin/2D-RPG/blob/master/img/%E7%94%B5%E5%87%BB.gif)

 
* 冷冻

![gif](https://github.com/peinanlin/2D-RPG/blob/master/img/%E5%86%B7%E5%86%BB.gif)

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

本项目采用 **ScriptableObject + 数据驱动架构** 来管理游戏中的技能、装备、消耗品等配置数据。
通过这种设计，游戏逻辑与游戏配置完全解耦。所有技能数值、物品属性、掉落概率等都可以直接在 Unity Inspector 中进行配置，而无需修改代码。
项目中的主要数据包括：

* 技能数据（Skill Data）
* 装备数据（Equipment Data）
* 消耗品数据（Consumable Data）
* 物品效果数据（Item Effect Data）

所有数据统一存放在：

```
Assets/Data
```

---

# 技能数据

技能配置通过 **SkillDataSO** ScriptableObject 管理。

技能数据目录：

```
Assets/Data/Skill Data
```

![image](https://github.com/peinanlin/2D-RPG/blob/master/img/%E6%8A%80%E8%83%BD%E6%95%B0%E6%8D%AE.png)

当前项目包含以下技能类：

* Dash（冲刺）
* Domain Expansion（领域技能）
* Sword Throw（飞剑）
* Time Echo（时间回响）
* Time Shard（时间碎片）

每个技能都有一个独立的 ScriptableObject，例如：

```
Skill data - Quick dash
```

技能数据字段说明：

* Display Name  技能名称
* Description  技能说明
* Icon  技能图标
* Cost  解锁技能消耗的技能点
* Unlocked By Default  是否默认解锁
* Skill Type  技能类型
* Upgrade Type  技能升级类型
* Cooldown  技能冷却时间
* Damage Scale Data  技能伤害倍率数据

这些配置会在游戏运行时被技能系统读取，并由对应技能脚本执行逻辑，例如：

```
Skill_Dash.cs
Skill_SwordThrow.cs
Skill_TimeEcho.cs
```

通过这种方式可以直接在 Inspector 中调整技能数值。

---

# 装备数据

装备配置通过 **EquipmentDataSO** ScriptableObject 管理。

装备数据目录：

```
Assets/Data/Equipment Data
```
![image](https://github.com/peinanlin/2D-RPG/blob/master/img/%E8%A3%85%E5%A4%87%E6%95%B0%E6%8D%AE.png)

装备数据包含以下字段：

* Item Price  商店出售价格
* Min Stack Size At Shop  商店最小出售数量
* Max Stack Size At Shop  商店最大出售数量

---

### 掉落系统配置

* Item Rarity  物品稀有度
* Drop Chance  基础掉落概率
* Max Drop Chance  最大掉落概率

这些参数会被 **Entity_DropManager 掉落系统**使用。

---

### 合成系统配置

* Craft Recipe  合成配方列表

用于武器合成系统。

---

### 装备基础信息

* Item Name  物品名称
* Item Icon  物品图标
* Item Type  物品类型
* Max Stack Size  最大堆叠数量

---

### 装备属性加成

装备可以提供多个属性加成。

每个 Modifier 包含：

* Stat Type  属性类型
* Value  属性数值

例如：

```
Strength +10
Intelligence +4
Health Regen +2.5
```

这些属性会在装备时添加到 **Entity_Stats 属性系统**中。

---

# 消耗品数据

消耗品通过 **ConsumableItemDataSO** 配置。

消耗品数据目录：

```
Assets/Data/Consumable Items Data
```
![image](https://github.com/peinanlin/2D-RPG/blob/master/img/%E6%B6%88%E8%80%97%E5%93%81%E6%95%B0%E6%8D%AE.png)

消耗品字段说明：

* Item Price  商店价格
* Min Stack Size At Shop  商店最小出售数量
* Max Stack Size At Shop  商店最大出售数量

---

### 掉落系统配置

* Item Rarity  掉落稀有度
* Drop Chance  掉落概率
* Max Drop Chance  最大掉落概率

---

### 合成配置

* Craft Recipe  合成配方

---

### 消耗品基础信息

* Item Name  物品名称
* Item Icon  物品图标
* Item Type  物品类型
* Max Stack Size  最大堆叠数量

---

### 消耗品效果

* Item Effect  物品使用时触发的效果

例如：

```
Portal Scroll
```

该效果会在使用时触发对应的 **ItemEffect_DataSO**。

---

# 物品效果数据

物品效果通过 **ItemEffect_DataSO** ScriptableObject 管理。

不同效果对应不同 ScriptableObject，例如：

* Heal On Damage
* Portal Scroll
* Buff Effects

当玩家使用消耗品或装备时，系统会读取对应效果并执行。
![image](https://github.com/peinanlin/2D-RPG/blob/master/img/%E7%89%A9%E5%93%81%E6%95%88%E6%9E%9C%E6%95%B0%E6%8D%AE.png)
---

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

