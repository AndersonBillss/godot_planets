using Godot;
using System;

public partial class Camera : Camera3D {
	[Export] float distance = 4f;
	[Export] float speed = 2f;

	public float angle = 0f;

	public override void _Ready() {
		base._Ready();
		_UpdatePosition();
	}

	public override void _Process(double delta){
		base._Process(delta);
		float dt = (float)delta;

		if(Input.IsKeyPressed(Key.Left)){
			angle += speed * dt;
		}
		if(Input.IsKeyPressed(Key.Right)){
			angle -= speed * dt;
		}
		_UpdatePosition();
	}



	void _UpdatePosition() {
		float PosX = distance * (float)Math.Cos(angle);
		float PosZ = distance * (float)Math.Sin(angle);
		Position = new(PosX, 0, PosZ);
		Rotation = new(0, -angle + 90, 0);
	}
}
