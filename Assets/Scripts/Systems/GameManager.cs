// TODO: GameManager implementation
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour {
    public LevelLoader loader;
    public Raycaster raycaster;
    public GridController grid;
    public BeamDrawer drawer;

    public int currentIndex = 1;
    public int maxIndex = 10;

    void Awake(){
        grid ??= FindAnyObjectByType<GridController>() ?? new GameObject("Grid").AddComponent<GridController>();
        drawer ??= FindAnyObjectByType<BeamDrawer>() ?? new GameObject("BeamDrawer").AddComponent<BeamDrawer>();
        raycaster ??= FindAnyObjectByType<Raycaster>() ?? new GameObject("Raycaster").AddComponent<Raycaster>();
        loader ??= FindAnyObjectByType<LevelLoader>() ?? new GameObject("LevelLoader").AddComponent<LevelLoader>();

        raycaster.grid = grid; raycaster.drawer = drawer; loader.grid = grid;

        // ensure StreamingAssets/Levels/level_001.json
        var dir = System.IO.Path.Combine(Application.streamingAssetsPath,"Levels");
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        var lvl = System.IO.Path.Combine(dir,"level_001.json");
        if (!File.Exists(lvl)) File.WriteAllText(lvl,
            "{ \"width\": 12, \"height\": 8, \"objects\": [" +
            "{ \"type\":\"Emitter\",\"x\":0,\"y\":4,\"dir\":\"Right\",\"color\":7 }," +
            "{ \"type\":\"Target\",\"x\":10,\"y\":4,\"color\":7 }," +
            "{ \"type\":\"Mirror\",\"x\":5,\"y\":4,\"mirror\":\"Backslash\" }" +
            "]}");
    }

    void Start(){ LoadLevel(currentIndex); }

    public void LoadLevel(int idx){
        currentIndex = idx;
        loader.Load($"level_{idx:000}.json");
    }

    public void ReloadLevel() => LoadLevel(currentIndex);
    public void LoadNextLevel(){ if (currentIndex<maxIndex) LoadLevel(currentIndex+1); }
}
