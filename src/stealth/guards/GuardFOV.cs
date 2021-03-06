using System.Collections.Generic;
using Godot;
using Godot.Collections;

public class GuardFOV : Node2D
{
    [Export] float fieldOfView = 50;
    [Export] float radiusWarn = 350;
    [Export] float radiusDanger = 350;

    [Export] bool showCircle = false;
    [Export] bool showFov = true;
    [Export] bool showTargetLine = false;

    [Export] Color circleColor = new Color("9f185c0b");

    [Export] Color fovColor = new Color("b2ff0000");
    [Export] Color fovWarnColor = new Color("ffff00");
    [Export] Color fovDangerColor = new Color("ff0000");

    [Export] float viewDetail = 60;

    [Export] Array<string> enemyGroups = new() { "scientist" };

    public Array<Godot.Object> inWarnArea = new();
    public Array<Godot.Object> inDangerArea = new();

    record ArcPoint(Vector2 pos, int level);

    // Buffer to target points
    List<ArcPoint> pointsArc = new();
    bool isUpdate = false;

    public override void _Process(float delta)
    {
        isUpdate = true;
        CheckView();
        Update();
    }

    public override void _Draw()
    {
        if (showCircle)
        {
            DrawCircle(Position, radiusWarn, circleColor);
        }

        if (showFov && isUpdate)
        {
            DrawCircleArc();
        }
    }

    void DrawCircleArc()
    {
        foreach (ArcPoint aux in pointsArc)
        {
            if (aux == null) continue;

            if (aux.level == 1 && showTargetLine)
            {
                DrawLine(Position, aux.pos, fovWarnColor);
            }
            else if (aux.level == 2 && showTargetLine)
            {
                DrawLine(Position, aux.pos, fovDangerColor, 3);
            }
            else
            {
                DrawLine(Position, aux.pos, fovColor);
            }
        }
    }

    Vector2 DegToVector(float deg)
    {
        return new Vector2(
            Mathf.Cos(Mathf.Deg2Rad(deg)),
            Mathf.Sin(Mathf.Deg2Rad(deg))
        );
    }

    void CheckView()
    {
        var directionDeg = Mathf.Rad2Deg(Transform.Rotation);
        var startAngle = directionDeg - (fieldOfView / 2);
        var endAngle = startAngle + fieldOfView;

        pointsArc = new();
        inDangerArea = new();
        inWarnArea = new();

        var spaceState = GetWorld2d().DirectSpaceState;

        for (int i = 0; i < viewDetail + 1; i++)
        {
            var currentAngle = startAngle + (i * (fieldOfView / viewDetail)) + 90;
            //! NOTE: change `radiusDanger` to `radiusWarn` if you want to use that value, AND change `radiusWarn` to be same as `radiusDanger` in existing scenes
            Vector2 point = Position + DegToVector(currentAngle) * radiusDanger;

            // use global coordinates, not local to node
            Dictionary result = spaceState.IntersectRay(
                GetGlobalTransform().origin, ToGlobal(point), new Array() { GetParent() }
            );

            if (result.Count > 0)
            {
                Vector2 resultPosition = (Vector2)result["position"];
                Node2D resultCollider = (Node2D)result["collider"];
                var localPos = ToLocal(resultPosition);
                var dist = Position.DistanceTo(localPos);
                int level = 0;
                bool isEnemy = false;

                if (inDangerArea.Contains(resultCollider) || inWarnArea.Contains(resultCollider))
                {
                    pointsArc.Add(new ArcPoint(localPos, level));
                }
                else
                {
                    foreach (var g in enemyGroups)
                    {
                        if (resultCollider.GetGroups().Contains(g))
                        {
                            isEnemy = true;
                        }
                    }

                    if (isEnemy)
                    {
                        level = 1;
                        if (dist < radiusDanger)
                        {
                            level = 2;
                            inDangerArea.Add(resultCollider);

                            Events.publishLevelFailed();
                        }
                        else
                        {
                            inWarnArea.Add(resultCollider);
                        }
                        // check if directly to target, we can "shoot"
                        var tgtPos = resultCollider.GetGlobalTransform().origin;
                        var thisPos = GetGlobalTransform().origin;
                        var tgtDir = (tgtPos - thisPos).Normalized();
                        var viewAngle = Mathf.Rad2Deg
                        (
                            DegToVector(
                                Mathf.Rad2Deg(
                                    GetGlobalTransform().Rotation
                                ) + 90
                            ).AngleTo(tgtDir)
                        );

                        if (viewAngle > startAngle && viewAngle < endAngle)
                        {
                            Dictionary result2 = spaceState.IntersectRay(thisPos, tgtPos, new Array() { GetParent() });
                            Node2D result2Collider = (Node2D)result2["collider"];

                            if (result2 != null && result2Collider == resultCollider)
                            {
                                // we can then use this as line
                                pointsArc.Add(new ArcPoint(localPos, 0));
                                if (showTargetLine)
                                {
                                    pointsArc.Add(new ArcPoint(ToLocal(localPos), level));
                                }
                            }
                            else
                            {
                                pointsArc.Add(new ArcPoint(localPos, level));
                            }
                        }
                        else
                        {
                            pointsArc.Add(new ArcPoint(localPos, level));
                        }
                    }
                    else
                    {
                        pointsArc.Add(new ArcPoint(localPos, level));
                    }
                }
            }
            else
            {
                pointsArc.Add(new ArcPoint(point, 0));
            }
        }
    }
}