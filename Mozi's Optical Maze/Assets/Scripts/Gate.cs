using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public LightGateSwitch lightGateSwitch; // ���ù�բ����
    public float openSpeed = 2f; // �����ٶ�
    public Vector3 openPosition; // ���ź��λ��
    private Vector3 closedPosition;
    private bool isOpening = false;

    private void Start()
    {
        closedPosition = transform.position;
        if (lightGateSwitch != null)
        {
            lightGateSwitch.onActivated.AddListener(OpenGate);
        }
    }

    private void Update()
    {
        if (isOpening)
        {
            transform.position = Vector3.MoveTowards(transform.position, openPosition, openSpeed * Time.deltaTime);
            if (transform.position == openPosition)
            {
                isOpening = false;
            }
        }
    }

    public void OpenGate()
    {
        isOpening = true;
    }
}