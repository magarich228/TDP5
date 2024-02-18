using Godot;
using System;
using System.Text.RegularExpressions;
using Godot.Collections;
using Tdp5.player;

public partial class stickplayer : CharacterBody2D
{
	public PlayerState State { get; set; }	
	public const float Speed = 300.0f;
	public const float JumpVelocity = -500.0f;

	public float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	
	private AnimationPlayer _animationPlayer;
	private Dictionary<PlayerState, string> _stateAnimations;

	public override void _Ready()
	{
		_animationPlayer = GetNode("AnimationPlayer") as AnimationPlayer ??
						   throw new ApplicationException($"Не получен узел {nameof(AnimationPlayer)}");

		_animationPlayer.Play("stand");

		_stateAnimations = new Dictionary<PlayerState, string>();
		
		Console.WriteLine(Enum.GetValues<PlayerState>().Length);
		
		foreach (var state in Enum.GetValues<PlayerState>())
		{
			var stateName = Enum.GetName(state) ??
								throw new ApplicationException("Не получено имя анимации состояния.");
			
			var animationName = _animationPlayer.GetAnimation(Regex.Replace(
						stateName,
						"[A-Z]",
						"_$0")
					.Substring(1)
					.ToLower()).ResourceName;

			if (!string.IsNullOrWhiteSpace(animationName))
			{
				_stateAnimations.Add(state, animationName);
			}
		}
		
		base._Ready();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		// TODO: Замена ui событий на кастомные.
		// TODO: отрефачить код.
		// TODO: передвижение в присяде.
		// TODO: фикс странной механики замедленного движения при зажатых ui_up, ui_down.
		Vector2 velocity = Velocity;

		if (IsOnFloor() && velocity.X.Equals(0) && !Input.IsActionPressed("ui_down"))
		{
			_animationPlayer.Stop();
		}
		
		if (!IsOnFloor())
			velocity.Y += Gravity * (float)delta;

		if (Input.IsActionJustPressed("ui_up") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
			_animationPlayer.Stop();
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
			_animationPlayer.Play("run");
		}
		else if (velocity.X < 0)
		{
			_animationPlayer.Play("run");
		}

		if (Input.IsActionJustPressed("ui_down"))
		{
			_animationPlayer.Play("sit");
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

	private void Animate()
	{
		
	}
}
