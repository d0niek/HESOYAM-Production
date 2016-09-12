Texture2D inputTexture;
float time;

float4 TripShader(float4 coords: VPOS) : COLOR0
{
	uint2 pos_xy = { coords.x, coords.y };
	pos_xy.y += sin(((float)pos_xy.x + (time)) / 2.21) * 7;
	pos_xy.x += sin(((float)pos_xy.y + (time)) / 2.234) * 4;
	float4 color = inputTexture.mips[0][pos_xy];
	color.x += sin(((float)pos_xy.x + (time)) / 1.2313) * 0.1;
	color.y += cos(((float)pos_xy.y + (time)) / 2.852) * 0.1;
	color.z += sin(((float)pos_xy.y + (time)) / 1.65) * 0.1 * cos(((float)pos_xy.x + (time)) / 3) * 0.1;
	color.x *= sin(((float)pos_xy.x + (time * 500)) / 1.543) + 1;
	color.y *= cos(((float)pos_xy.y + (time * 100)) / 2.852) + 1;
	color.z *= sin(((float)pos_xy.y + (time * 215)) / 1.232) + 1;
	return color;
}

technique Textured
{
	pass Pass1
	{
		PixelShader = compile ps_4_0 TripShader();
	}
}