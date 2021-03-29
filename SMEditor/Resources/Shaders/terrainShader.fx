cbuffer cbMatrices : register(b0) {
	float4x4 m;
	float4x4 v;
	float4x4 p;
}

cbuffer alphasBuffer : register (b1)
{
	uint4 alphas[64 * 64];
};


Texture2DArray tex;
SamplerState samplerState
{
	Filter = ANISOTROPIC;
	MaxAnisotropy = 4;

	AddressU = WRAP;
	AddressV = WRAP;
	AddressW = WRAP;
};

struct vs_input
{
	float3 pos : POSITION_IN;
	float2 uv : UV_IN;
	float3 normal : NORMAL_IN;
	int vindex : INDEX_IN;
};

struct gsps_input
{
	float4 pos : SV_POSITION;
	float4 color : COLOR_GSPS;
	float3 normal : NORMAL_GSPS;
	float2 uv : UV_GSPS;
	int vindex : INDEX_GSPS;
};

gsps_input vs(vs_input input)
{
	gsps_input output;
	float4x4 mv = mul(m, v);
	float4x4 mvp = mul(mv, p);
	output.pos = mul(float4(input.pos, 1), mvp);
	output.normal = normalize(mul(input.normal, m));
	output.uv = input.uv;
	output.vindex = input.vindex;
	return output;
}

//[maxvertexcount(12)]
//void gs(triangle gsps_input input[3] : SV_POSITION, inout TriangleStream<gsps_input> tris)
//{
//	gsps_input output;
//
//	for (int i = 0; i < 3; i++)
//	{
//		output.pos = input[i].pos;
//		if (i == 0)output.color = float4(100/255.0F, 149/255.0F, 237/255.0F, 1); //cornflower blue
//		if (i == 1)output.color = float4(100 / 255.0F, 255 / 255.0F, 255 / 255.0F, 1); //cornflower blue
//		if (i == 2)output.color = float4(255 / 255.0F, 149 / 255.0F, 237 / 255.0F, 1); //cornflower blue
//		output.normal = input[i].normal;
//		output.uv = input[i].uv;
//		output.vindex = input[i].vindex;
//		tris.Append(output);
//	}
//	tris.RestartStrip();
//
//}

float4 BlendTexture(float4 A, float4 B)
{
	float4 C;
	C.a = A.a + (1 - A.a) * B.a;
	C.rgb = (1 / C.a) * (A.a * A.rgb + (1 - A.a) * B.a * B.rgb);
	return C;
}
float4 GetTexture(gsps_input input)
{
	float4 finalColor;

	float4 a = tex.Sample(samplerState, float3(input.uv, 0));
	a.a = (((alphas[input.vindex].x) >> 0) & 0xFF) / 255.f;
	float4 b = tex.Sample(samplerState, float3(input.uv, 1));
	b.a = (((alphas[input.vindex].x) >> 8) & 0xFF) / 255.f;
	finalColor = BlendTexture(a, b);

	float4 next;
	next = tex.Sample(samplerState, float3(input.uv, 2));
	next.a = (((alphas[input.vindex].x) >> 16) & 0xFF) / 255.f;
	finalColor = BlendTexture(finalColor, next);

	next = tex.Sample(samplerState, float3(input.uv, 3));
	next.a = (((alphas[input.vindex].x) >> 24) & 0xFF) / 255.f;
	finalColor = BlendTexture(finalColor, next);



	next = tex.Sample(samplerState, float3(input.uv, 4));
	next.a = (((alphas[input.vindex].y) >> 0) & 0xFF) / 255.f;
	finalColor = BlendTexture(finalColor, next);

	next = tex.Sample(samplerState, float3(input.uv, 5));
	next.a = (((alphas[input.vindex].y) >> 8) & 0xFF) / 255.f;
	finalColor = BlendTexture(finalColor, next);

	next = tex.Sample(samplerState, float3(input.uv, 6));
	next.a = (((alphas[input.vindex].y) >> 16) & 0xFF) / 255.f;
	finalColor = BlendTexture(finalColor, next);

	next = tex.Sample(samplerState, float3(input.uv, 7));
	next.a = (((alphas[input.vindex].y) >> 24) & 0xFF) / 255.f;
	finalColor = BlendTexture(finalColor, next);



	next = tex.Sample(samplerState, float3(input.uv, 8));
	next.a = (((alphas[input.vindex].z) >> 0) & 0xFF) / 255.f;
	finalColor = BlendTexture(finalColor, next);

	next = tex.Sample(samplerState, float3(input.uv, 9));
	next.a = (((alphas[input.vindex].z) >> 8) & 0xFF) / 255.f;
	finalColor = BlendTexture(finalColor, next);

	next = tex.Sample(samplerState, float3(input.uv, 10));
	next.a = (((alphas[input.vindex].z) >> 16) & 0xFF) / 255.f;
	finalColor = BlendTexture(finalColor, next);

	next = tex.Sample(samplerState, float3(input.uv, 11));
	next.a = (((alphas[input.vindex].z) >> 24) & 0xFF) / 255.f;
	finalColor = BlendTexture(finalColor, next);



	next = tex.Sample(samplerState, float3(input.uv, 12));
	next.a = (((alphas[input.vindex].w) >> 0) & 0xFF) / 255.f;
	finalColor = BlendTexture(finalColor, next);

	next = tex.Sample(samplerState, float3(input.uv, 13));
	next.a = (((alphas[input.vindex].w) >> 8) & 0xFF) / 255.f;
	finalColor = BlendTexture(finalColor, next);

	next = tex.Sample(samplerState, float3(input.uv, 14));
	next.a = (((alphas[input.vindex].w) >> 16) & 0xFF) / 255.f;
	finalColor = BlendTexture(finalColor, next);

	next = tex.Sample(samplerState, float3(input.uv, 15));
	next.a = (((alphas[input.vindex].w) >> 24) & 0xFF) / 255.f;
	finalColor = BlendTexture(finalColor, next);

	return finalColor;
}


float4 ps(gsps_input input) : SV_TARGET
{
	return GetTexture(input);
}



