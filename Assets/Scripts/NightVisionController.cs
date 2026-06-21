using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.InputSystem;

public class NightVisionController : MonoBehaviour
{
    private Volume nightVisionVolume;
    private bool isNightVisionActive = false;

    void Start()
    {
        // 1. TẠO BỘ LỌC KÍNH NHÌN ĐÊM (Phím N)
        nightVisionVolume = gameObject.AddComponent<Volume>();
        nightVisionVolume.isGlobal = true;
        nightVisionVolume.weight = 0f; 
        nightVisionVolume.priority = 999;

        VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();
        nightVisionVolume.sharedProfile = profile;

        ColorAdjustments colorAdj = profile.Add<ColorAdjustments>();
        colorAdj.colorFilter.overrideState = true;
        colorAdj.colorFilter.value = new Color(0.5f, 1.0f, 0.5f); 
        
        colorAdj.postExposure.overrideState = true;
        colorAdj.postExposure.value = 6f; 

        Vignette vignette = profile.Add<Vignette>();
        vignette.intensity.overrideState = true;
        vignette.intensity.value = 0.5f;

        FilmGrain grain = profile.Add<FilmGrain>();
        grain.intensity.overrideState = true;
        grain.intensity.value = 0.8f;
        grain.type.overrideState = true;
        grain.type.value = FilmGrainLookup.Large01;
    }

    void Update()
    {
        // Bấm phím N để Bật/Tắt kính nhìn đêm
        if (Keyboard.current != null && Keyboard.current.nKey.wasPressedThisFrame)
        {
            isNightVisionActive = !isNightVisionActive;
            nightVisionVolume.weight = isNightVisionActive ? 1f : 0f;
        }
    }
}
