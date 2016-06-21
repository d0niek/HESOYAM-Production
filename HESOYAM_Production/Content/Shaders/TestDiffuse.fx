#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;
float4 DiffuseColor = float4(1, 1, 1, 1);
float4 AmbientColor = float4(1, 1, 1, 1);
float3 DiffuseLightDirection = normalize(float3(1, 0, 0));
float AmbientIntensity = 0.7;
float DiffuseIntensity = 1.0;
float4x4 Wit;

struct VertexShaderInput
{
    float4 Position : SV_POSITION;    
    float4 Normal : NORMAL0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    float4 normal = mul(input.Normal, Wit);
    float lightIntensity = dot(normal, DiffuseLightDirection);
    output.Color = saturate(DiffuseColor * DiffuseIntensity * lightIntensity);

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
    return saturate(input.Color + AmbientColor * AmbientIntensity);
}

technique Ambient
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}
