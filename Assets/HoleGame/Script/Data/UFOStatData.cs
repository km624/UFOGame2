[System.Serializable]
public class UFOStatData
{
    public UFOStatEnum StatType;
    public int BaseValue;
    public int MaxValue;

    public UFOStatData() { }

    public UFOStatData(UFOStatData other)
    {
        StatType = other.StatType;
        BaseValue = other.BaseValue;
        MaxValue = other.MaxValue;
    }
}