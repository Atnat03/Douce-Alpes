public enum NatureType { Dominant, Peureux, Solitaire, Standard }

public static class NatureFactory
{
    public static INatureStrategy Create(NatureType type)
    {
        return type switch
        {
            NatureType.Dominant => new DominantNature(),
            NatureType.Peureux => new PeureuxNature(),
            NatureType.Solitaire => new SolitaireNature(),
            NatureType.Standard => new StandardNature(),
        };
    }
}