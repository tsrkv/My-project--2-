// TODO: LevelEvents implementation
using System;

public static class LevelEvents {
    public static event Action OnComplete;
    public static void RaiseComplete() => OnComplete?.Invoke();
}
