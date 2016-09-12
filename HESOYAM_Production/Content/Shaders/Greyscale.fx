Texture2D inputTexture;

float4 GreyscaleShader(float4 coords: VPOS) : COLOR0
{
	uint2 pos_xy = { coords.x, coords.y };
	float4 color = inputTexture.mips[0][pos_xy];
	float maxColor = max(max(color.x, color.y), color.z);
	return float4(maxColor, maxColor, maxColor, 1);
}

technique Textured
{
	pass Pass1
	{
		PixelShader = compile ps_4_0 GreyscaleShader();
	}
}