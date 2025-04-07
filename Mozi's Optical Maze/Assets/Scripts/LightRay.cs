using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LightRay : MonoBehaviour
{
    public Transform lightSource;
    public Transform target;
    public LineRenderer lineRenderer;
    public int maxReflections = 10; // ��������
    public static event System.Action OnLevelCleared;
    public bool isLevelCleared = false;
    private bool isLightOn = false; // ��ʼ��ԴΪ�ر�״̬
    private bool canToggleLight = true; // �Ƿ�����л���Դ״̬
    public GameObject victoryPanel; // ʤ��UI���
    public GameObject Target; // Ŀ��UI���

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

        // ȷ��ʤ������ʼʱ�����ص�
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
    }

    public void Update()
    {
        // ���ո�������¼������canToggleLightΪfalse������Ӧ
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
                        Debug.Log("�����ؿ�ͨ�أ�");
                        if (OnLevelCleared != null)
                        {
                            OnLevelCleared();
                        }
                        // ͨ�غ��ֹ�л���Դ״̬�������ֹ�Դ����
                        canToggleLight = false;
                        isLightOn = true;
                        if (lightSource.GetComponent<Renderer>() != null)
                        {
                            lightSource.GetComponent<Renderer>().material.color = Color.yellow;
                        }
                        // �������о��ӵ���ת����
                        Mirror[] allMirrors = FindObjectsOfType<Mirror>();
                        foreach (Mirror mirror in allMirrors)
                        {
                            mirror.DisableRotation();
                        }
                        // ��ʾʤ��UI���͹ر�Ŀ��UI���
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

    // �������䷽��
    Vector3 CalculateRefraction(Vector3 incomingDirection, Lens lens)
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

    void HideLightRay()
    {
        lineRenderer.positionCount = 0;
    }
}