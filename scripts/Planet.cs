using System.Collections.Generic;
using Godot;
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

	int cubeSections = 10;
	private void _GenerateSphere(float radius) {
		List<Vector3> vertices = [];
		List<Vector3> normals = [];
		List<int> indices = [];

		ConstructSphereCube(normals, vertices, indices, cubeSections, radius);
		ArrayMesh mesh = new();

        CreateMesh(normals, vertices, indices, mesh);
        CreateOutline(vertices, mesh);

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
	private static void CreateOutline(List<Vector3> vertices, ArrayMesh mesh) {
		SurfaceTool st = new();
		st.Begin(Mesh.PrimitiveType.Lines);
		Color lineColor = new(1, 0, 0);

		for (int i = 0; i < vertices.Count; i += 3) {
			Vector3 v0 = vertices[i];
			Vector3 v1 = vertices[i + 1];
			Vector3 v2 = vertices[i + 2];

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
}
