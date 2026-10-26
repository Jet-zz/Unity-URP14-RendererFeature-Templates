using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable,VolumeComponentMenu("CustomPostProcessing/ColorAdjust")]
public class ColorAdjustVolume : VolumeComponent,IPostProcessComponent
{
    public FloatParameter Intensity = new ClampedFloatParameter(0,0,1); 
    public FloatParameter Brightness = new ClampedFloatParameter(0,-1,1); 
    public FloatParameter Contrast = new ClampedFloatParameter(1,0,10); 
    public FloatParameter Gamma = new ClampedFloatParameter(1, 0.01f, 10); 
    public FloatParameter Hue = new ClampedFloatParameter(0,0,1); 
    public FloatParameter Saturation = new ClampedFloatParameter(1,0,2); 

    public bool IsActive()
    {
        return active && Intensity.value > 0.0001f;
    }

    public bool IsTileCompatible()
    {
        return false;
    }
}
