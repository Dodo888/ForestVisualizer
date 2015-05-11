namespace ForestCitizens
{
    public interface ILoader
    {
        IForest GetForest(string mapFile, string citizensFile);
        IForest GetForest(string mapFile);
    }
}
