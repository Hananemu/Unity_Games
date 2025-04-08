using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportationPad : MonoBehaviour
{
    private float timer = 0f;
    public float teleportTime = 3f; // ����ʱ��
    private bool isPlayerOnPad = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnPad = true;
            timer = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnPad = false;
            timer = 0f;
        }
    }

    private void Update()
    {
        if (isPlayerOnPad)
        {
            timer += Time.deltaTime;
            if (timer >= teleportTime)
            {
                // ������һ������
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                int nextSceneIndex = currentSceneIndex + 1;
                if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
                {
                    SceneManager.LoadScene(nextSceneIndex);
                }
                else
                {
                    Debug.Log("�Ѿ������һ��������û����һ�������ɼ��ء�");
                }
            }
        }
    }
}