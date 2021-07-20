using System.Collections.Generic;
using System.Linq;
using Godot;

public class DinoInfo : Node
{
    public static DinoInfo Instance;

    public Dictionary<Enums.Dinos, PackedScene> dinoList;
    public Dictionary<Enums.Dinos, StreamTexture> dinoIcons;
    public Dictionary<Enums.Dinos, UpgradeInfo> upgradesInfo;
    public Dictionary<Enums.Dinos, Enums.SpecialAbilities> dinoTypesAndAbilities;
    public Dictionary<Enums.SpecialAbilities, StreamTexture> specialAbilityIcons;
    public Dictionary<Enums.SpecialAbilities, VideoStream> specialAbilityVidPreviews;

    public DinoInfo()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        Instance = this;

        dinoList = new Dictionary<Enums.Dinos, PackedScene>()
        {
            {Enums.Dinos.Mega, GD.Load<PackedScene>("res://src/combat/dinos/MegaDino.tscn")}, 
            {Enums.Dinos.Tanky, GD.Load<PackedScene>("res://src/combat/dinos/TankyDino.tscn")},
            {Enums.Dinos.Warrior, GD.Load<PackedScene>("res://src/combat/dinos/WarriorDino.tscn")},
            {Enums.Dinos.Gator, GD.Load<PackedScene>("res://src/combat/dinos/GatorGecko.tscn")},
        };

        dinoIcons = new Dictionary<Enums.Dinos, StreamTexture>()
        {
            {Enums.Dinos.Mega, GD.Load<StreamTexture>("res://assets/dinos/mega_dino/mega_dino.png")},
            {Enums.Dinos.Tanky, GD.Load<StreamTexture>("res://assets/dinos/tanky_dino/Armored_Dino_ICON.png")},
            {Enums.Dinos.Warrior, GD.Load<StreamTexture>("res://assets/dinos/warrior_dino/Tribal_Dino_icon.png")},
            {Enums.Dinos.Gator, GD.Load<StreamTexture>("res://assets/dinos/gator_gecko/gator_gecko_icon.png")},
        };

        upgradesInfo = new Dictionary<Enums.Dinos, UpgradeInfo>() 
        {
            {Enums.Dinos.Mega, new UpgradeInfo(GetDinoSaveLocation(Enums.Dinos.Mega))},
            {Enums.Dinos.Tanky, new UpgradeInfo(GetDinoSaveLocation(Enums.Dinos.Tanky))},
            {Enums.Dinos.Warrior, new UpgradeInfo(GetDinoSaveLocation(Enums.Dinos.Warrior))},
            {Enums.Dinos.Gator, new UpgradeInfo(GetDinoSaveLocation(Enums.Dinos.Gator))}
        };

        dinoTypesAndAbilities = new Dictionary<Enums.Dinos, Enums.SpecialAbilities>()
        {
            {Enums.Dinos.Mega, Enums.SpecialAbilities.None},
            {Enums.Dinos.Tanky, Enums.SpecialAbilities.IceProjectile},
            {Enums.Dinos.Warrior, Enums.SpecialAbilities.FireProjectile},
            {Enums.Dinos.Gator, Enums.SpecialAbilities.None},
        };

        specialAbilityIcons = new Dictionary<Enums.SpecialAbilities, StreamTexture>()
        {
            {Enums.SpecialAbilities.IceProjectile, GD.Load<StreamTexture>("res://assets/dinos/misc/ice.png")},
            {Enums.SpecialAbilities.FireProjectile, GD.Load<StreamTexture>("res://assets/dinos/misc/fire.png")},
        };

        specialAbilityVidPreviews = new Dictionary<Enums.SpecialAbilities, VideoStream>()
        {
            {Enums.SpecialAbilities.IceProjectile, GD.Load<VideoStream>("res://assets/abilities/previews/ice-preview.ogv")},
            {Enums.SpecialAbilities.FireProjectile, new VideoStreamWebm()},
        };

    }

    public UpgradeInfo GetDinoInfo(Enums.Dinos dino)
    {
        return upgradesInfo[dino];
    }

    public Enums.Dinos GetDinoTypeFromAbility(Enums.SpecialAbilities ability)
    {
        // lookup dictionary to get key by value
        // ty :) https://stackoverflow.com/questions/2444033/get-dictionary-key-by-value#2444064
        return dinoTypesAndAbilities.FirstOrDefault(x => x.Value == ability).Key;
    }

    public int GetDinoDeployCost(Enums.Dinos type)
    {
        UpgradeInfo info = GetDinoInfo(type);
        return info.deployCost;
    }
    public bool CanAffordDino(Enums.Dinos type)
    {
        return CombatInfo.Instance.creds >= GetDinoDeployCost(type);
    }

    public float GetDinoTimerDelay(Enums.Dinos dinoType)
    {
        return (float)GetDinoProperty(dinoType, "spawnDelay");
    }

    // Instance dino, get variable we want, then remove it
    public object GetDinoProperty(Enums.Dinos dinoType, string property)
    {
        PackedScene DinoScene = dinoList[dinoType];
        BaseDino DinoInstance = (BaseDino)DinoScene.Instance();

        object DinoProperty = DinoInstance.Get(property);

        DinoInstance.QueueFree();
        return DinoProperty;
    }

    // returns location of Dino stats save
    public string GetDinoSaveLocation(Enums.Dinos dinoType)
    {
        string dinoNameNoSpaces = EnumUtils.GetDinoName(dinoType).Replace(" ", string.Empty);
        string userSaveLocation = $"user://{dinoNameNoSpaces}StatsSave.tres";

        // make a savefile in the user:// location if none exists yet
        if (!ResourceLoader.Exists(userSaveLocation))
        {
            string projectStatsLocation = $"res://src/combat/dinos/stats/{dinoNameNoSpaces}.tres";
            ResourceSaver.Save(userSaveLocation, ResourceLoader.Load<DinoInfoResource>(projectStatsLocation, null, true));
        }

        return userSaveLocation;
    }

}