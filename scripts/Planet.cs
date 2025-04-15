using Godot;
using System;

public partial class Planet : MeshInstance3D
{
	[Export] public float Size = 1f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		Mesh = _GenerateSphere(Size);
	}

	private ArrayMesh _GenerateSphere(float size){
		SurfaceTool st = new();
		st.Begin(Mesh.PrimitiveType.Triangles);

		float h = size * .5f;
		Vector3[] vertices = [
			new(-h, -h,  h), new( h, -h,  h), new( h,  h,  h), new(-h,  h,  h), // Front
			new( h, -h, -h), new(-h, -h, -h), new(-h,  h, -h), new( h,  h, -h), // Back
			new(-h, -h, -h), new(-h, -h,  h), new(-h,  h,  h), new(-h,  h, -h), // Left
			new( h, -h,  h), new( h, -h, -h), new( h,  h, -h), new( h,  h,  h), // Right
			new(-h,  h,  h), new( h,  h,  h), new( h,  h, -h), new(-h,  h, -h), // Top
			new(-h, -h, -h), new( h, -h, -h), new( h, -h,  h), new(-h, -h,  h)  // Bottom
		];

		Vector3[] normals = [
			Vector3.Forward, Vector3.Forward, Vector3.Forward, Vector3.Forward,
			Vector3.Back, Vector3.Back, Vector3.Back, Vector3.Back,
			Vector3.Left, Vector3.Left, Vector3.Left, Vector3.Left,
			Vector3.Right, Vector3.Right, Vector3.Right, Vector3.Right,
			Vector3.Up, Vector3.Up, Vector3.Up, Vector3.Up,
			Vector3.Down, Vector3.Down, Vector3.Down, Vector3.Down
		];

		int[] indices = [
			0, 1, 2, 2, 3, 0,
			4, 5, 6, 6, 7, 4,
			8, 9,10,10,11, 8,
		   12,13,14,14,15,12,
		   16,17,18,18,19,16,
		   20,21,22,22,23,20
		];

		for (int i = 0; i < vertices.Length; i++) {
			st.SetNormal(normals[i]);
			st.AddVertex(vertices[i]);
		}

		foreach (int index in indices) {
			st.AddIndex(index);
		}

		ArrayMesh mesh = new();
		st.Commit(mesh);
		return mesh;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}
}
