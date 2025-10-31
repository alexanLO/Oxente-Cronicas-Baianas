using Godot;
using Godot.Collections;
using System;
using Array = Godot.Collections.Array;
using Vector2 = Godot.Vector2;

public partial class EnemyTemplate : CharacterBody2D
{
    [Export] private Sprite2D texture;
    [Export] private RayCast2D floorRay;
    [Export] private AnimationPlayer animation;

    private Signal kill;

    private bool canDie = false;
    private bool canHit = false;
    private bool canAttack = false;

    private Player playerRef = null;
    private Dictionary dropList;
    private int dropBonus = 1;
    private Vector2 velocity;
    private String attackAnimationSufflix = "_left";

    [Export] private int speed { get; set; } = 0;
    [Export] private int gravitySpeed { get; set; } = 0;
    [Export] private int proximityThreshould { get; set; } = 0;
    [Export] private int rayCastDefaultPosition { get; set; } = 0;
    [Export] private int enemyExp { get; set; } = 0;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        Gravity(delta);
        MoveBehavior();
        VerifyPosition();
        //texture.DrawAnimationSlice(Velocity);
        MoveAndSlide();
    }

    //* =================== Moviments ===================
    private void Gravity(double delta)
    {
        velocity.Y += (float)(delta * gravitySpeed);
    }

    private void MoveBehavior()
    {
        if (playerRef != null)
        {
            Vector2 distance = playerRef.GlobalPosition - GlobalPosition;
            Vector2 direction = distance.Normalized();

            if (MathF.Abs(distance.X) <= proximityThreshould)
            {
                velocity.X = 0;
                canAttack = true;
            }
            else if (FloorCollision() && !canAttack)
            {
                velocity.X = direction.X * speed;
            }
            else
            {
                velocity.X = 0;
            }

            velocity.X = 0;
        }
    }

    private void VerifyPosition()
    {
        if (playerRef != null)
        {
            float direction = Signal(playerRef.GlobalPosition.X - GlobalPosition.X);

            if (direction > 0)
            {
                texture.FlipH = true;
                attackAnimationSufflix = "_right";
                floorRay.Position.X = MathF.Abs(rayCastDefaultPosition);
            }
            else if (direction < 0)
            {
                texture.FlipH = false;
                attackAnimationSufflix = "_left";
                floorRay.Position.X = rayCastDefaultPosition;
            }
        }
    }

    private bool FloorCollision()
    {
        if (floorRay.IsColliding())
        {
            return true;
        }

        return false;
    }

    private void KillEnemy()
    {
        EmitSignal("kill");
        animation.Play("kill");
        SpawnItemProbability();
    }

    //* =================== Drop Item ===================
    private void SpawnItemProbability()
    {
        uint randomNumber = GD.Randi() % 21;
        if (randomNumber <= 6)
        {
            dropBonus = 1;
        }
        else if (randomNumber >= 7 && randomNumber <= 13)
        {
            dropBonus = 2;
        }
        else
        {
            dropBonus = 3;
        }

        foreach (String key in dropList.Keys)
        {
            uint rng = GD.Randi() % 100 + 1;
            float dropChance = (float)dropList[key];
            if (rng <= dropChance * dropBonus)
            {
                CompressedTexture2D itemTexture = GD.Load(dropList[key][0]);
                Array itemInfo = [
                    dropList[key][0],
                    dropList[key][2],
                    dropList[key][3],
                    dropList[key][4],
                    1
                ];

                SpawnPhysicItem(key, itemTexture, itemInfo);
            }
        }
    }

    private void SpawnPhysicItem(String key, CompressedTexture2D itemTexture, Array itemInfo)
    {
        PackedScene physicItemScene = GD.Load("");
        PhysicItem item = physicItemScene.Instantiate();
        GetTree().Root.CallDeferred("addChild", item);
        item.GlobalPosition = GlobalPosition;
        item.updateItemInfo(key, itemTexture, itemInfo);
    }
}
