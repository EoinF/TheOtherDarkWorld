
sampler s0;
texture lightMask;
sampler lightSampler = sampler_state{Texture = lightMask;
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;};
  
float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0
{  
    float4 color = tex2D(s0, coords);
    float4 lightColor = tex2D(lightSampler, coords);
	if (color.a == 0)
		return color * 0;
    return lightColor * color;
}  

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 DistortPS();
    }
    pass Pass2
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

float4 DistortPS(float2 TexCoord : TEXCOORD0) : COLOR0
{
//translate u and v into [-1 , 1] domain
float u0 = TexCoord.x * 2 - 1;
float v0 = TexCoord.y * 2 - 1;
 
//then, as u0 approaches 0 (the center), v should also approach 0
v0 = v0 * abs(u0);
//convert back from [-1,1] domain to [0,1] domain
v0 = (v0 + 1) / 2;
//we now have the coordinates for reading from the initial image
float2 newCoords = float2(TexCoord.x, v0);
 
//read for both horizontal and vertical direction and store them in separate channels
 
float horizontal = tex2D(inputSampler, newCoords).r;
float vertical = tex2D(inputSampler, newCoords.yx).r;
return float4(horizontal,vertical ,0,1);
}

float4 HorizontalReductionPS(float2 TexCoord : TEXCOORD0) : COLOR0
{
float2 color = tex2D(inputSampler, TexCoord);
float2 colorR = tex2D(inputSampler, TexCoord + float2(TextureDimensions.x,0));
float2 result = min(color,colorR);
return float4(result,0,1);
}

float4 DrawShadowsPS(float2 TexCoord: TEXCOORD0) : COLOR0
{
// distance of this pixel from the center
float distance = length(TexCoord - 0.5f);
distance *= renderTargetSize.x;
 
//apply a 2-pixel bias
distance -=2;
 
//distance stored in the shadow map
float shadowMapDistance;
//coords in [-1,1]
float nY = 2.0f*( TexCoord.y - 0.5f);
float nX = 2.0f*( TexCoord.x - 0.5f);
 
//we use these to determine which quadrant we are in
if(abs(nY)&lt;abs(nX))
{
shadowMapDistance = GetShadowDistanceH(TexCoor
}
else
{
shadowMapDistance = GetShadowDistanceV(TexCoord);
}
 
//if distance to this pixel is lower than distance from shadowMap,
//then we are not in shadow
float light = distance < shadowMapDistance ? 1:0;
float4 result = light;
result.b = length(TexCoord - 0.5f);
result.a = 1;
return result;
}

float GetShadowDistanceH(float2 TexCoord)
{
 float u = TexCoord.x;
 float v = TexCoord.y;
 
 u = abs(u-0.5f) * 2;
 v = v * 2 - 1;
 float v0 = v/u;
 v0 = (v0 + 1) / 2;
 
 float2 newCoords = float2(TexCoord.x,v0);
 //horizontal info was stored in the Red component
 return tex2D(shadowMapSampler, newCoords).r;
}
 
float GetShadowDistanceV(float2 TexCoord)
{
 float u = TexCoord.y;
 float v = TexCoord.x;
 
 u = abs(u-0.5f) * 2;
 v = v * 2 - 1;
 float v0 = v/u;
 v0 = (v0 + 1) / 2;
 
 float2 newCoords = float2(TexCoord.y,v0);
 //vertical info was stored in the Green component
 return tex2D(shadowMapSampler, newCoords).g;
}