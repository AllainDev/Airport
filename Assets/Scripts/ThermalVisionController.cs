using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ThermalVisionController : MonoBehaviour
{
    private Volume thermalVolume;
    private bool isThermalActive = false;
    
    // Lưu trữ vật liệu thật của nhân vật để trả lại khi tắt
    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    private Material thermalMaterial;

    void Start()
    {
        // 1. Tạo lớp phủ làm lạnh môi trường (Màu tím than/Xanh đen)
        thermalVolume = gameObject.AddComponent<Volume>();
        thermalVolume.isGlobal = true;
        thermalVolume.weight = 0f;
        thermalVolume.priority = 1000; // Ưu tiên cao nhất

        VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();
        thermalVolume.sharedProfile = profile;

        ColorAdjustments colorAdj = profile.Add<ColorAdjustments>();
        colorAdj.colorFilter.overrideState = true;
        colorAdj.colorFilter.value = new Color(0.1f, 0.1f, 0.6f); // Màu xanh dương/Tím lạnh
        
        colorAdj.contrast.overrideState = true;
        colorAdj.contrast.value = 30f; // Tăng độ tương phản để làm gắt hình ảnh

        // 2. Tạo Vật liệu phát sáng (Nhiệt)
        Shader shader = Shader.Find("HDRP/Unlit");
        if (shader == null) shader = Shader.Find("Unlit/Color"); // Fallback an toàn
        
        thermalMaterial = new Material(shader);
        
        // Tạo màu Cam/Đỏ rực rỡ phát sáng (Dùng giá trị > 1 để tạo độ rực HDR)
        Color hotColor = new Color(5.0f, 1.5f, 0.0f); 
        thermalMaterial.SetColor("_UnlitColor", hotColor);
        thermalMaterial.SetColor("_Color", hotColor); // Cho shader Unlit cũ nếu không có HDRP
    }

    void Update()
    {
        // Bấm phím M để Bật/Tắt Cảm biến nhiệt
        if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
        {
            isThermalActive = !isThermalActive;
            ToggleThermalVision(isThermalActive);
        }
    }

    void ToggleThermalVision(bool active)
    {
        // 1. Bật/Tắt làm lạnh môi trường
        thermalVolume.weight = active ? 1f : 0f;

        // 2. Quét tất cả nhân vật (con người) đang đi trên đường băng
        MoveToTarget[] people = Object.FindObjectsByType<MoveToTarget>(FindObjectsSortMode.None);

        foreach (var person in people)
        {
            // Lấy tất cả các bộ phận hiển thị của nhân vật (Da, Quần áo, Tóc...)
            Renderer[] renderers = person.GetComponentsInChildren<Renderer>();

            foreach (var r in renderers)
            {
                if (active)
                {
                    // Cất giữ đồ thật vào tủ (Dictionary) trước khi thay
                    if (!originalMaterials.ContainsKey(r))
                    {
                        originalMaterials[r] = r.materials;
                    }

                    // Mặc bộ đồ phát sáng nhiệt (Thay thế toàn bộ vật liệu bằng thermalMaterial)
                    Material[] hotMaterials = new Material[r.materials.Length];
                    for (int i = 0; i < hotMaterials.Length; i++)
                    {
                        hotMaterials[i] = thermalMaterial;
                    }
                    r.materials = hotMaterials;
                }
                else
                {
                    // Trả lại đồ thật khi tắt chế độ Nhiệt
                    if (originalMaterials.ContainsKey(r))
                    {
                        r.materials = originalMaterials[r];
                    }
                }
            }
        }

        Debug.Log(active ? "🔴 ĐÃ BẬT: Cảm Biến Nhiệt (Thermal Vision)" : "⚫ ĐÃ TẮT: Cảm Biến Nhiệt");
    }
}
