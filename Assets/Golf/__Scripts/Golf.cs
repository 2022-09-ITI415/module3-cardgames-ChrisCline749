﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Golf : MonoBehaviour {

	static public Golf 	S;

	[Header("Set in Inspector")]
	public TextAsset			deckXML;
	public TextAsset layoutXML;
	public float xOffset = 3;
	public float yOffset = -2.5f;
	public Vector3 layoutCenter;
	public Vector2 fsPosMid = new Vector2(0.5f, 0.9f);
	public Vector2 fsPosRun = new Vector2(0.5f, 0.75f);
	public Vector2 fsPosMid2 = new Vector2(0.4f, 1);
	public Vector2 fsPosEnd = new Vector2(0.5f, 0.95f);
	public float reloadDelay = 2;
	public Text gameOverText, roundResultText, highScoreText;

	[Header("Set Dynamically")]
	public Deck					deck;
	public Layout layout;
	public List<CardGolf> drawPile;
	public Transform layoutAnchor;
	public CardGolf target;
	public List<CardGolf> tableau;
	public List<CardGolf> discardPile;
	public FloatingScore fsRun;

	void Awake(){
		S = this;
		SetUpUiTexts();
	}

	void Start() {
		Scoreboard.S.score = ScoreManager.StatScore;

		deck = GetComponent<Deck> ();
		deck.InitDeck (deckXML.text);
		Deck.Shuffle(ref deck.cards);

		layout = GetComponent<Layout>();
		layout.ReadLayout(layoutXML.text);
		drawPile = ConvertListCardsToListCardGolfs(deck.cards);
		LayoutGame();
	}

	void SetUpUiTexts()
    {
		GameObject go = GameObject.Find("HighScore");
		if (go != null) highScoreText = go.GetComponent<Text>();

		int highscore = ScoreManager.highScore;
		string hScore = "Highscore: " + Utils.AddCommasToNumber(highscore);
		go.GetComponent<Text>().text = hScore;

		go = GameObject.Find("RoundResult");
		if (go != null) roundResultText = go.GetComponent<Text>();

		go = GameObject.Find("GameOver");
		if (go != null) gameOverText = go.GetComponent<Text>();

		ShowResultsUi(false);
	}

	public void ShowResultsUi(bool show)
    {
		gameOverText.gameObject.SetActive(show);
		roundResultText.gameObject.SetActive(show);
	}

	List<CardGolf> ConvertListCardsToListCardGolfs(List<Card> lCD)
    {
		List<CardGolf> lCP = new List<CardGolf>();
		CardGolf tCP;
		foreach (Card tCD in lCD)
        {
			tCP = tCD as CardGolf; //a
			lCP.Add(tCP);
        }
		return lCP;
    }

	CardGolf Draw()
    {
		CardGolf cd = drawPile[0];
		drawPile.RemoveAt(0);
		return cd;
    }

	void LayoutGame()
    {
		if (layoutAnchor == null)
        {
			GameObject tGo = new GameObject("_LayoutAnchor");
			layoutAnchor = tGo.transform;
			layoutAnchor.transform.position = layoutCenter;
        }

		CardGolf cp;

		foreach(SlotDef tSD in layout.slotDefs)
        {
			cp = Draw();
			cp.faceUp = tSD.faceUp;

			cp.transform.parent = layoutAnchor;
			cp.transform.localPosition = new Vector3(layout.multiplier.x * tSD.x, layout.multiplier.y * tSD.y, -tSD.layerID);
			cp.layoutID = tSD.id;
			cp.slotDef = tSD;
			cp.state = eCardState.tableau;

			cp.SetSortingLayerName(tSD.layerName);

			tableau.Add(cp);
        }

		foreach (CardGolf tCP in tableau)
        {
			foreach (int hid in tCP.slotDef.hiddenBy)
            {
				cp = FindCardByLayoutID(hid);
				tCP.hiddenBy.Add(cp);
            }
        }

		MoveToTarget(Draw());
		UpdateDrawPile();
    }

	CardGolf FindCardByLayoutID(int layoutID)
    {
		foreach (CardGolf tCP in tableau)
        {
			if (tCP.layoutID == layoutID) return tCP;
        }
		return null;
    }

	void SetTableauFaces()
    {
		foreach( CardGolf cd in tableau)
        {
			bool faceUp = true;
			foreach (CardGolf cover in cd.hiddenBy)
            {
				if (cover.state == eCardState.tableau) faceUp = false;
            }
			cd.faceUp = faceUp;
        }
		
    }

	void MoveToDiscard(CardGolf cd)
    {
		cd.state = eCardState.discard;
		discardPile.Add(cd);
		cd.transform.parent = layoutAnchor;

		cd.transform.localPosition = new Vector3(
			layout.multiplier.x * layout.discardPile.x,
			layout.multiplier.y * layout.discardPile.y,
			-layout.discardPile.layerID+0.5f);
		cd.faceUp = true;
		cd.SetSortingLayerName(layout.discardPile.layerName);
		cd.SetSortOrder(-100 + discardPile.Count);
    }

	void MoveToTarget(CardGolf cd)
	{
		if (target != null) MoveToDiscard(target);
		target = cd;
		cd.state = eCardState.target;
		cd.transform.parent = layoutAnchor;

		cd.transform.localPosition = new Vector3(
			layout.multiplier.x * layout.discardPile.x,
			layout.multiplier.y * layout.discardPile.y,
			-layout.discardPile.layerID);

		cd.faceUp = true;
		cd.SetSortingLayerName(layout.discardPile.layerName);
		cd.SetSortOrder(0);
	}

	void UpdateDrawPile()
    {
		CardGolf cd;

		for (int i = 0; i<drawPile.Count; i++)
        {
			cd = drawPile[i];
			cd.transform.parent = layoutAnchor;

			Vector2 dpStagger = layout.drawPile.stagger;

			cd.transform.localPosition = new Vector3(
			layout.multiplier.x * (layout.drawPile.x + i*dpStagger.x),
			layout.multiplier.y * (layout.drawPile.y + i * dpStagger.y),
			-layout.drawPile.layerID+0.1f*i);

			cd.faceUp = false;
			cd.state = eCardState.drawpile;
			cd.SetSortingLayerName(layout.drawPile.layerName);
			cd.SetSortOrder(-10*i);
		}
    }

	public void CardClicked(CardGolf cd)
    {
		switch(cd.state)
        {
			case eCardState.target:
				break;

			case eCardState.drawpile:
				MoveToDiscard(target);
				MoveToTarget(Draw());
				UpdateDrawPile();
				ScoreManager.EventCheck(eScoreEvent.draw);
				FloatingScoreHandler(eScoreEvent.draw);
				break;

			case eCardState.tableau:
				bool validMatch = true;
				if(!cd.faceUp) validMatch = false;
				if(!AdjacentRank(cd, target)) validMatch = false;
				if (!validMatch) return;
				tableau.Remove(cd);
				MoveToTarget(cd);
				SetTableauFaces();
				ScoreManager.EventCheck(eScoreEvent.mine);
				FloatingScoreHandler(eScoreEvent.mine);
				break;
        }
		CheckForGameOver();
    }

	void CheckForGameOver()
    {
		if (tableau.Count == 0)
        {
			GameOver(true);
			return;
        }

		if (drawPile.Count > 0) return;

		foreach (CardGolf cd in tableau)
        {
			if (AdjacentRank(cd, target)) return;
        }

		GameOver(false);
    }

	void GameOver (bool won)
    {
		int score = ScoreManager.StatScore;
		if (fsRun != null) score += fsRun.score;

		if (won)
		{
			gameOverText.text = "Round Over";
			roundResultText.text = "Round Score: " + score;
			ShowResultsUi(true);
			ScoreManager.EventCheck(eScoreEvent.gameWin);
			FloatingScoreHandler(eScoreEvent.gameWin);
		}
		else {
			gameOverText.text = "Game Over";
			if (ScoreManager.highScore <= score)
            {
				roundResultText.text = "New Highscore!\nFinal Score: " + score;
			}
			else
            {
				roundResultText.text = "Final Score: " + score;
			}
			ShowResultsUi(true);
			ScoreManager.EventCheck(eScoreEvent.gameLoss);
			FloatingScoreHandler(eScoreEvent.gameLoss);
		}
		Invoke("ReloadLevel", reloadDelay);
    }

	void ReloadLevel()
    {
		SceneManager.LoadScene("__Prospector_Scene_0");
	}

	public bool AdjacentRank(CardGolf c0, CardGolf c1)
    {
		if (!c0.faceUp || !c1.faceUp) return false;

		if (Mathf.Abs(c0.rank - c1.rank) == 1) return true;
		if (c0.rank == 1 && c1.rank == 13) return true;
		if (c0.rank == 13 && c1.rank == 1) return true;
		return false;
        
    }

	void FloatingScoreHandler (eScoreEvent evt)
    {
		List<Vector2> fsPts;

		switch (evt)
        {
			case eScoreEvent.draw:
			case eScoreEvent.gameWin:
			case eScoreEvent.gameLoss:
				if(fsRun != null)
                {
					fsPts = new List<Vector2>();
					fsPts.Add(fsPosRun);
					fsPts.Add(fsPosMid2);
					fsPts.Add(fsPosEnd);
					fsRun.reportFinishTo = Scoreboard.S.gameObject;
					fsRun.Init(fsPts, 0, 1);
					fsRun.fontSize = new List<float>(new float[] { 28, 36, 4 });
					fsRun = null;
				}
				break;

			case eScoreEvent.mine:
				FloatingScore fs;
				Vector2 p0 = Input.mousePosition;
				p0.x /= Screen.width;
				p0.y /= Screen.height;
				fsPts = new List<Vector2>();
				fsPts.Add(p0);
				fsPts.Add(fsPosMid);
				fsPts.Add(fsPosRun);
				fs = Scoreboard.S.CreateFloatingScore(ScoreManager.StatChain, fsPts);
				if(fsRun == null)
                {
					fsRun = fs;
					fsRun.reportFinishTo = null;
                }
                else
                {
					fs.reportFinishTo = fsRun.gameObject;
                }

				break;
        }
    }
}
