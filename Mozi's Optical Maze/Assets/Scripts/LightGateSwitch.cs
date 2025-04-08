using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class LightGateSwitch : MonoBehaviour
{
    public bool isActivated = false; // �����Ƿ񱻼���
    public UnityEvent onActivated; // ���ؼ���ʱ�������¼�
    public MeshRenderer switchRenderer; // ���ص���Ⱦ�������ڸı���ɫ
    public Color inactiveColor = Color.gray; // δ����ʱ����ɫ
    public Color activeColor = Color.green; // ����ʱ����ɫ
    public Animator doorAnimator; // ���ô��ŵ� Animator ���
    public string openDoorTrigger = "Open"; // �������Ŷ����Ĳ�������
    public Transform lightSource; // ���ط�����ߵ����
    public LineRenderer lightLineRenderer; // ���ڻ��ƹ���·���� LineRenderer
    public float lightRange = 10f; // ���ߵ���̷�Χ
    public LayerMask lightLayerMask; // ���߼��Ĳ�
    public int maxReflections = 5; // ��������
    public int maxRefractions = 3; // ����������

    private List<Vector3> lightPath = new List<Vector3>(); // ���ڱ������·��
    private LightRay lightRay; // ���ڴ洢 LightRay �ű�������

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

        // ���ҳ����е� LightRay �ű�ʵ��
        lightRay = FindObjectOfType<LightRay>();
        if (lightRay == null)
        {
            Debug.LogError("δ�ҵ� LightRay �ű�ʵ����");
        }
    }

    // ����������ʱ����
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

            // �������ŵĿ��Ŷ���
            if (doorAnimator != null)
            {
                doorAnimator.SetTrigger(openDoorTrigger);
            }

            // ��ʼ�������
            StartCoroutine(ShootLight());
        }
    }

    // �����뿪ʱ���ã���ѡ��
    public void Deactivate()
    {
        if (isActivated)
        {
            isActivated = false;
            if (switchRenderer != null)
            {
                switchRenderer.material.color = inactiveColor;
            }
            StopAllCoroutines(); // ֹͣ���߷���
            HideLight();
        }
    }

    private IEnumerator ShootLight()
    {
        while (isActivated) // ֻҪ���ش��ڼ���״̬���ͳ����������
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
                        // ��ʾ��������͸��������ת�������ǰ��
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
                            Debug.LogError("LightGateSwitch ���δ����ײ�������ҵ���");
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
                    lightPath.Add(currentPosition + currentDirection * lightRange);
                    break;
                }
            }

            UpdateLightRenderer(); // ���¹���·��
            yield return null; // �ȴ���һ֡
        }
    }


    private void UpdateLightRenderer()
    {
        // ���� LineRenderer ��·��
        lightLineRenderer.positionCount = lightPath.Count;
        for (int i = 0; i < lightPath.Count; i++)
        {
            lightLineRenderer.SetPosition(i, lightPath[i]);
        }
    }

    private void HideLight()
    {
        lightLineRenderer.positionCount = 0;
        lightPath.Clear(); // ��չ���·��
    }
}