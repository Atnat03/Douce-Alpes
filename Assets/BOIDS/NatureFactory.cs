public enum NatureType { Dominant, Peureux, Curieux, Suiveur }

public static class NatureFactory
{
    public static INatureStrategy Create(NatureType type)
    {
        return type switch
        {
            NatureType.Dominant => new DominantNature(),
            _ => new PassifNature(),
        };
    }
}