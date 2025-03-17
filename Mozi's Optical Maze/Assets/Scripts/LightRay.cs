using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRay : MonoBehaviour
{
    public Transform lightSource;
    public Transform target;
    public LineRenderer lineRenderer;
    public int maxReflections = 10;//��������
    public static event System.Action OnLevelCleared;
    public bool isLevelCleared = false;
    private bool isLightOn = false; // ��ʼ��ԴΪ�ر�״̬
    private bool canToggleLight = true; // �Ƿ�����л���Դ״̬

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