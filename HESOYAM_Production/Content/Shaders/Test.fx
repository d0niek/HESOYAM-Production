float4x4 World;
float4x4 View;
float4x4 Projection;

float3 lightDirection = normalize(float3(-1, 1, 0));
float4 ambient = float4(0.2, 0.2, 0.2, 1);

Texture2D ModelTexture;
Texture2D BumpMap;

SamplerState textureSampler
{
	Filter = Point;
	AddressU = Wrap;
	AddressV = Wrap;
};

struct VertexShaderInput
{
	float4 Position : POSITION;
	float3 Normal : NORMAL;
	float2 TextureUV : TEXCOORD;
};

struct PixelShaderInput
{
	float4 Position : POSITION;
	float3 Normal : NORMAL;
	float2 TextureUV : TEXCOORD;
};

PixelShaderInput VertexShaderFunction(VertexShaderInput input)
{
	PixelShaderInput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.Normal = mul(input.Normal, World);
	output.TextureUV = input.TextureUV;

	return output;
}

float4 PixelShaderFunction(PixelShaderInput input) : COLOR
{
	float4 textureColor = ModelTexture.Sample(textureSampler, input.TextureUV);
	float4 bump = BumpMap.Sample(textureSampler, input.TextureUV);
	float3 bumpedNormal = input.Normal + bump;
	bumpedNormal = normalize(bumpedNormal);
	float3 i = mul(bumpedNormal, lightDirection);
	float4 lightMultiplier = float4(i.x, i.y, i.z, 1);
	return saturate((textureColor * lightMultiplier) + (textureColor * ambient));
}

technique Textured
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunction();
	}
}