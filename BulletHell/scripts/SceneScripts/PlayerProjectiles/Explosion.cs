using Godot;
using System;
using System.Collections;
using System.Drawing;

public partial class Explosion : PlayerProjectile
{
	/*////////////////////////////////////////////////////////////////////////////////
	
		EXPLOSION CLASS

		Purpose: Kaboom!

		What was Learned:
			Particles are actually kinda difficult to mess with, changing their Scale only 
				changes the spawn position radius, I had to use this hacky method of changing
				the LifeSpan of the particles (Which ended up making the loose cannon look way
				more powerful than it is)

	*/////////////////////////////////////////////////////////////////////////////////

	[Export]
	public float LingerTime = 0.2f;
	[Export]
	public float Radius = 150;

	public override void _Ready()
	{
		SetRadius(this.Radius);
		var vfx = ResourceLoader.Load<PackedScene>("res://BulletHell/scenes/ExplosionEffect.tscn").Instantiate<GpuParticles2D>();
		vfx.Emitting = true;
		vfx.Finished += vfx.QueueFree;
		vfx.Position = this.GlobalPosition;
		vfx.Lifetime *= (Radius*0.6/150)+0.2; //(Mathf.Pow(Radius,2)/2)/(75 * Radius);
		/*var mat = (ParticleProcessMaterial)vfx.ProcessMaterial;
		var curve = (CurveTexture)mat.RadialAccelCurve;
		Point p1 = 
		curve.Curve.Point
		//curve.Curve.SetPointOffset(0, 0.5f);
		//vfx.Scale = new Vector2(Radius/600, Radius/600);*/
		GetTree().Root.AddChild(vfx);
		Timer tt = new Timer()
		{
			Autostart = true,
			OneShot = true,
			WaitTime = LingerTime
		};
		tt.Timeout += QueueFree;
	}

	public void SetRadius(float Radius)
	{
		var hb = GetNode<CollisionShape2D>("Hitbox");
		var c = new CircleShape2D();
		c.Radius = Radius;
		hb.Shape = c;
		//(GetNode<CollisionShape2D>("Hitbox").Shape as CircleShape2D).Radius = Radius;
	}

	public override void OnCollision()
	{
		
	}

	private void _on_body_entered(Node2D body)
	{
		GD.Print("AHH");
		GetTree().CallGroup("Player", "TakeDamage", 1);
	}
}
