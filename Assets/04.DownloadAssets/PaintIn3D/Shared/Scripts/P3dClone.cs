using UnityEngine;
using System.Collections.Generic;

namespace PaintIn3D
{
	/// <summary>This is the base class for all components that repeat paint commands (e.g. mirroring).</summary>
	public abstract class P3dClone : P3dLinkedBehaviour<P3dClone>, IClone
	{
		[System.NonSerialized]
		public static int MatrixCount;

		[System.NonSerialized]
		public static int ClonerCount;

		public abstract void Transform(ref Matrix4x4 posMatrix, ref Matrix4x4 rotMatrix);

		[System.NonSerialized]
		private static List<Matrix4x4> tempPosMatrices = new List<Matrix4x4>();

		[System.NonSerialized]
		private static List<Matrix4x4> tempRotMatrices = new List<Matrix4x4>();

		[System.NonSerialized]
		private static List<IClone> tempCloners = new List<IClone>();

		public static void BuildCloners(List<IClone> cloners = null)
		{
			tempCloners.Clear();
			tempPosMatrices.Clear();
			tempRotMatrices.Clear();

			tempPosMatrices.Add(Matrix4x4.identity);
			tempRotMatrices.Add(Matrix4x4.identity);

			if (cloners != null)
			{
				for (var i = 0; i < cloners.Count; i++)
				{
					var cloner = cloners[i];

					if (cloner != null)
					{
						tempCloners.Add(cloner);
					}
				}
			}
			else
			{
				var cloner = P3dClone.FirstInstance;

				for (var i = 0; i < P3dClone.InstanceCount; i++)
				{
					tempCloners.Add(cloner);

					cloner = cloner.NextInstance;
				}
			}

			MatrixCount = 1;
			ClonerCount = tempCloners.Count;
		}

		public static void Clone(P3dCommand command, int clonerIndex, int matrixIndex)
		{
			if (matrixIndex == 0)
			{
				MatrixCount = tempPosMatrices.Count;
			}

			var posMatrix = tempPosMatrices[matrixIndex];
			var rotMatrix = tempRotMatrices[matrixIndex];

			tempCloners[clonerIndex].Transform(ref posMatrix, ref rotMatrix);

			tempPosMatrices.Add(posMatrix);
			tempRotMatrices.Add(rotMatrix);

			command.Transform(posMatrix, rotMatrix);
		}
	}
}