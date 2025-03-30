using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��ɫ���ƽű���
public class Role : MonoBehaviour
{
    // �ƶ�����
    [Header("Movement Settings")]
    public float walkSpeed = 5.0f; // �����ٶ�
    public float runSpeed = 8.0f;  // �����ٶ�

    // �ӽ�����
    [Header("Look Settings")]
    public float mouseSensitivity = 2.0f; // ���������
    public float maxVertRotation = 90.0f; // �����ֱ�����ת�Ƕ�
    public float minVertRotation = -90.0f; // �����ֱ��С��ת�Ƕ�

    private CharacterController controller; // ��ɫ���������
    private float verticalRotation = 0f;    // �����ֱ��ת�Ƕ�
    private Transform cameraTra;            // ����� Transform

    // �������
    private GameObject currentMirror; // ��ǰ��⵽�ľ���
    public GameObject interactionButton; // UI��ť
    public float unselectDistance = 3f; // ȡ��ѡ�еľ���

    // �ű�����ʱ��ʼ��
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        cameraTra = transform.Find("CameraTra");
        if (cameraTra == null)
            Debug.LogError("CameraTra δ�ҵ�");

        // ���������������
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ÿ֡����
    private void Update()
    {
        HandleMouseLook(); // �����ӽǿ���
        HandleMovement();  // �����ɫ�ƶ�
        CheckMirrorInteraction(); // ��⾵�ӽ���
        CheckUnselectMirror(); // ����Ƿ���Ҫȡ��ѡ��
    }

    // ��������ӽǿ���
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX); // ˮƽ��ת��ɫ

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, minVertRotation, maxVertRotation);
        cameraTra.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f); // ��ֱ��ת���
    }

    // �����ɫ�ƶ�
    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move * currentSpeed * Time.deltaTime); // �ƶ���ɫ
    }

    // ��⾵�ӽ���
    void CheckMirrorInteraction()
    {
        // ��ȡ���λ�ú����λ��
        Vector3 playerPosition = transform.position;
        Vector3 cameraPosition = cameraTra.position;

        // �������о���
        Mirror[] mirrors = FindObjectsOfType<Mirror>();
        foreach (Mirror mirror in mirrors)
        {
            // ��������뾵�ӵľ���
            float distance = Vector3.Distance(playerPosition, mirror.transform.position);

            // ������λ��ת��Ϊ��Ļ����
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(mirror.transform.position);

            // ����Ƿ����ӽ����ķ�Χ��
            bool isInCenter = Mathf.Abs(screenPosition.x - Screen.width / 2) <= Screen.width * 0.1f &&
                              Mathf.Abs(screenPosition.y - Screen.height / 2) <= Screen.height * 0.1f;

            // �����ҿ��������Ҿ������ӽ�����
            if (distance <= mirror.maxDistance && isInCenter)
            {
                // ���浱ǰ����
                currentMirror = mirror.gameObject;

                // ��ʾUI��ť
                if (interactionButton != null)
                {
                    interactionButton.SetActive(true);
                }

                // ���F�������¼�
                if (Input.GetKeyDown(KeyCode.F))
                {
                    SelectMirror(mirror);
                }

                return; // ֻ���һ������
            }
        }

        // ���û�����������ľ��ӣ�����UI��ť
        if (interactionButton != null)
        {
            interactionButton.SetActive(false);
        }
    }

    // ����Ƿ���Ҫȡ��ѡ��
    void CheckUnselectMirror()
    {
        if (currentMirror != null)
        {
            // ��������뵱ǰѡ�о��ӵľ���
            float distance = Vector3.Distance(transform.position, currentMirror.transform.position);

            // ������볬��ȡ��ѡ�еķ�Χ
            if (distance > unselectDistance)
            {
                // ȡ��ѡ��
                Mirror selectedMirror = currentMirror.GetComponent<Mirror>();
                if (selectedMirror != null)
                {
                    selectedMirror.isSelected = false;
                }

                // ���õ�ǰ����
                currentMirror = null;

                // ����UI��ť
                if (interactionButton != null)
                {
                    interactionButton.SetActive(false);
                }
            }
        }
    }

    // ѡ�о���
    void SelectMirror(Mirror mirror)
    {
        mirror.SelectMirror();
    }

    // �ű�����ʱ��������ʾ�����
    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}