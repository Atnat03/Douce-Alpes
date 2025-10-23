using System.Collections.Generic;
using UnityEngine;

public abstract class NatureBase : INature
{
    public int Id { get; set; }
    public string Name { get; set; }

    public virtual void InitialiseNature(Dictionary<string, object> parameters) { }
    public virtual void OnCaresse(Sheep sheep) { }
    public virtual void OnTick(Sheep sheep, float deltaTime) { }
}