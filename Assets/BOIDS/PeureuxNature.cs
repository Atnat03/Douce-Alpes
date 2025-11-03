using UnityEngine;

public class PeureuxNature : INatureStrategy
{
    public void ApplyNature(SheepBoid self, ref Vector3 separation, ref Vector3 alignment, ref Vector3 cohesion, SheepBoid other)
    {
        if(other == null) return;
        
        Vector3 awayFromOther = self.transform.position - other.transform.position;
        separation += awayFromOther.normalized * 2f;
    }

    public void PostProcess(SheepBoid self, ref Vector3 velocity)
    {
        velocity *= 0.9f;
    }
}
