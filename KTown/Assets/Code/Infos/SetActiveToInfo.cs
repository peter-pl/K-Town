using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SetActiveToInfo : InfoUnit
{
    public GameObject ObjOnWhenRevealed; //, ObjOnWhenFaked, ObjOnWhenIKnowFake;
    
    protected override void Refresh()
    {
        //ObjOnWhenFaked.SetActive(false);
        ObjOnWhenRevealed.SetActive(false);
        //ObjOnWhenIKnowFake.SetActive(false);
        if (IsReveled)
        {
            ObjOnWhenRevealed.SetActive(true);
        }
        else if (IsFaked)
        {
            //if (IKnowThis) ObjOnWhenIKnowFake.SetActive(true);
            //else ObjOnWhenFaked.SetActive(true);
        }
    }
}