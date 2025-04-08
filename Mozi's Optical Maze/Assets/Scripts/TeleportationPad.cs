using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportationPad : MonoBehaviour
{
    private float timer = 0f;
    public float teleportTime = 3f; // 传送时间
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
                // 加载下一个场景
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                int nextSceneIndex = currentSceneIndex + 1;
                if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
                {
                    SceneManager.LoadScene(nextSceneIndex);
                }
                else
                {
                    Debug.Log("已经是最后一个场景，没有下一个场景可加载。");
                }
            }
        }
    }
}