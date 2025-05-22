using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;

[Tool]
public partial class Planet : MeshInstance3D {
	[Export] public float Radius = 1f;
	private float _previousRadius = -1f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		if (Engine.IsEditorHint()) {
			_GeneratePreview(Radius);
		}
		else {
			_GenerateSphere(Radius);
		}
	}

	int cubeSections = 3;
	private void _GenerateSphere(float size) {
		SurfaceTool st = new();
		st.Begin(Mesh.PrimitiveType.Triangles);
		List<Vector3> vertices = [];
		List<Vector3> normals = [];
		List<int> indices = [];

		ConstructTop(normals, vertices);
		ConstructRight(normals, vertices);
		ConstructFront(normals, vertices);

		ConstructBottom(normals, vertices);
		ConstructLeft(normals, vertices);
		ConstructBack(normals, vertices);


		for (int i = 0; i < vertices.Count; i++) {
			indices.Add(i);
		}

		for (int i = 0; i < vertices.Count; i++) {
			st.SetNormal(normals[i]);
			st.AddVertex(vertices[i]);
		}

		foreach (int index in indices) {
			st.AddIndex(index);
		}

		ArrayMesh mesh = new();
		st.Commit(mesh);
		Mesh = mesh;
	}

	private void ConstructBottom(List<Vector3> normals, List<Vector3> vertices) {
		for (int x = 0; x < cubeSections; x++) {
			for (int z = 0; z < cubeSections; z++) {
				int y = 0;
				Vector3 point1 = new(x, y, z);
				Vector3 point2 = new(x + 1, y, z);
				Vector3 point3 = new(x, y, z + 1);
				Vector3 point4 = new(x + 1, y, z + 1);

				AddSquareTriangles(point3, point4, point1, point2, normals, vertices);
			}
		}
	}
	private void ConstructTop(List<Vector3> normals, List<Vector3> vertices) {
		for (int x = 0; x < cubeSections; x++) {
			for (int z = 0; z < cubeSections; z++) {
				int y = 0;
				Vector3 point1 = new(x, y + cubeSections, z);
				Vector3 point2 = new(x + 1, y + cubeSections, z);
				Vector3 point3 = new(x, y + cubeSections, z + 1);
				Vector3 point4 = new(x + 1, y + cubeSections, z + 1);

				AddSquareTriangles(point1, point2, point3, point4, normals, vertices);
			}
		}
	}
	private void ConstructLeft(List<Vector3> normals, List<Vector3> vertices) {
		for (int x = 0; x < cubeSections; x++) {
			for (int y = 0; y < cubeSections; y++) {
				int z = 0;
				Vector3 point1 = new(x, y, z + cubeSections);
				Vector3 point2 = new(x + 1, y, z + cubeSections);
				Vector3 point3 = new(x, y + 1, z + cubeSections);
				Vector3 point4 = new(x + 1, y + 1, z + cubeSections);

				AddSquareTriangles(point3, point4, point1, point2, normals, vertices);
			}
		}
	}
	private void ConstructRight(List<Vector3> normals, List<Vector3> vertices) {
		for (int x = 0; x < cubeSections; x++) {
			for (int y = 0; y < cubeSections; y++) {
				int z = 0;
				Vector3 point1 = new(x, y, z);
				Vector3 point2 = new(x + 1, y, z);
				Vector3 point3 = new(x, y + 1, z);
				Vector3 point4 = new(x + 1, y + 1, z);

				AddSquareTriangles(point1, point2, point3, point4, normals, vertices);
			}
		}
	}
	private void ConstructFront(List<Vector3> normals, List<Vector3> vertices) {
		for (int y = 0; y < cubeSections; y++) {
			for (int z = 0; z < cubeSections; z++) {
				int x = 0;
				Vector3 point1 = new(x + cubeSections, y, z);
				Vector3 point2 = new(x + cubeSections, y + 1, z);
				Vector3 point3 = new(x + cubeSections, y, z + 1);
				Vector3 point4 = new(x + cubeSections, y + 1, z + 1);

				AddSquareTriangles(point3, point4, point1, point2, normals, vertices);
			}
		}
	}
	private void ConstructBack(List<Vector3> normals, List<Vector3> vertices) {
		for (int y = 0; y < cubeSections; y++) {
			for (int z = 0; z < cubeSections; z++) {
				int x = 0;
				Vector3 point1 = new(x, y, z);
				Vector3 point2 = new(x, y + 1, z);
				Vector3 point3 = new(x, y, z + 1);
				Vector3 point4 = new(x, y + 1, z + 1);

				AddSquareTriangles(point1, point2, point3, point4, normals, vertices);
			}
		}
	}

	private void AddSquareTriangles(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4, List<Vector3> normals, List<Vector3> vertices) {
		Vector3 shift = new(.5f, .5f, .5f);
		Vector3 adjustedPoint1 = (point1 / cubeSections) - shift;
		Vector3 adjustedPoint2 = (point2 / cubeSections) - shift;
		Vector3 adjustedPoint3 = (point3 / cubeSections) - shift;
		Vector3 adjustedPoint4 = (point4 / cubeSections) - shift;

		vertices.Add(adjustedPoint1);
		vertices.Add(adjustedPoint2);
		vertices.Add(adjustedPoint3);
		normals.Add(adjustedPoint1);
		normals.Add(adjustedPoint1);
		normals.Add(adjustedPoint1);
		// normals.Add(adjustedPoint2);
		// normals.Add(adjustedPoint3);

		vertices.Add(adjustedPoint2);
		vertices.Add(adjustedPoint4);
		vertices.Add(adjustedPoint3);
		normals.Add(adjustedPoint2);
		normals.Add(adjustedPoint2);
		normals.Add(adjustedPoint2);
		// normals.Add(adjustedPoint4);
		// normals.Add(adjustedPoint3);
	}

	private Vector3 GetCubeSphereCoords(Vector3 pointPosition, Vector3 shapeCenter) {
		Vector3 toCenter = pointPosition - shapeCenter;
		return shapeCenter + toCenter.Normalized() * Radius;
	}

	public static Vector3 GetOrientedTriangleNormal(Vector3 a, Vector3 b, Vector3 c, Vector3 shapeCenter) {
		// Compute the normal using the cross product of two triangle edges
		Vector3 edge1 = b - a;
		Vector3 edge2 = c - a;
		Vector3 normal = edge1.Cross(edge2).Normalized();

		// Ensure the normal is facing away from the shape center
		Vector3 triangleCenter = (a + b + c) / 3.0f;
		Vector3 toCenter = shapeCenter - triangleCenter;

		// If the normal is pointing towards the shape center, flip it
		if (normal.Dot(toCenter) > 0) {
			normal = -normal;
		}

		return normal;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (Engine.IsEditorHint() && (_previousRadius != Radius)) {
			_ProcessDebug(delta);
			return;
		}
	}

	void _ProcessDebug(double delta) {
		_previousRadius = Radius;
		_GeneratePreview(Radius);
	}

	private void _GeneratePreview(float radius) {
		Mesh = new SphereMesh() { Height = radius * 2, Radius = radius };
	}
}
