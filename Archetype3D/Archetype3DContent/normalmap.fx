// Effect applies normalmapped lighting to a 2D sprite.
float4x4 MatrixTransform;

uniform float3 entityPos3D;
uniform float3 topLeft3D;
float3 LightPosition;
float3 ObjectPosition;
float3 AmbientColor = 0.2;
float ObjectHeight;

int textureWidth;
int textureHeight;

sampler TextureSampler : register(s0);
sampler NormalSampler : register(s1);
sampler HeightSampler : register(s2);

struct VertexToPixel
{
	float4 Position		: POSITION;
	float2 texCoord		: TEXCOORD0;
	float3 normalCoord	: TEXCOORD1;
	float4 screenCoord	: TEXCOORD2;
};

struct PS_OUTPUT
{
	float4 Color	: COLOR0;
	float4 Normal	: COLOR1;
	float4 Height	: COLOR2;
	float4 Depth3D	: COLOR3;
	float Depth		: DEPTH;
};


//

VertexToPixel SpriteVertexShader(
	float4 inPos: POSITION0,
	//float3 inNormal: NORMAL0,
	float2 inTexCoords: TEXCOORD0
	)
{
	VertexToPixel Output = (VertexToPixel)0;
	
	Output.Position = mul(inPos, MatrixTransform);
	
	Output.screenCoord = Output.Position;
	Output.normalCoord = 0;

	Output.texCoord = inTexCoords;

	return Output;
}

float DotProduct(float3 lightPos, float3 pos3D, float3 normal)
{
    float3 lightDir = normalize(pos3D - lightPos);
    return dot(-lightDir, normal);    
}

PS_OUTPUT SpritePixelShader(VertexToPixel PSIn)
{
	PS_OUTPUT Output = (PS_OUTPUT)0;

	float4 colormap = tex2D(TextureSampler, PSIn.texCoord);
	float4 normalmap = tex2D(NormalSampler, PSIn.texCoord);
	float4 heightmap = tex2D(HeightSampler, PSIn.texCoord);

	//DEFINE LIGHT RADIUS
	float LightRadius = 60.0;

	//DEFINE LIGHT INTENSITY
	float LightIntensity = 1.0;

	//DEFINE LIGHT COLOR
	float3 LightColor = 1.5;

	//screenCoord = position3d

	float diffuseLightingFactor = DotProduct(LightPosition, PSIn.screenCoord, normalmap);
	diffuseLightingFactor = saturate(diffuseLightingFactor);
	diffuseLightingFactor *= LightIntensity;



	Output.Color.rgb = colormap * (diffuseLightingFactor + AmbientColor);
	Output.Color.a = 1.0;
	Output.Normal.xyzw = normalmap;

	Output.Height = heightmap;

	Output.Depth3D = Output.Height.r;
	//Output.Depth3D.a = 1.0;
	//Output.Depth3D = saturate(tex2D(HeightSampler, PSIn.texCoord).r);
	Output.Depth = 1 - Output.Height.r;

	return Output;
}

float4 nm_output(float4 color: COLOR0, float2 texCoord : TEXCOORD0) : COLOR
{
	float4 normal = tex2D(NormalSampler, texCoord);
	normal.a = 1;
	return normal;
}
float4 height_output(float4 color: COLOR0, float2 texCoord : TEXCOORD0) : COLOR
{
	float4 height = tex2D(HeightSampler, texCoord);
	color.rgb = height.rgb;
	color.a = height.r;
	return color;
}

/*
PS_OUTPUT main(float4 color : COLOR0, float2 texCoord : TEXCOORD0)
{
	PS_OUTPUT output;
	//DEFINE LIGHT RADIUS
	float LightRadius = 250.0;

	//DEFINE LIGHT INTENSITY
	float LightIntensity = 10.0;

	//DEFINE LIGHT COLOR
	float3 LightColor = 1.5;
	//LightColor.b *= 5;

	// Look up the texture, normalmap and heightmap values.
    float4 tex = tex2D(TextureSampler, texCoord);
    float3 normal = tex2D(NormalSampler, texCoord);
	float4 height = tex2D(HeightSampler, texCoord);

	//get 3d position of current pixel
	float3 pixelPosition;

	//base is world space center front object pos
	pixelPosition = entityPos3D;

	//x is current pixel * textureWidth
	pixelPosition.x = texCoord.x + 395/2;

	//z is the white value of the heightmap * objectHeight
	pixelPosition.z = height.y * ObjectHeight;// * 5.12;

	pixelPosition.y = texCoord.y/2;

    //surface-to-light vector
	//LightPosition and pixelPosition should be in world coordinates now
    float3 lightVector = LightPosition - pixelPosition;

    //compute attenuation based on distance - linear attenuation
    float attenuation = saturate(1.0f - length(lightVector)/LightRadius); 
    
	//normalize light vector
    lightVector = normalize(lightVector); 
	
    // Compute lighting.
    float lightAmount = max(0, dot(normal, lightVector));

    color.rgb *= AmbientColor + pow((attenuation * LightIntensity * lightAmount) * LightColor, 4);
    //return tex * attenuation * LightIntensity * color;

    output.Color = tex * color;
	output.Depth = height.r;
	return output;
}
*/

technique Normalmap
{
    pass Pass1
    {
		//CullMode = NONE;
		ZEnable = TRUE;
		ZWriteEnable = TRUE;
		AlphaBlendEnable = TRUE;
		VertexShader = compile vs_2_0 SpriteVertexShader();
		PixelShader = compile ps_2_0 SpritePixelShader();
    }
}

technique nm_only
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 SpriteVertexShader();
		PixelShader = compile ps_2_0 nm_output();
	}
	pass Pass1
	{
		VertexShader = compile vs_2_0 SpriteVertexShader();
		PixelShader = compile ps_2_0 height_output();
	}
}

/*
float2 computeParallaxOffset()
{
	const float2 screenSpacePos = float2(entityPos3D.x, entityPos3D.y)-cameraPos;
	return ((screenSpacePos-parallaxOrigin)/screenSize.x)*entityPos3D.z*parallaxIntensity;
}

SPRITE_TRANSFORM transformSprite_ppl(float3 position)
{
	SPRITE_TRANSFORM r;
	float4 newPos = float4(position, 1);
	newPos = newPos * float4(size,1,1) - float4(center, 0, 0);
	newPos = mul(rotationMatrix, newPos)+float4(entityPos,0,0);
	r.vertPos = newPos.xy/screenSize;

	// project the vertex on the screen
	newPos -= float4(screenSize/2,0,0)+float4(cameraPos,0,0);
	newPos += float4(computeParallaxOffset(),0,0);
	newPos *= float4(1,-1,1,1);
	r.position = mul(viewMatrix, newPos);
	return r;
}

void verticalSprite_ppl(float3 position : POSITION,
				float2 texCoord : TEXCOORD0,
				out float4 oPosition : POSITION,
				out float4 oColor    : COLOR0,
				out float2 oTexCoord : TEXCOORD0,
				out float3 oVertPos3D : TEXCOORD1,
				uniform float3 topLeft3DPos,
				uniform float spaceLength)
{
	//SPRITE_TRANSFORM transform = transformSprite_ppl(position);

	transform.position.z = (1-depth) - (((1-position.y)*rectSize.y)/spaceLength);

	oPosition = transform.position;
	oVertPos3D = topLeft3DPos + (float3(position.x,0,position.y)*float3(size.x,0,-size.y));
	oTexCoord = transformCoord(texCoord);
	oColor = color0;
}



// returns the texture coordinate according to the rect
float2 transformCoord(float2 texCoord)
{
	float2 newCoord = texCoord * (rectSize/bitmapSize);
	newCoord += (rectPos/bitmapSize);
	return (newCoord+(scroll/bitmapSize))*multiply;
}
*/