using UnityEngine;

public class DominantNature : INatureStrategy
{
    public void ApplyNature(SheepBoid self, ref Vector3 separation, ref Vector3 alignment, ref Vector3 cohesion, SheepBoid other)
    {
        if (other == null) return;

        if (other.natureStrategy is DominantNature)
        {
            Vector3 awayFromOther = self.transform.position - other.transform.position;
            separation += awayFromOther.normalized * 2f;
        }
        else
        {
            Vector3 towardOther = other.transform.position - self.transform.position;
            cohesion += towardOther.normalized * 2f;
        }
    }

    public void PostProcess(SheepBoid self, ref Vector3 velocity)
    {
        velocity *= 1.1f;
    }
}