using UnityEngine;

public class CollideModelCadenas : MonoBehaviour
{
    [SerializeField] private ParticleSystem touchGroundEffect;

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Debug.Log("CollideModelCadenas");
            touchGroundEffect.Play();
        }
    }
}
