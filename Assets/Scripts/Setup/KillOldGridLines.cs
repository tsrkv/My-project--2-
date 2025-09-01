using UnityEngine;

[DefaultExecutionOrder(-10000)]
public class KillOldGridLines : MonoBehaviour
{
    void Awake()
    {
        var all = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (var mb in all)
            if (mb && mb.GetType().Name == "GridBackgroundLines")
                mb.enabled = false;

        var gl = GameObject.Find("GridLines");
        if (gl) Destroy(gl);
    }
}
