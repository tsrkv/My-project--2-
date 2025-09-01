// TODO: LevelLoader implementation
using UnityEngine;
using System.IO;

[System.Serializable] public class LevelData { public int width=12, height=8; public Obj[] objects; }
[System.Serializable] public class Obj {
    public string type; public int x; public int y;
    public string dir; public int color; public string mirror; public int pass;
}
// Вверху файла оставляем как есть

public class LevelLoader : MonoBehaviour {
    public GridController grid;

    public void Load(string fileName){
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "Levels", fileName);
        if (!System.IO.File.Exists(path)){ Debug.LogError("No level: " + path); return; }
        var json = System.IO.File.ReadAllText(path);
        var data = JsonUtility.FromJson<LevelData>(json);

        // очистка старых объектов
        foreach (var o in Object.FindObjectsByType<Optic>(FindObjectsSortMode.None))
            Destroy(o.gameObject);
        foreach (var e in Object.FindObjectsByType<Emitter>(FindObjectsSortMode.None))
            Destroy(e.gameObject);

        // ← ВАЖНО: корректно меняем размер сетки (см. патч 2)
        grid.Resize(new Vector2Int(data.width, data.height));

        foreach (var o in data.objects){
            var cell = new Vector2Int(o.x, o.y);
            switch (o.type) {
                case "Emitter": {
                    var go = new GameObject("Emitter");
                    var c = go.AddComponent<Emitter>();
                    c.direction = ParseDir(o.dir);
                    c.emitColor = (BeamColor)o.color;
                    go.transform.position = new Vector3(cell.x, cell.y, 0);
                    // Эмиттер НЕ регистрируем в grid!
                } break;

                case "Target": {
                    var go = new GameObject("Target"); var c = go.AddComponent<Target>();
                    c.required = (BeamColor)(o.color == 0 ? (int)BeamColor.White : o.color);
                    grid.RegisterOptic(cell, c);
                } break;

                case "Mirror": {
                    var go = new GameObject("Mirror"); var c = go.AddComponent<Mirror>();
                    c.type = (o.mirror=="Backslash") ? MirrorType.Backslash : MirrorType.Slash;
                    grid.RegisterOptic(cell, c);
                } break;

                case "Wall": {
                    var go = new GameObject("Wall"); var c = go.AddComponent<Wall>();
                    grid.RegisterOptic(cell, c);
                } break;

                case "Filter": {
                    var go = new GameObject("Filter"); var c = go.AddComponent<Filter>();
                    c.pass = (BeamColor)(o.pass==0 ? (int)BeamColor.R : o.pass);
                    grid.RegisterOptic(cell, c);
                } break;
            }
        }

        Object.FindFirstObjectByType<Raycaster>()?.Recompute();
    }

    Direction ParseDir(string s) =>
        s=="Up"?Direction.Up : s=="Right"?Direction.Right : s=="Down"?Direction.Down : Direction.Left;
}
