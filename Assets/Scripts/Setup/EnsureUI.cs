using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem.UI;
#endif

[DefaultExecutionOrder(-10000)]
public class EnsureUI : MonoBehaviour
{
    void Awake()
    {
        var es = FindFirstObjectByType<EventSystem>();
        if (!es)
        {
            var go = new GameObject("EventSystem");
            es = go.AddComponent<EventSystem>();
        }
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        if (!es.GetComponent<InputSystemUIInputModule>()) es.gameObject.AddComponent<InputSystemUIInputModule>();
        var legacy = es.GetComponent<StandaloneInputModule>();
        if (legacy) Destroy(legacy);
#else
        if (!es.GetComponent<StandaloneInputModule>()) es.gameObject.AddComponent<StandaloneInputModule>();
        var newIM = es.GetComponent<InputSystemUIInputModule>();
        if (newIM) Destroy(newIM);
#endif
    }
}
