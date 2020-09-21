using UnityEngine;
using System.Collections.Generic;

namespace PaintIn3D
{
	/// <summary>This is the base class for all paint commands. These commands (e.g. paint decal) are added to the command list for each P3dPaintableTexture, and are executed at the end of the frame to optimize state changes.</summary>
	public abstract class P3dCommand
	{
		/// <summary>This is the original array index, used to stable sort between two commands if they have the same priority.</summary>
		public int Index;

		public bool         Preview;
		public bool         Double = true;
		public int          Priority;
		public P3dBlendMode Blend;
		public Material     Material;
		public P3dModel     Model;
		public int          Submesh;

		public static int Compare(P3dCommand a, P3dCommand b)
		{
			var delta = a.Priority.CompareTo(b.Priority);
			
			if (delta > 0)
			{
				return 1;
			}
			else if (delta < 0)
			{
				return -1;
			}

			return a.Index.CompareTo(b.Index);
		}

		public abstract bool RequireMesh
		{
			get;
		}

		public void SetState(bool preview, int priority)
		{
			Preview  = preview;
			Priority = priority;
			Index    = 0;
		}

		public abstract void Apply();
		public abstract void Pool();
		public abstract void Transform(Matrix4x4 posMatrix, Matrix4x4 rotMatrix);
		public abstract P3dCommand SpawnCopy();

		public P3dCommand SpawnCopyLocal(Transform transform)
		{
			var copy   = SpawnCopy();
			var matrix = transform.worldToLocalMatrix;

			copy.Transform(matrix, matrix);

			return copy;
		}

		public P3dCommand SpawnCopyWorld(Transform transform)
		{
			var copy   = SpawnCopy();
			var matrix = transform.localToWorldMatrix;

			copy.Transform(matrix, matrix);

			return copy;
		}

		protected T SpawnCopy<T>(Stack<T> pool)
			where T : P3dCommand, new()
		{
			var command = pool.Count > 0 ? pool.Pop() : new T();

			command.Preview  = Preview;
			command.Priority = Priority;
			command.Index    = Index;
			command.Blend    = Blend;
			command.Material = Material;
			command.Model    = Model;
			command.Submesh  = Submesh;

			return command;
		}
	}
}