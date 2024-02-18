using Godot;
using System;

public partial class stickplayer : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -500.0f;

	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
		// TODO: отрефачить код.
		// TODO: баг с анимацией руки при приседании.
		// TODO: передвижение в присяде.
		// TODO: фикс странной механики замедленного движения при зажатых ui_up, ui_down.
		AnimationPlayer animationPlayer = GetNode("AnimationPlayer") as AnimationPlayer ??
										  throw new ApplicationException($"Не получен узел {nameof(AnimationPlayer)}");
		Vector2 velocity = Velocity;

		if (IsOnFloor() && velocity.X.Equals(0) && !Input.IsActionPressed("ui_down"))
		{
			animationPlayer.Stop();
		}
		
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;

		if (Input.IsActionJustPressed("ui_up") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
			animationPlayer.Stop();
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}
		
		if (velocity.X > 0)
		{
			animationPlayer.Play("run");
		}
		else if (velocity.X < 0)
		{
			animationPlayer.Play("run");
		}

		if (Input.IsActionJustPressed("ui_down"))
		{
			animationPlayer.Play("sit_down");
		}

		var localMousePosition = GetLocalMousePosition();
		var globalMousePosition = GetGlobalMousePosition();

		if (localMousePosition.X < 0)
		{
			var scale = Transform.Scale;

			Scale = new Vector2()
			{
				Y = scale.Y,
				X = -scale.X
			};
		}

		var rtLau = GetNode<RemoteTransform2D>(new NodePath("Skeleton2D/torse/RTlau"));
		var rtRau = GetNode<RemoteTransform2D>(new NodePath("Skeleton2D/torse/RTrau"));
		rtLau.LookAt(globalMousePosition);
		rtRau.LookAt(globalMousePosition);
		
		Velocity = velocity;
		MoveAndSlide();
	}

	private int count;
}

public enum PlayerPosition : int
{
	Stand = 0,
	Sit = 1,
	Lie = 2
}
