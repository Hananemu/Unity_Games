using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Start_Game : MonoBehaviour
{
    public void StartMenu() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);//������һ������
    }
}
