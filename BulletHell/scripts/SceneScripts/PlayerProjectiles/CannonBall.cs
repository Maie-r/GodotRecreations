using Godot;
using System;

public partial class CannonBall : PlayerProjectile
{
	Node2D ExplosionNode;
	PackedScene Explosion;
	[Export]
	public float ExplodeAfter = 1;
	[Export]
	public float ExplosionRadius = 200;
	public override void _Ready()
	{
		Explosion = ResourceLoader.Load<PackedScene>("res://BulletHell/scenes/PlayerProjectiles/Explosion.tscn");
		//GD.Print(ExplodeAfter);
		Speed = BaseSpeed;
		CheckCrit();
		Timer tt = new Timer();
		tt.OneShot = true;
		tt.WaitTime = ExplodeAfter;
		tt.Autostart = true;
		tt.Timeout += Explode;
		AddChild(tt);
	}
	
	private void _on_area_entered(Area2D area)
	{
		switch (area.CollisionLayer)
		{
			case 4:
				if (area is RoamingProjectile r)
				{
					r.AddDonkStatus();
					//if (IsCrit) r.AddCritStatus();
				}
				break;
			case 9:
				QueueFree();
				break;
		}
	}

	public override void OnCollision()
	{
		if (Hitsound != "") TempAudioGlobal.PlayRandomPitch(Hitsound, 0.1f, this);
	}

	public void Explode()
	{
		Explosion e = Explosion.Instantiate<Explosion>();
		Angle = new Vector2(0, 0);
		e.Damage = this.Damage * 3f;
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
