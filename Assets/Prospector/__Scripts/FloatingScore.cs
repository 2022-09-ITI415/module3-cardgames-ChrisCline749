using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum eFSState
{
    idle,
    pre,
    active,
    post
}

public class FloatingScore : MonoBehaviour
{
    [Header ("Set Dynamicly")]
    public eFSState state = eFSState.idle;

    [SerializeField]
    protected int _score = 0;
    public string scoreString;

    public List<Vector2> bezierPts;
    public List<float> fontSize;
    public float timeStart = -1f;
    public float timeDuration = 1f;
    public string easingCurve = Easing.InOut;

    public GameObject reportFinishTo = null;

    private RectTransform rectTrans;
    private Text txt;

    public int score
    {
        get { return (_score); }
        set { 
            _score = value;
            scoreString = _score.ToString("N0");
            GetComponent<Text>().text = scoreString;
        }
    }
    
    void Update()
    {
        if (state == eFSState.idle) return;

        float u = (Time.time - timeStart) / timeDuration;
        float uc = Easing.Ease(u, easingCurve);

        if (u<0)
        {
            state = eFSState.pre;
            txt.enabled = false;
        }
        else
        {
            if (u>=1)
            {
                uc = 1;
                state = eFSState.post;
                if (reportFinishTo != null)
                {
                    reportFinishTo.SendMessage("FSCallback", this);
                    Destroy(gameObject);
                }
                else
                {
                    state = eFSState.idle
                }
            }
            else
            {
                state = eFSState.active;
                txt.enabled = true;
            }
            Vector2 pos = Utils.Bezier(uc, bezierPts);

            rectTrans.anchorMin = rectTrans.anchorMax = pos;

            if (fontSize != null && fontSize.Count > 0)
            {
                int size = Mathf.RoundToInt(Utils.Bezier(uc, fontSize));
                GetComponent<Text>().fontSize = size;
            }
        }
    }

    public void Init(List<Vector2> ePts, float eTimeS = 0, float eTimeD = 1)
    {
        rectTrans = GetComponent<RectTransform>();
        rectTrans.anchoredPosition = Vector2.zero;

        txt = GetComponent<Text>();

        bezierPts = new List<Vector2>(ePts);

        if (ePts.Count == 1)
        {
            transform.position = ePts[0];
            return;
        }

        if (eTimeS == 0) eTimeS = Time.time;

        timeStart = eTimeS;
        timeDuration = eTimeD;

        state = eFSState.pre;
    }

    public void FSCallback (FloatingScore fs)
    {
        score += fs.score;
    }


}
