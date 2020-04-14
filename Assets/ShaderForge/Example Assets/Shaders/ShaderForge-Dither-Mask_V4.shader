// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:LgAuAC8AUwBoAGEAZABlAHIAcwAvAEQAaQB0AGgAZQByACAARgB1AG4AYwB0AGkAbwBuAHMA,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.01775543,fgcg:0.0702396,fgcb:0.1792453,fgca:1,fgde:0.01,fgrn:12,fgrf:36,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:34631,y:32887,varname:node_2865,prsc:2|diff-1527-OUT,spec-358-OUT,gloss-1813-OUT,normal-9394-OUT,emission-8727-OUT,clip-9048-OUT;n:type:ShaderForge.SFN_Multiply,id:6343,x:33289,y:31247,varname:node_6343,prsc:2|A-7736-RGB,B-6665-RGB;n:type:ShaderForge.SFN_Color,id:6665,x:33063,y:31266,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5019608,c2:0.5019608,c3:0.5019608,c4:1;n:type:ShaderForge.SFN_Tex2d,id:7736,x:33063,y:31081,ptovrint:True,ptlb:Base Color,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:358,x:33706,y:31993,ptovrint:False,ptlb:Metallic,ptin:_Metallic,varname:node_358,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:1813,x:33706,y:32103,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Metallic_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8,max:1;n:type:ShaderForge.SFN_Code,id:7740,x:32043,y:34485,varname:node_7740,prsc:2,code:LwAvACAAcwBjAHIAZQBlAG4AIABzAHAAYQBjAGUAIABwAG8AcwBpAHQAaQBvAG4AIABpAG4AIABwAGkAeABlAGwAcwAKAGYAbABvAGEAdAAyACAAcwBwAG8AcwAgAD0AIAAoAHMAYwBvAG8AcgBkACAALwAgADIAIAArACAAZgBsAG8AYQB0ADIAKAAwAC4ANQAsACAAMAAuADUAKQApADsACgAKAC8ALwAgAFMAaABhAGQAZQByAEYAbwByAGcAZQAgAHMAdQBiAHQAcgBhAGMAdABzACAAMAAuADUAIABmAHIAbwBtACAAdABoAGUAIAAnAGMAbABpAHAAJwAgAHYAYQBsAHUAZQAKAHIAZQB0AHUAcgBuACAAYwBlAGkAbAAoAGkAcwBEAGkAdABoAGUAcgBlAGQAKABzAHAAbwBzACwAIABhAGwAcABoAGEAKQApADsA,output:0,fname:DitherResult,width:410,height:137,input:1,input:0,input_1_label:scoord,input_2_label:alpha|A-6686-UVOUT,B-2743-OUT;n:type:ShaderForge.SFN_ScreenPos,id:6686,x:31845,y:34411,varname:node_6686,prsc:2,sctp:0;n:type:ShaderForge.SFN_Slider,id:2743,x:31691,y:34600,ptovrint:False,ptlb:DitherAlpha,ptin:_DitherAlpha,varname:node_2743,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Color,id:8466,x:33152,y:33194,ptovrint:False,ptlb:Emissive Color,ptin:_EmissiveColor,varname:node_8466,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector3,id:8930,x:34116,y:32942,varname:node_8930,prsc:2,v1:0,v2:0,v3:0;n:type:ShaderForge.SFN_SwitchProperty,id:8727,x:34289,y:33012,ptovrint:False,ptlb:Emissive,ptin:_Emissive,varname:node_8727,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-8930-OUT,B-5267-OUT;n:type:ShaderForge.SFN_Multiply,id:4,x:33430,y:33101,varname:node_4,prsc:2|A-5934-OUT,B-8466-RGB;n:type:ShaderForge.SFN_TexCoord,id:751,x:31075,y:32190,varname:node_751,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2d,id:6911,x:31301,y:32190,varname:node_6911,prsc:2,ntxv:0,isnm:False|UVIN-751-UVOUT,TEX-7908-TEX;n:type:ShaderForge.SFN_Slider,id:3471,x:30655,y:34126,ptovrint:False,ptlb:Depth Phase,ptin:_DepthPhase,varname:node_2206,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1.2,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:8185,x:32132,y:33779,varname:node_8185,prsc:2|A-2948-OUT,B-509-OUT;n:type:ShaderForge.SFN_Panner,id:4046,x:31346,y:34143,varname:node_4046,prsc:2,spu:1,spv:0|UVIN-3495-UVOUT,DIST-7112-OUT;n:type:ShaderForge.SFN_TexCoord,id:3495,x:31143,y:34059,varname:node_3495,prsc:2,uv:2,uaff:False;n:type:ShaderForge.SFN_Tex2d,id:6740,x:31507,y:33999,varname:_Albedo_copy,prsc:2,ntxv:0,isnm:False|UVIN-4046-UVOUT,TEX-4796-TEX;n:type:ShaderForge.SFN_Multiply,id:7112,x:31143,y:34216,varname:node_7112,prsc:2|A-3471-OUT,B-5245-OUT;n:type:ShaderForge.SFN_Vector1,id:5245,x:30951,y:34278,varname:node_5245,prsc:2,v1:-0.5;n:type:ShaderForge.SFN_Tex2d,id:7607,x:30572,y:32469,varname:node_7607,prsc:2,ntxv:0,isnm:False|UVIN-7693-UVOUT,TEX-9960-TEX;n:type:ShaderForge.SFN_TexCoord,id:3611,x:30170,y:32469,varname:node_3611,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:7693,x:30374,y:32469,varname:node_7693,prsc:2,spu:0,spv:-0.02|UVIN-3611-UVOUT;n:type:ShaderForge.SFN_Desaturate,id:5737,x:30765,y:32469,varname:node_5737,prsc:2|COL-7607-RGB;n:type:ShaderForge.SFN_Add,id:66,x:31866,y:32533,varname:node_66,prsc:2|A-6911-RGB,B-1687-OUT;n:type:ShaderForge.SFN_RemapRange,id:8526,x:30799,y:32700,varname:node_8526,prsc:2,frmn:0,frmx:1,tomn:-0.8,tomx:0.8|IN-2318-OUT;n:type:ShaderForge.SFN_Add,id:9527,x:30996,y:32680,varname:node_9527,prsc:2|A-5737-OUT,B-8526-OUT;n:type:ShaderForge.SFN_RemapRange,id:6481,x:31264,y:32683,varname:node_6481,prsc:2,frmn:0,frmx:1,tomn:-3.99,tomx:3.99|IN-9527-OUT;n:type:ShaderForge.SFN_Clamp01,id:1687,x:31450,y:32683,varname:node_1687,prsc:2|IN-6481-OUT;n:type:ShaderForge.SFN_Clamp01,id:3197,x:32078,y:32533,varname:node_3197,prsc:2|IN-66-OUT;n:type:ShaderForge.SFN_Panner,id:2630,x:31600,y:33266,varname:node_2630,prsc:2,spu:1,spv:0|UVIN-2380-UVOUT,DIST-305-OUT;n:type:ShaderForge.SFN_TexCoord,id:2380,x:31370,y:33183,varname:node_2380,prsc:2,uv:2,uaff:False;n:type:ShaderForge.SFN_Tex2d,id:5078,x:31829,y:33266,varname:_Ramp_copy,prsc:2,ntxv:0,isnm:False|UVIN-2630-UVOUT,TEX-4796-TEX;n:type:ShaderForge.SFN_Multiply,id:305,x:31397,y:33339,varname:node_305,prsc:2|A-2803-OUT,B-302-OUT;n:type:ShaderForge.SFN_Vector1,id:302,x:31183,y:33437,varname:node_302,prsc:2,v1:-0.5;n:type:ShaderForge.SFN_Multiply,id:4948,x:32441,y:32561,varname:node_4948,prsc:2|A-3197-OUT,B-9733-OUT;n:type:ShaderForge.SFN_Add,id:2803,x:31183,y:33276,varname:node_2803,prsc:2|A-3471-OUT,B-7774-OUT;n:type:ShaderForge.SFN_OneMinus,id:3733,x:32226,y:33266,varname:node_3733,prsc:2|IN-3883-OUT;n:type:ShaderForge.SFN_Desaturate,id:3883,x:32033,y:33266,varname:node_3883,prsc:2|COL-5078-RGB;n:type:ShaderForge.SFN_Slider,id:7774,x:30807,y:33299,ptovrint:False,ptlb:Dissolve Distance,ptin:_DissolveDistance,varname:node_7774,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.016,max:0.12;n:type:ShaderForge.SFN_Desaturate,id:2948,x:31672,y:33709,varname:node_2948,prsc:2|COL-1725-RGB;n:type:ShaderForge.SFN_Multiply,id:9048,x:32542,y:33878,varname:node_9048,prsc:2|A-8185-OUT,B-7740-OUT;n:type:ShaderForge.SFN_Desaturate,id:509,x:31699,y:34143,varname:node_509,prsc:2|COL-6740-RGB;n:type:ShaderForge.SFN_Multiply,id:5934,x:33041,y:32991,varname:node_5934,prsc:2|A-8100-OUT,B-8185-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:4796,x:29241,y:33438,ptovrint:False,ptlb:Ramp,ptin:_Ramp,varname:node_4796,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2dAsset,id:7908,x:29241,y:33225,ptovrint:False,ptlb:Mask_Decoupe,ptin:_Mask_Decoupe,varname:node_7908,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_TexCoord,id:900,x:31294,y:33702,varname:node_900,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2d,id:1725,x:31487,y:33603,varname:_Mask_Decoupe_copy,prsc:2,ntxv:0,isnm:False|UVIN-900-UVOUT,TEX-7908-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:6130,x:29229,y:32622,ptovrint:False,ptlb:Lines_Mask,ptin:_Lines_Mask,varname:node_6130,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:139,x:31746,y:31240,varname:node_139,prsc:2,ntxv:0,isnm:False|TEX-6130-TEX;n:type:ShaderForge.SFN_Add,id:8100,x:32721,y:32482,varname:node_8100,prsc:2|A-9984-OUT,B-4948-OUT;n:type:ShaderForge.SFN_Multiply,id:1439,x:31981,y:31240,varname:node_1439,prsc:2|A-139-RGB,B-7304-OUT;n:type:ShaderForge.SFN_Slider,id:7304,x:31589,y:31424,ptovrint:False,ptlb:Lines_Intensity,ptin:_Lines_Intensity,varname:node_7304,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:2;n:type:ShaderForge.SFN_Tex2dAsset,id:2718,x:32138,y:31953,ptovrint:False,ptlb:AnimMask,ptin:_AnimMask,varname:node_2718,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:1309,x:32350,y:31741,varname:node_1309,prsc:2,ntxv:0,isnm:False|UVIN-6329-UVOUT,TEX-2718-TEX;n:type:ShaderForge.SFN_Multiply,id:9984,x:32638,y:31636,varname:node_9984,prsc:2|A-1439-OUT,B-1309-RGB;n:type:ShaderForge.SFN_TexCoord,id:857,x:31818,y:31664,varname:node_857,prsc:2,uv:2,uaff:False;n:type:ShaderForge.SFN_Panner,id:6329,x:32131,y:31696,varname:node_6329,prsc:2,spu:0,spv:-1|UVIN-857-UVOUT,DIST-7428-OUT;n:type:ShaderForge.SFN_Time,id:2353,x:29233,y:31863,varname:node_2353,prsc:2;n:type:ShaderForge.SFN_Slider,id:3138,x:31769,y:32003,ptovrint:False,ptlb:LinesSpeed,ptin:_LinesSpeed,varname:node_3138,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:2;n:type:ShaderForge.SFN_Multiply,id:7428,x:31940,y:31822,varname:node_7428,prsc:2|A-2353-TSL,B-3138-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:9960,x:29229,y:32918,ptovrint:False,ptlb:DissolveNoise,ptin:_DissolveNoise,varname:node_9960,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Vector1,id:2318,x:30605,y:32700,varname:node_2318,prsc:2,v1:0.6;n:type:ShaderForge.SFN_Add,id:9733,x:32435,y:33081,varname:node_9733,prsc:2|A-452-OUT,B-3733-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:1205,x:31605,y:32928,ptovrint:False,ptlb:Emissif_Stroke,ptin:_Emissif_Stroke,varname:node_1205,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4360,x:31805,y:32888,varname:node_4360,prsc:2,ntxv:0,isnm:False|TEX-1205-TEX;n:type:ShaderForge.SFN_Multiply,id:452,x:32082,y:32962,varname:node_452,prsc:2|A-4360-RGB,B-3922-OUT;n:type:ShaderForge.SFN_Slider,id:3922,x:31751,y:33108,ptovrint:False,ptlb:Stroke Opacity,ptin:_StrokeOpacity,varname:node_3922,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:4;n:type:ShaderForge.SFN_Tex2dAsset,id:8375,x:33282,y:32509,ptovrint:False,ptlb:Normal,ptin:_Normal,varname:node_8375,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:9574,x:33628,y:32481,varname:node_9574,prsc:2,ntxv:0,isnm:False|TEX-8375-TEX;n:type:ShaderForge.SFN_Tex2d,id:6253,x:33603,y:31637,varname:node_6253,prsc:2,ntxv:0,isnm:False|TEX-4302-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:4302,x:33320,y:31674,ptovrint:False,ptlb:Micro texture,ptin:_Microtexture,varname:node_4302,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1527,x:33963,y:31573,varname:node_1527,prsc:2|A-6343-OUT,B-6253-RGB;n:type:ShaderForge.SFN_Multiply,id:9394,x:33855,y:32676,varname:node_9394,prsc:2|A-9574-RGB,B-250-OUT;n:type:ShaderForge.SFN_Slider,id:811,x:33238,y:32782,ptovrint:False,ptlb:Normal Intensity,ptin:_NormalIntensity,varname:node_811,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1.9,cur:1.9,max:1.918;n:type:ShaderForge.SFN_HsvToRgb,id:250,x:33613,y:32754,varname:node_250,prsc:2|H-811-OUT,S-811-OUT,V-811-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:5267,x:33999,y:33082,ptovrint:False,ptlb:Stars,ptin:_Stars,varname:node_5267,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-4-OUT,B-1145-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:7401,x:33923,y:33971,ptovrint:False,ptlb:Stars Texture,ptin:_StarsTexture,varname:_AnimMask_copy,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_TexCoord,id:4707,x:33603,y:33682,varname:node_4707,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:1022,x:33916,y:33714,varname:node_1022,prsc:2,spu:0,spv:1|UVIN-4707-UVOUT,DIST-6879-OUT;n:type:ShaderForge.SFN_Multiply,id:6879,x:33725,y:33840,varname:node_6879,prsc:2|A-2353-TSL,B-4752-OUT;n:type:ShaderForge.SFN_Add,id:1145,x:33770,y:33174,varname:node_1145,prsc:2|A-4-OUT,B-8521-OUT;n:type:ShaderForge.SFN_Tex2d,id:2829,x:34182,y:33799,varname:node_2829,prsc:2,ntxv:0,isnm:False|UVIN-1022-UVOUT,TEX-7401-TEX;n:type:ShaderForge.SFN_Multiply,id:8521,x:34031,y:33459,varname:node_8521,prsc:2|A-2829-RGB,B-4068-OUT;n:type:ShaderForge.SFN_Slider,id:4068,x:34277,y:33599,ptovrint:False,ptlb:Stars intensity,ptin:_Starsintensity,varname:node_4068,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:4;n:type:ShaderForge.SFN_Slider,id:4752,x:33312,y:33948,ptovrint:False,ptlb:Stars Speed,ptin:_StarsSpeed,varname:node_4752,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;proporder:6665-7736-4302-358-1813-8375-811-2743-3471-8727-8466-7774-4796-7908-6130-7304-3138-2718-9960-1205-3922-5267-7401-4068-4752;pass:END;sub:END;*/

Shader "Example/ShaderForge-Dither-Mask_V4" {
    Properties {
        [HDR]_Color ("Color", Color) = (0.5019608,0.5019608,0.5019608,1)
        _MainTex ("Base Color", 2D) = "white" {}
        _Microtexture ("Micro texture", 2D) = "white" {}
        _Metallic ("Metallic", Range(0, 1)) = 0
        _Gloss ("Gloss", Range(0, 1)) = 0.8
        _Normal ("Normal", 2D) = "bump" {}
        _NormalIntensity ("Normal Intensity", Range(1.9, 1.918)) = 1.9
        _DitherAlpha ("DitherAlpha", Range(0, 1)) = 0.5
        _DepthPhase ("Depth Phase", Range(-1.2, 1)) = 0
        [MaterialToggle] _Emissive ("Emissive", Float ) = 0
        [HDR]_EmissiveColor ("Emissive Color", Color) = (0.5,0.5,0.5,1)
        _DissolveDistance ("Dissolve Distance", Range(0, 0.12)) = 0.016
        _Ramp ("Ramp", 2D) = "white" {}
        _Mask_Decoupe ("Mask_Decoupe", 2D) = "white" {}
        _Lines_Mask ("Lines_Mask", 2D) = "white" {}
        _Lines_Intensity ("Lines_Intensity", Range(0, 2)) = 1
        _LinesSpeed ("LinesSpeed", Range(0, 2)) = 1
        _AnimMask ("AnimMask", 2D) = "white" {}
        _DissolveNoise ("DissolveNoise", 2D) = "white" {}
        _Emissif_Stroke ("Emissif_Stroke", 2D) = "white" {}
        _StrokeOpacity ("Stroke Opacity", Range(0, 4)) = 2
        [MaterialToggle] _Stars ("Stars", Float ) = 0
        _StarsTexture ("Stars Texture", 2D) = "white" {}
        _Starsintensity ("Stars intensity", Range(0, 4)) = 1
        _StarsSpeed ("Stars Speed", Range(-1, 1)) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "../Shaders/Dither Functions.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Metallic;
            uniform float _Gloss;
            float DitherResult( float2 scoord , float alpha ){
            // screen space position in pixels
            float2 spos = (scoord / 2 + float2(0.5, 0.5));
            
            // ShaderForge subtracts 0.5 from the 'clip' value
            return ceil(isDithered(spos, alpha));
            }
            
            uniform float _DitherAlpha;
            uniform float4 _EmissiveColor;
            uniform fixed _Emissive;
            uniform float _DepthPhase;
            uniform float _DissolveDistance;
            uniform sampler2D _Ramp; uniform float4 _Ramp_ST;
            uniform sampler2D _Mask_Decoupe; uniform float4 _Mask_Decoupe_ST;
            uniform sampler2D _Lines_Mask; uniform float4 _Lines_Mask_ST;
            uniform float _Lines_Intensity;
            uniform sampler2D _AnimMask; uniform float4 _AnimMask_ST;
            uniform float _LinesSpeed;
            uniform sampler2D _DissolveNoise; uniform float4 _DissolveNoise_ST;
            uniform sampler2D _Emissif_Stroke; uniform float4 _Emissif_Stroke_ST;
            uniform float _StrokeOpacity;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform sampler2D _Microtexture; uniform float4 _Microtexture_ST;
            uniform float _NormalIntensity;
            uniform fixed _Stars;
            uniform sampler2D _StarsTexture; uniform float4 _StarsTexture_ST;
            uniform float _Starsintensity;
            uniform float _StarsSpeed;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 projPos : TEXCOORD7;
                LIGHTING_COORDS(8,9)
                UNITY_FOG_COORDS(10)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD11;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 node_9574 = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0, _Normal)));
                float3 normalLocal = (node_9574.rgb*(lerp(float3(1,1,1),saturate(3.0*abs(1.0-2.0*frac(_NormalIntensity+float3(0.0,-1.0/3.0,1.0/3.0)))-1),_NormalIntensity)*_NormalIntensity));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
                float4 _Mask_Decoupe_copy = tex2D(_Mask_Decoupe,TRANSFORM_TEX(i.uv0, _Mask_Decoupe));
                float2 node_4046 = (i.uv2+(_DepthPhase*(-0.5))*float2(1,0));
                float4 _Albedo_copy = tex2D(_Ramp,TRANSFORM_TEX(node_4046, _Ramp));
                float node_8185 = (dot(_Mask_Decoupe_copy.rgb,float3(0.3,0.59,0.11))*dot(_Albedo_copy.rgb,float3(0.3,0.59,0.11)));
                clip((node_8185*DitherResult( (sceneUVs * 2 - 1).rg , _DitherAlpha )) - 0.5);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float perceptualRoughness = 1.0 - _Gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _Metallic;
                float specularMonochrome;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float4 node_6253 = tex2D(_Microtexture,TRANSFORM_TEX(i.uv0, _Microtexture));
                float3 diffuseColor = ((_MainTex_var.rgb*_Color.rgb)*node_6253.rgb); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                indirectSpecular *= surfaceReduction;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float4 node_139 = tex2D(_Lines_Mask,TRANSFORM_TEX(i.uv0, _Lines_Mask));
                float4 node_2353 = _Time;
                float2 node_6329 = (i.uv2+(node_2353.r*_LinesSpeed)*float2(0,-1));
                float4 node_1309 = tex2D(_AnimMask,TRANSFORM_TEX(node_6329, _AnimMask));
                float4 node_6911 = tex2D(_Mask_Decoupe,TRANSFORM_TEX(i.uv0, _Mask_Decoupe));
                float4 node_3757 = _Time;
                float2 node_7693 = (i.uv0+node_3757.g*float2(0,-0.02));
                float4 node_7607 = tex2D(_DissolveNoise,TRANSFORM_TEX(node_7693, _DissolveNoise));
                float4 node_4360 = tex2D(_Emissif_Stroke,TRANSFORM_TEX(i.uv0, _Emissif_Stroke));
                float2 node_2630 = (i.uv2+((_DepthPhase+_DissolveDistance)*(-0.5))*float2(1,0));
                float4 _Ramp_copy = tex2D(_Ramp,TRANSFORM_TEX(node_2630, _Ramp));
                float3 node_4 = (((((node_139.rgb*_Lines_Intensity)*node_1309.rgb)+(saturate((node_6911.rgb+saturate(((dot(node_7607.rgb,float3(0.3,0.59,0.11))+(0.6*1.6+-0.8))*7.98+-3.99))))*((node_4360.rgb*_StrokeOpacity)+(1.0 - dot(_Ramp_copy.rgb,float3(0.3,0.59,0.11))))))*node_8185)*_EmissiveColor.rgb);
                float2 node_1022 = (i.uv0+(node_2353.r*_StarsSpeed)*float2(0,1));
                float4 node_2829 = tex2D(_StarsTexture,TRANSFORM_TEX(node_1022, _StarsTexture));
                float3 emissive = lerp( float3(0,0,0), lerp( node_4, (node_4+(node_2829.rgb*_Starsintensity)), _Stars ), _Emissive );
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "../Shaders/Dither Functions.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Metallic;
            uniform float _Gloss;
            float DitherResult( float2 scoord , float alpha ){
            // screen space position in pixels
            float2 spos = (scoord / 2 + float2(0.5, 0.5));
            
            // ShaderForge subtracts 0.5 from the 'clip' value
            return ceil(isDithered(spos, alpha));
            }
            
            uniform float _DitherAlpha;
            uniform float4 _EmissiveColor;
            uniform fixed _Emissive;
            uniform float _DepthPhase;
            uniform float _DissolveDistance;
            uniform sampler2D _Ramp; uniform float4 _Ramp_ST;
            uniform sampler2D _Mask_Decoupe; uniform float4 _Mask_Decoupe_ST;
            uniform sampler2D _Lines_Mask; uniform float4 _Lines_Mask_ST;
            uniform float _Lines_Intensity;
            uniform sampler2D _AnimMask; uniform float4 _AnimMask_ST;
            uniform float _LinesSpeed;
            uniform sampler2D _DissolveNoise; uniform float4 _DissolveNoise_ST;
            uniform sampler2D _Emissif_Stroke; uniform float4 _Emissif_Stroke_ST;
            uniform float _StrokeOpacity;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform sampler2D _Microtexture; uniform float4 _Microtexture_ST;
            uniform float _NormalIntensity;
            uniform fixed _Stars;
            uniform sampler2D _StarsTexture; uniform float4 _StarsTexture_ST;
            uniform float _Starsintensity;
            uniform float _StarsSpeed;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 projPos : TEXCOORD7;
                LIGHTING_COORDS(8,9)
                UNITY_FOG_COORDS(10)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 node_9574 = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0, _Normal)));
                float3 normalLocal = (node_9574.rgb*(lerp(float3(1,1,1),saturate(3.0*abs(1.0-2.0*frac(_NormalIntensity+float3(0.0,-1.0/3.0,1.0/3.0)))-1),_NormalIntensity)*_NormalIntensity));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
                float4 _Mask_Decoupe_copy = tex2D(_Mask_Decoupe,TRANSFORM_TEX(i.uv0, _Mask_Decoupe));
                float2 node_4046 = (i.uv2+(_DepthPhase*(-0.5))*float2(1,0));
                float4 _Albedo_copy = tex2D(_Ramp,TRANSFORM_TEX(node_4046, _Ramp));
                float node_8185 = (dot(_Mask_Decoupe_copy.rgb,float3(0.3,0.59,0.11))*dot(_Albedo_copy.rgb,float3(0.3,0.59,0.11)));
                clip((node_8185*DitherResult( (sceneUVs * 2 - 1).rg , _DitherAlpha )) - 0.5);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float perceptualRoughness = 1.0 - _Gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _Metallic;
                float specularMonochrome;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float4 node_6253 = tex2D(_Microtexture,TRANSFORM_TEX(i.uv0, _Microtexture));
                float3 diffuseColor = ((_MainTex_var.rgb*_Color.rgb)*node_6253.rgb); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "../Shaders/Dither Functions.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            float DitherResult( float2 scoord , float alpha ){
            // screen space position in pixels
            float2 spos = (scoord / 2 + float2(0.5, 0.5));
            
            // ShaderForge subtracts 0.5 from the 'clip' value
            return ceil(isDithered(spos, alpha));
            }
            
            uniform float _DitherAlpha;
            uniform float _DepthPhase;
            uniform sampler2D _Ramp; uniform float4 _Ramp_ST;
            uniform sampler2D _Mask_Decoupe; uniform float4 _Mask_Decoupe_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float2 uv1 : TEXCOORD2;
                float2 uv2 : TEXCOORD3;
                float4 posWorld : TEXCOORD4;
                float4 projPos : TEXCOORD5;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
                float4 _Mask_Decoupe_copy = tex2D(_Mask_Decoupe,TRANSFORM_TEX(i.uv0, _Mask_Decoupe));
                float2 node_4046 = (i.uv2+(_DepthPhase*(-0.5))*float2(1,0));
                float4 _Albedo_copy = tex2D(_Ramp,TRANSFORM_TEX(node_4046, _Ramp));
                float node_8185 = (dot(_Mask_Decoupe_copy.rgb,float3(0.3,0.59,0.11))*dot(_Albedo_copy.rgb,float3(0.3,0.59,0.11)));
                clip((node_8185*DitherResult( (sceneUVs * 2 - 1).rg , _DitherAlpha )) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #include "../Shaders/Dither Functions.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Metallic;
            uniform float _Gloss;
            uniform float4 _EmissiveColor;
            uniform fixed _Emissive;
            uniform float _DepthPhase;
            uniform float _DissolveDistance;
            uniform sampler2D _Ramp; uniform float4 _Ramp_ST;
            uniform sampler2D _Mask_Decoupe; uniform float4 _Mask_Decoupe_ST;
            uniform sampler2D _Lines_Mask; uniform float4 _Lines_Mask_ST;
            uniform float _Lines_Intensity;
            uniform sampler2D _AnimMask; uniform float4 _AnimMask_ST;
            uniform float _LinesSpeed;
            uniform sampler2D _DissolveNoise; uniform float4 _DissolveNoise_ST;
            uniform sampler2D _Emissif_Stroke; uniform float4 _Emissif_Stroke_ST;
            uniform float _StrokeOpacity;
            uniform sampler2D _Microtexture; uniform float4 _Microtexture_ST;
            uniform fixed _Stars;
            uniform sampler2D _StarsTexture; uniform float4 _StarsTexture_ST;
            uniform float _Starsintensity;
            uniform float _StarsSpeed;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : SV_Target {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float4 node_139 = tex2D(_Lines_Mask,TRANSFORM_TEX(i.uv0, _Lines_Mask));
                float4 node_2353 = _Time;
                float2 node_6329 = (i.uv2+(node_2353.r*_LinesSpeed)*float2(0,-1));
                float4 node_1309 = tex2D(_AnimMask,TRANSFORM_TEX(node_6329, _AnimMask));
                float4 node_6911 = tex2D(_Mask_Decoupe,TRANSFORM_TEX(i.uv0, _Mask_Decoupe));
                float4 node_2560 = _Time;
                float2 node_7693 = (i.uv0+node_2560.g*float2(0,-0.02));
                float4 node_7607 = tex2D(_DissolveNoise,TRANSFORM_TEX(node_7693, _DissolveNoise));
                float4 node_4360 = tex2D(_Emissif_Stroke,TRANSFORM_TEX(i.uv0, _Emissif_Stroke));
                float2 node_2630 = (i.uv2+((_DepthPhase+_DissolveDistance)*(-0.5))*float2(1,0));
                float4 _Ramp_copy = tex2D(_Ramp,TRANSFORM_TEX(node_2630, _Ramp));
                float4 _Mask_Decoupe_copy = tex2D(_Mask_Decoupe,TRANSFORM_TEX(i.uv0, _Mask_Decoupe));
                float2 node_4046 = (i.uv2+(_DepthPhase*(-0.5))*float2(1,0));
                float4 _Albedo_copy = tex2D(_Ramp,TRANSFORM_TEX(node_4046, _Ramp));
                float node_8185 = (dot(_Mask_Decoupe_copy.rgb,float3(0.3,0.59,0.11))*dot(_Albedo_copy.rgb,float3(0.3,0.59,0.11)));
                float3 node_4 = (((((node_139.rgb*_Lines_Intensity)*node_1309.rgb)+(saturate((node_6911.rgb+saturate(((dot(node_7607.rgb,float3(0.3,0.59,0.11))+(0.6*1.6+-0.8))*7.98+-3.99))))*((node_4360.rgb*_StrokeOpacity)+(1.0 - dot(_Ramp_copy.rgb,float3(0.3,0.59,0.11))))))*node_8185)*_EmissiveColor.rgb);
                float2 node_1022 = (i.uv0+(node_2353.r*_StarsSpeed)*float2(0,1));
                float4 node_2829 = tex2D(_StarsTexture,TRANSFORM_TEX(node_1022, _StarsTexture));
                o.Emission = lerp( float3(0,0,0), lerp( node_4, (node_4+(node_2829.rgb*_Starsintensity)), _Stars ), _Emissive );
                
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float4 node_6253 = tex2D(_Microtexture,TRANSFORM_TEX(i.uv0, _Microtexture));
                float3 diffColor = ((_MainTex_var.rgb*_Color.rgb)*node_6253.rgb);
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, _Metallic, specColor, specularMonochrome );
                float roughness = 1.0 - _Gloss;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
