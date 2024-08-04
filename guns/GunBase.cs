using Godot;

namespace Tdp5.guns;

public abstract partial class GunBase : Node2D
{
	public abstract PackedScene GunScene { get; }

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.ButtonIndex == MouseButton.Left)
		{
			if (mouseButtonEvent.IsPressed())
			{
				Shoot();
			}
		}

		base._Input(@event);
	}
	public abstract void Shoot();
}
