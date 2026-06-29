# Soldier 与步兵营配置指南

## 1. 创建 CombatUnitConfig（步兵营）

1. 在 Project 窗口右键：`Create > Combat > Combat Unit Config`
2. 命名为 `InfantryBarracksConfig`
3. 填写字段：

| 字段 | 说明 | 建议值 |
|------|------|--------|
| id | 唯一标识 | `infantry_barracks` |
| displayName | 显示名 | 步兵营 |
| buildCost | 建造成本 | 50 |
| barracksPrefab | 步兵营建筑预制体 | 挂有 `InfantryBarracks` 的 Prefab |
| soldierPrefab | 士兵预制体 | 见下方「士兵预制体」 |
| baseSoldierStats | 士兵基础数值 | 见下方 |
| maxSoldierCapacity | 同时在场士兵上限 | 3 |
| spawnInterval | 生成间隔（秒） | 3 |

### baseSoldierStats（士兵数值）

在 `CombatUnitConfig` 的 `Base Soldier Stats` 折叠项中填写：

| 字段 | 说明 | 建议值 |
|------|------|--------|
| maxHealth | 最大生命 | 80 |
| moveSpeed | 移动速度 | 3 |
| damage | 单次攻击伤害 | 12 |
| attackInterval | 攻击间隔（秒） | 0.8 |

士兵生成时，步兵营会把这组数值注入 `SoldierCombat` 和 `CombatStats`，**不需要在 Soldier 预制体上重复填写移速/伤害**。

## 2. 士兵预制体组件清单

士兵 Prefab 必须包含：

| 组件 | 作用 |
|------|------|
| `Solider` | 状态机：上路 → 反向巡逻 → 追击 → 攻击 |
| `SoldierCombat` | 从 BaseSoldierStats 初始化，负责造成伤害 |
| `Health` | 血量 |
| `CombatStats` | 运行时数值容器（由配置注入） |
| `TeamMember` | Team = Ally |
| `TargetFollowing` | 目标跟随 |
| `TargetFinding` | 目标检测（同路径 + 敌方） |
| `Collider2D` | 碰撞 |

`Solider` 上已用 `[RequireComponent]` 强制要求 Health、CombatStats、TeamMember、SoldierCombat、TargetFollowing。

### 场景内单独放置测试士兵

若不放步兵营、直接在场景里放士兵：

1. 在 `SoldierCombat` 的 `Default Stats` 中填写与 `CombatUnitConfig.baseSoldierStats` 相同的数值
2. 或在运行时调用 `soldierCombat.Initialize(combatUnitConfig)`

## 3. 步兵营预制体

1. 创建空物体或建筑 Sprite，挂 `InfantryBarracks`
2. 指定 `Config` = 上一步创建的 `InfantryBarracksConfig`
3. 指定 `Spawn Point` = 士兵出生子物体 Transform
4. （可选）`Assigned Path` 仅作编辑器参考；士兵启用后会通过 `MoveToPathState` 自动找最近路径

## 4. 数据流

```text
CombatUnitConfig.baseSoldierStats
    → InfantryBarracks.SpawnSoldier()
    → SoldierCombat.Initialize(stats)
    → CombatStats + Health
    → Solider 状态机使用 CombatStats.MoveSpeed
    → AttackState 调用 SoldierCombat.TryAttack()
```

## 5. 攻击动画事件（必做）

伤害在 **Attack 动画最后一帧** 触发，不是代码每帧结算。

1. 在士兵 **Animator 所在子物体** 上挂 `CombatAnimationEvents`
2. 打开 Attack 动画片段（或 Animator 里该状态的 Motion）
3. 在**最后一帧**添加 Animation Event：
   - Function: `OnAttackHit`
   - 无需参数
4. 确保 `CombatUnitConfig.baseSoldierStats.damage > 0`

若 Attack 片段未勾选 Loop，代码会在播放结束后自动重播，以便连续攻击。

## 6. 敌人对照

敌人仍使用 `EnemyConfig`（`Create > Combat > Enemy Config`），由 `EnemyCombat` 初始化，与士兵的 `CombatUnitConfig` 分开管理。
