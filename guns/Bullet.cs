using Godot;

public partial class Bullet : RigidBody2D
{
	// Godot public properties.
	// ReSharper disable MemberCanBePrivate.Global
	public const float Speed = 1800.0f;
	// ReSharper restore MemberCanBePrivate.Global
	
	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		LinearVelocity += new Vector2((float)Speed * (float)delta, 0)
			.Rotated(GlobalRotation + Mathf.DegToRad(-90f));
		
		MoveAndCollide(LinearVelocity);
		
		if (Position.X > GetViewportRect().Size.X)
		{
			QueueFree();
		}
		
		if (Position.X < 0)
		{
			QueueFree();
		}
		
		base._PhysicsProcess(delta);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}
}
