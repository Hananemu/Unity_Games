using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Mirror : MonoBehaviour
{
    public float rotationAngle = 15f;
    public GameObject buttonContainerPrefab; // 按钮容器预制体
    private GameObject buttonContainerInstance;
    private Button clockwiseButton;
    private Button counterClockwiseButton;
    private bool isButtonVisible = false;

    void Start()
    {
        // 实例化按钮容器
        buttonContainerInstance = Instantiate(buttonContainerPrefab, GameObject.Find("Canvas").transform);
        buttonContainerInstance.SetActive(false);

        // 获取按钮引用
        clockwiseButton = buttonContainerInstance.transform.Find("ClockwiseButton").GetComponent<Button>();
        counterClockwiseButton = buttonContainerInstance.transform.Find("CounterClockwiseButton").GetComponent<Button>();

        if (clockwiseButton == null)
        {
            Debug.LogError("顺时针按钮引用为空，检查预制体中按钮名称是否为 ClockwiseButton");
        }
        if (counterClockwiseButton == null)
        {
            Debug.LogError("逆时针按钮引用为空，检查预制体中按钮名称是否为 CounterClockwiseButton");
        }

        // 为按钮添加点击事件
        if (clockwiseButton != null)
        {
            clockwiseButton.onClick.AddListener(() => RotateMirror(-rotationAngle));
        }
        if (counterClockwiseButton != null)
        {
            counterClockwiseButton.onClick.AddListener(() => RotateMirror(rotationAngle));
        }
    }
    void OnMouseDown()
    {
        if (!isButtonVisible)
        {
            ShowButtons();
        }
        else
        {
            HideButtons();
        }
    }

    void Update()
    {
        if (isButtonVisible && Input.GetMouseButtonDown(0))
        {
            // 检测是否点击了镜子或按钮以外的区域
            if (!IsClickOnMirrorOrButton())
            {
                HideButtons();
            }
        }
    }

    void ShowButtons()
    {
        // 设置按钮位置在镜子上方
        buttonContainerInstance.transform.position = Camera.main.WorldToScreenPoint(transform.position) + new Vector3(0, 100, 0);

        buttonContainerInstance.SetActive(true);
        isButtonVisible = true;
    }

    void HideButtons()
    {
        buttonContainerInstance.SetActive(false);
        isButtonVisible = false;
    }

    void RotateMirror(float angle)
    {
        transform.Rotate(Vector3.up, angle);
        // 可以选择旋转后是否隐藏按钮
        // HideButtons();
    }

    bool IsClickOnMirrorOrButton()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        GraphicRaycaster gr = buttonContainerInstance.GetComponentInParent<GraphicRaycaster>();
        if (gr != null)
        {
            System.Collections.Generic.List<RaycastResult> results = new System.Collections.Generic.List<RaycastResult>();
            gr.Raycast(eventDataCurrentPosition, results);
            foreach (RaycastResult result in results)
            {
                if (result.gameObject == clockwiseButton.gameObject || result.gameObject == counterClockwiseButton.gameObject)
                {
                    return true;
                }
            }
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                return true;
            }
        }

        return false;
    }
}