using System;
using System.Collections.Generic;
using Godot;

[Tool]
public partial class Planet : MeshInstance3D
{
	[Export] public float Radius = 1f;
	private float _previousRadius = -1f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		if (Engine.IsEditorHint()) {
			_GeneratePreview(Radius);
		} else {
			_GenerateSphere(Radius);
		}
	}

	float longitudeLines = 50;
	float lattitudeLines = 30;
	private void _GenerateSphere(float size){
		SurfaceTool st = new();
		st.Begin(Mesh.PrimitiveType.Triangles);
        List<Vector3> vertices = [];
        List<Vector3> normals = [];
		List<int> indices = [];

        // Loop over lattitude and longitude lines
        for(int i=0; i<longitudeLines; i++){
            for(int j=0; j<lattitudeLines; j++){
				Vector3 a = GetSphereCoords(i, j);
				Vector3 b = GetSphereCoords((int)((i+1)%longitudeLines), j);
				Vector3 c = GetSphereCoords(i, (int)((j+1)%lattitudeLines));
				vertices.Add(a);
				vertices.Add(b);
				vertices.Add(c);
				Vector3 normal = GetOrientedTriangleNormal(a, b, c, new Vector3(0,0,0));
				normals.Add(normal);
				normals.Add(normal);
				normals.Add(normal);

				Vector3 d = GetSphereCoords((int)((i+1)%longitudeLines), (int)((j+1)%lattitudeLines));
				vertices.Add(b);
				vertices.Add(d);
				vertices.Add(c);
				normals.Add(normal);
				normals.Add(normal);
				normals.Add(normal);
            }
        }
		for(int i=0; i<vertices.Count; i++){
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

	private Vector3 GetSphereCoords(int i, int j){
		float pitch = i * (2*(float)Math.PI / longitudeLines);
		float yaw = j * (2*(float)Math.PI / lattitudeLines);
		float cosPitch = (float)Math.Cos(pitch);
		
		float posX = Radius * (float)Math.Cos(yaw) * cosPitch;
		float posY = Radius * (float)Math.Sin(pitch);
		float posZ = Radius * (float)Math.Sin(yaw) * cosPitch;

		return new Vector3(posX, posY, posZ);
	}

	public static Vector3 GetOrientedTriangleNormal(Vector3 a, Vector3 b, Vector3 c, Vector3 shapeCenter)
    {
        // Compute the normal using the cross product of two triangle edges
        Vector3 edge1 = b - a;
        Vector3 edge2 = c - a;
        Vector3 normal = edge1.Cross(edge2).Normalized();

        // Ensure the normal is facing away from the shape center
        Vector3 triangleCenter = (a + b + c) / 3.0f;
        Vector3 toCenter = shapeCenter - triangleCenter;

        // If the normal is pointing towards the shape center, flip it
        if (normal.Dot(toCenter) > 0)
        {
            normal = -normal;
        }

        return normal;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if(Engine.IsEditorHint() && (_previousRadius != Radius)) {
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
