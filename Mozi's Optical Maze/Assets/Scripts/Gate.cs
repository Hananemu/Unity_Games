using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public LightGateSwitch lightGateSwitch; // 引用光闸开关
    public float openSpeed = 2f; // 开门速度
    public Vector3 openPosition; // 开门后的位置
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