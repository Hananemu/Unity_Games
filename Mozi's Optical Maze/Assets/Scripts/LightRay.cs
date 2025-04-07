using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LightRay : MonoBehaviour
{
    public Transform lightSource;
    public Transform target;
    public LineRenderer lineRenderer;
    public int maxReflections = 10; // 最大反射次数
    public static event System.Action OnLevelCleared;
    public bool isLevelCleared = false;
    private bool isLightOn = false; // 初始光源为关闭状态
    private bool canToggleLight = true; // 是否可以切换光源状态
    public GameObject victoryPanel; // 胜利UI面板
    public GameObject Target; // 目标UI面板

    public void Start()
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

        // 确保胜利面板初始时是隐藏的
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
    }

    public void Update()
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
                else if (hit.collider.CompareTag("Lens"))
                {
                    Lens lens = hit.collider.GetComponent<Lens>();
                    if (lens != null)
                    {
                        currentPosition = hit.point;
                        currentDirection = CalculateRefraction(currentDirection, lens);
                    }
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
                        // 显示胜利UI面板和关闭目标UI面板
                        if (victoryPanel != null)
                        {
                            victoryPanel.SetActive(true);
                            Target.SetActive(false);
                        }
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

    // 计算折射方向
    Vector3 CalculateRefraction(Vector3 incomingDirection, Lens lens)
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

    void HideLightRay()
    {
        lineRenderer.positionCount = 0;
    }
}