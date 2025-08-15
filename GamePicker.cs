using Godot;
using System;

public partial class GamePicker : CanvasLayer
{
	[Export]
	public int AutoStart = -1;

	public override void _Ready()
	{
		switch (AutoStart)
		{
			case -1:
				break;
			case 0:
				_on_bullet_hell_pressed();
				break;
		}
	}
	private void _on_bullet_hell_pressed()
	{
		GetTree().ChangeSceneToFile("res://BulletHell/BulletHell.tscn");
	}
}
