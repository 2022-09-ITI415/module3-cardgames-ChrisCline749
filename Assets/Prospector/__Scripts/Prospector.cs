using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum eCardState
{
    drawpile,
    tableau,
    target,
    discard
}

public class Prospector : Card
{
    [Header("Set Dynamically: CardProspector")]

    public eCardState state = eCardState.drawpile;
    public List<CardProspector> hiddenBy = new List<CardProspector>;
    public int layoutID;
    public SlotDef slotDef;
}
