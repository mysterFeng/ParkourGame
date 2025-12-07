using System.Collections;
using System.Collections.Generic;

using System;

// 这是一个静态类，用于集中存储和触发所有游戏事件
public static class GameEvents
{
    // 1. 定义事件：使用 Action<T> 简化委托定义
    // OnPlayerJump 无参数，用于简单的“跳跃”通知
    public static event Action OnPlayerJump; 
    
    // OnPlayerDied 无参数，用于“玩家死亡”通知
    public static event Action OnPlayerDied; 

    // 2. 触发事件的公共方法（供广播者调用，如 InputManager 或 PlayerMovement）
    public static void PlayerJump()
    {
        // 安全地触发事件：检查是否有订阅者，然后 Invoke
        OnPlayerJump?.Invoke();
    }
    
    public static void PlayerDied()
    {
        OnPlayerDied?.Invoke();
    }
}
