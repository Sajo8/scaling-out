using Godot;
using b = Enums.GameButtonModes;

[Tool]
public class GameButton : Button
{
    [Export] public b buttonMode;

    public override void _Ready()
    {
        if (Engine.EditorHint)
        {
            return;
        }

        if (buttonMode == b.None)
        {
            GD.PushError("You must set buttonMode for GameButton! Located at: " + GetTree().CurrentScene.Filename);
            GD.PrintStack();
            GetTree().Quit(1);
        }
    }

    public override void _Process(float delta)
    {
        if (Engine.EditorHint)
        {
            if (Text == "example text")
            {
                SetButtonText();
            }
        }
    }

    void SetButtonText()
    {
        switch (buttonMode)
        {
            case b.Quit:
                Text = "Quit";
                break;
            case b.Play:
                Text = "Play";
                this.GrabFocus();
                break;
            case b.RetryCombat:
                Text = "Retry";
                this.GrabFocus();
                break;
            case b.RetryStealth:
                Text = "Retry";
                this.GrabFocus();
                break;
            case b.ReturnHomeScreen:
                Text = "Return Home";
                this.GrabFocus();
                break;
            case b.ReturnUpgradeSelect:
                Text = "Return";
                this.GrabFocus();
                break;
            case b.StealthIce:
                Text = "Ice Stealth";
                break;
            case b.StealthFire:
                Text = "Fire Stealth";
                break;
            case b.PlusDino:
                Text = "+";
                break;
            case b.MinusDino:
                Text = "-";
                break;
            case b.BuyDinos:
                Text = "Purchase";
                break;
            case b.ContinueConquest:
                Text = "Continue Conquest";
                break;
        }
    }

    void OnButtonPressed()
    {
        SceneChanger scnChng = SceneChanger.Instance;
        StealthInfo s = StealthInfo.Instance;

        switch (buttonMode)
        {
            case b.Quit:
                GetTree().Quit();
                break;
            case b.Play:
                scnChng.GoToScene("res://src/GUI/screens/HomeScreen.tscn");
                break;
            case b.RetryCombat:
                CombatInfo.Instance.Reset();
                scnChng.GoToScene("res://src/combat/CombatScreen.tscn");
                break;
            case b.RetryStealth:
                var stealthMap = StealthInfo.Instance.geneStealthMaps[StealthInfo.geneBeingPursued];
                scnChng.GoToScene(stealthMap.ResourcePath);
                break;
            case b.ReturnHomeScreen:
                scnChng.GoToScene("res://src/GUI/screens/HomeScreen.tscn");
                break;
            case b.ReturnUpgradeSelect:
                scnChng.GoToScene("res://src/GUI/screens/DinoUpgradeSelectScreen.tscn");
                break;
            case b.StealthIce:
                StealthInfo.geneBeingPursued = Enums.Genes.Cryo;
                scnChng.GoToScene(s.geneStealthMaps[StealthInfo.geneBeingPursued].ResourcePath);
                break;
            case b.StealthFire:
                StealthInfo.geneBeingPursued = Enums.Genes.Fire;
                scnChng.GoToScene(s.geneStealthMaps[StealthInfo.geneBeingPursued].ResourcePath);
                break;
            // nothing for plus, minus, or buy dinos: those are handled in their own scenes
            case b.ContinueConquest:
                scnChng.NewRoundAnimation();
                break;
        }
    }

}