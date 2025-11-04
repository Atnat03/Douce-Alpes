using UnityEngine;

public interface INatureStrategy
{
    // Modifie les forces selon la nature
    void ApplyNature(SheepBoid self, ref Vector3 separation, ref Vector3 alignment, ref Vector3 cohesion, SheepBoid other);

    // Modifie éventuellement la vitesse ou direction finale
    void PostProcess(SheepBoid self, ref Vector3 velocity);
}