using Godot;
using System;

public partial class Ak47Gun : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void Shoot()
	{
		// var bullet = (Bullet)ResourceLoader.Load<PackedScene>("res://player/bullet.tscn").Instantiate();
		// bullet.Position = Position;
		// GetTree().Root.AddChild(bullet);
	}
}
