using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModelListeSO", menuName = "Scriptable Objects/ModelListeSO")]
public class ModelListeSO : ScriptableObject
{
    public List<ModelDrawSO> listeModel = new List<ModelDrawSO>();
}
