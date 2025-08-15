using Godot;
using System;

public partial class PowerUp : Area2D
{
	/*////////////////////////////////////////////////////////////////////////////////
	
		POWERUP CLASS

		Purpose: Store a pickup, and add that pickup's effects to the player when collected
				(The actual effect application logic is within player)

		What was Learned:
			Kinda cool wavy patten with Sine, That always swings to the center

	*/////////////////////////////////////////////////////////////////////////////////

	public IPickup Pickup;
	
	[Export]
	public float StartPosX = -1;
	[Export]
	public float Speed = 300;

	public float sineCoef = 3;

	float sine;

	public Vector2 angle = new Vector2(0, 1);

	[Signal]
	public delegate void CollectedEventHandler(PowerUp p);

	public override void _Ready()
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		if (StartPosX < 0)
		{
			int width = (int)GetViewport().GetVisibleRect().Size[0]/2;
			StartPosX = rng.RandfRange(-width, width);
		}
		this.Position = new Vector2(StartPosX, -400);
		if (StartPosX < 0)
			sineCoef *= -1;
		sine = sineCoef;
	}

	public override void _Process(double delta)
	{
		//GD.Print($"sine: {sine} angle: {Mathf.Sin(sine)}");
		angle.X = Mathf.Sin(sine);
		sine += sineCoef/2 * (float)delta;
		this.Position += angle * Speed * (float)delta;
	}
	
	private void _on_body_entered(Node2D body)
	{
		/*if (this.Pickup is Buff b)
			GD.Print($"{b.Category} UP");
		else if (this.Pickup is Weapon w)
			GD.Print($"{w.Name} Got!");*/
		EmitSignal(SignalName.Collected, this);
		QueueFree();
	}
	
	private void _on_area_entered(Area2D area)
	{
		if (area.CollisionLayer == 9) QueueFree();
	}
}

public interface IPickup
{
	
}
