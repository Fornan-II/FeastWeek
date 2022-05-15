float4 _FogColor;
float _FogStartDistance;
float _FogEndDistance;
float _FogExponent;

void GetFogData_float(out float4 FogColor, out float FogStartDistance, out float FogEndDistance, out float FogExponent)
{
	FogColor = _FogColor;
	FogStartDistance = _FogStartDistance;
	FogEndDistance = _FogEndDistance;
	FogExponent = _FogExponent;
}