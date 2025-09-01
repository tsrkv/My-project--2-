// TODO: GridController implementation
using UnityEngine;

public class GridController : MonoBehaviour {
    public Vector2Int size = new Vector2Int(12, 8);
    private Optic[,] cells;

    void Awake() { cells = new Optic[size.x, size.y]; }

    public void Resize(Vector2Int newSize){
        size = newSize;
        cells = new Optic[size.x, size.y];
    }

    public bool InBounds(Vector2Int c) => c.x>=0 && c.y>=0 && c.x<size.x && c.y<size.y;

    public void RegisterOptic(Vector2Int c, Optic o) {
        if (!InBounds(c)) return;
        cells[c.x, c.y] = o;
        o.transform.position = new Vector3(c.x, c.y, 0);
    }

    public void ClearAt(Vector2Int c) { if (InBounds(c)) cells[c.x,c.y] = null; }
    public Optic Get(Vector2Int c) => InBounds(c) ? cells[c.x, c.y] : null;
}
