using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRay : MonoBehaviour
{
    public Transform lightSource;
    public Transform target;
    public LineRenderer lineRenderer;
    public int maxReflections = 10;//最大发射次数
    public static event System.Action OnLevelCleared;
    public bool isLevelCleared = false;
    private bool isLightOn = false; // 初始光源为关闭状态
    private bool canToggleLight = true; // 是否可以切换光源状态

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

        if (lightSource.GetComponent<Renderer>() != null)
        {
            lightSource.GetComponent<Renderer>().material.color = Color.gray;
        }

        HideLightRay();
    }

    void Update()
    {
        // 检测空格键按下事件，如果canToggleLight为false，则不响应
        if (canToggleLight && Input.GetKeyDown(KeyCode.Space))
        {
            ToggleLight();
        }

        if (isLightOn)
        {
            SimulateLightRay();
        }
        else
        {
            HideLightRay();
        }
    }

    public void ToggleLight()
    {
        if (!canToggleLight) return;

        isLightOn = !isLightOn;

        if (lightSource.GetComponent<Renderer>() != null)
        {
            Color newColor = isLightOn ? Color.yellow : Color.gray;
            lightSource.GetComponent<Renderer>().material.color = newColor;
        }

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
                    if (!isLevelCleared)
                    {
                        isLevelCleared = true;
                        Debug.Log("初级关卡通关！");
                        if (OnLevelCleared != null)
                        {
                            OnLevelCleared();
                        }
                        // 通关后禁止切换光源状态，但保持光源开启
                        canToggleLight = false;
                        isLightOn = true;
                        if (lightSource.GetComponent<Renderer>() != null)
                        {
                            lightSource.GetComponent<Renderer>().material.color = Color.yellow;
                        }
                        // 禁用所有镜子的旋转功能
                        Mirror[] allMirrors = FindObjectsOfType<Mirror>();
                        foreach (Mirror mirror in allMirrors)
                        {
                            mirror.DisableRotation();
                        }
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