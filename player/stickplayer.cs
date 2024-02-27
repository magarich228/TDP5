using Godot;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Godot.Collections;
using Tdp5.player;

public partial class stickplayer : CharacterBody2D
{
	// Godot public properties.
	// ReSharper disable MemberCanBePrivate.Global
	public const float JumpVelocity = -500.0f;
	public const float Speed = 300.0f;

	public PlayerState State { get; set; }
	public float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	// ReSharper restore MemberCanBePrivate.Global
	
	private AnimationPlayer _animationPlayer;
	private Dictionary<PlayerState, string> _stateAnimations;
	private Stopwatch _watch;

	public override void _Ready()
	{
		_watch = new Stopwatch();
		_animationPlayer = GetNode("AnimationPlayer") as AnimationPlayer ??
						   throw new ApplicationException($"Не получен узел {nameof(AnimationPlayer)}");

		_animationPlayer.Play("stand");

		_stateAnimations = new Dictionary<PlayerState, string>();
		
		foreach (var state in Enum.GetValues<PlayerState>())
		{
			var animationName = GetAnimationName(state);

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
			State = State != PlayerState.Lie && State != PlayerState.Creep? PlayerState.Stand : PlayerState.Lie;
		}
		
		if (!IsOnFloor())
			velocity.Y += Gravity * (float)delta;

		if (Input.IsActionJustPressed("ui_up"))
		{
			if (State == PlayerState.Lie || State == PlayerState.Creep)
			{
				State = PlayerState.Stand;
			}
			else if (IsOnFloor())
			{
				velocity.Y = JumpVelocity;
			}
		}

		var speed = State switch
		{
			PlayerState.Run or PlayerState.Stand or PlayerState.Jump => Speed,
			PlayerState.SitWalk or PlayerState.Sit => Speed / 1.5f,
			PlayerState.Creep or PlayerState.Lie => Speed / 2f,
			_ => 0
		};
		
		if (Input.IsActionPressed("ui_right"))
		{
			velocity.X = speed;
		}
		else if (Input.IsActionPressed("ui_left"))
		{
			velocity.X = -speed;
		} 
		else
		{
			velocity.X = 0;	
		}
		
		if (velocity.X != 0 && velocity.Y >= 0)
		{
			State = State == PlayerState.Lie || State == PlayerState.Creep ? PlayerState.Creep : PlayerState.Run;
		}
		
		if (State != PlayerState.Lie && State != PlayerState.Creep && !IsOnFloor() && !Input.IsActionPressed("ui_down"))
		{
			State = PlayerState.Jump;
		}
		
		if (Input.IsActionJustPressed("ui_down"))
		{
			if (_watch.IsRunning)
			{
				if (_watch.ElapsedMilliseconds > 500)
				{
					_watch.Reset();
					_watch.Stop();
				}
				else
				{
					State = PlayerState.Lie;
				}
			}
			
			_watch.Start();
		}

		if (State != PlayerState.Lie && Input.IsActionPressed("ui_down"))
		{
			State = PlayerState.Sit;
		}

		if (State != PlayerState.Lie && velocity.X != 0 && Input.IsActionPressed("ui_down") &&
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
			_animationPlayer.Play(GetAnimationName(State), fromEnd: true);
		
		if (State == PlayerState.Lie && oldState == PlayerState.Creep)
			_animationPlayer.Play(GetAnimationName(State), fromEnd: true);
		
		if (State != oldState)
		{
			Animate();
			Console.WriteLine($"State: {State}");
		}
	}

	private void Animate()
	{
		if (_animationPlayer.CurrentAnimation != _stateAnimations[State])
		{
			_animationPlayer.Play(_stateAnimations[State]);
		}
	}

	private string GetAnimationName(PlayerState state)
	{
		var stateName = Enum.GetName(state) ??
						throw new ApplicationException("Не получено имя анимации состояния.");
		
		return _animationPlayer.GetAnimation(Regex.Replace(
				stateName,
				"[A-Z]",
				"_$0")
			.Substring(1)
			.ToLower()).ResourceName;
	}
}
