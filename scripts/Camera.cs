using Godot;
using System;

[Tool]
public partial class Camera : Camera3D {
	[Export] float lookSpeed = 1f;
	[Export] float scrollSpeed = .05f;

	public float yaw = 0f;
	public float pitch = 0f;

	float distance = 6f;
	private float maxDistance = 25f;
	private float minDistance;
	public Planet planet;

	public override void _Ready() {
		base._Ready();
		_UpdatePosition();
		planet = GetParent<Planet>();
		minDistance = planet.Radius + .1f;
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseButton mouseEvent) {
			if (mouseEvent.ButtonIndex == MouseButton.WheelUp) {
				distance -= GetScrollDistace();
				if (distance < minDistance) distance = minDistance;
			}
			if (mouseEvent.ButtonIndex == MouseButton.WheelDown) {
				distance += GetScrollDistace();
				if (distance > maxDistance) distance = maxDistance;
			}
		}
	}

	private float GetScrollDistace() {
		return scrollSpeed * (distance - minDistance);
	}
	private float GetRotateDistace() {
		return (float)Math.Sqrt(distance - minDistance);
	}

	public override void _Process(double delta) {
		// If in editor, use custom debug process
		if (Engine.IsEditorHint()) {
			_ProcessDebug(delta);
			return;
		}

		base._Process(delta);
		float dt = (float)delta;

		if (Input.IsKeyPressed(Key.Left) || Input.IsKeyPressed(Key.A)) {
			yaw += lookSpeed * GetRotateDistace() * dt;
		}
		if (Input.IsKeyPressed(Key.Right) || Input.IsKeyPressed(Key.D)) {
			yaw -= lookSpeed * GetRotateDistace() * dt;
		}
		if (Input.IsKeyPressed(Key.Up) || Input.IsKeyPressed(Key.W)) {
			pitch += lookSpeed * GetRotateDistace() * dt;
			if (pitch > Math.PI / 2) pitch = (float)Math.PI / 2;
		}
		if (Input.IsKeyPressed(Key.Down) || Input.IsKeyPressed(Key.S)) {
			pitch -= lookSpeed * GetRotateDistace() * dt;
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
