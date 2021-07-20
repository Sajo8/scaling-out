using Godot;

public class DinoInfoResource : Resource
{
    [Export] public Enums.Dinos dinoType;
    [Export] public UnlockCost unlockCost;
    [Export] public int deployCost = 10;
    [Export] public Stats hpStat;
    [Export] public Stats delayStat;
    [Export] public Stats defStat;
    [Export] public Stats dodgeStat;
    [Export] public Stats dmgStat;
    [Export] public Stats speedStat;
    [Export] public SpecialStat specialStat;
    [Export] public Enums.Genes requiredGene = Enums.Genes.None;

    public void UpgradeStat(Stats stats)
    {
        stats.level++;
        SaveResource();
    }

    public void SaveResource()
    {
        string saveLocation = DinoInfo.Instance.GetDinoSaveLocation(dinoType);
        ResourceSaver.Save(saveLocation, this);
    }

}