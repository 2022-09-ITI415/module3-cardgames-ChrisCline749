using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGolf : Card
{
        [Header("Set Dynamically: CardProspector")]

        public eCardState state = eCardState.drawpile;
        public List<CardGolf> hiddenBy = new List<CardGolf>();
        public int layoutID;
        public SlotDef slotDef;

    public override void OnMouseUpAsButton()
    {
        Golf.S.CardClicked(this);
        base.OnMouseUpAsButton();
    }
}
