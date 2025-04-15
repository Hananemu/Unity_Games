using UnityEngine;
using UnityEngine.UI;

public class LightRay : MonoBehaviour
{
    public string lightRayTag = "LightRay"; // ���ñ�ǩ����
    public Transform lightSource; // ��Դλ��
    public Transform target; // Ŀ��λ��
    public int maxReflections = 10; // ��������
    public static event System.Action OnLevelCleared; // ͨ���¼�
    public GameObject victoryPanel; // ʤ��UI���
    public GameObject targetUI; // Ŀ��UI���
    public GameObject teleportationPad; // ���������
    public float updateInterval = 0.1f; // ����·�����¼��
    private float timer = 0f;

    private void Start()
    {
        // ��ʼ��ʤ�����ʹ�����
        SetActive(victoryPanel, false);
        SetActive(teleportationPad, false);

        // �������д���ָ����ǩ�Ĺ��߶��󲢳�ʼ��
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
        // ���ո�������¼�
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleLight();
        }

        // ��̬���¹���·��
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            UpdateLightRays();
        }
    }

    private void ToggleLight()
    {
        // �������д���ָ����ǩ�Ĺ��߶���
        GameObject[] lightRays = GameObject.FindGameObjectsWithTag(lightRayTag);
        foreach (GameObject lightRay in lightRays)
        {
            LineRenderer lineRenderer = lightRay.GetComponent<LineRenderer>();
            if (lineRenderer != null)
            {
                // �л����ߵ���ʾ״̬
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
        // �������д���ָ����ǩ�Ĺ��߶���
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
        // ��ȡ��Դλ�úͷ���
        Transform lightSourceTransform = lightSource;
        Vector3 currentPosition = lightSourceTransform.position;
        Vector3 currentDirection = lightSourceTransform.forward;

        // ��ʼ�� LineRenderer
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
                    //�Ƴ� break ��䣬�ù��߼����������Ŀ���
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
                // ͹͸��ʹ���߻��
                Vector3 focalPoint = lens.transform.position + lens.transform.forward * lens.focalLength;
                return (focalPoint - lens.transform.position).normalized;
            case LensType.Concave:
                // ��͸��ʹ���߷�ɢ
                Vector3 virtualFocalPoint = lens.transform.position - lens.transform.forward * lens.focalLength;
                return (lens.transform.position - virtualFocalPoint).normalized;
            default:
                return incomingDirection;
        }
    }

    private void HandleTargetHit()
    {
        Debug.Log("�ؿ�ͨ�أ�");
        OnLevelCleared?.Invoke();

        // ��ʾʤ��UI���͹ر�Ŀ��UI���
        SetActive(victoryPanel, true);
        SetActive(targetUI, false);
     
        // �������
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