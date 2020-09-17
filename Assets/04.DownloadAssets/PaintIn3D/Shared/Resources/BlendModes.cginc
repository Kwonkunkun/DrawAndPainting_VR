#include "UnityCG.cginc"
#include "UnityStandardUtils.cginc"

sampler2D _Buffer;
float2    _BufferSize;

// REPLACE_ORIGINAL + REPLACE_CUSTOM
float4    _ReplaceColor;
sampler2D _ReplaceTexture;
float2    _ReplaceTextureSize;

float2 SnapToPixel(float2 coord, float2 size)
{
	float2 pixel = floor(coord * size);
#ifndef UNITY_HALF_TEXEL_OFFSET
	pixel += 0.5f;
#endif
	return pixel / size;
}

float4 SampleMip0(sampler2D s, float2 coord)
{
	return tex2Dbias(s, float4(coord.x, coord.y, 0, -15.0));
}

float4 PackNormal(float3 v)
{
//#if defined(UNITY_NO_DXT5nm)
	v = v * 0.5f + 0.5f; return float4(v.x, v.y, v.z, 1.0f);
//#else
//	v = v * 0.5f + 0.5f; return float4(v.x, v.y, 0.0f, 1.0f);
//#endif
}

float3 RotateNormal(float3 v, float a)
{
	float s = sin(a); float c = cos(a); return float3(v.x * c - v.y * s, v.x * s + v.y * c, v.z);
}
float4 DoBlend(float4 current, float4 color, float strength, float2 coord, float rot, float4 channels)
{
	float4 old = current;
#if BLEND_MODE_INDEX == 0 // ALPHA_BLEND
	color.a *= strength;
	float str = 1.0f - color.a;
	float div = color.a + current.a * str;

	current.rgb = (color.rgb * color.a + current.rgb * current.a * str) / div;
	current.a   = div;
#elif BLEND_MODE_INDEX == 1 // ALPHA_BLEND_INVERSE
	color.a *= strength;
	float str = 1.0f - current.a;
	float div = current.a + color.a * str;

	current.rgb = (current.rgb * current.a + color.rgb * color.a * str) / div;
	current.a   = div;
#elif BLEND_MODE_INDEX == 2 // PREMULTIPLIED
	color.a *= strength;
	color.rgb *= color.a;
	current = color + (1.0f - color.a) * current;
#elif BLEND_MODE_INDEX == 3 // ADDITIVE
	current += color * strength;
#elif BLEND_MODE_INDEX == 4 // ADDITIVE_SOFT
	current += color * strength * (1.0f - current);
#elif BLEND_MODE_INDEX == 5 // SUBTRACTIVE
	current -= color * strength;
#elif BLEND_MODE_INDEX == 6 // SUBTRACTIVE_SOFT
	current -= color * strength * current;
#elif BLEND_MODE_INDEX == 7 // REPLACE
	current += (color - current) * strength;
#elif BLEND_MODE_INDEX == 8 // REPLACE_ORIGINAL
	float4 rep = SampleMip0(_ReplaceTexture, coord) * _ReplaceColor;
	current += (rep - current) * strength;
#elif BLEND_MODE_INDEX == 9 // REPLACE_CUSTOM
	float4 rep = SampleMip0(_ReplaceTexture, coord) * _ReplaceColor;
	current += (rep - current) * strength;
#elif BLEND_MODE_INDEX == 10 // MULTIPLY_INVERSE_RGB
	//current.rgb *= 1.0f - (1.0f - color.rgb) * strength;
	color.rgb = lerp(color.rgb, float3(1, 1, 1), 1 - color.a * strength);
	current.rgb *= color.rgb;
#elif BLEND_MODE_INDEX == 11 // BLUR
	float2 k = 1.0f / _BufferSize;
	float4 a = SampleMip0(_Buffer, coord + float2(-k.x, 0.0f));
	float4 b = SampleMip0(_Buffer, coord + float2(+k.x, 0.0f));
	float4 c = SampleMip0(_Buffer, coord + float2(0.0f, -k.y));
	float4 d = SampleMip0(_Buffer, coord + float2(0.0f, +k.y));
	current += ((a + b + c + d) * 0.25f - current) * strength;
#elif BLEND_MODE_INDEX == 12 // NORMAL BLEND
	float3 curVec = UnpackNormal(current);
	float3 dstVec = UnpackNormal(color);
	dstVec = RotateNormal(dstVec, rot);
	dstVec = lerp(float3(0.0f, 0.0f, 1.0f), dstVec, strength);
	curVec = normalize(float3(curVec.xy + dstVec.xy, curVec.z * dstVec.z));
	current = PackNormal(curVec);
#elif BLEND_MODE_INDEX == 13 // NORMAL REPLACE
	float3 curVec = UnpackNormal(current);
	float3 dstVec = UnpackNormal(color);
	dstVec = RotateNormal(dstVec, rot);
	curVec = normalize(lerp(curVec, dstVec, strength));
	current = PackNormal(curVec);
#endif
	return old + (current - old) * channels;
}

float4 Blend(float4 color, float strength, float2 coord, float rot, float4 channels)
{
	coord = SnapToPixel(coord, _BufferSize);
	float4 current = SampleMip0(_Buffer, coord);

	return DoBlend(current, color, strength, coord, rot, channels);
}

float4 Blend(float4 color, float strength, float2 coord, float4 channels)
{
	return Blend(color, strength, coord, 0.0f, channels);
}

float4 BlendMinimum(float4 color, float strength, float2 coord, float4 step, float4 channels)
{
	coord = SnapToPixel(coord, _BufferSize);
	float4 current = SampleMip0(_Buffer, coord);
	float4 result  = DoBlend(current, color, 1.0f, coord, 0.0f, channels);
	float4 change  = result - current;
	float4 maximum = abs(change);

	return current + sign(change) * clamp(strength, step, maximum);
}