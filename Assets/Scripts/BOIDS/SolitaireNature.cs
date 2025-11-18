using UnityEngine;

public class SolitaireNature : INatureStrategy
{
    public void ApplyNature(SheepBoid self, ref Vector3 separation, ref Vector3 alignment, ref Vector3 cohesion, SheepBoid other)
    {
        if (other == null) return;

        cohesion = Vector3.zero;
    }

    public void PostProcess(SheepBoid self, ref Vector3 velocity)
    {
        
    }
}
