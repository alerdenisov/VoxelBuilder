using System;

namespace VoxelBuilder.DataTypes
{
	public enum CullingFaces : byte
	{
		None = 0,

		East            = 1 << 0, // 1
		West            = 1 << 1, // 2
		North           = 1 << 2, // 4
		South           = 1 << 3, // 8
		Up              = 1 << 4, // 16
		Down            = 1 << 5, // 32

		All = East | West | North | South | Up | Down
	}
}

