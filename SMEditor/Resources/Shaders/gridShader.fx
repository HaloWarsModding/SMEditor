﻿cbuffer cbMatrices : register(b0) {
	float4x4 mvp;
}

struct vs_input
{
	float3 position_in : POSITION_IN;
	float3 color_in : COLOR_IN;
};

struct vs_output
{
	float4 position_out : SV_POSITION;
	float3 color_out : COLOR_OUT;
};

vs_output vs(vs_input input)
{
	vs_output output;
	output.position_out = mul(float4(input.position_in, 1), mvp);
	output.color_out = input.color_in;
	return output;
}

float4 ps(vs_output output) : SV_TARGET
{
	return float4(output.color_out, 1.0f);
}