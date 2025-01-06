using Godot;
using Tdp5.guns;

public partial class Ak47Gun : GunBase
{
	public override PackedScene GunScene { get; protected set; } =
		ResourceLoader.Load<PackedScene>("res://guns/Ak47Gun.tscn");

	protected override PackedScene BulletScene { get; set; } =
		ResourceLoader.Load<PackedScene>("res://guns/bullet.tscn");
}
