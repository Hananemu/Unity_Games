using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LightRayManager : MonoBehaviour
{
    public LightRay[] lightRays;

    void Start()
    {
        foreach (LightRay lightRay in lightRays)
        {
            lightRay.Start();
        }
    }

    void Update()
    {
        foreach (LightRay lightRay in lightRays)
        {
            lightRay.Update();
        }
    }
}    