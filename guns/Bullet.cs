using Godot;
using System;

public partial class Bullet : Node2D
{
	// Godot public properties.
	// ReSharper disable MemberCanBePrivate.Global
	public const float Speed = 1800.0f;
	// ReSharper restore MemberCanBePrivate.Global
	
	public override void _Ready()
	{
	}
	
	public override void _Process(double delta)
	{
		Position += new Vector2((float)Speed * (float)delta, 0)
			.Rotated(GlobalRotation);
		
		// TODO: Проверить, что выходит за границы экрана.
		if (Position.X > GetViewportRect().Size.X)
		{
			QueueFree();
		}
		
		if (Position.X < 0)
		{
			QueueFree();
		}
	}
}
