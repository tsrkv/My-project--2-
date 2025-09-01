using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem.UI;
#endif

/// Добавь этот компонент на пустой объект "Setup" в сцене.
/// Нажми Play — появятся камера, сетка, HUD и базовый уровень с лазером.
public class OneClickBootstrap : MonoBehaviour
{
    [Header("Field")]
    public int width = 12;
    public int height = 8;
    public float cellSize = 1f;
    public float topMarginWorld = 1.2f;

    void Awake()
    {
        // 1) Camera
        var cam = Camera.main;
        if (!cam)
        {
            var camGO = new GameObject("Main Camera");
            cam = camGO.AddComponent<Camera>();
            camGO.tag = "MainCamera";
        }
        cam.orthographic = true;
        cam.orthographicSize = 6f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        cam.transform.position = new Vector3(0, 0, -10);

        // 2) EventSystem (для UI)
        if (!FindAnyObjectByType<EventSystem>())
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            es.AddComponent<InputSystemUIInputModule>();
#else
            es.AddComponent<StandaloneInputModule>();
#endif
        }

        // 3) Grid visual (сетка на LineRenderer’ах)
        EnsureGridLines(cam);

        // 4) Core systems
        var grid = FindAnyObjectByType<GridController>() ?? new GameObject("GridController").AddComponent<GridController>();
        grid.Resize(new Vector2Int(width, height));

        var drawer = FindAnyObjectByType<BeamDrawer>() ?? new GameObject("BeamDrawer").AddComponent<BeamDrawer>();
        var ray    = FindAnyObjectByType<Raycaster>()   ?? new GameObject("Raycaster").AddComponent<Raycaster>();
        var loader = FindAnyObjectByType<LevelLoader>() ?? new GameObject("LevelLoader").AddComponent<LevelLoader>();
        ray.grid = grid; ray.drawer = drawer; loader.grid = grid;

        // 5) HUD
        if (!FindAnyObjectByType<RuntimeEditorHUD>())
        {
            var hud = new GameObject("HUD").AddComponent<RuntimeEditorHUD>();
            // ничего настраивать не нужно — он сам создаст Canvas + TopBar
        }

        // 6) Уровень по умолчанию (если в сцене пусто)
        var hasEmitter = FindAnyObjectByType<Emitter>();
        var hasAnyOpt  = FindAnyObjectByType<Optic>();
        if (!hasEmitter && !hasAnyOpt)
        {
            // emitter
            {
                var go = new GameObject("Emitter");
                var e = go.AddComponent<Emitter>();
                e.direction = Direction.Right;
                e.emitColor = BeamColor.White;
                go.transform.position = new Vector3(0, 4, 0);
            }
            // target
            {
                var go = new GameObject("Target");
                var t = go.AddComponent<Target>();
                t.required = BeamColor.White;
                grid.RegisterOptic(new Vector2Int(10, 4), t);
            }
            // mirror пример
            {
                var go = new GameObject("Mirror");
                var m = go.AddComponent<Mirror>();
                m.type = MirrorType.Backslash; // '\'
                grid.RegisterOptic(new Vector2Int(5, 4), m);
            }
        }

        // 7) Пересчёт лучей
        ray.Recompute();
    }

    void EnsureGridLines(Camera cam)
    {
        // удалим старую сетку, если была
        var old = GameObject.Find("GridLines");
        if (old) DestroyImmediate(old);

        // создаём корень
        var root = new GameObject("GridLines");
        // рассчитываем origin так, чтобы поле было по центру и чуть ниже верхнего края
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        Vector2 center = cam.transform.position;
        float gridW = width * cellSize;
        float gridH = height * cellSize;
        Vector2 origin = new Vector2(center.x - gridW * 0.5f,
                                     center.y - gridH * 0.5f - topMarginWorld);

        // материал
        var mat = new Material(Shader.Find("Unlit/Color"));
        mat.color = new Color(1,1,1,0.35f);

        // линии
        for (int x = 0; x <= width; x++)
            MakeLine(root.transform, mat,
                new Vector3(origin.x + x * cellSize, origin.y, 0),
                new Vector3(origin.x + x * cellSize, origin.y + height * cellSize, 0));

        for (int y = 0; y <= height; y++)
            MakeLine(root.transform, mat,
                new Vector3(origin.x, origin.y + y * cellSize, 0),
                new Vector3(origin.x + width * cellSize, origin.y + y * cellSize, 0));
    }

    void MakeLine(Transform parent, Material mat, Vector3 a, Vector3 b)
    {
        var go = new GameObject("Line");
        go.transform.SetParent(parent, false);
        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.positionCount = 2;
        lr.SetPosition(0, a);
        lr.SetPosition(1, b);
        lr.material = mat;
        lr.startWidth = lr.endWidth = 0.025f;
        lr.sortingOrder = -100;
        lr.numCapVertices = 2;
    }
}
