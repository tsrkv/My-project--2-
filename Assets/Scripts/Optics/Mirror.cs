// TODO: Mirror implementation
using UnityEngine;
using System.Collections.Generic;

public enum MirrorType { Slash, Backslash } // '/' и '\'

public class Mirror : Optic {
    public MirrorType type = MirrorType.Slash;

    public override (bool stop, List<Beam> outs) Interact(Beam incoming) {
        var outDir = incoming.Dir;

        if (type == MirrorType.Slash) {
            // '/' отражает: Up->Left, Right->Down, Down->Right, Left->Up
            outDir = incoming.Dir switch {
                Direction.Up    => Direction.Left,
                Direction.Right => Direction.Down,
                Direction.Down  => Direction.Right,
                Direction.Left  => Direction.Up,
                _ => incoming.Dir
            };
        } else {
            // '\' отражает: Up->Right, Right->Up, Down->Left, Left->Down
            outDir = incoming.Dir switch {
                Direction.Up    => Direction.Right,
                Direction.Right => Direction.Up,
                Direction.Down  => Direction.Left,
                Direction.Left  => Direction.Down,
                _ => incoming.Dir
            };
        }

        return (false, new List<Beam>{ new Beam{ Cell = incoming.Cell, Dir = outDir, Color = incoming.Color } });
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.white;
        var p = transform.position;
        if (type == MirrorType.Slash) {
            Gizmos.DrawLine(p + new Vector3(-0.4f, -0.4f), p + new Vector3(0.4f, 0.4f));
        } else {
            Gizmos.DrawLine(p + new Vector3(-0.4f, 0.4f), p + new Vector3(0.4f, -0.4f));
        }
    }
}
