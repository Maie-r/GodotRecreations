using Godot;
using System;

public partial class PlayerProjectile : Area2D
{
	/*////////////////////////////////////////////////////////////////////////////////
	
		PLAYER PROJECTILE CLASS

		Purpose: Represent a projectile shot by the player, that can have specific attibutes

		What was Learned:
			This *problably* didn't need to be assigned to the Player Directly, if the game had enemies
				they could prob use a projectile like this
			Just because you copied the method name of a signal, doesn't mean it is active. You need to
				activate it in Godot itself for it to work

	*/////////////////////////////////////////////////////////////////////////////////

	[Export]
	public float BaseSpeed = 100;
	public float Speed;

	[Export]
	public float Multiplier = 1;
	public float Damage = 1;
	[Export]
	public Vector2 Angle = new Vector2(0, -1);
	[Export]
	public float ConstantRotation = 0;

	public bool IsCrit = false;

	public string Hitsound = "";
	bool hitsoundended = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Speed = BaseSpeed;
		CheckCrit();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		this.Position += Angle * Speed * (float)delta;
		this.Rotate(ConstantRotation);
	}

	public void SetPos(Vector2 pos)
	{
		this.Position = pos;
	}

	public virtual void OnCollision()
	{
		if (Hitsound != "")
		{
			this.Visible = false;
			DisableCollisions();
			TempAudioGlobal.PlayAndDispose(Hitsound, this);
		}
		else
			QueueFree();
	}

	private void _on_area_entered(Area2D area) // this is too inconsistent, collision logic should happen only from one side, then attribute all logic there
	{
		//if (area.CollisionLayer == 4 && area is RoamingProjectile rm)
			//if (IsCrit) rm.AddCritStatus(); 
		if (area.CollisionLayer == 9) QueueFree();
	}

	public virtual void DisableCollisions() // done because disabling the hitbox itself didn't work? Maybe if I Set it Deffered it would
	{
		this.CollisionLayer = 0;
		this.CollisionMask = 0;
	}

	public virtual bool CheckCrit()
	{
		Node2D Crit = GetNodeOrNull<Node2D>("CritEffect");
		if (Crit != null)
		{
			this.IsCrit = true;
			this.Material = Crit.Material;
			return true;
		}

		return false;
	}
}



