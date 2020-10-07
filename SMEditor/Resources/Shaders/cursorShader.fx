cbuffer cbMatrices : register(b0) {
	float4x4 m;
	float4x4 v;
	float4x4 p;
}

struct vs_input
{
	float3 pos : POSITION_IN;
	float3 color_in : COLOR_IN;
};

struct vs_output
{
	float4 pos : SV_POSITION;
	float3 color_out : COLOR_OUT;
};

vs_output vs(vs_input input)
{
	vs_output output;
	float4x4 mv = mul(m, v);
	float4x4 mvp = mul(mv, p);
	output.pos = mul(float4(input.pos, 1), mvp);
	output.color_out = input.color_in;
	return output;
}

[maxvertexcount(12)]
void gs(triangle vs_output input[3] : SV_POSITION, inout TriangleStream<vs_output> tris)
{
	vs_output output;

	for (int i = 0; i < 3; i++)
	{
		output.pos = input[i].pos;
		output.color_out = float4(.4F, .4F, .4F, 1);
		tris.Append(output);
	}
	tris.RestartStrip();

}

float4 ps(vs_output output) : SV_TARGET
{
	return float4(output.color_out, 1.0f);
}
