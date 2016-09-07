using System;

namespace VoxelBuilder.Interfaces
{
	public interface IBuilder
	{
		ThreadableMesh Build(VoxelData voxels);
	}
}

