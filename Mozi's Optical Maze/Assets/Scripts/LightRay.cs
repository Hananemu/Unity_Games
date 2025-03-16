using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightRay : MonoBehaviour
{
    public Transform lightSource;
    public Transform target;
    public LineRenderer lineRenderer;
    public int maxReflections = 1;
    public static event System.Action OnLevelCleared;
    public bool isLevelCleared = false;
    private bool isLightOn = false; // 初始光源为关闭状态

    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.yellow;
            lineRenderer.endColor = Color.yellow;
        }

        // 初始化光源材质颜色
        if (lightSource.GetComponent<Renderer>() != null)
        {
            lightSource.GetComponent<Renderer>().material.color = Color.gray;
        }

        // 初始时隐藏光线
        HideLightRay();
    }

    void Update()
    {
        if (isLightOn)
        {
            SimulateLightRay();
        }
        else
        {
            // 光源关闭时隐藏光线
            HideLightRay();
        }
    }

    // 处理按钮点击事件的公共方法
    public void ToggleLight()
    {
        isLightOn = !isLightOn;

        // 切换光源材质颜色
        if (lightSource.GetComponent<Renderer>() != null)
        {
            Color newColor = isLightOn ? Color.yellow : Color.gray;
            lightSource.GetComponent<Renderer>().material.color = newColor;
        }

        // 根据光源状态显示或隐藏光线
        if (isLightOn)
        {
            SimulateLightRay();
        }
        else
        {
            HideLightRay();
        }
    }

    void SimulateLightRay()
    {
        Vector3 currentPosition = lightSource.position;
        Vector3 currentDirection = lightSource.forward;
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, currentPosition);

        for (int i = 0; i < maxReflections; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(currentPosition, currentDirection, out hit))
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

                if (hit.collider.CompareTag("Mirror"))
                {
                    currentPosition = hit.point;
                    currentDirection = Vector3.Reflect(currentDirection, hit.normal);
                }
                else if (hit.collider.CompareTag("Target"))
                {
                    isLevelCleared = true;
                    Debug.Log("初级关卡通关！");
                    if (OnLevelCleared != null)
                    {
                        OnLevelCleared();
                    }
                    break;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
    }

    void HideLightRay()
    {
        lineRenderer.positionCount = 0;
    }
}