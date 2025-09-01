public enum Direction { Up, Right, Down, Left }

public static class DirUtil {
    public static UnityEngine.Vector2Int Step(Direction d) =>
        d == Direction.Up ? new UnityEngine.Vector2Int(0,1) :
        d == Direction.Right ? new UnityEngine.Vector2Int(1,0) :
        d == Direction.Down ? new UnityEngine.Vector2Int(0,-1) :
        new UnityEngine.Vector2Int(-1,0);

    public static Direction TurnRight(Direction d) =>
        d == Direction.Up ? Direction.Right :
        d == Direction.Right ? Direction.Down :
        d == Direction.Down ? Direction.Left : Direction.Up;
}
