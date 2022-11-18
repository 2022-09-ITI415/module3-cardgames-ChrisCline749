using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eGolfScoreEvent
{
    draw,
    mine,
    mineGold,
    gameWin,
    gameLoss
}

public class GolfScoreManager : MonoBehaviour
{
    static private GolfScoreManager S;

    static public int scoreFromLastRound;
    static public int highScore;

    [Header("Set Dynamically")]
    public int chain = 0;
    public int scoreRun = 0;
    public int score = 0;

    void Awake()
    {
        if (S == null) S = this;

        if (PlayerPrefs.HasKey("GolfHighScore")) highScore = PlayerPrefs.GetInt("GolfHighScore");

        score += scoreFromLastRound;
        scoreFromLastRound = 0;
    }

    static public void EventCheck (eScoreEvent evt)
    {
        try { S.Event(evt); }
        catch (System.NullReferenceException nre)
        {
            Debug.LogError("EventCheck called, but S=null.\n" + nre);
        }
    }

    void Event(eScoreEvent evt)
    {
        switch (evt)
        {
            case eScoreEvent.draw:
            case eScoreEvent.gameWin:
            case eScoreEvent.gameLoss:
                chain = 0;
                score += scoreRun;
                scoreRun = 0;
                break;

            case eScoreEvent.mine:
                chain++;
                scoreRun += chain;
                break;
        }

        switch (evt)
        {
            case eScoreEvent.gameWin:
                scoreFromLastRound = score;
                print("You won the round! Round Score: " + score);
                break;

            case eScoreEvent.gameLoss:
                if(highScore <= score)
                {
                    print("New High Score! Score: " + score);
                    highScore = score;
                    PlayerPrefs.SetInt("ProspectorHighScore", score);
                }
                else
                {
                    print("Final Score: " + score);
                }
                break;

            default:
                print("score: " + score + "  Run Score: " + scoreRun + "  Chain: " + chain);
                break;
        }
    }

    static public int StatChain { get { return S.chain; } }
    static public int StatScore { get { return S.score; } }
    static public int StatScoreRun { get { return S.scoreRun; } }
}
