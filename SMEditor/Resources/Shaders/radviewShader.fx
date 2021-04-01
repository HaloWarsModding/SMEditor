cbuffer cbMatrices : register(b0) {
	float4x4 m;
	float4x4 v;
	float4x4 p;
}
cbuffer colorBuffer : register(b3)
{
	float4 radiusColor;
}
SamplerState samplerState
{
	Filter = ANISOTROPIC;
	MaxAnisotropy = 4;

	AddressU = WRAP;
	AddressV = WRAP;
	AddressW = WRAP;
};
Texture2D depthTex;


struct vs_input
{
	float3 pos : POSITION_IN;
};
struct ps_input
{
	float4 pos : SV_POSITION;
	float2 screenPos : SCR_POS;
};

ps_input vs(vs_input input)
{
	ps_input output;
	float4x4 mv = mul(m, v);
	float4x4 mvp = mul(mv, p);

	float4 finalPos = mul(float4(input.pos, 1), mvp);
	output.pos = finalPos;
	output.screenPos = mul(float4(input.pos, 1), m);
	return output;
}

float4 ps(ps_input input) : SV_TARGET {

float4 finalColor;
float depthValue = input.pos.z / input.pos.w;

if (depthValue < 0.9f)
{
	finalColor = float4(1.0, 0.0f, 0.0f, 1.0f);
}
if (depthValue > 0.9f)
{
	finalColor = float4(0.0, 1.0f, 0.0f, 1.0f);
}
if (depthValue > 0.925f)
{
	finalColor = float4(0.0, 0.0f, 1.0f, 1.0f);
}

float4 depth = depthTex.Sample(samplerState, input.screenPos);
float lineardepth = (2.0f * .1f) / (1000.0f + .1f - depth.r * (1000.0f - .1f));

finalColor = float4(lineardepth.r, 0, 0, 1.0f);

return finalColor;
}
