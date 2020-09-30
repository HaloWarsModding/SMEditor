cbuffer cbMatrices : register( b0 ) {
	float4x4 mvp;
}


struct vs_input
{
	float3 position_in : POSITION_IN;
};

struct vs_output
{
	float4 position_out : SV_POSITION;
};

vs_output vs(vs_input input)
{
	vs_output output;
	output.position_out =  mul(float4(input.position_in, 1), mvp);
	return output;
}

float4 ps(vs_output input) : SV_TARGET
{
	return float4(1.0f, 1.0f, 0.0f, 1.0f);
}
