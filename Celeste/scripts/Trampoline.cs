using System;
using Godot;

public partial class Trampoline : Area2D
{
    [Export]
    public float PushForce = 100;

    public delegate OnJumpedEventHandler;
    
    private void _on_body_entered(Node2D body)
    {
       if (body is IPushable b)
          b.ApplyImpulse(this.GetNormal() * PushForce)
    }
}