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
    static public int roundCount;

    [Header("Set Dynamically")]
    public int chain = 0;
    public int roundScore = 0;
    public int score = 0;

    void Awake()
    {
        if (S == null)
        {
            S = this;
        }

        if (roundCount == 0)
        {
            roundCount = 1;
        }

        if (!PlayerPrefs.HasKey("GolfHighScore")) PlayerPrefs.SetInt("GolfHighScore", 1000);
        highScore = PlayerPrefs.GetInt("GolfHighScore");

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
            case eScoreEvent.gameWin:
            case eScoreEvent.gameLoss:
                for (int i = 0; i < this.GetComponent<Golf>().tableau.Count; i++)
                {
                    roundScore++;
                }
                chain = 0;
                score += roundScore;
                break;
        }

        switch (evt)
        {
            case eScoreEvent.gameWin:
                scoreFromLastRound = score;
                print("Round Score: " + roundScore + "Round: " + roundCount);
                roundCount++;
                break;

            case eScoreEvent.gameLoss:
                if(highScore >= score)
                {
                    print("New High Score! Score: " + score);
                    highScore = score;
                    PlayerPrefs.SetInt("GolfHighScore", score);
                }
                else
                {
                    print("Final Score: " + score);
                }
                roundCount = 1;
                break;

            default:
                print("score: " + score + "  Run Score: " + roundScore + "  Chain: " + chain);
                break;
        }
    }

    static public int StatChain { get { return S.chain; } }
    static public int StatScore { get { return S.score; } }
    static public int StatScoreRun { get { return S.roundScore; } }
}
