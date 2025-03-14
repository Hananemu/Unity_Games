using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRay : MonoBehaviour
{
    public Transform lightSource;
    public Transform target;
    public LineRenderer lineRenderer;
    public int maxReflections = 2;

    public static event System.Action OnLevelCleared; // ͨ���¼�ί��
    public bool isLevelCleared = false; // ͨ�ر�־λ

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
    }

    void Update()
    {
        if (!isLevelCleared) // ֻ����δͨ��ʱ��ģ�����
        {
            SimulateLightRay();
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
                    isLevelCleared = true; // ����ͨ�ر�־λ
                    Debug.Log("�����ؿ�ͨ�أ�");
                    if (OnLevelCleared != null)
                    {
                        OnLevelCleared(); // ����ͨ���¼�
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
}