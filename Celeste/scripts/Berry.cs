using System;
using Godot;

public partial class Berry : Area2D
{
    private void _on_body_entered(Node2D body)
    {
        if (body is Madeline m)
           BerryGot(m)
        
    }

    public void BerryGot(Madeline m)
    {
        m.GroundSafetyTimer.Timeout += Collected;
    }

    public void Collected()
    {
        GD.Print("Berry Got!");
        QueueFree();
    }
}