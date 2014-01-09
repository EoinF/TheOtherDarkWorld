float br00;
float br01;
float br10;
float br11;
bool vis00;
bool vis01;
bool vis10;
bool vis11;
float4 lightcolor00;
float4 lightcolor01;
float4 lightcolor10;
float4 lightcolor11;

sampler s0;

float4 PS_RegularLighting(float4 color : COLOR0, float2 coords: TEXCOORD0) : COLOR0
{
	float4 pcolor = tex2D(s0, coords) * color;
	float brightness = br00;

	return lightcolor00 * float4(pcolor.r * brightness, pcolor.g * brightness, pcolor.b * brightness, pcolor.a);
}

float4 PS_TileLighting(float4 color : COLOR0, float2 coords: TEXCOORD0) : COLOR0
{
	float4 pcolor = tex2D(s0, coords) * color;

	float4 light;
	if (coords.x < 0.5)
	{
		if (coords.y < 0.5) //Top left corner
		{
			if (!vis00)
				return float4(0,0,0,0);
			light = br00 * lightcolor00;
			light.a = lightcolor00.a;
		}
		else //bottom left corner
		{
			if (!vis01)
				return float4(0,0,0,0);
			light = br01 * lightcolor01;
			light.a = lightcolor01.a;
		}
	}
	else
	{
		if (coords.y < 0.5)//Top right corner
		{
			if (!vis10)
				return float4(0,0,0,0);
			light = br10 * lightcolor10;
			light.a = lightcolor10.a;
		}
		else //bottom right corner
		{
			if (!vis11)
				return float4(0,0,0,0);
			light = br11 * lightcolor11;
			light.a = lightcolor11.a;
		}
	}

	return  float4(pcolor.r * light.r, pcolor.g * light.g, pcolor.b * light.b, pcolor.a * light.a);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PS_TileLighting();
    }
    pass Pass2
    {
        PixelShader = compile ps_2_0 PS_RegularLighting();
    }
}
