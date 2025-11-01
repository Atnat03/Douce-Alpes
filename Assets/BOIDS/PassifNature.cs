using UnityEngine;

public class PassifNature : INatureStrategy
{
    public void ApplyNature(SheepBoid self, ref Vector3 separation, ref Vector3 alignment, ref Vector3 cohesion, SheepBoid other)
    {
        // comportement standard : ne change rien
    }

    public void PostProcess(SheepBoid self, ref Vector3 velocity) { }
}