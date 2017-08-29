﻿#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix Matrix;
matrix LightMatrix;

struct VertexShaderInput
{
	float4 Position : POSITION;
	float4 Color : COLOR;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 ShadowCoord : TEXCOORD0;
	float Depth : TEXCOORD1;
	float4 Color : COLOR0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(input.Position, Matrix);

	float4 lightPixelPosition = mul(input.Position, LightMatrix);
	output.ShadowCoord = mad(0.5f, lightPixelPosition.xy / lightPixelPosition.w, float2(0.5f, 0.5f));
	output.ShadowCoord.y = 1 - output.ShadowCoord.y;

	output.Depth = lightPixelPosition.z / lightPixelPosition.w;
	output.Color = input.Color;

	return output;
}

Texture2D shadowTexture;
SamplerState shadowSampler
{
	Texture = (shadowTexture);
	/*MinFilter = linear;
	MagFilter = linear;
	MipFilter = ;*/
	Filter = Anisotropic;
	MaxAnisotropy = 8;
	AddressU = Wrap;
	AddressV = Wrap;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float size = 1 / 4096;
	float values[4];
	values[0] = (shadowTexture.Sample(shadowSampler, input.ShadowCoord)).x;
	values[1] = (shadowTexture.Sample(shadowSampler, input.ShadowCoord + float2(size, 0))).x;
	values[2] = (shadowTexture.Sample(shadowSampler, input.ShadowCoord + float2(0, size))).x;
	values[3] = (shadowTexture.Sample(shadowSampler, input.ShadowCoord + float2(size, size))).x;

	float _middle = (values[0] + values[1] + values[2] + values[3]) / 4;

	values[0] = values[0] + 0.001 < input.Depth;
	values[1] = values[1] + 0.001 < input.Depth;
	values[2] = values[2] + 0.001 < input.Depth;
	values[3] = values[3] + 0.001 < input.Depth;

	float allShadow = values[0] * values[1] * values[2] * values[3];
	float allNotShadow = (1 - values[0]) * (1 - values[1]) * (1 - values[2]) * (1 - values[3]);

	float shadow = allShadow * max(-0.4, (_middle - input.Depth) * 10);// +allNotShadow * min(0.4, (input.Depth - _middle) * 10);
	//if (abs(_middle - input.Depth) < 0.02)
		//shadow = 0;

	return float4(input.Color.x + shadow, input.Color.y + shadow, input.Color.z + shadow, 1);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};