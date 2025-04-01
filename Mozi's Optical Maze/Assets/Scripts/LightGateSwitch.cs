using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightGateSwitch : MonoBehaviour
{
    public bool isActivated = false; // �����Ƿ񱻼���
    public UnityEvent onActivated; // ���ؼ���ʱ�������¼�
    public MeshRenderer switchRenderer; // ���ص���Ⱦ�������ڸı���ɫ
    public Color inactiveColor = Color.gray; // δ����ʱ����ɫ
    public Color activeColor = Color.green; // ����ʱ����ɫ

    private void Start()
    {
        if (switchRenderer != null)
        {
            switchRenderer.material.color = inactiveColor;
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
        }
    }
}