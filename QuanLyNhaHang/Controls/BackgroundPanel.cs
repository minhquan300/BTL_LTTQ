using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.ComponentModel;

namespace QuanLyNhaHang.Controls;

/// <summary>
/// Left-panel surface that paints a cached, softened background behind transparent child controls.
/// The source image is never blurred during painting.
/// </summary>
public sealed class BackgroundPanel : UserControl
{
    public const int DefaultBlurRadius = 12;
    public const int DefaultOverlayAlpha = 150;
    public static readonly Color DefaultOverlayColor = Color.FromArgb(15, 42, 74);
    private const int ResizeDebounceMilliseconds = 180;

    private readonly System.Windows.Forms.Timer _resizeTimer;
    private Image? _sourceImage;
    private Bitmap? _blurredCache;
    private bool _ownsSourceImage;
    private int _blurRadius = DefaultBlurRadius;
    private int _overlayAlpha = DefaultOverlayAlpha;
    private Color _overlayColor = DefaultOverlayColor;

    public BackgroundPanel()
    {
        SetStyle(ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor, true);
        UpdateStyles();
        DoubleBuffered = true;
        BackColor = Color.Transparent;
        Margin = Padding.Empty;

        _resizeTimer = new System.Windows.Forms.Timer { Interval = ResizeDebounceMilliseconds };
        _resizeTimer.Tick += (_, _) =>
        {
            _resizeTimer.Stop();
            RebuildBlurCache();
            Invalidate();
        };

        _sourceImage = CreatePlaceholderImage();
        _ownsSourceImage = true;
        LoadConfiguredImageIfPresent();
        RebuildBlurCache();
    }

    /// <summary>The original source image. Assigning an image creates a new cached blurred copy.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new Image? BackgroundImage
    {
        get => _sourceImage;
        set
        {
            if (ReferenceEquals(_sourceImage, value)) return;
            var oldSource = _sourceImage;
            bool disposeOld = _ownsSourceImage;
            _sourceImage = value;
            _ownsSourceImage = false;
            RebuildBlurCache();
            if (disposeOld) oldSource?.Dispose();
            Invalidate();
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int BlurRadius
    {
        get => _blurRadius;
        set
        {
            int adjusted = Math.Clamp(value, 1, 48);
            if (_blurRadius == adjusted) return;
            _blurRadius = adjusted;
            RebuildBlurCache();
            Invalidate();
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int OverlayAlpha
    {
        get => _overlayAlpha;
        set
        {
            int adjusted = Math.Clamp(value, 0, 255);
            if (_overlayAlpha == adjusted) return;
            _overlayAlpha = adjusted;
            Invalidate();
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color OverlayColor
    {
        get => _overlayColor;
        set
        {
            if (_overlayColor == value) return;
            _overlayColor = value;
            Invalidate();
        }
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        ScheduleBlurRebuild();
    }

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        ScheduleBlurRebuild();
    }

    private void ScheduleBlurRebuild()
    {
        if (IsDisposed || Width <= 0 || Height <= 0) return;
        _resizeTimer.Stop();
        _resizeTimer.Start();
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        // OnPaint paints the complete surface so transparent child controls see this same background.
        e.Graphics.Clear(DefaultOverlayColor);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var bounds = ClientRectangle;
        if (bounds.Width <= 0 || bounds.Height <= 0) return;

        e.Graphics.Clear(DefaultOverlayColor);
        if (_blurredCache != null)
        {
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            DrawCover(e.Graphics, _blurredCache, bounds);
        }

        using (var overlay = new SolidBrush(Color.FromArgb(_overlayAlpha, _overlayColor)))
            e.Graphics.FillRectangle(overlay, bounds);

        using (var gradient = new LinearGradientBrush(bounds,
                   Color.FromArgb(24, _overlayColor),
                   Color.FromArgb(72, _overlayColor),
                   LinearGradientMode.Vertical))
            e.Graphics.FillRectangle(gradient, bounds);

        DrawDecoration(e.Graphics, bounds);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _resizeTimer.Stop();
            _resizeTimer.Dispose();
            _blurredCache?.Dispose();
            _blurredCache = null;
            if (_ownsSourceImage) _sourceImage?.Dispose();
            _sourceImage = null;
        }
        base.Dispose(disposing);
    }

    private void LoadConfiguredImageIfPresent()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Resources", "login_bg.jpg");
        if (!File.Exists(path)) return;

        try
        {
            using var stream = File.OpenRead(path);
            using var loaded = Image.FromStream(stream);
            BackgroundImage = new Bitmap(loaded);
        }
        catch
        {
            // Keep the local placeholder if the optional image is missing or invalid.
        }
    }

    private void RebuildBlurCache()
    {
        if (_sourceImage == null || Width <= 0 || Height <= 0) return;
        var oldCache = _blurredCache;
        _blurredCache = CreateBlurredCache(_sourceImage, _blurRadius, Math.Max(Width, Height));
        oldCache?.Dispose();
    }

    private static Bitmap CreateBlurredCache(Image source, int radius, int targetExtent)
    {
        // Downsample by roughly 1/8 at the default radius, then upscale with a high-quality filter.
        float factor = 1f / Math.Clamp(radius, 2, 16);
        int smallWidth = Math.Max(1, (int)Math.Ceiling(source.Width * factor));
        int smallHeight = Math.Max(1, (int)Math.Ceiling(source.Height * factor));
        using var small = new Bitmap(smallWidth, smallHeight, PixelFormat.Format32bppPArgb);
        using (var graphics = Graphics.FromImage(small))
        {
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.DrawImage(source, new Rectangle(0, 0, smallWidth, smallHeight));
        }

        // Cache at panel size so OnPaint only performs the cover draw and overlays.
        float coverScale = Math.Max(targetExtent / (float)smallWidth, targetExtent / (float)smallHeight);
        int cacheWidth = Math.Max(1, (int)Math.Ceiling(smallWidth * coverScale));
        int cacheHeight = Math.Max(1, (int)Math.Ceiling(smallHeight * coverScale));
        var cache = new Bitmap(cacheWidth, cacheHeight, PixelFormat.Format32bppPArgb);
        using (var graphics = Graphics.FromImage(cache))
        {
            graphics.Clear(Color.Transparent);
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.DrawImage(small, new Rectangle(0, 0, cacheWidth, cacheHeight));
        }
        return cache;
    }

    private static void DrawCover(Graphics graphics, Image image, Rectangle target)
    {
        float scale = Math.Max(target.Width / (float)image.Width, target.Height / (float)image.Height);
        float width = image.Width * scale;
        float height = image.Height * scale;
        var destination = new RectangleF(
            target.Left + (target.Width - width) / 2f,
            target.Top + (target.Height - height) / 2f,
            width,
            height);
        graphics.DrawImage(image, destination);
    }

    private static void DrawDecoration(Graphics graphics, Rectangle bounds)
    {
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(Color.FromArgb(20, Color.White), 1.5f);
        graphics.DrawEllipse(pen, bounds.Width - 80, 30, 120, 120);
        graphics.DrawEllipse(pen, bounds.Width - 62, 48, 84, 84);
        graphics.DrawEllipse(pen, -52, bounds.Height - 112, 116, 116);
    }

    private void InitializeComponent()
    {

    }

    private static Bitmap CreatePlaceholderImage()
    {
        var bitmap = new Bitmap(1200, 900, PixelFormat.Format32bppPArgb);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using (var gradient = new LinearGradientBrush(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                   Color.FromArgb(34, 92, 112), Color.FromArgb(17, 41, 69), 35f))
            graphics.FillRectangle(gradient, 0, 0, bitmap.Width, bitmap.Height);

        using var circleA = new SolidBrush(Color.FromArgb(30, 244, 180, 80));
        using var circleB = new SolidBrush(Color.FromArgb(24, 119, 184, 166));
        using var circleC = new SolidBrush(Color.FromArgb(18, 255, 255, 255));
        graphics.FillEllipse(circleA, 720, 100, 420, 420);
        graphics.FillEllipse(circleB, -120, 470, 540, 540);
        graphics.FillEllipse(circleC, 440, 600, 330, 330);
        return bitmap;
    }
}
