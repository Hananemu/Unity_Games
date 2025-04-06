using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class LightGateSwitch : MonoBehaviour
{
    public bool isActivated = false; // 开关是否被激活
    public UnityEvent onActivated; // 开关激活时触发的事件
    public MeshRenderer switchRenderer; // 开关的渲染器，用于改变颜色
    public Color inactiveColor = Color.gray; // 未激活时的颜色
    public Color activeColor = Color.green; // 激活时的颜色
    public Animator doorAnimator; // 引用大门的 Animator 组件
    public string openDoorTrigger = "Open"; // 触发开门动画的参数名称

    public Transform lightSource; // 开关发射光线的起点
    public LineRenderer lightLineRenderer; // 用于绘制光线路径的 LineRenderer
    public float lightRange = 10f; // 光线的射程范围
    public LayerMask lightLayerMask; // 光线检测的层

    private List<Vector3> lightPath = new List<Vector3>(); // 用于保存光线路径

    private void Start()
    {
        if (switchRenderer != null)
        {
            switchRenderer.material.color = inactiveColor;
        }

        if (lightLineRenderer == null)
        {
            lightLineRenderer = gameObject.AddComponent<LineRenderer>();
            lightLineRenderer.startWidth = 0.1f;
            lightLineRenderer.endWidth = 0.1f;
            lightLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lightLineRenderer.startColor = Color.yellow;
            lightLineRenderer.endColor = Color.yellow;
        }

        HideLight();
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

            // 触发大门的开门动画
            if (doorAnimator != null)
            {
                doorAnimator.SetTrigger(openDoorTrigger);
            }

            // 开始发射光线
            StartCoroutine(ShootLight());
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
            StopAllCoroutines(); // 停止光线发射
            HideLight();
        }
    }

    private IEnumerator ShootLight()
    {
        while (isActivated) // 只要开关处于激活状态，就持续发射光线
        {
            Vector3 origin = lightSource.position;
            Vector3 direction = lightSource.forward;

            RaycastHit hit;
            if (Physics.Raycast(origin, direction, out hit, lightRange, lightLayerMask))
            {
                // 如果光线击中物体
                lightPath.Add(origin);
                lightPath.Add(hit.point);
            }
            else
            {
                // 如果光线没有击中物体
                lightPath.Add(origin);
                lightPath.Add(origin + direction * lightRange);
            }

            UpdateLightRenderer(); // 更新光线路径
            yield return null; // 等待下一帧
        }
    }

    private void UpdateLightRenderer()
    {
        // 更新 LineRenderer 的路径
        lightLineRenderer.positionCount = lightPath.Count;
        for (int i = 0; i < lightPath.Count; i++)
        {
            lightLineRenderer.SetPosition(i, lightPath[i]);
        }
    }

    private void HideLight()
    {
        lightLineRenderer.positionCount = 0;
        lightPath.Clear(); // 清空光线路径
    }
}