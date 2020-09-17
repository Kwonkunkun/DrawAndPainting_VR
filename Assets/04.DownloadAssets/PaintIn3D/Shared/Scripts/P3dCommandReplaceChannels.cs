using System.Collections.Generic;
using UnityEngine;

namespace PaintIn3D
{
	/// <summary>This class manages the replace channels painting command.</summary>
	public class P3dCommandReplaceChannels : P3dCommand
	{
		public Texture TextureR;
		public Texture TextureG;
		public Texture TextureB;
		public Texture TextureA;
		public Vector4 ChannelR;
		public Vector4 ChannelG;
		public Vector4 ChannelB;
		public Vector4 ChannelA;

		public static P3dCommandReplaceChannels Instance = new P3dCommandReplaceChannels();

		private static Stack<P3dCommandReplaceChannels> pool = new Stack<P3dCommandReplaceChannels>();

		private static Material cachedMaterial;

		public override bool RequireMesh { get { return false; } }

		static P3dCommandReplaceChannels()
		{
			cachedMaterial = P3dShader.BuildMaterial("Hidden/Paint in 3D/Replace Channels");
		}

		public static void Blit(RenderTexture renderTexture, Texture textureR, Texture textureG, Texture textureB, Texture textureA, Vector4 channelR, Vector4 channelG, Vector4 channelB, Vector4 channelA, Vector4 channels)
		{
			Instance.SetMaterial(textureR, textureG, textureB, textureA, channelR, channelG, channelB, channelA, channels);

			Instance.Apply();

			P3dHelper.Blit(renderTexture, Instance.Material);
		}

		public static void BlitFast(RenderTexture renderTexture, Texture textureR, Texture textureG, Texture textureB, Texture textureA, Vector4 channelR, Vector4 channelG, Vector4 channelB, Vector4 channelA, Vector4 channels)
		{
			Instance.SetMaterial(textureR, textureG, textureB, textureA, channelR, channelG, channelB, channelA, channels);

			Instance.Apply();

			Graphics.Blit(default(Texture), renderTexture, Instance.Material);
		}

		public override void Apply()
		{
			Material.SetTexture(P3dShader._TextureR, TextureR);
			Material.SetTexture(P3dShader._TextureG, TextureG);
			Material.SetTexture(P3dShader._TextureB, TextureB);
			Material.SetTexture(P3dShader._TextureA, TextureA);
			Material.SetVector(P3dShader._ChannelR, ChannelR);
			Material.SetVector(P3dShader._ChannelG, ChannelG);
			Material.SetVector(P3dShader._ChannelB, ChannelB);
			Material.SetVector(P3dShader._ChannelA, ChannelA);
		}

		public override void Pool()
		{
			pool.Push(this);
		}

		public override void Transform(Matrix4x4 posMatrix, Matrix4x4 rotMatrix)
		{
		}

		public override P3dCommand SpawnCopy()
		{
			var command = SpawnCopy(pool);

			command.TextureR = TextureR;
			command.TextureG = TextureG;
			command.TextureB = TextureB;
			command.TextureA = TextureA;
			command.ChannelR = ChannelR;
			command.ChannelG = ChannelG;
			command.ChannelB = ChannelB;
			command.ChannelA = ChannelA;

			return command;
		}

		public void SetMaterial(Texture textureR, Texture textureG, Texture textureB, Texture textureA, Vector4 channelR, Vector4 channelG, Vector4 channelB, Vector4 channelA, Vector4 channels)
		{
			Blend    = P3dBlendMode.Replace(channels);
			Material = cachedMaterial;
			TextureR = textureR;
			TextureG = textureG;
			TextureB = textureB;
			TextureA = textureA;
			ChannelR = channelR;
			ChannelG = channelG;
			ChannelB = channelB;
			ChannelA = channelA;
		}
	}
}