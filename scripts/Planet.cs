using System.Collections.Generic;
using Godot;
using godot_planets.scripts;
using static godot_planets.scripts.CubeSphere;

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

	int cubeSections = 15;
	private void _GenerateSphere(float radius) {
		List<Vector3> vertices = [];
		List<Vector3> normals = [];
		List<int> indices = [];

		// ConstructSphereCube(normals, vertices, indices, cubeSections, radius);
		_ConstructPlane(normals, vertices, indices, radius, cubeSections);

		ArrayMesh mesh = new();

		CreateMesh(normals, vertices, indices, mesh);
		CreateOutline(vertices, indices, mesh);

		Mesh = mesh;
	}

	private static void CreateMesh(List<Vector3> normals, List<Vector3> vertices, List<int> indices, ArrayMesh mesh) {
		SurfaceTool st = new();
		st.Begin(Mesh.PrimitiveType.Triangles);

		for (int i = 0; i < vertices.Count; i++) {
			st.SetNormal(normals[i]);
			st.AddVertex(vertices[i]);
		}

		foreach (int index in indices) {
			st.AddIndex(index);
		}

		st.Commit(mesh);
	}
	private static void CreateOutline(List<Vector3> vertices, List<int> indices, ArrayMesh mesh) {
		SurfaceTool st = new();
		st.Begin(Mesh.PrimitiveType.Lines);
		Color lineColor = new(1, 0, 0);

		for (int i = 0; i < indices.Count; i += 3) {
			Vector3 v0 = vertices[indices[i]];
			Vector3 v1 = vertices[indices[i + 1]];
			Vector3 v2 = vertices[indices[i + 2]];

			st.SetColor(lineColor); st.AddVertex(v0);
			st.SetColor(lineColor); st.AddVertex(v1);

			st.SetColor(lineColor); st.AddVertex(v1);
			st.SetColor(lineColor); st.AddVertex(v2);

			st.SetColor(lineColor); st.AddVertex(v2);
			st.SetColor(lineColor); st.AddVertex(v0);
		}

		st.Commit(mesh);

		StandardMaterial3D lineMaterial = new() {
			ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
			AlbedoColor = lineColor
		};
		int lastSurface = mesh.GetSurfaceCount() - 1;
		mesh.SurfaceSetMaterial(lastSurface, lineMaterial);
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


	private static void _ConstructPlane(List<Vector3> normals, List<Vector3> vertices, List<int> indices, float width, int cubeSections) {
		for (int i = 0; i < cubeSections - 1; i++) {
			for (int j = 0; j < cubeSections - 1; j++) {
				int seed = 1264236727;
				float scale = .5f;

				float step = width / cubeSections;
				float xInitial = (i - cubeSections / 2) * step;
				float zInitial = (j - cubeSections / 2) * step;
				float yInitial = 0;

				indices.Add(vertices.Count + 3);
				indices.Add(vertices.Count);
				indices.Add(vertices.Count + 1);

				indices.Add(vertices.Count + 3);
				indices.Add(vertices.Count + 2);
				indices.Add(vertices.Count);

				float x = xInitial;
				float z = zInitial;
				float y = yInitial + PerlinNoise.PerlinNoise2D(seed, x, z) * scale;
				Vector3 v0 = new(x, y, z);

				x = xInitial + step;
				z = zInitial;
				y = yInitial + PerlinNoise.PerlinNoise2D(seed, x, z) * scale;
				Vector3 v1 = new(x, y, z);

				x = xInitial;
				z = zInitial + step;
				y = yInitial + PerlinNoise.PerlinNoise2D(seed, x, z) * scale;
				Vector3 v2 = new(x, y, z);

				x = xInitial + step;
				z = zInitial + step;
				y = yInitial + PerlinNoise.PerlinNoise2D(seed, x, z) * scale;
				Vector3 v3 = new(x, y, z);

				vertices.Add(v0);
				vertices.Add(v1);
				vertices.Add(v2);
				vertices.Add(v3);

				Vector3 normal1 = (v2 - v0).Cross(v1 - v0).Normalized();
				normals.Add(normal1);
				normals.Add(normal1);

				Vector3 normal2 = (v2 - v0).Cross(v3 - v0).Normalized();
				normals.Add(normal2);
				normals.Add(normal2);
			}
		}
	}
}
