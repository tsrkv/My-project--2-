using UnityEngine;

[DefaultExecutionOrder(-500)]
public class EnsureCore : MonoBehaviour
{
    void Awake()
    {
        // Grid + BeamDrawer + Raycaster + LevelLoader
        var grid = FindAnyObjectByType<GridController>() ?? new GameObject("GridController").AddComponent<GridController>();
        var drawer = FindAnyObjectByType<BeamDrawer>() ?? new GameObject("BeamDrawer").AddComponent<BeamDrawer>();
        var ray = FindAnyObjectByType<Raycaster>() ?? new GameObject("Raycaster").AddComponent<Raycaster>();
        var loader = FindAnyObjectByType<LevelLoader>() ?? new GameObject("LevelLoader").AddComponent<LevelLoader>();

        ray.grid = grid; ray.drawer = drawer; loader.grid = grid;

        // размер поля (под твою сетку)
        if (grid.size == Vector2Int.zero) grid.Resize(new Vector2Int(12, 8));

        // если в сцене пусто — ставим демо-уровень
        if (!FindAnyObjectByType<Emitter>() && !FindAnyObjectByType<Optic>())
        {
            var eGo = new GameObject("Emitter");
            var e = eGo.AddComponent<Emitter>();
            e.direction = Direction.Right; e.emitColor = BeamColor.White;
            eGo.transform.position = new Vector3(0, 4, 0);

            var tGo = new GameObject("Target");
            var t = tGo.AddComponent<Target>(); t.required = BeamColor.White;
            grid.RegisterOptic(new Vector2Int(10, 4), t);

            var mGo = new GameObject("Mirror");
            var m = mGo.AddComponent<Mirror>(); m.type = MirrorType.Backslash;
            grid.RegisterOptic(new Vector2Int(5, 4), m);
        }
        else
        {
            // регистрируем оптику, размещённую вручную в сцене
            var optics = Object.FindObjectsByType<Optic>(FindObjectsSortMode.None);
            foreach (var o in optics)
            {
                var p = o.transform.position;
                var c = new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.y));
                grid.RegisterOptic(c, o);
            }
        }

        // пуск луча
        ray.Recompute();
    }
}
