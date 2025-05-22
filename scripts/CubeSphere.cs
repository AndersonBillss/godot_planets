using System.Collections.Generic;
using Godot;

namespace godot_planets.scripts {
    public class CubeSphere {
        public static void ConstructSphereCube(List<Vector3> normals, List<Vector3> vertices, List<int> indices, int cubeSections, float radius) {
            ConstructTop(normals, vertices, cubeSections, radius);
            ConstructRight(normals, vertices, cubeSections, radius);
            ConstructFront(normals, vertices, cubeSections, radius);

            ConstructBottom(normals, vertices, cubeSections, radius);
            ConstructLeft(normals, vertices, cubeSections, radius);
            ConstructBack(normals, vertices, cubeSections, radius);

            for (int i = 0; i < vertices.Count; i++) {
                indices.Add(i);
            }
        }
        private static void ConstructBottom(List<Vector3> normals, List<Vector3> vertices, int cubeSections, float radius) {
            for (int x = 0; x < cubeSections; x++) {
                for (int z = 0; z < cubeSections; z++) {
                    int y = 0;
                    Vector3 point1 = new(x, y, z);
                    Vector3 point2 = new(x + 1, y, z);
                    Vector3 point3 = new(x, y, z + 1);
                    Vector3 point4 = new(x + 1, y, z + 1);

                    AddSquareTriangles(point3, point4, point1, point2, normals, vertices, cubeSections, radius);
                }
            }
        }
        private static void ConstructTop(List<Vector3> normals, List<Vector3> vertices, int cubeSections, float radius) {
            for (int x = 0; x < cubeSections; x++) {
                for (int z = 0; z < cubeSections; z++) {
                    int y = 0;
                    Vector3 point1 = new(x, y + cubeSections, z);
                    Vector3 point2 = new(x + 1, y + cubeSections, z);
                    Vector3 point3 = new(x, y + cubeSections, z + 1);
                    Vector3 point4 = new(x + 1, y + cubeSections, z + 1);

                    AddSquareTriangles(point1, point2, point3, point4, normals, vertices, cubeSections, radius);
                }
            }
        }
        private static void ConstructLeft(List<Vector3> normals, List<Vector3> vertices, int cubeSections, float radius) {
            for (int x = 0; x < cubeSections; x++) {
                for (int y = 0; y < cubeSections; y++) {
                    int z = 0;
                    Vector3 point1 = new(x, y, z + cubeSections);
                    Vector3 point2 = new(x + 1, y, z + cubeSections);
                    Vector3 point3 = new(x, y + 1, z + cubeSections);
                    Vector3 point4 = new(x + 1, y + 1, z + cubeSections);

                    AddSquareTriangles(point3, point4, point1, point2, normals, vertices, cubeSections, radius);
                }
            }
        }
        private static void ConstructRight(List<Vector3> normals, List<Vector3> vertices, int cubeSections, float radius) {
            for (int x = 0; x < cubeSections; x++) {
                for (int y = 0; y < cubeSections; y++) {
                    int z = 0;
                    Vector3 point1 = new(x, y, z);
                    Vector3 point2 = new(x + 1, y, z);
                    Vector3 point3 = new(x, y + 1, z);
                    Vector3 point4 = new(x + 1, y + 1, z);

                    AddSquareTriangles(point1, point2, point3, point4, normals, vertices, cubeSections, radius);
                }
            }
        }
        private static void ConstructFront(List<Vector3> normals, List<Vector3> vertices, int cubeSections, float radius) {
            for (int y = 0; y < cubeSections; y++) {
                for (int z = 0; z < cubeSections; z++) {
                    int x = 0;
                    Vector3 point1 = new(x + cubeSections, y, z);
                    Vector3 point2 = new(x + cubeSections, y + 1, z);
                    Vector3 point3 = new(x + cubeSections, y, z + 1);
                    Vector3 point4 = new(x + cubeSections, y + 1, z + 1);

                    AddSquareTriangles(point3, point4, point1, point2, normals, vertices, cubeSections, radius);
                }
            }
        }
        private static void ConstructBack(List<Vector3> normals, List<Vector3> vertices, int cubeSections, float radius) {
            for (int y = 0; y < cubeSections; y++) {
                for (int z = 0; z < cubeSections; z++) {
                    int x = 0;
                    Vector3 point1 = new(x, y, z);
                    Vector3 point2 = new(x, y + 1, z);
                    Vector3 point3 = new(x, y, z + 1);
                    Vector3 point4 = new(x, y + 1, z + 1);

                    AddSquareTriangles(point1, point2, point3, point4, normals, vertices, cubeSections, radius);
                }
            }
        }

        private static void AddSquareTriangles(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4, List<Vector3> normals, List<Vector3> vertices, int cubeSections, float radius) {
            Vector3 shift = new(.5f, .5f, .5f);
            Vector3 adjustedPoint1 = GetCubeSphereCoords(point1 / cubeSections - shift, new Vector3(0, 0, 0), radius);
            Vector3 adjustedPoint2 = GetCubeSphereCoords(point2 / cubeSections - shift, new Vector3(0, 0, 0), radius);
            Vector3 adjustedPoint3 = GetCubeSphereCoords(point3 / cubeSections - shift, new Vector3(0, 0, 0), radius);
            Vector3 adjustedPoint4 = GetCubeSphereCoords(point4 / cubeSections - shift, new Vector3(0, 0, 0), radius);

            vertices.Add(adjustedPoint1);
            vertices.Add(adjustedPoint2);
            vertices.Add(adjustedPoint3);
            normals.Add(adjustedPoint1);
            normals.Add(adjustedPoint2);
            normals.Add(adjustedPoint3);

            vertices.Add(adjustedPoint2);
            vertices.Add(adjustedPoint4);
            vertices.Add(adjustedPoint3);
            normals.Add(adjustedPoint2);
            normals.Add(adjustedPoint4);
            normals.Add(adjustedPoint3);
        }

        private static Vector3 GetCubeSphereCoords(Vector3 pointPosition, Vector3 shapeCenter, float radius) {
            Vector3 toCenter = pointPosition - shapeCenter;
            return shapeCenter + toCenter.Normalized() * radius;
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
    }
}