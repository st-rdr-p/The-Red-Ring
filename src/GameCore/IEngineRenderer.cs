namespace GameCore
{
    public interface IEngineRenderer
    {
        void DrawSprite(string spriteId, float x, float y, float rotation, float scaleX = 1.0f, float scaleY = 1.0f, float opacity = 1.0f);
        void DrawScreenFlash(float intensity, string color = "red");
        
        /// <summary>
        /// Apply pixelation/retro graphics post-processing to the render target.
        /// </summary>
        void ApplyRetroGraphics(RetroGraphicsEffect effect);
        
        /// <summary>
        /// Get the current screen width in pixels.
        /// </summary>
        int GetScreenWidth();
        
        /// <summary>
        /// Get the current screen height in pixels.
        /// </summary>
        int GetScreenHeight();
    }

    public interface IEngineInput
    {
        bool IsKeyDown(string key);
        float GetAxis(string axis);  // horizontal, vertical etc.
        float GetMouseX();
        float GetMouseY();
        void LockMouse(bool locked);
        bool IsMouseLocked { get; }
    }

    public interface IEngineAudio
    {
        void PlaySound(string soundId);
        void StopSound(string soundId);
    }
}
