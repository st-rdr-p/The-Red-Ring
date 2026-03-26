namespace GameCore
{
    /// <summary>
    /// Example engine bridge implementations showing how to integrate GameCore
    /// with actual game engines (Unity, Godot, Unreal, etc.)
    /// </summary>

    // ==================== UNITY EXAMPLE ====================
    // public class UnityInputBridge : IEngineInput
    // {
    //     private bool _mouseLocked = true;
    //
    //     public bool IsKeyDown(string key) => Input.GetKey(key);
    //
    //     public float GetAxis(string axis)
    //     {
    //         if (axis == "Horizontal") return Input.GetAxis("Horizontal");
    //         if (axis == "Vertical") return Input.GetAxis("Vertical");
    //         return 0;
    //     }
    //
    //     public float GetMouseX() => Input.mousePosition.x;
    //
    //     public float GetMouseY() => Input.mousePosition.y;
    //
    //     public void LockMouse(bool locked)
    //     {
    //         _mouseLocked = locked;
    //         Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
    //     }
    //
    //     public bool IsMouseLocked => _mouseLocked;
    // }
    //
    // public class UnityRendererBridge : IEngineRenderer
    // {
    //     private Color _flashColor;
    //     private float _flashIntensity;
    //
    //     public void DrawSprite(string spriteId, float x, float y, float rotation, 
    //         float scaleX = 1.0f, float scaleY = 1.0f, float opacity = 1.0f)
    //     {
    //         // Load and instantiate mesh from spriteId
    //         var mesh = Resources.Load<GameObject>($"Meshes/{spriteId}");
    //         var instance = Instantiate(mesh);
    //         instance.transform.position = new Vector3(x, y, 0);
    //         // ... setup renderer
    //     }
    //
    //     public void DrawScreenFlash(float intensity, string color = "red")
    //     {
    //         _flashIntensity = intensity;
    //         _flashColor = color == "red" ? Color.red : Color.white;
    //         // Render overlay at screen-space
    //     }
    // }

    // ==================== GODOT EXAMPLE ====================
    // public class GodotInputBridge : IEngineInput
    // {
    //     private bool _mouseLocked = true;
    //
    //     public bool IsKeyDown(string key) => Input.IsActionPressed(key);
    //
    //     public float GetAxis(string axis)
    //     {
    //         if (axis == "Horizontal") return Input.GetAxis("ui_right", "ui_left");
    //         if (axis == "Vertical") return Input.GetAxis("ui_down", "ui_up");
    //         return 0;
    //     }
    //
    //     public float GetMouseX() => (float)GetViewport().GetMousePosition().x;
    //
    //     public float GetMouseY() => (float)GetViewport().GetMousePosition().y;
    //
    //     public void LockMouse(bool locked)
    //     {
    //         _mouseLocked = locked;
    //         Input.MouseMode = locked ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
    //     }
    //
    //     public bool IsMouseLocked => _mouseLocked;
    // }

    public class EngineBridgeExamples
    {
        // This file serves as documentation only.
        // Copy these implementations into your engine-specific layer.
    }
}
