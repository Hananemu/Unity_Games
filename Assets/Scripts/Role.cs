using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 角色控制脚本类
public class Role : MonoBehaviour
{
    // 移动设置
    [Header("Movement Settings")]
    public float walkSpeed = 5.0f; // 行走速度
    public float runSpeed = 8.0f;  // 奔跑速度

    // 视角设置
    [Header("Look Settings")]
    public float mouseSensitivity = 2.0f; // 鼠标灵敏度
    public float maxVertRotation = 90.0f; // 相机垂直最大旋转角度
    public float minVertRotation = -90.0f; // 相机垂直最小旋转角度

    private CharacterController controller; // 角色控制器组件
    private float verticalRotation = 0f;    // 相机垂直旋转角度
    private Transform cameraTra;            // 相机的 Transform

    // 镜子相关
    private GameObject currentMirror; // 当前检测到的镜子
    public GameObject interactionButton; // UI按钮
    public float unselectDistance = 3f; // 取消选中的距离

    // 脚本加载时初始化
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        cameraTra = transform.Find("CameraTra");
        if (cameraTra == null)
            Debug.LogError("CameraTra 未找到");

        // 锁定并隐藏鼠标光标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // 每帧更新
    private void Update()
    {
        HandleMouseLook(); // 处理视角控制
        HandleMovement();  // 处理角色移动
        CheckMirrorInteraction(); // 检测镜子交互
        CheckUnselectMirror(); // 检测是否需要取消选中
    }

    // 处理鼠标视角控制
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX); // 水平旋转角色

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, minVertRotation, maxVertRotation);
        cameraTra.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f); // 垂直旋转相机
    }

    // 处理角色移动
    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move * currentSpeed * Time.deltaTime); // 移动角色
    }

    // 检测镜子交互
    void CheckMirrorInteraction()
    {
        // 获取玩家位置和相机位置
        Vector3 playerPosition = transform.position;
        Vector3 cameraPosition = cameraTra.position;

        // 遍历所有镜子
        Mirror[] mirrors = FindObjectsOfType<Mirror>();
        foreach (Mirror mirror in mirrors)
        {
            // 计算玩家与镜子的距离
            float distance = Vector3.Distance(playerPosition, mirror.transform.position);

            // 将镜子位置转换为屏幕坐标
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(mirror.transform.position);

            // 检查是否在视角中心范围内
            bool isInCenter = Mathf.Abs(screenPosition.x - Screen.width / 2) <= Screen.width * 0.1f &&
                              Mathf.Abs(screenPosition.y - Screen.height / 2) <= Screen.height * 0.1f;

            // 如果玩家靠近镜子且镜子在视角中心
            if (distance <= mirror.maxDistance && isInCenter)
            {
                // 保存当前镜子
                currentMirror = mirror.gameObject;

                // 显示UI按钮
                if (interactionButton != null)
                {
                    interactionButton.SetActive(true);
                }

                // 检测F键按下事件
                if (Input.GetKeyDown(KeyCode.F))
                {
                    SelectMirror(mirror);
                }

                return; // 只检测一个镜子
            }
        }

        // 如果没有满足条件的镜子，隐藏UI按钮
        if (interactionButton != null)
        {
            interactionButton.SetActive(false);
        }
    }

    // 检测是否需要取消选中
    void CheckUnselectMirror()
    {
        if (currentMirror != null)
        {
            // 计算玩家与当前选中镜子的距离
            float distance = Vector3.Distance(transform.position, currentMirror.transform.position);

            // 如果距离超过取消选中的范围
            if (distance > unselectDistance)
            {
                // 取消选中
                Mirror selectedMirror = currentMirror.GetComponent<Mirror>();
                if (selectedMirror != null)
                {
                    selectedMirror.isSelected = false;
                }

                // 重置当前镜子
                currentMirror = null;

                // 隐藏UI按钮
                if (interactionButton != null)
                {
                    interactionButton.SetActive(false);
                }
            }
        }
    }

    // 选中镜子
    void SelectMirror(Mirror mirror)
    {
        mirror.SelectMirror();
    }

    // 脚本禁用时解锁并显示鼠标光标
    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}