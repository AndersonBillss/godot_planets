using Godot;
using System;

[Tool]
public partial class Camera : Camera3D {
	[Export] float distance = 4f;
	[Export] float speed = 2f;

	public float yaw = 0f;
	public float pitch = 0f;

	public override void _Ready() {
		base._Ready();
		_UpdatePosition();
	}

	public override void _Process(double delta) {
		// If in editor, use custom debug process
		if (Engine.IsEditorHint()) {
			_ProcessDebug(delta);
			return;
		}

		base._Process(delta);
		float dt = (float)delta;

		if (Input.IsKeyPressed(Key.Left)) {
			yaw += speed * dt;
		}
		if (Input.IsKeyPressed(Key.Right)) {
			yaw -= speed * dt;
		}
		if (Input.IsKeyPressed(Key.Up)) {
			pitch += speed * dt;
			if (pitch > Math.PI / 2) pitch = (float)Math.PI / 2;
		}
		if (Input.IsKeyPressed(Key.Down)) {
			pitch -= speed * dt;
			if (pitch < -Math.PI / 2) pitch = -(float)Math.PI / 2;
		}
		_UpdatePosition();
	}

	private float _prevDistance = -1f;
	void _ProcessDebug(double delta) {
		if (_prevDistance != distance) _UpdatePosition();
		_prevDistance = distance;
	}


	void _UpdatePosition() {
		float cosPitch = (float)Math.Cos(pitch);
		float posX = distance * (float)Math.Cos(yaw) * cosPitch;
		float posY = distance * (float)Math.Sin(pitch);
		float posZ = distance * (float)Math.Sin(yaw) * cosPitch;
		Position = new(posX, posY, posZ);
		Rotation = new(-pitch, -yaw + (float)Math.PI / 2, 0);
	}
}
