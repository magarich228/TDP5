using System;
using Godot;
using Tdp5.guns;

public partial class Ak47Gun : GunBase
{
	public override PackedScene GunScene => 
		ResourceLoader.Load<PackedScene>("res://guns/Ak47Gun.tscn");
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{ 
	}


	public override void Shoot()
	{
		// TODO: фикс поворота пули.
		var bullet = (Bullet)ResourceLoader.Load<PackedScene>("res://guns/bullet.tscn").Instantiate();
		
		bullet.ZIndex = 1;
		bullet.Position = new Vector2(
			GlobalPosition.X,
			GlobalPosition.Y);
		bullet.GlobalRotation = GlobalRotation;
		
		GetTree().Root.AddChild(bullet);
	}
}
