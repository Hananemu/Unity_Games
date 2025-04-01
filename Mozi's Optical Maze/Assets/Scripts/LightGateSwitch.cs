using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightGateSwitch : MonoBehaviour
{
    public bool isActivated = false; // 开关是否被激活
    public UnityEvent onActivated; // 开关激活时触发的事件
    public MeshRenderer switchRenderer; // 开关的渲染器，用于改变颜色
    public Color inactiveColor = Color.gray; // 未激活时的颜色
    public Color activeColor = Color.green; // 激活时的颜色

    private void Start()
    {
        if (switchRenderer != null)
        {
            switchRenderer.material.color = inactiveColor;
        }
    }

    // 被光线照射时调用
    public void Activate()
    {
        if (!isActivated)
        {
            isActivated = true;
            if (switchRenderer != null)
            {
                switchRenderer.material.color = activeColor;
            }
            onActivated.Invoke();
        }
    }

    // 光线离开时调用（可选）
    public void Deactivate()
    {
        if (isActivated)
        {
            isActivated = false;
            if (switchRenderer != null)
            {
                switchRenderer.material.color = inactiveColor;
            }
        }
    }
}