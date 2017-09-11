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

float AnimationValue;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TextureCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 NearShadowCoord : TEXCOORD0;
	float2 FarShadowCoord : TEXCOORD1;
	float NearShadowDepth : TEXCOORD2;
	float FarShadowDepth : TEXCOORD3;
	float2 TextureCoord : TEXCOORD4;
	float ActualDepth : TEXCOORD5;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	if (input.TextureCoord.x >= 0.5)
		output.Position = float4(input.Position.x, input.Position.y + 0.5 * 0.125 * sin(input.Position.x * 1.57079633 /* magic number. something with 16 and pi */ + AnimationValue) + 0.5 * 0.125 * cos(input.Position.z * 1.17809725 /* another magic number. something with 16 and pi */ + AnimationValue * 0.23719372 /* third magic number. just random. not a fraction of 1 */), input.Position.z, input.Position.w);
	else
		output.Position = input.Position;
	output.Position = mul(output.Position, Matrix);

	float4 nearPos = mul(input.Position, NearLightMatrix);
	output.NearShadowCoord = mad(0.5f, nearPos.xy / nearPos.w, float2(0.5f, 0.5f));
	output.NearShadowCoord.y = 1 - output.NearShadowCoord.y;
	output.NearShadowDepth = nearPos.z / nearPos.w;

	float4 farPos = mul(input.Position, FarLightMatrix);
	output.FarShadowCoord = mad(0.5f, farPos.xy / farPos.w, float2(0.5f, 0.5f));
	output.FarShadowCoord.y = 1 - output.FarShadowCoord.y;
	output.FarShadowDepth = farPos.z / farPos.w;

	output.TextureCoord = input.TextureCoord;
	output.ActualDepth = output.Position.z / output.Position.w;

	return output;
}

Texture2D nearShadowTexture;
SamplerState nearShadowSampler
{
	Texture = (nearShadowTexture);
	MinFilter = linear;
	MagFilter = linear;
	//MipFilter = ;*/
	//Filter = Anisotropic;
	//MaxAnisotropy = 8;
	AddressU = Wrap;
	AddressV = Wrap;
};

Texture2D farShadowTexture;
SamplerState farShadowSampler
{
	Texture = (farShadowTexture);
	MinFilter = linear;
	MagFilter = linear;
	//MipFilter = ;*/
	//Filter = Anisotropic;
	//MaxAnisotropy = 8;
	AddressU = Wrap;
	AddressV = Wrap;
};

Texture2D otherTexture;
SamplerState textureSampler
{
	Texture = (otherTexture);
	MinFilter = linear;
	MagFilter = linear;
	//MipFilter = 
	/*Filter = Anisotropic;
	MaxAnisotropy = 8;*/
	AddressU = Wrap;
	AddressV = Wrap;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	//float x = (shadowTexture.Sample(shadowSampler, input.ShadowCoord)).x;
	float inNearShadow = min(min(input.NearShadowCoord.x - 0.01, 0.99 - input.NearShadowCoord.x), min(input.NearShadowCoord.y - 0.01, 0.99 - input.NearShadowCoord.y)) > 0;
	float inFarShadow = min(min(input.FarShadowCoord.x - 0.01, 0.99 - input.FarShadowCoord.x), min(input.FarShadowCoord.y - 0.01, 0.99 - input.FarShadowCoord.y)) > 0;

	clip(inFarShadow + inNearShadow - 0.5);

	float shadow = 0;

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
	//return float4(shadowTexture.Sample(shadowSampler, input.Color.xy).x, input.Depth, 0, 1);

	if (input.ActualDepth < 0.9)
		input.TextureCoord.y = input.TextureCoord.y * 2 / 3.0;
	else if (input.ActualDepth < 0.95)
	{
		input.TextureCoord.y = input.TextureCoord.y / 3.0 + 2 / 3.0;
		input.TextureCoord.x = input.TextureCoord.x / 2.0;
	}
	else if (input.ActualDepth < 0.975)
	{
		input.TextureCoord.y = input.TextureCoord.y / 6.0 + 2 / 3.0;
		input.TextureCoord.x = input.TextureCoord.x / 4 + 0.5;
	}
	else
	{
		input.TextureCoord.y = input.TextureCoord.y / 12.0 + 2 / 3.0;
		input.TextureCoord.x = input.TextureCoord.x / 8.0 + 0.75;
	}


	float4 a = otherTexture.Sample(textureSampler, input.TextureCoord);
    return float4((a.x + shadow),
	              (a.y + shadow),
                  (a.z + shadow), 1);
	//return float4(shadowTexture.Sample(shadowSampler, input.Color.xy).x, input.Depth, 0, 1);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
