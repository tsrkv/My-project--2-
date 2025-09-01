using UnityEngine;

public class RegisterSceneOptics : MonoBehaviour
{
    void Start()
    {
        var grid = FindAnyObjectByType<GridController>();
        var ray  = FindAnyObjectByType<Raycaster>();
        if (!grid || !ray) { Debug.LogWarning("GridController/Raycaster not found"); return; }

        // на всякий случай
        if (grid.size == Vector2Int.zero) grid.Resize(new Vector2Int(12, 8));

        // зарегистрировать все Optic в сетке (Target/Mirror/Wall/Filter и т.п.)
        foreach (var o in Object.FindObjectsByType<Optic>(FindObjectsSortMode.None))
        {
            var p = o.transform.position;
            var cell = new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.y));
            grid.RegisterOptic(cell, o);
        }

        ray.Recompute();
        Destroy(this); // одноразово
    }
}
