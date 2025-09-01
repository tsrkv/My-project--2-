using UnityEngine;

[DefaultExecutionOrder(-10000)]
public class KillOldGridLines : MonoBehaviour
{
    void Awake()
    {
        // Отключаем любые компоненты GridBackgroundLines (если остались где-то в сцене)
        var all = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (var mb in all)
            if (mb && mb.GetType().Name == "GridBackgroundLines")
                mb.enabled = false;

        // Удаляем уже созданный объект "GridLines"
        var gl = GameObject.Find("GridLines");
        if (gl) Destroy(gl);
    }
}
