using UnityEngine;

[RequireComponent(typeof(Emitter))]
public class EmitterView : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        var p = transform.position;
        Gizmos.DrawWireCube(p, Vector3.one * 0.8f);
        // стрелка направления
        Gizmos.color = Color.red;
        Vector3 dir = Vector3.right;
        var e = GetComponent<Emitter>();
        switch (e.direction)
        {
            case Direction.Right: dir = Vector3.right; break;
            case Direction.Left: dir = Vector3.left; break;
            case Direction.Up: dir = Vector3.up; break;
            case Direction.Down: dir = Vector3.down; break;
        }
        Gizmos.DrawLine(p, p + dir * 0.6f);
    }
}
