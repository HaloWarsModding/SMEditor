cbuffer cbMatrices : register(b0) {
	float4x4 m;
	float4x4 v;
	float4x4 p;
}

struct vs_input
{
	float3 position_in : POSITION_IN;
	float3 color_in : COLOR_IN;
	float3 location : INST_POS;
};

struct vs_output
{
	float4 position_out : SV_POSITION;
	float3 color_out : COLOR_OUT;
};

vs_output vs(vs_input input)
{
	vs_output output;
	float4x4 mN =
	{
		m._m00, m._m01, m._m02, m._m03,
		m._m10, m._m11, m._m12, m._m13,
		m._m20, m._m21, m._m22, m._m23,
		m._m30 + input.location.x, m._m31 + input.location.y, m._m32 + input.location.z, m._m33
	};	
	float4x4 mv = mul(mN, v);
	float4x4 mvp = mul(mv, p);
	output.position_out = mul(float4(input.position_in, 1), mvp);
	output.color_out = input.color_in;
	return output;
}

float4 ps(vs_output output) : SV_TARGET
{
	return float4(output.color_out, 1.0F);
}
