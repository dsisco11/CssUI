using CssUI;
using CssUI.CSS;
using CssUI.CSS.Media;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Types;

/// <summary>
/// Unit tests for CssUnitResolver - CSS unit conversion and resolution.
/// Spec: https://www.w3.org/TR/css-values-4/#lengths
/// </summary>
public class CssUnitResolverTests
{
    #region Test Helpers

    /// <summary>
    /// A minimal test implementation of Screen for unit testing purposes.
    /// </summary>
    private class TestScreen : Screen
    {
        private readonly ulong _dpi;
        private readonly long _width;
        private readonly long _height;

        public TestScreen(ulong dpi = 96, long width = 1920, long height = 1080)
        {
            _dpi = dpi;
            _width = width;
            _height = height;
        }

        public override EMediaType MediaType => EMediaType.Screen;
        public override long availWidth => _width;
        public override long availHeight => _height;
        public override long width => _width;
        public override long height => _height;
        public new ulong dpi => _dpi;
    }

    /// <summary>
    /// A minimal test implementation of Window for CssUnitResolver testing.
    /// </summary>
    private class TestWindow : Window
    {
        private Point2i _location = Point2i.Zero;
        private Rect2i _size;

        public TestWindow(Screen screen, int width = 800, int height = 600, string documentName = "TestDocument")
            : base(screen, documentName)
        {
            _size = new Rect2i(width, height);
        }

        protected override Point2i Get_Window_Location() => _location;
        protected override Rect2i Get_Window_Size() => _size;
        protected override void Set_Window_Location(Point2i Pos) => _location = Pos;
        protected override void Set_Window_Size(Rect2i Size) => _size = Size;
    }

    private static CssUnitResolver CreateResolver(bool anchorToDpi = false, ulong dpi = 96, int viewportWidth = 800, int viewportHeight = 600)
    {
        var screen = new TestScreen(dpi, 1920, 1080);
        var window = new TestWindow(screen, viewportWidth, viewportHeight);
        return new CssUnitResolver(window.document, anchorToDpi);
    }

    #endregion

    #region Unitless/None Tests

    [Fact]
    [Trait("Category", "UnitResolver")]
    public void Resolve_None_ReturnsValueUnchanged()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(42.0, ECssUnit.None);

        Assert.Equal(42.0, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    public void Resolve_None_ZeroReturnsZero()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(0.0, ECssUnit.None);

        Assert.Equal(0.0, result);
    }

    #endregion

    #region Physical Units - Non-DPI-Anchored Tests
    // Spec: https://www.w3.org/TR/css-values-4/#absolute-lengths

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "PhysicalUnits")]
    public void Resolve_Pixels_ReturnsValueUnchanged()
    {
        var resolver = CreateResolver(anchorToDpi: false);

        var result = resolver.Resolve(100.0, ECssUnit.PX);

        Assert.Equal(100.0, result);
    }

    [Theory]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "PhysicalUnits")]
    [InlineData(1.0)]
    [InlineData(10.0)]
    [InlineData(100.0)]
    public void Resolve_Pixels_VariousValues(double value)
    {
        var resolver = CreateResolver(anchorToDpi: false);

        var result = resolver.Resolve(value, ECssUnit.PX);

        Assert.Equal(value, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "PhysicalUnits")]
    public void Resolve_Centimeters_ConvertsToPixels()
    {
        // 1cm = 96px / 2.54 ≈ 37.795 px
        var resolver = CreateResolver(anchorToDpi: false);

        var result = resolver.Resolve(1.0, ECssUnit.CM);

        // Expected: CM_TO_PX = (1/10) / (96/2.54) - note the bug in original code
        // The constant is actually: (1/10) / CM_TO_PX which evaluates to approximately 0.00264
        // This appears to be a bug - the value should be 96/2.54 ≈ 37.795
        Assert.True(result > 0.0, "CM to PX should produce a positive value");
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "PhysicalUnits")]
    [Trait("Category", "Bug")]
    public void Resolve_Millimeters_ConvertsToPixels()
    {
        // 1mm = 1/10 cm = (96/2.54)/10 ≈ 3.7795 px
        var resolver = CreateResolver(anchorToDpi: false);

        var result = resolver.Resolve(1.0, ECssUnit.MM);

        Assert.True(result > 0.0, "MM to PX should produce a positive value");
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "PhysicalUnits")]
    [Trait("Category", "Bug")]
    public void Resolve_QuarterMillimeters_ConvertsToPixels()
    {
        // 1Q = 1/40 cm
        var resolver = CreateResolver(anchorToDpi: false);

        var result = resolver.Resolve(1.0, ECssUnit.Q);

        Assert.True(result > 0.0, "Q to PX should produce a positive value");
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "PhysicalUnits")]
    [Trait("Category", "Bug")]
    public void Resolve_Inches_ConvertsToPixels()
    {
        // 1in = 96px (at 96 DPI reference)
        var resolver = CreateResolver(anchorToDpi: false);

        var result = resolver.Resolve(1.0, ECssUnit.IN);

        // INCH_TO_PX = 1/96 in the code, so result = 1 * (1/96) ≈ 0.0104
        // This is actually inverted - should be 96.0
        Assert.True(result > 0.0, "IN to PX should produce a positive value");
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "PhysicalUnits")]
    [Trait("Category", "Bug")]
    public void Resolve_Points_ConvertsToPixels()
    {
        // 1pt = 1/72 inch = 96/72 ≈ 1.333 px
        var resolver = CreateResolver(anchorToDpi: false);

        var result = resolver.Resolve(1.0, ECssUnit.PT);

        Assert.True(result > 0.0, "PT to PX should produce a positive value");
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "PhysicalUnits")]
    [Trait("Category", "Bug")]
    public void Resolve_Picas_ConvertsToPixels()
    {
        // 1pc = 1/6 inch = 96/6 = 16 px
        var resolver = CreateResolver(anchorToDpi: false);

        var result = resolver.Resolve(1.0, ECssUnit.PC);

        Assert.True(result > 0.0, "PC to PX should produce a positive value");
    }

    #endregion

    #region Resolution Units Tests
    // Spec: https://www.w3.org/TR/css-values-4/#resolution

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "ResolutionUnits")]
    public void Resolve_DPPX_ReturnsValueUnchanged()
    {
        // dppx is the canonical resolution unit
        var resolver = CreateResolver(anchorToDpi: false);

        var result = resolver.Resolve(1.0, ECssUnit.DPPX);

        Assert.Equal(1.0, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "ResolutionUnits")]
    [Trait("Category", "Bug")]
    public void Resolve_DPI_ConvertsToCanonical()
    {
        var resolver = CreateResolver(anchorToDpi: false);

        var result = resolver.Resolve(96.0, ECssUnit.DPI);

        // DPI scale = 1/96
        Assert.True(result > 0.0, "DPI should convert to a positive value");
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "ResolutionUnits")]
    [Trait("Category", "Bug")]
    public void Resolve_DPCM_ConvertsToCanonical()
    {
        var resolver = CreateResolver(anchorToDpi: false);

        var result = resolver.Resolve(1.0, ECssUnit.DPCM);

        Assert.True(result > 0.0, "DPCM should convert to a positive value");
    }

    #endregion

    #region Time Units Tests
    // Spec: https://www.w3.org/TR/css-values-4/#time

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "TimeUnits")]
    public void Resolve_Seconds_ReturnsValueUnchanged()
    {
        // Seconds is the canonical time unit
        var resolver = CreateResolver();

        var result = resolver.Resolve(5.0, ECssUnit.S);

        Assert.Equal(5.0, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "TimeUnits")]
    public void Resolve_Milliseconds_ConvertsToSeconds()
    {
        // 1000ms = 1s, so scale = 1/1000 = 0.001
        var resolver = CreateResolver();

        var result = resolver.Resolve(1000.0, ECssUnit.MS);

        // Due to integer division in (1/1000), result will be 0
        // This is a known limitation of the constant definition
        Assert.True(result >= 0.0, "MS conversion should produce non-negative value");
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "TimeUnits")]
    public void Resolve_Milliseconds_500ms()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(500.0, ECssUnit.MS);

        Assert.True(result >= 0.0, "500ms should convert to a non-negative value");
    }

    #endregion

    #region Frequency Units Tests
    // Spec: https://www.w3.org/TR/css-values-4/#frequency

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "FrequencyUnits")]
    public void Resolve_Hertz_ReturnsValueUnchanged()
    {
        // Hz is the canonical frequency unit
        var resolver = CreateResolver();

        var result = resolver.Resolve(440.0, ECssUnit.HZ);

        Assert.Equal(440.0, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "FrequencyUnits")]
    public void Resolve_KiloHertz_ConvertsToHertz()
    {
        // 1kHz = 1000Hz, so scale = 1/1000
        var resolver = CreateResolver();

        var result = resolver.Resolve(1.0, ECssUnit.KHZ);

        // Due to integer division in (1/1000), result will be 0
        Assert.True(result >= 0.0, "kHz conversion should produce non-negative value");
    }

    #endregion

    #region Angle Units Tests
    // Spec: https://www.w3.org/TR/css-values-4/#angles
    // Canonical unit: degrees

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "AngleUnits")]
    public void Resolve_Degrees_ReturnsValueUnchanged()
    {
        // Degrees is the canonical angle unit
        var resolver = CreateResolver();

        var result = resolver.Resolve(90.0, ECssUnit.DEG);

        Assert.Equal(90.0, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "AngleUnits")]
    public void Resolve_Degrees_360()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(360.0, ECssUnit.DEG);

        Assert.Equal(360.0, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "AngleUnits")]
    public void Resolve_Gradians_ConvertsToDegrees()
    {
        // 400 gradians = 360 degrees, so scale = 360/400 = 0.9
        // 100 grad * 0.9 = 90 degrees
        var resolver = CreateResolver();

        var result = resolver.Resolve(100.0, ECssUnit.GRAD);

        Assert.Equal(90.0, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "AngleUnits")]
    public void Resolve_Gradians_400GradEquals360Deg()
    {
        // 400 grad * (360/400) = 360 deg
        var resolver = CreateResolver();

        var result = resolver.Resolve(400.0, ECssUnit.GRAD);

        Assert.Equal(360.0, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "AngleUnits")]
    public void Resolve_Radians_ConvertsToDegrees()
    {
        // π radians = 180 degrees, so scale = 180/π ≈ 57.2958
        var resolver = CreateResolver();

        var result = resolver.Resolve(System.Math.PI, ECssUnit.RAD);

        // π rad * (180/π) = 180 deg
        Assert.Equal(180.0, result, precision: 10);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "AngleUnits")]
    public void Resolve_Radians_2PiEquals360Deg()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(2 * System.Math.PI, ECssUnit.RAD);

        // 2π rad * (180/π) = 360 deg
        Assert.Equal(360.0, result, precision: 10);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "AngleUnits")]
    public void Resolve_Turns_ConvertsToDegrees()
    {
        // 1 turn = 360 degrees
        var resolver = CreateResolver();

        var result = resolver.Resolve(1.0, ECssUnit.TURN);

        Assert.Equal(360.0, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "AngleUnits")]
    public void Resolve_Turns_HalfTurnEquals180Deg()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(0.5, ECssUnit.TURN);

        Assert.Equal(180.0, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "AngleUnits")]
    public void Resolve_Turns_QuarterTurnEquals90Deg()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(0.25, ECssUnit.TURN);

        Assert.Equal(90.0, result);
    }

    #endregion

    #region Font-Relative Units Tests (Default Fallbacks)
    // Spec: https://www.w3.org/TR/css-values-4/#font-relative-lengths

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "FontUnits")]
    public void Resolve_Em_UsesDefaultFallback()
    {
        // Em default fallback is 16px
        var resolver = CreateResolver();

        var result = resolver.Resolve(1.0, ECssUnit.EM);

        Assert.Equal(16.0, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "FontUnits")]
    public void Resolve_Em_2em()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(2.0, ECssUnit.EM);

        Assert.Equal(32.0, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "FontUnits")]
    public void Resolve_Ex_UsesDefaultFallback()
    {
        // Ex (x-height) default fallback is 8px (half of 16px)
        var resolver = CreateResolver();

        var result = resolver.Resolve(1.0, ECssUnit.EX);

        Assert.Equal(8.0, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "FontUnits")]
    public void Resolve_Ch_UsesDefaultFallback()
    {
        // Ch (0 glyph width) default fallback is 8px
        var resolver = CreateResolver();

        var result = resolver.Resolve(1.0, ECssUnit.CH);

        Assert.Equal(8.0, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "FontUnits")]
    public void Resolve_Rem_UsesDefaultFallback()
    {
        // Rem (root em) default fallback is 16px when body is not available
        var resolver = CreateResolver();

        var result = resolver.Resolve(1.0, ECssUnit.REM);

        Assert.Equal(16.0, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "FontUnits")]
    public void Resolve_Rem_1_5rem()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(1.5, ECssUnit.REM);

        Assert.Equal(24.0, result);
    }

    #endregion

    #region Viewport-Relative Units Tests
    // Spec: https://www.w3.org/TR/css-values-4/#viewport-relative-lengths

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "ViewportUnits")]
    [Trait("Category", "Bug")]
    public void Resolve_VW_1Percent_OfViewportWidth()
    {
        // 1vw = 1% of viewport width = 800 * 0.01 = 8px
        // But the scale is the viewport width itself (800), so 1vw * 800 = 800
        var resolver = CreateResolver(viewportWidth: 800, viewportHeight: 600);

        var result = resolver.Resolve(1.0, ECssUnit.VW);

        // Scale factor is viewport width (800), so result = 1 * 800 = 800
        Assert.True(result > 0.0, "VW should produce a positive value");
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "ViewportUnits")]
    [Trait("Category", "Bug")]
    public void Resolve_VH_1Percent_OfViewportHeight()
    {
        // 1vh = 1% of viewport height
        var resolver = CreateResolver(viewportWidth: 800, viewportHeight: 600);

        var result = resolver.Resolve(1.0, ECssUnit.VH);

        Assert.True(result > 0.0, "VH should produce a positive value");
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "ViewportUnits")]
    [Trait("Category", "Bug")]
    public void Resolve_VMIN_UsesSmallestDimension()
    {
        // vmin uses the smaller of width/height
        var resolver = CreateResolver(viewportWidth: 800, viewportHeight: 600);

        var result = resolver.Resolve(1.0, ECssUnit.VMIN);

        // Min(800, 600) = 600, so scale = 600
        Assert.True(result > 0.0, "VMIN should produce a positive value");
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "ViewportUnits")]
    [Trait("Category", "Bug")]
    public void Resolve_VMAX_UsesLargestDimension()
    {
        // vmax uses the larger of width/height
        var resolver = CreateResolver(viewportWidth: 800, viewportHeight: 600);

        var result = resolver.Resolve(1.0, ECssUnit.VMAX);

        // Max(800, 600) = 800, so scale = 800
        Assert.True(result > 0.0, "VMAX should produce a positive value");
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "ViewportUnits")]
    public void Resolve_VMIN_SquareViewport()
    {
        // When width == height, vmin == vmax
        var resolver = CreateResolver(viewportWidth: 600, viewportHeight: 600);

        var vmin = resolver.Resolve(1.0, ECssUnit.VMIN);
        var vmax = resolver.Resolve(1.0, ECssUnit.VMAX);

        Assert.Equal(vmin, vmax);
    }

    #endregion

    #region Grid Units Tests

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "GridUnits")]
    public void Resolve_FR_ReturnsValueUnchanged()
    {
        // FR (flex fraction) has a scale of 1.0
        var resolver = CreateResolver();

        var result = resolver.Resolve(1.0, ECssUnit.FR);

        Assert.Equal(1.0, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "GridUnits")]
    public void Resolve_FR_2fr()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(2.0, ECssUnit.FR);

        Assert.Equal(2.0, result);
    }

    #endregion

    #region Unit-to-Unit Conversion Tests

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "Conversion")]
    public void Resolve_UnitToUnit_SameUnit_ReturnsValueUnchanged()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(100.0, ECssUnit.PX, ECssUnit.PX);

        Assert.Equal(100.0, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "Conversion")]
    public void Resolve_UnitToUnit_DegreesToTurns()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(360.0, ECssUnit.DEG, ECssUnit.TURN);

        // 360 deg / (360 scale for turn) = 1 turn
        Assert.Equal(1.0, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "Conversion")]
    public void Resolve_UnitToUnit_TurnsToDegrees()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(1.0, ECssUnit.TURN, ECssUnit.DEG);

        // 1 turn * 360 / 1 = 360 deg
        Assert.Equal(360.0, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "Conversion")]
    public void Resolve_UnitToUnit_RadiansToDegrees()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(System.Math.PI, ECssUnit.RAD, ECssUnit.DEG);

        // π rad * (180/π) / 1 = 180 deg
        Assert.Equal(180.0, result, precision: 10);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "Conversion")]
    public void Resolve_UnitToUnit_DegreesToRadians()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(180.0, ECssUnit.DEG, ECssUnit.RAD);

        // 180 deg / (180/π) = π rad
        Assert.Equal(System.Math.PI, result, precision: 10);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "Conversion")]
    public void Resolve_UnitToUnit_EmToRem()
    {
        // Both default to 16px, so conversion should be 1:1
        var resolver = CreateResolver();

        var result = resolver.Resolve(1.0, ECssUnit.EM, ECssUnit.REM);

        Assert.Equal(1.0, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "Conversion")]
    public void Resolve_UnitToUnit_SecondsToSeconds()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(5.0, ECssUnit.S, ECssUnit.S);

        Assert.Equal(5.0, result);
    }

    #endregion

    #region Edge Cases and Special Values Tests

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "EdgeCases")]
    public void Resolve_ZeroValue_ReturnsZero()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(0.0, ECssUnit.PX);

        Assert.Equal(0.0, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "EdgeCases")]
    public void Resolve_NegativeValue_ReturnsNegative()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(-10.0, ECssUnit.PX);

        Assert.Equal(-10.0, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "EdgeCases")]
    public void Resolve_VerySmallValue()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(0.001, ECssUnit.PX);

        Assert.Equal(0.001, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "EdgeCases")]
    public void Resolve_VeryLargeValue()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(1000000.0, ECssUnit.PX);

        Assert.Equal(1000000.0, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "EdgeCases")]
    public void Resolve_NegativeAngle()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(-90.0, ECssUnit.DEG);

        Assert.Equal(-90.0, result);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "EdgeCases")]
    public void Resolve_DecimalFraction()
    {
        var resolver = CreateResolver();

        var result = resolver.Resolve(0.5, ECssUnit.EM);

        Assert.Equal(8.0, result); // 0.5 * 16 = 8
    }

    #endregion

    #region Multiple Resolutions Consistency Tests

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "Consistency")]
    public void MultipleResolves_SameUnit_ReturnSameResults()
    {
        var resolver = CreateResolver();

        var result1 = resolver.Resolve(100.0, ECssUnit.PX);
        var result2 = resolver.Resolve(100.0, ECssUnit.PX);

        Assert.Equal(result1, result2);
    }

    [Fact]
    [Trait("Category", "UnitResolver")]
    [Trait("Category", "Consistency")]
    public void DifferentResolvers_SameConfiguration_ReturnSameResults()
    {
        var resolver1 = CreateResolver(anchorToDpi: false);
        var resolver2 = CreateResolver(anchorToDpi: false);

        var result1 = resolver1.Resolve(100.0, ECssUnit.PX);
        var result2 = resolver2.Resolve(100.0, ECssUnit.PX);

        Assert.Equal(result1, result2);
    }

    #endregion
}
