using System.Windows;
using System.Windows.Media;

namespace ProjektClicker
{
    public class ThemeService
    {
        private static readonly Dictionary<GameTheme, ThemePalette> Palettes = new()
        {
            [GameTheme.Default] = new ThemePalette(
                Background:    "#1E1E2E",
                PanelPrimary:  "#2A2A40",
                PanelSecondary:"#25253A",
                Surface:       "#3A3A5A",
                Accent:        "#6C63FF",
                Text:          "#FFFFFF"
            ),
            [GameTheme.MidnightBlue] = new ThemePalette(
                Background:    "#0D1B2A",
                PanelPrimary:  "#1B2A3B",
                PanelSecondary:"#162130",
                Surface:       "#1F3550",
                Accent:        "#1E90FF",
                Text:          "#FFFFFF"
            ),
            [GameTheme.Midnight] = new ThemePalette(
                Background:    "#1A1A1A",
                PanelPrimary:  "#2B2B2B",
                PanelSecondary:"#222222",
                Surface:       "#333333",
                Accent:        "#C0C0C0",
                Text:          "#E8E8E8"
            )
        };

        public void Apply(GameTheme theme)
        {
            var palette = Palettes[theme];
            var resources = Application.Current.Resources;

            resources["ThemeBackground"]    = BrushFrom(palette.Background);
            resources["ThemePanelPrimary"]  = BrushFrom(palette.PanelPrimary);
            resources["ThemePanelSecondary"]= BrushFrom(palette.PanelSecondary);
            resources["ThemeSurface"]       = BrushFrom(palette.Surface);
            resources["ThemeAccent"]        = BrushFrom(palette.Accent);
            resources["ThemeText"]          = BrushFrom(palette.Text);
        }

        private static SolidColorBrush BrushFrom(string hex)
        {
            var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
            brush.Freeze();
            return brush;
        }
    }

    public record ThemePalette(
        string Background,
        string PanelPrimary,
        string PanelSecondary,
        string Surface,
        string Accent,
        string Text);
}