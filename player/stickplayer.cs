using Godot;
using System;
using System.Text.RegularExpressions;
using Godot.Collections;
using Tdp5.player;

public partial class stickplayer : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -500.0f;

	public PlayerState State { get; set; }
	public float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	
	private AnimationPlayer _animationPlayer;
	private Dictionary<PlayerState, string> _stateAnimations;

	public override void _Ready()
	{
		_animationPlayer = GetNode("AnimationPlayer") as AnimationPlayer ??
						   throw new ApplicationException($"Не получен узел {nameof(AnimationPlayer)}");

		_animationPlayer.Play("stand");

		_stateAnimations = new Dictionary<PlayerState, string>();
		
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
		var oldState = State;
		Vector2 velocity = Velocity;

		if (IsOnFloor() && velocity.X.Equals(0) && !Input.IsActionPressed("ui_down"))
		{
			Console.WriteLine("stand");
			State = PlayerState.Stand;
		} else { Console.WriteLine($"not stand ({State})"); }
		
		if (!IsOnFloor())
			velocity.Y += Gravity * (float)delta;

		if (Input.IsActionJustPressed("ui_up") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
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
		
		if (velocity.X != 0 && velocity.Y == 0)
		{
			State = PlayerState.Run;
		}
		
		if (velocity.Y != 0 && !Input.IsActionPressed("ui_down"))
		{
			State = PlayerState.Jump;
		}

		if (Input.IsActionPressed("ui_down"))
		{
			State = PlayerState.Sit;
		}

		if (velocity.X != 0 && Input.IsActionPressed("ui_down") &&
			(Input.IsActionPressed("ui_left") || Input.IsActionPressed("ui_right")))
		{
			State = PlayerState.SitWalk;
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
		
		if (State == PlayerState.Sit && oldState == PlayerState.SitWalk)
			_animationPlayer.Play("sit", fromEnd: true);
		
		if (State != oldState)
			Animate();
	}

	private void Animate()
	{
		if (_animationPlayer.CurrentAnimation != _stateAnimations[State])
		{
			_animationPlayer.Play(_stateAnimations[State]);
		}
	}
}
