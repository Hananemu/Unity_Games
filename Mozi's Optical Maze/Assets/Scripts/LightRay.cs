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
    private bool isLightOn = false; // ��ʼ��ԴΪ�ر�״̬

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

        // ��ʼ����Դ������ɫ
        if (lightSource.GetComponent<Renderer>() != null)
        {
            lightSource.GetComponent<Renderer>().material.color = Color.gray;
        }

        // ��ʼʱ���ع���
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
            // ��Դ�ر�ʱ���ع���
            HideLightRay();
        }
    }

    // ����ť����¼��Ĺ�������
    public void ToggleLight()
    {
        isLightOn = !isLightOn;

        // �л���Դ������ɫ
        if (lightSource.GetComponent<Renderer>() != null)
        {
            Color newColor = isLightOn ? Color.yellow : Color.gray;
            lightSource.GetComponent<Renderer>().material.color = newColor;
        }

        // ���ݹ�Դ״̬��ʾ�����ع���
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
                    Debug.Log("�����ؿ�ͨ�أ�");
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