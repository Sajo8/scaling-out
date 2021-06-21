using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

public class DinoSelector : Node2D
{
    Enums.Dinos selectedDinoType;
    PackedScene selectorScene = GD.Load<PackedScene>("res://src/GUI/combat_selector/SelectorSprite.tscn");
    List<SelectorSprite> selectorList = new List<SelectorSprite>();

    HBoxContainer hBox;

    DinoInfo d = DinoInfo.Instance;

    bool allDinosExpended = false;

    public override void _Ready()
    {
        Events.dinoFullySpawned += OnDinoFullySpawned;
        Events.dinoDied += OnDinoDied;
        Events.allDinosExpended += OnAllDinosExpended;
        Events.dinosPurchased += OnDinosPurchased;
        Events.selectorSelected += OnSelectorSelected;

        hBox = (HBoxContainer)FindNode("HBoxContainer");

        SetupSelectors();
        // get a list of children
        selectorList = hBox.GetChildren().Cast<SelectorSprite>().ToList<SelectorSprite>();
        selectorList[0].ShowParticles();
    }

    public override void _ExitTree()
    {
        Events.dinoFullySpawned -= OnDinoFullySpawned;
        Events.dinoDied -= OnDinoDied;
        Events.allDinosExpended -= OnAllDinosExpended;
        Events.dinosPurchased -= OnDinosPurchased;
    }

    void SetupSelectors()
    {
        int iconId = 0;
        foreach (KeyValuePair<Enums.Dinos, StreamTexture> n in d.dinoIcons)
        {
            // skip if not unlocked the dino yet
            if (!PlayerStats.dinosUnlocked.Contains(n.Key))
            {
                continue;
            }

            SelectorSprite newSelector = (SelectorSprite)selectorScene.Instance();
            newSelector.spriteTexture = n.Value;
            newSelector.dinoType = n.Key;
            newSelector.text = (iconId + 1).ToString();

            // key 0 number + 1 (to make the lowest number key 1) + iconId 
            // iconId starts from 0, so that's why we add 1 manually
            newSelector.Shortcut = new ShortCut();
            newSelector.Shortcut.Shortcut = new InputEventKey();
            ((InputEventKey)newSelector.Shortcut.Shortcut).Scancode = ((int)Godot.KeyList.Key0) + 1 + (uint)iconId;

            hBox.AddChild(newSelector);

            iconId++;
        }

        int abilityId = 0;
        foreach (KeyValuePair<Enums.Dinos, StreamTexture> n in d.dinoAbilityIcons)
        {
            // skip if no icon
            if (n.Value == null)
            {
                continue;
            }

            // skip if not unlocked the speical for the dino yet or if not unlocked the dino itself
            if (!PlayerStats.dinosUnlocked.Contains(n.Key) || !DinoInfo.Instance.GetDinoInfo(n.Key).HasSpecial())
            {
                continue;
            }

            // TODO: FOR THE LOVE OF GOD PLEASE CHANGE THIS LOGIC
            // find the filename of the image, which is also the name of the ability itself
            var fileName = n.Value.ResourcePath;
            var abilityStart = fileName.FindLast("/");
            var abilityEnd = fileName.Find(".png");
            var abilityString = fileName.Substr(abilityStart, abilityEnd);

            SelectorSprite newSelector = (SelectorSprite)selectorScene.Instance();
            newSelector.spriteTexture = n.Value;
            newSelector.dinoType = n.Key;
            newSelector.text = (abilityId + d.dinoIcons.Count + 1).ToString(); // position in list + number of dinos

            newSelector.abilityMode = abilityString;
            newSelector.customScale = new Vector2((float)0.07, (float)0.07);

            // key 0 number + 1 (to make the lowest number key 1) + iconId 
            // iconId starts from 0, so that's why we add 1 manually
            // iconId is *also* used in this for loop because it ensures the key id is correct
            newSelector.Shortcut = new ShortCut();
            newSelector.Shortcut.Shortcut = new InputEventKey();
            ((InputEventKey)newSelector.Shortcut.Shortcut).Scancode = ((int)Godot.KeyList.Key0) + 1 + (uint)iconId;

            hBox.AddChild(newSelector);

            abilityId++;
        }
    }

    // turn on particles for this selector and turns off all other particles
    void EnableExclusiveParticles(SelectorSprite selectorToKeepOn)
    {
        var selectors = hBox.GetChildren().Cast<SelectorSprite>().ToList<SelectorSprite>();
        foreach (SelectorSprite s in selectors)
        {
            s.HideParticles();
            if (s == selectorToKeepOn) s.ShowParticles();
        }
    }

    void OnSelectorSelected(SelectorSprite selector)
    {
        this.selectedDinoType = selector.dinoType;
        EnableExclusiveParticles(selector);
    }

    public override void _Input(InputEvent @event)
    {
        if (allDinosExpended)
        {
            return;
        }

        // TODO: change this. #73, https://app.gitkraken.com/glo/view/card/75f5162699514eddb32954a7629c6423
        if (@event.IsActionPressed("dino_5"))
        {
            if (!(d.GetDinoInfo(Enums.Dinos.Tanky).UnlockedSpecial()) || CombatInfo.Instance.shotIce)
            {
                return;
            }

            foreach (BaseDino d in GetTree().GetNodesInGroup("dinos"))
            {
                if (d.dinoType == Enums.Dinos.Tanky)
                {
                    TankyDino tanky = (TankyDino)d;
                    tanky.ShootProjectile();
                    selectorList[4].DisableSprite();
                    CombatInfo.Instance.shotIce = true;
                }
            }

        }
        else if (@event.IsActionPressed("dino_6"))
        {
            if (!(d.GetDinoInfo(Enums.Dinos.Warrior).UnlockedSpecial()) || CombatInfo.Instance.shotFire)
            {
                return;
            }

            foreach (BaseDino d in GetTree().GetNodesInGroup("dinos"))
            {
                if (d.dinoType == Enums.Dinos.Warrior)
                {
                    WarriorDino warrior = (WarriorDino)d;
                    warrior.ShootProjectile();
                    selectorList[5].DisableSprite();
                    CombatInfo.Instance.shotFire = true;
                }
            }

        }

        if (this.selectedDinoType != Enums.Dinos.None)
        {
            CombatInfo.Instance.selectedDinoType = this.selectedDinoType;
        }
    }

    void OnDinoFullySpawned(Enums.Dinos dinoType)
    {
        // TODO: do this better
        // TODO: fix this lol

        if (dinoType == Enums.Dinos.Tanky)
        {
            if (CombatInfo.Instance.IsAbilityDeployable(dinoType))
            {
                selectorList[4].EnableSprite();
            }
        }

        if (dinoType == Enums.Dinos.Warrior)
        {
            if (CombatInfo.Instance.IsAbilityDeployable(dinoType))
            {
                selectorList[5].EnableSprite();
            }
        }

    }

    void OnDinoDied(Enums.Dinos type)
    {
        var dinosLeft = GetTree().GetNodesInGroup("dinos");

        // TODO: do this better

        if (type == Enums.Dinos.Tanky)
        {
            // go through each dino
            // if any of them are tanky dinos, then shooting ice is still possible
            // if so, then exit out. 
            // but if there are no more tanky dinos, then disable sprite
            foreach (BaseDino d in dinosLeft)
            {
                if (d.dinoType == Enums.Dinos.Tanky)
                {
                    return;
                }
            }
            selectorList[4].DisableSprite();
        }

        if (type == Enums.Dinos.Warrior)
        {
            // same as above for warrior dinos
            foreach (BaseDino d in dinosLeft)
            {
                if (d.dinoType == Enums.Dinos.Warrior)
                {
                    return;
                }
            }
            selectorList[5].DisableSprite();
        }
    }

    void OnAllDinosExpended()
    {
        allDinosExpended = true;
    }

    async void OnDinosPurchased(int numDinos)
    {
        // Wait for 0.1 seconds to allow for the same signal to be registered and executed in Combat.cs
        // Only once that code executes will our un-fading code work properly
        // Kinda hacky but oh well
        await ToSignal(GetTree().CreateTimer((float)0.1), "timeout");

        // reset switch, re-enable all sprites once more dinos are bought
        if (CombatInfo.Instance.dinosRemaining > 0)
        {
            allDinosExpended = false;
            foreach (SelectorSprite ss in selectorList)
            {
                ss.EnableSprite();
            }
        }
    }

}