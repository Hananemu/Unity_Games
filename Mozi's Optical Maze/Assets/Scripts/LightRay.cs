using UnityEngine;
using UnityEngine.UI;

public class LightRay : MonoBehaviour
{
    public string lightRayTag = "LightRay"; // 设置标签名称
    public Transform lightSource; // 光源位置
    public Transform target; // 目标位置
    public int maxReflections = 10; // 最大反射次数
    public static event System.Action OnLevelCleared; // 通关事件
    public GameObject victoryPanel; // 胜利UI面板
    public GameObject targetUI; // 目标UI面板
    public GameObject teleportationPad; // 传送阵对象
    public float updateInterval = 0.1f; // 光线路径更新间隔
    private float timer = 0f;

    private void Start()
    {
        // 初始化胜利面板和传送阵
        SetActive(victoryPanel, false);
        SetActive(teleportationPad, false);

        // 查找所有带有指定标签的光线对象并初始化
        GameObject[] lightRays = GameObject.FindGameObjectsWithTag(lightRayTag);
        foreach (GameObject lightRay in lightRays)
        {
            LineRenderer lineRenderer = lightRay.GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = lightRay.AddComponent<LineRenderer>();
                InitializeLineRenderer(lineRenderer);
            }
        }
    }

    private void Update()
    {
        // 检测空格键按下事件
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleLight();
        }

        // 动态更新光线路径
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            UpdateLightRays();
        }
    }

    private void ToggleLight()
    {
        // 查找所有带有指定标签的光线对象
        GameObject[] lightRays = GameObject.FindGameObjectsWithTag(lightRayTag);
        foreach (GameObject lightRay in lightRays)
        {
            LineRenderer lineRenderer = lightRay.GetComponent<LineRenderer>();
            if (lineRenderer != null)
            {
                // 切换光线的显示状态
                if (lineRenderer.positionCount == 0)
                {
                    SimulateLightRay(lightRay, lineRenderer);
                }
                else
                {
                    lineRenderer.positionCount = 0;
                }
            }
        }
    }

    private void UpdateLightRays()
    {
        // 查找所有带有指定标签的光线对象
        GameObject[] lightRays = GameObject.FindGameObjectsWithTag(lightRayTag);
        foreach (GameObject lightRay in lightRays)
        {
            LineRenderer lineRenderer = lightRay.GetComponent<LineRenderer>();
            if (lineRenderer != null && lineRenderer.positionCount > 0)
            {
                SimulateLightRay(lightRay, lineRenderer);
            }
        }
    }

    private void SimulateLightRay(GameObject lightRay, LineRenderer lineRenderer)
    {
        // 获取光源位置和方向
        Transform lightSourceTransform = lightSource;
        Vector3 currentPosition = lightSourceTransform.position;
        Vector3 currentDirection = lightSourceTransform.forward;

        // 初始化 LineRenderer
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
                else if (hit.collider.CompareTag("Lens"))
                {
                    Lens lens = hit.collider.GetComponent<Lens>();
                    if (lens != null)
                    {
                        currentPosition = hit.point;
                        currentDirection = CalculateRefraction(currentDirection, lens);
                    }
                    else
                    {
                        Debug.LogError("Lens component not found on the hit object.");
                        break;
                    }
                }
                else if (hit.collider.CompareTag("Target"))
                {
                    HandleTargetHit();
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
                        Debug.LogError("LightGateSwitch component not found on the hit object.");
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
                break;
            }
        }
    }

    private Vector3 CalculateRefraction(Vector3 incomingDirection, Lens lens)
    {
        switch (lens.lensType)
        {
            case LensType.Convex:
                // 凸透镜使光线汇聚
                Vector3 focalPoint = lens.transform.position + lens.transform.forward * lens.focalLength;
                return (focalPoint - lens.transform.position).normalized;
            case LensType.Concave:
                // 凹透镜使光线发散
                Vector3 virtualFocalPoint = lens.transform.position - lens.transform.forward * lens.focalLength;
                return (lens.transform.position - virtualFocalPoint).normalized;
            default:
                return incomingDirection;
        }
    }

    private void HandleTargetHit()
    {
        Debug.Log("关卡通关！");
        OnLevelCleared?.Invoke();

        // 显示胜利UI面板和关闭目标UI面板
        SetActive(victoryPanel, true);
        SetActive(targetUI, false);
     
        // 激活传送阵
        SetActive(teleportationPad, true);
        
    }
    public void CallHandleTargetHit()
    {
        HandleTargetHit();
    }

    private void InitializeLineRenderer(LineRenderer lineRenderer)
    {
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.yellow;
    }

    private void SetActive(GameObject obj, bool active)
    {
        if (obj != null)
        {
            obj.SetActive(active);
        }
    }
}