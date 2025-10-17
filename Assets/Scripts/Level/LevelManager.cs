using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    
    public static LevelManager Instance { get; private set; }

    public ILevel level;

    private void Awake() {
        Instance = this;
    }

}
