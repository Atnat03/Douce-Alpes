using System;
using System.Collections.Generic;
using UnityEngine;

public class SkinBuildManagerGrange : SkinBuildManager
{
    public Grange grange;   
    
    public new void SwapSkin(int id)
    {
        base.SwapSkin(id);

        grange.doorAnimator = skins[id].GetComponent<Animator>();
    }
}
