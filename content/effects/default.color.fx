#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix Matrix;
matrix NearLightMatrix;
matrix FarLightMatrix;

float4 Color;

struct VertexShaderInput
{
	float4 Position : POSITION0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 NearShadowCoord : TEXCOORD0;
	float2 FarShadowCoord : TEXCOORD1;
	float NearShadowDepth : TEXCOORD2;
	float FarShadowDepth : TEXCOORD3;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(input.Position, Matrix);

	float4 nearPos = mul(input.Position, NearLightMatrix);
	output.NearShadowCoord = mad(0.5f, nearPos.xy / nearPos.w, float2(0.5f, 0.5f));
	output.NearShadowCoord.y = 1 - output.NearShadowCoord.y;
	output.NearShadowDepth = nearPos.z / nearPos.w;

	float4 farPos = mul(input.Position, FarLightMatrix);
	output.FarShadowCoord = mad(0.5f, farPos.xy / farPos.w, float2(0.5f, 0.5f));
	output.FarShadowCoord.y = 1 - output.FarShadowCoord.y;
	output.FarShadowDepth = farPos.z / farPos.w;

	//output.ActualDepth = output.Position.z / output.Position.w;

	return output;
}

Texture2D nearShadowTexture;
SamplerState nearShadowSampler
{
	Texture = (nearShadowTexture);
	/*MinFilter = linear;
	MagFilter = linear;
	MipFilter = ;*/
	Filter = Anisotropic;
	MaxAnisotropy = 8;
	AddressU = Wrap;
	AddressV = Wrap;
};

Texture2D farShadowTexture;
SamplerState farShadowSampler
{
	Texture = (farShadowTexture);
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
	//float x = (shadowTexture.Sample(shadowSampler, input.ShadowCoord)).x;
	float inNearShadow = min(min(input.NearShadowCoord.x - 0.01, 0.99 - input.NearShadowCoord.x), min(input.NearShadowCoord.y - 0.01, 0.99 - input.NearShadowCoord.y)) > 0;
	float inFarShadow = min(min(input.FarShadowCoord.x - 0.01, 0.99 - input.FarShadowCoord.x), min(input.FarShadowCoord.y - 0.01, 0.99 - input.FarShadowCoord.y)) > 0;
	
	clip(inFarShadow - 0.5);

	float shadow;

	if (inNearShadow)
	{
		float size = 1 / 512;
		float values[4];
		values[0] = (nearShadowTexture.Sample(nearShadowSampler, input.NearShadowCoord)).x;
		values[1] = (nearShadowTexture.Sample(nearShadowSampler, input.NearShadowCoord + float2(size, 0))).x;
		values[2] = (nearShadowTexture.Sample(nearShadowSampler, input.NearShadowCoord + float2(0, size))).x;
		values[3] = (nearShadowTexture.Sample(nearShadowSampler, input.NearShadowCoord + float2(size, size))).x;

		float _middle = (values[0] + values[1] + values[2] + values[3]) / 4;

		values[0] = values[0] + 0.001 < input.NearShadowDepth;
		values[1] = values[1] + 0.001 < input.NearShadowDepth;
		values[2] = values[2] + 0.001 < input.NearShadowDepth;
		values[3] = values[3] + 0.001 < input.NearShadowDepth;

		float allShadow = values[0] * values[1] * values[2] * values[3];
		float allNotShadow = (1 - values[0]) * (1 - values[1]) * (1 - values[2]) * (1 - values[3]);
		float shadowCount = values[0] + values[1] + values[2] + values[3];

		shadow = allShadow * max(-0.4, (_middle - input.NearShadowDepth) * 10);// +allNotShadow * min(0.4, (input.Depth - _middle) * 10);
	}
	else if (inFarShadow)
	{
		float size = 1 / 512;
		float values[4];
		values[0] = (farShadowTexture.Sample(farShadowSampler, input.FarShadowCoord)).x;
		values[1] = (farShadowTexture.Sample(farShadowSampler, input.FarShadowCoord + float2(size, 0))).x;
		values[2] = (farShadowTexture.Sample(farShadowSampler, input.FarShadowCoord + float2(0, size))).x;
		values[3] = (farShadowTexture.Sample(farShadowSampler, input.FarShadowCoord + float2(size, size))).x;

		float _middle = (values[0] + values[1] + values[2] + values[3]) / 4;

		values[0] = values[0] + 0.001 < input.FarShadowDepth;
		values[1] = values[1] + 0.001 < input.FarShadowDepth;
		values[2] = values[2] + 0.001 < input.FarShadowDepth;
		values[3] = values[3] + 0.001 < input.FarShadowDepth;

		float allShadow = values[0] * values[1] * values[2] * values[3];
		float allNotShadow = (1 - values[0]) * (1 - values[1]) * (1 - values[2]) * (1 - values[3]);
		float shadowCount = values[0] + values[1] + values[2] + values[3];

		shadow = allShadow * max(-0.4, (_middle - input.FarShadowDepth) * 10);// +allNotShadow * min(0.4, (input.Depth - _middle) * 10);
	}

    return float4(Color.x + shadow, Color.y + shadow, Color.z + shadow, 1);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};