using Godot;
using System.Threading.Tasks;

public class Test : Node
{
    public override void _Ready()
    {
        // UpgradeInfo u = new UpgradeInfo("res://src/actors/dinos/stats/MegaDino.tres");

        // var statsdict = u.stats;
        // var hp_stats = statsdict[Enums.Stats.Hp];

        // GD.Print(hp_stats);
        // GD.Print(hp_stats.GetType());


        // GD.Print("before statname ands tata");

        // GD.Print(hp_stats.statName);
        // GD.Print(hp_stats.stats);

        // GD.Print("after statname ands tats");

        // var hp = u.GetStat(Enums.Stats.Hp);
        // GD.Print(hp);

        PathFollow2D pathFollow = new PathFollow2D();
        AddChild(pathFollow);
        pathFollow.AddChild(new Area2D());

    }


}