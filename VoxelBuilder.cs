using System;
using System.Collections.Generic;
using UnityEngine;
using Uniful;
using VoxelBuilder.DataTypes;

namespace VoxelBuilder
{
    public static class VoxelBuilderbak
    {
        public static ThreadableMesh Build(VoxelData voxels)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Color> colors = new List<Color>();
            List<Vector2> uvs = new List<Vector2>();
			List<Vector2> uvs1 = new List<Vector2> ();
            List<Vector3> normals = new List<Vector3>();
            int triIndex = 0;
            for (int y = 0; y < voxels.Height; y++)
            {
                for (int z = 0; z < voxels.Depth; z++)
                {
                    for (int x = 0; x < voxels.Width; x++)
                    {
                        var voxel = voxels.GetVoxel(x, y, z);
                        if (voxel.Color.r == 0 && voxel.Color.g == 0 && voxel.Color.b == 0 && voxel.Color.a == 0)
                            continue;

                        // build
                        var cullingFaces = CalcCullingFaces(voxels, x, y, z);

                        if(cullingFaces == CullingFaces.All)
                            continue;

                        vertices.AddRange(MakeVertices(cullingFaces, x, y, z));
                        uvs.AddRange(MakeUVs(cullingFaces));
						uvs1.AddRange (MakeTimeUvs (cullingFaces, voxel));
                        triangles.AddRange(MakeTriangles(cullingFaces, ref triIndex));
                        colors.AddRange(MakeColors(cullingFaces, voxel));
                        normals.AddRange(MakeNormals(cullingFaces, x, y, z));
                    }
                }
            }

            var mesh = new ThreadableMesh();
            mesh.Vertices = vertices.ToArray();
            mesh.Triangles = triangles.ToArray();
            mesh.Colors = colors.ToArray();
			mesh.Uvs1 = uvs1.ToArray ();
            mesh.Uvs = uvs.ToArray();
            mesh.Normals = normals.ToArray();
            mesh.RecalculateTangets();// = tangets.ToArray();
            return mesh;
        }

        private static IEnumerable<Vector3> MakeNormals(CullingFaces cullingFaces, int x, int y, int z)
        {
            var count = 4 * (6 - ((byte)cullingFaces).PopCount());

            var vertices = new Vector3[count];
            var index = 0;

            foreach (var face in Enum.GetValues(typeof(CullingFaces)))
            {
                if (((byte)(CullingFaces)face).PopCount() != 1) continue;
                if (!cullingFaces.HasFlag((CullingFaces)face))
                {
                    Array.Copy(
                        MakeFaceNormals((CullingFaces)face, x, y, z),
                        0,
                        vertices,
                        index,
                        4);
                    index += 4;
                }

            }

            return vertices;
        }

        private static Vector2[] MakeUVs(CullingFaces cullingFaces)
        {
            var count = 4 * (6 - ((byte)cullingFaces).PopCount());
            var r = new Vector2[count];

            for (int i = 0; i < r.Length; i+=4)
            {
                r[i + 0] = new Vector2(0, 1);
                r[i + 1] = new Vector2(1, 1);
                r[i + 2] = new Vector2(0, 0);
                r[i + 3] = new Vector2(1, 0);
            }

            return r;
        }

		private static Vector2[] MakeTimeUvs(CullingFaces cullingFaces, Voxel v) 
		{
			var time = v.Time;
			var count = 4 * (6 - ((byte)cullingFaces).PopCount ());
			var u = new Vector2[count];
			for (int i = 0; i < count; i++) 
			{
				u[i] = new Vector2 (time, time);
			}

			return u;
		}

        private static Color[] MakeColors(CullingFaces cullingFaces, Voxel v)
        {
            var color = v.Color;
            var count = 4 * (6 - ((byte)cullingFaces).PopCount());
            var r = new Color[count];

            for (int i = 0; i < r.Length; i++)
            {
//                float red = EncodeColor(color);
				r[i] = color;//new Color(red, v.Time, 0f, 0f);
            }

            return r;
        }

        private static float EncodeColor(Color color)
        {
			Vector4 kDecodeDot = new Vector3 (1f, 1f / 255f, 1f / 65025f);
			return Vector3.Dot (new Vector3 (color.r, color.g, color.b), kDecodeDot);

//			float4 kEncodeMul = float4(1.0, 255.0, 65025.0, 160581375.0);
//			float kEncodeBit = 1.0/255.0;
//			float4 enc = kEncodeMul * v;
//			enc = frac (enc);
//			enc -= enc.yzww * kEncodeBit;
//			return enc;



//			var dot = new Vector4 (
//				          1f / (256 * 256 * 256),
//				          1f / (256 * 256),
//				          1f / 256,
//				          1f);
//			return Vector4.Dot (color, dot);
//			return color.r + color.g * 256 + color.b * 256 * 256;
//            Vector4 enc = color;
//
//            Vector4 kDecodeDot = new Vector4(1.0f, 1f / 255f, 1f / 65025f, 1f / 16581375f);
//            return Vector4.Dot(enc, kDecodeDot);
        }

        private static int[] MakeTriangles(CullingFaces face, ref int start)
        {
            var count = 6 * (6 - ((byte)face).PopCount());
            var triangles = new int[count];
            var index = 0;
            foreach (var f in Enum.GetValues(typeof(CullingFaces)))
            {
                if (((byte)(CullingFaces)f).PopCount() != 1) continue;
                if (!face.HasFlag((CullingFaces)f))
                {
                    Array.Copy(MakeFaceTriangles((CullingFaces)f, start), 0, triangles, index, 6);
                    index += 6;
                    start += 4;
                }
            }

            return triangles;
        }

        private static int[] MakeFaceTriangles(CullingFaces face, int i)
        {
            return new int[]
            {
                i + 0,
                i + 1,
                i + 2,
                i + 3,
                i + 2,
                i + 1
            };
        }

        private static Vector3[] MakeVertices(CullingFaces cullingFaces, int x, int y, int z)
        {
            var count = 4*(6 - ((byte) cullingFaces).PopCount());

            var vertices = new Vector3[count];
            var index = 0;

            foreach (var face in Enum.GetValues(typeof(CullingFaces)))
            {
                if (((byte)(CullingFaces)face).PopCount() != 1) continue;
                if (!cullingFaces.HasFlag((CullingFaces) face))
                {
                    Array.Copy(
                        MakeFaceVerts((CullingFaces) face, x, y, z),
                        0,
                        vertices,
                        index,
                        4);
                    index += 4;
                }

            }

            return vertices;
        }


        private static Vector3[] MakeFaceNormals(CullingFaces face, int x, int y, int z)
        {
            var q = GetRotation(face);
            var r = new[]
            {
                q*new Vector3(-.5f,  .5f, -.5f).normalized,
                q*new Vector3( .5f,  .5f, -.5f).normalized,
                q*new Vector3(-.5f, -.5f, -.5f).normalized,
                q*new Vector3( .5f, -.5f, -.5f).normalized,
//                q*Vector3.back,
//                q*Vector3.back,
//                q*Vector3.back,
//                q*Vector3.back
            };
            return r;
        }

        private static Vector3[] MakeFaceVerts(CullingFaces face, int x, int y, int z)
        {
            var o = new Vector3(x,y,z);
            var q = GetRotation(face);
            var r = new []
            {
                q*new Vector3(-.5f,  .5f, -.5f) + o,
                q*new Vector3( .5f,  .5f, -.5f) + o,
                q*new Vector3(-.5f, -.5f, -.5f) + o,
                q*new Vector3( .5f, -.5f, -.5f) + o,
            };
            return r;
        }

        private static Quaternion GetRotation(CullingFaces face)
        {
            switch (face)
            {
                case CullingFaces.West:
                    return Quaternion.Euler(0, 90, 0);
                case CullingFaces.North:
                    return Quaternion.Euler(0, 180, 0);
                case CullingFaces.East:
                    return Quaternion.Euler(0, 270, 0);
                case CullingFaces.Up:
                    return Quaternion.Euler(90, 0, 0);
                case CullingFaces.Down:
                    return Quaternion.Euler(-90, 0, 0);
            }

            return Quaternion.identity;
        }

        private static CullingFaces CalcCullingFaces(VoxelData voxels, int x, int y, int z)
        {
            var r = CullingFaces.All;
            if (!voxels.GetVoxel(x + 1, y, z).IsSolid)
                r -= CullingFaces.East;
            if (!voxels.GetVoxel(x - 1, y, z).IsSolid)
                r -= CullingFaces.West;
            if (!voxels.GetVoxel(x, y, z - 1).IsSolid)
                r -= CullingFaces.South;
            if (!voxels.GetVoxel(x, y, z + 1).IsSolid)
                r -= CullingFaces.North;
            if (!voxels.GetVoxel(x, y - 1, z).IsSolid)
                r -= CullingFaces.Down;
            if (!voxels.GetVoxel(x, y + 1, z).IsSolid)
                r -= CullingFaces.Up;

            return r;
        }
    }
}