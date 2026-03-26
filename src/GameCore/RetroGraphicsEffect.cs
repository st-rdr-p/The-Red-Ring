namespace GameCore
{
    /// <summary>
    /// Retro pixelated graphics effect component.
    /// Applies pixel art style rendering like classic Sonic games.
    /// </summary>
    public class RetroGraphicsEffect : Component
    {
        public bool Enabled { get; set; } = true;
        
        /// <summary>
        /// Size of each pixel in screen pixels. 2-8 recommended.
        /// 1 = no pixelation, 4 = 4x4 pixel blocks
        /// </summary>
        public int PixelSize { get; set; } = 4;
        
        /// <summary>
        /// Reduce color palette for authentic retro look.
        /// 0 = full colors, 1-4 bits per channel = reduced palette
        /// </summary>
        public int ColorBitsPerChannel { get; set; } = 5;  // ~32,000 colors (Sega Genesis style)
        
        /// <summary>
        /// Add scanlines for CRT monitor effect.
        /// </summary>
        public bool EnableScanlines { get; set; } = true;
        
        /// <summary>
        /// Scanline opacity (0.0 - 1.0).
        /// </summary>
        public float ScanlineOpacity { get; set; } = 0.3f;
        
        /// <summary>
        /// Use dithering for smoother color transitions.
        /// </summary>
        public bool EnableDithering { get; set; } = false;
        
        /// <summary>
        /// Enable aspect ratio correction (like old arcade cabinets).
        /// </summary>
        public bool EnableAspectCorrection { get; set; } = true;
        
        public RetroGraphicsEffect() { }
        
        public RetroGraphicsEffect(int pixelSize, int colorBits = 5, bool scanlines = true)
        {
            PixelSize = pixelSize;
            ColorBitsPerChannel = colorBits;
            EnableScanlines = scanlines;
        }
    }

    /// <summary>
    /// Preset retro graphics styles.
    /// </summary>
    public static class RetroPresets
    {
        /// <summary>
        /// Sega Genesis style (16-bit, 2x2 pixel blocks).
        /// </summary>
        public static RetroGraphicsEffect SegaGenesis => new(
            pixelSize: 2,
            colorBits: 5,      // 5 bits per channel = 32k colors
            scanlines: true
        );

        /// <summary>
        /// Sega Master System style (8-bit, 3x3 pixel blocks).
        /// </summary>
        public static RetroGraphicsEffect SegaMasterSystem => new(
            pixelSize: 3,
            colorBits: 4,      // 4 bits per channel = 4096 colors
            scanlines: true
        );

        /// <summary>
        /// Super Nintendo style (16-bit, 2x2 pixel blocks).
        /// </summary>
        public static RetroGraphicsEffect SuperNintendo => new(
            pixelSize: 2,
            colorBits: 5,
            scanlines: true
        );

        /// <summary>
        /// Arcade cabinet style (high contrast, harsher pixelation).
        /// </summary>
        public static RetroGraphicsEffect Arcade => new(
            pixelSize: 4,
            colorBits: 4,
            scanlines: true
        )
        {
            ScanlineOpacity = 0.5f,
            EnableAspectCorrection = true
        };

        /// <summary>
        /// Game Boy style (monochrome/limited palette, 4x4 pixel blocks).
        /// </summary>
        public static RetroGraphicsEffect GameBoy => new(
            pixelSize: 4,
            colorBits: 2,      // 4 colors max
            scanlines: false
        );

        /// <summary>
        /// Minimal pixel art (just 2x2 blocks, full colors).
        /// </summary>
        public static RetroGraphicsEffect MinimalPixel => new(
            pixelSize: 2,
            colorBits: 8,      // Full 24-bit color
            scanlines: false
        );
    }
}
