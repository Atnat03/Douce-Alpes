using System.Collections.Generic;
using UnityEngine;

public class Nature_Happy : NatureBase
{
    public int Id { get; set; }
    public string Name { get; set; }

    public string message;
    public float happinessMultiplier;

    public void InitialiseNature(Dictionary<string, object> parameters)
    {
        if (parameters.ContainsKey("Id"))
            Id = (int)parameters["Id"];

        if (parameters.ContainsKey("Name"))
            Name = (string)parameters["Name"];

        if (parameters.ContainsKey("Message"))
            message = (string)parameters["Message"];

        if (parameters.ContainsKey("HappinessMultiplier"))
            happinessMultiplier = (float)parameters["HappinessMultiplier"];

        Debug.Log($"Nature {Name} initialisée avec message : {message} et multiplier : {happinessMultiplier}");
    }
}