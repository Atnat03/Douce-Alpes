using UnityEngine;

public interface INatureStrategy
{
    void ApplyNature(SheepBoid self, ref Vector3 separation, ref Vector3 alignment, ref Vector3 cohesion, SheepBoid other);

    void PostProcess(SheepBoid self, ref Vector3 velocity);
}