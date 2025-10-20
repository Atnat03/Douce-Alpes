using System.Collections.Generic;

public interface INature
{
    public int Id { get; set; }
    public string Name { get; set; }

    void InitialiseNature(Dictionary<string, object> parameters);
}