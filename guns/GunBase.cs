using Godot;

namespace Tdp5.guns;

public abstract partial class GunBase : Node2D
{
	public abstract PackedScene GunScene { get; protected set; }
	protected abstract PackedScene BulletScene { get; set; }

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton {ButtonIndex: MouseButton.Left} mouseButtonEvent)
		{
			if (mouseButtonEvent.IsPressed())
			{
				Shoot();
			}
		}

		base._Input(@event);
	}

	private void Shoot()
	{
		var bullet = BulletScene.Instantiate<Bullet>();
		
		bullet.ZIndex = 1;
		bullet.Position = new Vector2(
			GlobalPosition.X,
			GlobalPosition.Y);
		bullet.GlobalRotation = GlobalRotation;
		bullet.Rotate(Mathf.DegToRad(90f));
		
		GetTree().Root.AddChild(bullet);
	}
}
