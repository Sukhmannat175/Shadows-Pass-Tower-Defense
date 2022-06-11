using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDifficultyManager : MonoBehaviour
{
    public enum GameDifficulty { EASY = 0, NORMAL = 1, DIFFICULT = 2 };

    // "Public" variables
    [Header("Game Difficulty")]
    [SerializeField] private GameDifficulty currentGameDifficulty;
  
    public static GameDifficultyManager instance;

    // Properties
    public GameDifficulty CurrentGameDifficulty { get { return currentGameDifficulty; } }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // Methods
    public void setEasyGameDifficulty()
    {
        currentGameDifficulty = GameDifficulty.EASY;
    }
    public void setNormalGameDifficulty()
    {
        currentGameDifficulty = GameDifficulty.NORMAL;
    }
    public void setDifficultGameDifficulty()
    {
        currentGameDifficulty = GameDifficulty.DIFFICULT;
    }
}
