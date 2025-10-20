using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameMode{
    idle,
    playing,
    levelEnd,
    gameOver
}

public class MissionDemolition : MonoBehaviour
{
    static private MissionDemolition S;

    [Header("Inscribed")]
    public Text uitLevel;
    public Text uitShots;
    public Text uitScore;
    public Vector3 castlePos;
    public GameObject[] castles;
    public GameObject gameOverPanel;
    public Text gameOverText;
    public Button playAgainButton;

    [Header("Dynamic")]
    public int level;
    public int levelMax;
    public int shotsTaken;
    public int totalScore;
    public GameObject castle;
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot";

    void Start()
    {
        S = this;

        level = 0;
        shotsTaken = 0;
        totalScore = 0;
        levelMax = castles.Length;
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(PlayAgain);
        }
        
        StartLevel();
    }

    void StartLevel(){
        if (castle != null){
            Destroy(castle);
        }

        Projectile.DESTROY_PROJECTILES();

        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;

        Goal.goalMet = false;

        UpdateGUI();

        mode = GameMode.playing;

        FollowCam.SWITCH_VIEW(FollowCam.eView.both);
    }

    void UpdateGUI(){
        uitLevel.text = "Level: "+(level+1)+" of "+levelMax;
        uitShots.text = "Shots Taken: "+shotsTaken;
        if (uitScore != null) {
            uitScore.text = "Score: "+totalScore;
        }
    }

    void Update()
    {
        UpdateGUI();

        if((mode ==GameMode.playing)&& Goal.goalMet){
            mode=GameMode.levelEnd;
            
            int baseScore = 1000;
            int shotPenalty = shotsTaken * 50;
            int levelScore = Mathf.Max(baseScore - shotPenalty, 100);
            totalScore += levelScore;

            FollowCam.SWITCH_VIEW(FollowCam.eView.both);

            Invoke("NextLevel", 2f);
        }
    }

    void NextLevel(){
        level++;
        if (level == levelMax){
            ShowGameOver();
        }
        else
        {
            StartLevel();
        }
    }

    void ShowGameOver(){
        mode = GameMode.gameOver;
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        if (gameOverText != null)
        {
            gameOverText.text = "Game Complete!\nTotal Score: " + totalScore + "\nTotal Shots: " + shotsTaken;
        }
    }

    void PlayAgain(){
        level = 0;
        shotsTaken = 0;
        totalScore = 0;
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        StartLevel();
    }

    static public void SHOT_FIRED(){
        S.shotsTaken++;
    }
    
    static public GameObject GET_CASTLE(){
        return S.castle;
    }
}