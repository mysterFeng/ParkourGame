using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // 自动在 Start() 中获取 Rigidbody
    private Rigidbody rb;
    
    // 地面状态，用于控制跳跃
    [HideInInspector] public bool isGrounded = true; 

    [Header("玩家属性")]
    public float forwardForce = 1000f;
    public float sidewaysForce = 50f;
    public float jumpForce = 500f;

    [Header("地面检测设置")]
    // 射线起点相对于玩家中心的位置（Y值通常为负，需根据玩家模型调整）
    public Vector3 raycastOffset = new Vector3(0, -0.9f, 0); 
    // 射线长度，比 offset 稍长
    public float raycastDistance = 0.1f; 
    // 💥 关键：只检测地面层
    public LayerMask groundLayer; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // 建议在 Start 中检查 Rigidbody 是否存在
        if (rb == null)
        {
            Debug.LogError("PlayerMovement requires a Rigidbody component!");
            enabled = false; // 如果没有 Rigidbody，禁用脚本
        }
    }

    // ===================================
    // 💥 事件订阅：处理跳跃输入 💥
    // ===================================

    private void OnEnable()
    {
        // 订阅 GameEvents 中的跳跃事件
        GameEvents.OnPlayerJump += HandleJumpEvent;
    }

    private void OnDisable()
    {
        // 始终取消订阅，防止内存泄漏！
        GameEvents.OnPlayerJump -= HandleJumpEvent;
    }

    // 响应跳跃事件的方法
    private void HandleJumpEvent()
    {
        // 只有在地面上才执行跳跃
        if (isGrounded) 
        {
            // 使用 Impulse 瞬间施加力
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            
            // 可以在这里调用 AudioManager.Instance.PlayJumpSFX();
        }
    }

    // ===================================
    // 💥 物理更新：移动和地面检测 💥
    // ===================================

    void FixedUpdate()
    {
        // 1. 始终先检查地面状态
        CheckIfGrounded();

        // 2. 检查 InputManager 是否已初始化
        if (InputManager.Instance == null) return;
        
        // 3. 从 InputManager 单例读取输入
        float horizontalInput = InputManager.Instance.HorizontalInput;
        float verticalInput = InputManager.Instance.VerticalInput;
        
        // 4. 施加前进力 (使用 Time.fixedDeltaTime 保证帧率独立性)
        rb.AddForce(0, 0, verticalInput * forwardForce * Time.fixedDeltaTime);
        
        // 5. 施加侧向力 (只在地面上施加，使用 ForceMode.VelocityChange 增强控制感)
        if (isGrounded)
        {
            rb.AddForce(horizontalInput * sidewaysForce * Time.fixedDeltaTime, 0, 0, ForceMode.VelocityChange);
        }
        
        // 6. 死亡检测（可选：如果玩家掉出地图）
        if (transform.position.y < -5f)
        {
             GameEvents.PlayerDied(); // 广播死亡事件
        }
    }
    
    // 射线地面检测的核心逻辑
    private void CheckIfGrounded()
    {
        // 射线起点：玩家位置 + 偏移
        Vector3 origin = transform.position + raycastOffset;
        
        // 射线方向：向下
        Vector3 direction = Vector3.down;
        
        // 执行射线检测：只检测 raycastDistance 长度内的 groundLayer
        bool hitGround = Physics.Raycast(origin, direction, raycastDistance, groundLayer);
        
        isGrounded = hitGround;

        // 【调试用】在 Scene 视图中绘制射线，以便调试 offset 和 distance
        // Debug.DrawRay(origin, direction * raycastDistance, isGrounded ? Color.green : Color.red);
    }
}