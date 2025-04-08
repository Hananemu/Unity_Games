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
    public int maxReflections = 5; // 最大反射次数
    public int maxRefractions = 3; // 最大折射次数

    private List<Vector3> lightPath = new List<Vector3>(); // 用于保存光线路径
    private LightRay lightRay; // 用于存储 LightRay 脚本的引用

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

        // 查找场景中的 LightRay 脚本实例
        lightRay = FindObjectOfType<LightRay>();
        if (lightRay == null)
        {
            Debug.LogError("未找到 LightRay 脚本实例！");
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
            lightPath.Clear();
            Vector3 currentPosition = lightSource.position;
            Vector3 currentDirection = lightSource.forward;
            int reflectionCount = 0;
            int refractionCount = 0;

            lightPath.Add(currentPosition);

            for (int i = 0; i < maxReflections + maxRefractions; i++)
            {
                RaycastHit hit;
                if (Physics.Raycast(currentPosition, currentDirection, out hit, lightRange, lightLayerMask))
                {
                    lightPath.Add(hit.point);

                    if (hit.collider.CompareTag("Mirror") && reflectionCount < maxReflections)
                    {
                        currentPosition = hit.point;
                        currentDirection = Vector3.Reflect(currentDirection, hit.normal);
                        reflectionCount++;
                    }
                    else if (hit.collider.CompareTag("Lens") && refractionCount < maxRefractions)
                    {
                        // 简单示例：假设透镜将光线转向自身的前方
                        currentPosition = hit.point;
                        currentDirection = hit.collider.transform.forward;
                        refractionCount++;
                    }
                    else if (hit.collider.CompareTag("Target"))
                    {
                        if (lightRay != null)
                        {
                            lightRay.CallHandleTargetHit();
                        }
                        break;
                    }
                    else if (hit.collider.CompareTag("LightGateSwitch"))
                    {
                        LightGateSwitch gateSwitch = hit.collider.GetComponent<LightGateSwitch>();
                        if (gateSwitch != null)
                        {
                            gateSwitch.Activate();
                        }
                        else
                        {
                            Debug.LogError("LightGateSwitch 组件未在碰撞对象上找到！");
                        }
                        //移除 break 语句，让光线继续检测后续的开关
                        //break;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    lightPath.Add(currentPosition + currentDirection * lightRange);
                    break;
                }
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