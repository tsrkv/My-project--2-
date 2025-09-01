using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem.UI;
#endif

[DefaultExecutionOrder(1000)]
public class RuntimeEditorHUD : MonoBehaviour
{
    [Header("Top bar")]
    public float topBarHeight = 48f;
    [Range(0f,1f)] public float topBarOpacity = 0.55f;
    public Vector2 buttonSize = new Vector2(100, 36);

    private Canvas canvas;
    private RectTransform bar;
    private Font builtinFont;

    void Awake()
    {
        // гарантируем EventSystem
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            es.AddComponent<InputSystemUIInputModule>();
#else
            es.AddComponent<StandaloneInputModule>();
#endif
        }
    }

    void Start()
    {
        // шрифт
        builtinFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (builtinFont == null)
        {
            try { builtinFont = Font.CreateDynamicFontFromOSFont("Arial", 14); }
            catch { builtinFont = new Font(); }
        }

        // Canvas (свой, как ребёнок HUD)
        var canvasGO = new GameObject("HUD_Canvas");
        canvasGO.transform.SetParent(this.transform, false);
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        // TopBar
        var barGO = new GameObject("TopBar");
        barGO.transform.SetParent(canvasGO.transform, false);

        var img = barGO.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0f, Mathf.Clamp01(topBarOpacity));

        // ⭐ ВАЖНО: безопасно получаем RectTransform
        bar = barGO.GetComponent<RectTransform>();
        if (bar == null) bar = barGO.AddComponent<RectTransform>();

        bar.anchorMin = new Vector2(0, 1);
        bar.anchorMax = new Vector2(1, 1);
        bar.pivot     = new Vector2(0.5f, 1f);
        bar.sizeDelta = new Vector2(0, topBarHeight);
        bar.anchoredPosition = Vector2.zero;

        var hlg = barGO.AddComponent<HorizontalLayoutGroup>();
        hlg.padding = new RectOffset(12, 12, 6, 6);
        hlg.spacing = 8;
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.childControlWidth = false;
        hlg.childControlHeight = false;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;

        // Кнопки
        AddButton("Restart", () =>
        {
            var gm = FindFirstObjectByType<GameManager>();
            if (gm != null) gm.ReloadLevel(); else Debug.LogWarning("[HUD] GameManager not found");
        });

        AddButton("Next", () =>
        {
            var gm = FindFirstObjectByType<GameManager>();
            if (gm != null) gm.LoadNextLevel(); else Debug.LogWarning("[HUD] GameManager not found");
        });

        AddButton("Recompute", () =>
        {
            var rc = FindFirstObjectByType<Raycaster>();
            if (rc != null) rc.Recompute(); else Debug.LogWarning("[HUD] Raycaster not found");
        });
    }

    Button AddButton(string label, System.Action onClick)
    {
        var go = new GameObject(label);
        go.transform.SetParent(bar, false);

        var bg = go.AddComponent<Image>();
        bg.color = new Color(1, 1, 1, 0.92f);

        var btn = go.AddComponent<Button>();
        if (onClick != null) btn.onClick.AddListener(() => onClick());

        var r = go.GetComponent<RectTransform>();
        r.sizeDelta = buttonSize;

        var tgo = new GameObject("Text");
        tgo.transform.SetParent(go.transform, false);
        var txt = tgo.AddComponent<Text>();
        txt.text = label;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.black;
        txt.font = builtinFont;

        var tr = tgo.GetComponent<RectTransform>();
        tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one;
        tr.offsetMin = Vector2.zero; tr.offsetMax = Vector2.zero;

        return btn;
    }
}
