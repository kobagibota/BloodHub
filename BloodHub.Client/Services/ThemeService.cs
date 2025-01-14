using MudBlazor;

namespace BloodHub.Client.Services
{
    public class ThemeService
    {
        private bool _isDarkMode;
        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                if (_isDarkMode != value)
                {
                    _isDarkMode = value;
                    NotifyStateChanged();
                }
            }
        }

        private MudTheme _lightTheme = new MudTheme
        {
            PaletteLight = new PaletteLight
            {
                Primary = Colors.Blue.Default,
                Secondary = Colors.Green.Accent3,
                Background = Colors.Gray.Lighten5,
                AppbarBackground = Colors.Blue.Darken3,
                TextPrimary = Colors.Shades.Black
            }
        };

        private MudTheme _darkTheme = new MudTheme
        {
            PaletteDark = new PaletteDark
            {
                Primary = Colors.Purple.Lighten3,
                Secondary = Colors.Green.Lighten3,
                Background = Colors.Gray.Darken4,
                AppbarBackground = Colors.Blue.Darken4,
                TextPrimary = Colors.Shades.White
            }
        };

        public MudTheme CurrentTheme => IsDarkMode ? _darkTheme : _lightTheme;

        public event Action? OnChange;

        public void ToggleDarkMode(bool isDarkMode)
        {
            IsDarkMode = isDarkMode;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
