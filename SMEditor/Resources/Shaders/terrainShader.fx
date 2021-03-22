cbuffer cbMatrices : register(b0) {
	float4x4 m;
	float4x4 v;
	float4x4 p;
}

struct vs_input
{
	float3 pos : POSITION_IN;
	float3 normal : NORMAL_IN;
};

struct gsps_input
{
	float4 pos : SV_POSITION;
	float4 color : COLOR_GSPS;
	float3 normal : NORMAL_GSPS;
};

gsps_input vs(vs_input input)
{
	gsps_input output;
	float4x4 mv = mul(m, v);
	float4x4 mvp = mul(mv, p);
	output.pos = mul(float4(input.pos, 1), mvp);
	output.normal = normalize(mul(input.normal, m));
	return output;
}

[maxvertexcount(12)]
void gs(triangle gsps_input input[3] : SV_POSITION, inout TriangleStream<gsps_input> tris)
{
	gsps_input output;

	for (int i = 0; i < 3; i++)
	{
		output.pos = input[i].pos;
		output.color = float4(100/255.0F, 149/255.0F, 237/255.0F, 1);
		output.normal = input[i].normal;
		tris.Append(output);
	}
	tris.RestartStrip();

}
[maxvertexcount(12)]
void gsVert(point gsps_input input[1] : SV_POSITION, inout PointStream<gsps_input> tris)
{
	gsps_input output;

	output.pos = input[0].pos + float4(0, .01F, 0, 0);
	output.color = float4(0, 0, 0, 1);
	tris.Append(output);

	tris.RestartStrip();
}

float4 ps(gsps_input input) : SV_TARGET
{
	float4 finalColor;
	float3 lightDir = -normalize(float3(0,-1,1));
	float lightIntensity = saturate(dot(input.normal, lightDir));

	finalColor = input.color * lightIntensity;

	return finalColor;
}

