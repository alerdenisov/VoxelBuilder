using VoxelBuilder.DataTypes;
using UnityEngine;

namespace VoxelBuilder.Interfaces
{
	public interface IMeshGenerator {}

	public interface IMeshGeneratorConfiguration : IMeshGenerator { 
		bool GenerateNormals { get; }
		bool GenerateTangents { get; }
		bool GenerateUV { get; } 
		bool GenerateUV2 { get; }
		bool GenerateColors { get; }
	}

	public interface IMeshCullingGenerator : IMeshGenerator {
		CullingFaces BuildCulling(ref VoxelData d, int x, int y, int z);
	}

	public interface IMeshVerticesGenerator : IMeshGenerator {
		Vector3[] BuildVertices(
			ref int count,
			ref CullingFaces c, 
			ref Voxel v, 
			int x, int y, int z);
	}

	public interface IMeshUVGenerator : IMeshGenerator {
		Vector2[] BuildUV(
			ref int count,
			ref CullingFaces c, 
			ref Voxel v, 
			int x, int y, int z);
	}

	public interface IMeshColorsGenerator : IMeshGenerator {
		Color[] BuildColors(
			ref int count,
			ref CullingFaces c, 
			ref Voxel v, 
			int x, int y, int z);
	}

	public interface IMeshNormalsGenerator : IMeshGenerator {
		Vector3[] BuildNormals(
			ref int count,
			ref CullingFaces c, 
			ref Voxel v, 
			int x, int y, int z);
	}

	public interface IMeshSecondUVGenerator : IMeshGenerator {
		Vector2[] BuildUV2(
			ref int count,
			ref CullingFaces c, 
			ref Voxel v, 
			int x, int y, int z);
	}

	public interface IMeshTrianglesGenerator : IMeshGenerator {
		int[] BuildTrianlges(
			ref int count,
			ref CullingFaces c, 
			ref Voxel v, 
			ref int triangleIndex, 
			int x, int y, int z);
	}
}

