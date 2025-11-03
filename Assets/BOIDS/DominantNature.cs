using UnityEngine;

public class DominantNature : INatureStrategy
{
    public void ApplyNature(SheepBoid self, ref Vector3 separation, ref Vector3 alignment, ref Vector3 cohesion, SheepBoid other)
    {
        if (other == null) return;
        // Les autres sont attirés par lui
        if (other.natureStrategy is DominantNature)
            cohesion += (other.transform.position - self.transform.position) * 2f;
    }

    public void PostProcess(SheepBoid self, ref Vector3 velocity)
    {
        velocity *= 1.1f;
    }
}