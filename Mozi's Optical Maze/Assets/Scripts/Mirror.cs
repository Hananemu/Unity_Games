using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Mirror : MonoBehaviour
{
    public float rotationAngle = 15f;
    public GameObject buttonContainerPrefab; // ��ť����Ԥ����
    private GameObject buttonContainerInstance;
    private Button clockwiseButton;
    private Button counterClockwiseButton;
    private bool isButtonVisible = false;

    void Start()
    {
        // ʵ������ť����
        buttonContainerInstance = Instantiate(buttonContainerPrefab, GameObject.Find("Canvas").transform);
        buttonContainerInstance.SetActive(false);

        // ��ȡ��ť����
        clockwiseButton = buttonContainerInstance.transform.Find("ClockwiseButton").GetComponent<Button>();
        counterClockwiseButton = buttonContainerInstance.transform.Find("CounterClockwiseButton").GetComponent<Button>();

        if (clockwiseButton == null)
        {
            Debug.LogError("˳ʱ�밴ť����Ϊ�գ����Ԥ�����а�ť�����Ƿ�Ϊ ClockwiseButton");
        }
        if (counterClockwiseButton == null)
        {
            Debug.LogError("��ʱ�밴ť����Ϊ�գ����Ԥ�����а�ť�����Ƿ�Ϊ CounterClockwiseButton");
        }

        // Ϊ��ť��ӵ���¼�
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
            // ����Ƿ����˾��ӻ�ť���������
            if (!IsClickOnMirrorOrButton())
            {
                HideButtons();
            }
        }
    }

    void ShowButtons()
    {
        // ���ð�ťλ���ھ����Ϸ�
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
        // ����ѡ����ת���Ƿ����ذ�ť
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