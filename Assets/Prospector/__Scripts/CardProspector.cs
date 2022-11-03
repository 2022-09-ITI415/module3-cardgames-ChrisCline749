using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardProspector : MonoBehaviour
{
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
        public List<CardProspector> hiddenBy = new List<CardProspector>();
        public int layoutID;
        public SlotDef slotDef;
    }
}
