// 2026.6.29 --v1 
// 全局游戏管理器 —— 负责游戏整体流程控制。
// 职责（持续扩展中）：
//   - 游戏状态管理（开始/暂停/结束）
//   - 全局资源配置与访问
//   - 跨模块事件协调
// 用法：作为单例挂载在场景中的 GameManager 对象上。
//       其他脚本通过 FindObjectOfType / 静态实例访问。
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {
    }
}
