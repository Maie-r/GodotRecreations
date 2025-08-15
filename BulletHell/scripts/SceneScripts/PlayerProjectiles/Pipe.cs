using Godot;
using System;

public partial class Pipe : PlayerProjectile
{
	/*////////////////////////////////////////////////////////////////////////////////
	
		PIPE CLASS

		Purpose: Represent the grenades from most launchers in tf2

		What was Learned:
			(DITTO STOCK:) You CAN make a projectile spawn a projectile, but it can be very scary if they are
				Parent and children, disposing of one will dispose the other (and their sounds)
				So its best to have a reference to your projectiles container and have your projectile
				spawn another in there instead of as it's child

	*/////////////////////////////////////////////////////////////////////////////////
	Node2D ExplosionNode;
	PackedScene Explosion;
	[Export]
	public float ExplosionRadius = 2000;
	public override void _Ready()
	{
		Explosion = ResourceLoader.Load<PackedScene>("res://BulletHell/scenes/PlayerProjectiles/Explosion.tscn");
		Speed = BaseSpeed;
		CheckCrit();
	}
	
	/*private void _on_area_entered(Area2D area)
	{
		switch (area.CollisionLayer)
		{
			case 4:
				//GD.Print("Can donk");
				//Explode();
				break;
			case 9:
				QueueFree();
				break;
		}
	}*/

	public override void OnCollision()
	{
		GetNode<Sprite2D>("Sprite2D").Visible = false;
		Explode();
		if (Hitsound != "") TempAudioGlobal.PlayRandomPitch(Hitsound, 0.1f, this);
	}

	public void Explode()
	{
		Explosion e = Explosion.Instantiate<Explosion>();
		Angle = new Vector2(0, 0);
		e.Damage = this.Damage * e.Multiplier;
		e.Radius = ExplosionRadius;
		e.LingerTime = 0.2f;
		//e.Position = this.GlobalPosition;
		CallDeferred(Node.MethodName.AddChild, e);
		var timeout = TempTimer.Get(0.2f);
		timeout.Timeout += QueueFree;
		AddChild(timeout);
		TempAudioGlobal.PlayRandomPitch("pipe_bomb1.wav", 0.2f, this);
	}
}
