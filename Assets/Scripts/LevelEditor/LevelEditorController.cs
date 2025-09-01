using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem; // новый Input System
#endif

public class LevelEditorController : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.E; // работает в fallback
    public bool editOn = true;

    GridController grid;
    Raycaster ray;

    void Awake() {
        grid = FindAnyObjectByType<GridController>();
        ray  = FindAnyObjectByType<Raycaster>();
    }

    void Update()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        // --- НОВЫЙ INPUT SYSTEM ---
        var kb = Keyboard.current;
        if (kb != null && kb.eKey.wasPressedThisFrame)
            editOn = !editOn;
        if (!editOn) return;

        var mouse = Mouse.current;
        if (mouse == null) return;
        if (mouse.leftButton.wasPressedThisFrame)  HandleLeft(mouse.position.ReadValue());
        if (mouse.rightButton.wasPressedThisFrame) HandleRight(mouse.position.ReadValue());
#else
        // --- LEGACY INPUT (если включён старый менеджер) ---
        if (Input.GetKeyDown(toggleKey)) editOn = !editOn;
        if (!editOn) return;

        if (Input.GetMouseButtonDown(0)) HandleLeft(Input.mousePosition);
        if (Input.GetMouseButtonDown(1)) HandleRight(Input.mousePosition);
#endif
    }

    Vector2Int ScreenToCell(Vector2 sp){
        var cam = Camera.main; if (!cam) return Vector2Int.zero;
        var wp = cam.ScreenToWorldPoint(new Vector3(sp.x, sp.y, -cam.transform.position.z));
        return new Vector2Int(Mathf.RoundToInt(wp.x), Mathf.RoundToInt(wp.y));
    }

    void HandleLeft(Vector2 sp){
        if (!grid) return;
        var c = ScreenToCell(sp);
        if (!grid.InBounds(c)) return;

        var opt = grid.Get(c);
        if (opt == null){
            // ставим зеркало
            var go = new GameObject("Mirror");
            var m = go.AddComponent<Mirror>();
            m.type = MirrorType.Slash;
            grid.RegisterOptic(c, m);
        } else {
            // удаляем объект
            Destroy(opt.gameObject);
            grid.ClearAt(c);
        }
        ray?.Recompute();
    }

    void HandleRight(Vector2 sp){
        if (!grid) return;
        var c = ScreenToCell(sp);
        var opt = grid.Get(c);
        if (opt is Mirror m){
            m.type = m.type == MirrorType.Slash ? MirrorType.Backslash : MirrorType.Slash;
            ray?.Recompute();
        }
    }
}
