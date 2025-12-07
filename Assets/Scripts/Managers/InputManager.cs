using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputManager : MonoBehaviour
{
    // 静态实例：全局唯一的访问点
    public static InputManager Instance { get; private set; }

    // 公共属性：存储移动轴的值（其他脚本可以直接读取）
    public float HorizontalInput { get; private set; }
    public float VerticalInput { get; private set; }

    private void Awake()
    {
        // 核心单例逻辑：保证唯一性并防止销毁
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad 确保在场景切换时管理器不会被销毁
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 如果场景中已存在实例，则销毁自身
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        // 1. 持续读取移动轴输入
        HorizontalInput = Input.GetAxis("Horizontal");
        VerticalInput = Input.GetAxis("Vertical");

        // 2. 💥 关键：检测跳跃输入，并广播事件 💥
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameEvents.PlayerJump();
        }
    }
}