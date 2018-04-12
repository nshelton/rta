float2 pseudo_knightyan(float3 p)
{
    float3 CSize = _FractalA.xyz;
    float DEfactor=1.;
    float orbit = 0;
    for(int i=0;i<6;i++){

        float3 start = p;
        p = 2.*clamp(p, -CSize, CSize)-p;
        float k = max(0.70968/dot(p,p),1.);
        p *= k;
        DEfactor *= k + 0.05;

        orbit += length(start - p);
    }
    float rxy=length(p.xy);
    float ds =  max(rxy-0.92784, abs(rxy*p.z) / length(p))/DEfactor;

    return float2(ds,  orbit);
}

float2 tglad_variant(float3 z0)
{
    // z0 = modc(z0, 2.0);
    float mr=0.25, mxr=1.0;

    float4 scale = (float)(-2) , p0=_FractalA;
    float4 z = float4(z0,1.0);
    float orbit = 0;
    for (int n = 0; n < 8; n++) {
        float3 start = z;
        z.xyz=clamp(z.xyz, -_FractalB.x, _FractalB.x)*2.0-z.xyz;
        z*=scale/clamp(dot(z.xyz,z.xyz),mr,mxr);
        z+=p0;
        orbit += length(start - z);
    }
    float dS=(length(max(abs(z.xyz)-_FractalC.xyz,0.0))-0.06)/z.w;
    return float2(dS,  orbit) * _RaymarchParam.y;
}


float2 tglad(float3 z0)
{
    // z0 = modc(z0, 2.0);

    float mr=0.25, mxr=1.0;
    float4 scale=float4(-3.12,-3.12,-3.12,3.12), p0=_FractalA;
    float4 z = float4(z0,1.0);
    float orbit = 0;

    for (int n = 0; n < 8; n++) {
        float3 start = z.xyz;

        z.xyz=clamp(z.xyz, -_FractalB.x, _FractalB.x)*2.0-z.xyz;
        z*=scale/clamp(dot(z.xyz,z.xyz),mr,mxr);
        z+=p0;
        orbit += length(start-z.xyz);
        

    }

    float dS=(length(max(abs(z.xyz)-float3(1.2,49.0,1.4),0.0))-0.06)/z.w;
    return float2(dS, orbit ) * _RaymarchParam.y;
}

// distance function from Hartverdrahtet
// ( http://www.pouet.net/prod.php?which=59086 )
float2 hartverdrahtet(float3 f)
{
    float3 cs = _FractalA.xyz;
    float  fs  = _FractalA.w;
    float3 fc=0;
    float fu=10.;
    float fd=.763;
    float orbit = 0.0;
    fc.z=-.38;
 
    float v=1.;
    for(int i=0; i<12; i++){
        float3 start = f;

        f=2.*clamp(f,-cs,cs)-f;
        float c=max(fs/dot(f,f),1.);
        f*=c;
        v*=c;
        f+=fc;

        orbit += length(start-f);
    }
    float z=length(f.xy)-fu;
    float d =  fd*max(z,abs(length(f.xy)*f.z)/sqrt(dot(f,f)))/abs(v);

    return float2(d, orbit);
}

// ( http://www.pouet.net/prod.php?which=59086 )
float2 hartverdrahtetBasic(float3 f)
{
    float3 cs= _FractalA.xyz / 2.0;
    float fs=_FractalB.x * 2.0;
    float3 fc=_FractalC.xyz;
    float fu = 10. * _RaymarchParam.y;
    float fd = _RaymarchParam.x;
    float orbit = 0.0;

    float v=1.;

    [unroll(12)] 
    for(int i=0; i<12; i++){
         float3 start = f;

        f=2.*clamp(f,-cs,cs)-f; // boxfold
        float c=max(fs/dot(f,f),1.);
        f*=c;
        v*=c;
        f+=fc;
        orbit += length(start-f);

    }
    float z=length(f.xy)-fu;

    float d =  fd*max(z,abs(length(f.xy)*f.z)/sqrt(dot(f,f)))/abs(v);

    return float2(d, orbit);
}

float udBox( float3 p, float3 b )
{
  return length(max(abs(p)-b,0.0));
}

float4 fromtwovectors(float3 u, float3 v)
{
    u = normalize(u);
    v = normalize(v);
    float m = sqrt(2.f + 2.f * dot(u, v));
    float3 w = (1.f / m) * cross(u, v);
    return float4( w.x, w.y, w.z, 0.5f * m);
}



float sdBox( float3 p, float3 b )
{
  float3 d = abs(p) - b;
  return min(max(d.x,max(d.y,d.z)),0.0) + length(max(d,0.0));
}


float2 polycrust(float3 p){
    float3 dim = _FractalB.xyz;

    float3 t = _FractalA.xyz;
    

    float d = 1e10;

    float scale = _RaymarchParam.x + 0.5;

    float s = 1.0;
    float orbit = 0.0;

        float3 p0 = p;
    for (int i = 0; i < 10; i++){

        p = rotate_vector(p-t/s, fromtwovectors(_FractalA.xyz, _FractalC.xyz));

        d = min (d, sdBox(p.xyz/s, dim)*s ) ;
        p = abs(p);


        // float circle =  fu/10.0 + 0.1 * sin(_Time.x + p.xyz);
        // d = min(d, length(p - t) -circle);

        s *= scale;
        orbit += length(p - p0);
        
    }

    return float2(d, orbit);
}


float2 DE(float3 d)
{
	float2 p =  hartverdrahtetBasic(d);

    if ( _RaymarchParam.w > 1)
     p = tglad_variant(d);

    if ( _RaymarchParam.w > 2)
        p = pseudo_knightyan(d);  

    if ( _RaymarchParam.w > 3)
        p = polycrust(d);  

    return p;
}