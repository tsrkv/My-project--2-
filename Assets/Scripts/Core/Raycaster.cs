// TODO: Raycaster implementation
using UnityEngine;
using System.Collections.Generic;

public class Raycaster : MonoBehaviour {
    public GridController grid;
    public BeamDrawer drawer;

    readonly HashSet<(Vector2Int, Direction, BeamColor)> visited = new();
    readonly List<(Vector3 a, Vector3 b, BeamColor col)> segments = new();

    public void Recompute(){
        visited.Clear(); segments.Clear();

        // Сбрасываем цели
        foreach (var t in Object.FindObjectsByType<Target>(FindObjectsSortMode.None))
            t.isSatisfied = false;

        // Пускаем лучи от всех эмиттеров
        foreach (var e in Object.FindObjectsByType<Emitter>(FindObjectsSortMode.None)) {
            var cell = RoundCell(e.transform.position);
            Cast(new Beam{ Cell = cell, Dir = e.direction, Color = e.emitColor });
        }
        drawer.Draw(segments);

        // Победа?
        bool allOk = true;
        var targets = Object.FindObjectsByType<Target>(FindObjectsSortMode.None);
        if (targets.Length>0){
            foreach (var t in targets) if (!t.isSatisfied) { allOk=false; break; }
            if (allOk) LevelEvents.RaiseComplete();
        }
    }

    void Cast(Beam start){
        var q = new Queue<Beam>(); q.Enqueue(start);
        while (q.Count>0){
            var b = q.Dequeue();
            if (!visited.Add((b.Cell,b.Dir,b.Color))) continue;

            var cur = b.Cell;
            while (true){
                var next = cur + DirUtil.Step(b.Dir);
                var fromW = new Vector3(cur.x, cur.y, 0);
                var toW   = new Vector3(next.x, next.y, 0);
                segments.Add((fromW, toW, b.Color));

                if (!grid.InBounds(next)) break;

                var opt = grid.Get(next);
                if (opt == null){ cur = next; continue; }

                if (opt is Target t){
                    if ((b.Color & t.required) == t.required) t.isSatisfied = true;
                }

                if (opt.Blocks) break;

                var (stop, outs) = opt.Interact(new Beam { Cell = next, Dir = b.Dir, Color = b.Color });
                if (outs != null) foreach (var nb in outs) q.Enqueue(nb);
                if (stop) break;

                // Если оптика «пропускает» луч дальше — продолжаем с next
                cur = next;
            }
        }
    }

    Vector2Int RoundCell(Vector3 wp) => new(Mathf.RoundToInt(wp.x), Mathf.RoundToInt(wp.y));
}
