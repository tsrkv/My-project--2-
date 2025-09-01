// TODO: InputController implementation
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

public class InputController : MonoBehaviour {
    void Update(){
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        var mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame) HandleClick(mouse.position.ReadValue());
#else
        if (Input.GetMouseButtonDown(0)) HandleClick(Input.mousePosition);
#endif
    }

    void HandleClick(Vector2 screenPos){
        var cam = Camera.main; if (!cam) return;
        var wp = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -cam.transform.position.z));
        var cell = new Vector2Int(Mathf.RoundToInt(wp.x), Mathf.RoundToInt(wp.y));

        // Найдём оптику в ячейке
        var grid = FindAnyObjectByType<GridController>(); if (grid==null) return;
        var opt = grid.Get(cell); if (opt==null) return;

        if (opt is Mirror m){
            // цикл: Slash -> Backslash -> Slash
            m.type = m.type == MirrorType.Slash ? MirrorType.Backslash : MirrorType.Slash;
            FindAnyObjectByType<Raycaster>()?.Recompute();
        }
        if (opt is Filter f){
            // переключаем R->G->B
            f.pass = f.pass==BeamColor.R ? BeamColor.G : f.pass==BeamColor.G ? BeamColor.B : BeamColor.R;
            FindAnyObjectByType<Raycaster>()?.Recompute();
        }
    }
}
