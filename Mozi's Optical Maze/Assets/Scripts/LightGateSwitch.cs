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

    private List<Vector3> lightPath = new List<Vector3>(); // ���ڱ������·��

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
            Vector3 origin = lightSource.position;
            Vector3 direction = lightSource.forward;

            RaycastHit hit;
            if (Physics.Raycast(origin, direction, out hit, lightRange, lightLayerMask))
            {
                // ������߻�������
                lightPath.Add(origin);
                lightPath.Add(hit.point);
            }
            else
            {
                // �������û�л�������
                lightPath.Add(origin);
                lightPath.Add(origin + direction * lightRange);
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