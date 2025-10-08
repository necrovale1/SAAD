using SAAD.Enums;

namespace SAAD
{
    public partial class App : Application
    {
        public Theme CurrentTheme { get; private set; }

        public App()
        {
            InitializeComponent();
            string themePreference = Preferences.Get("AppTheme", nameof(Theme.Light));
            CurrentTheme = (Theme)Enum.Parse(typeof(Theme), themePreference);
            SetTheme(CurrentTheme);
        }

        // Este é agora o local correto para definir a janela principal.
        protected override Window CreateWindow(IActivationState activationState)
        {
            return new Window(new AppShell());
        }

        public void SetTheme(Theme theme)
        {
            var mergedDictionaries = Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                mergedDictionaries.Clear();
                switch (theme)
                {
                    case Theme.Dark:
                        mergedDictionaries.Add(new Resources.Styles.DarkTheme());
                        break;
                    case Theme.Light:
                    default:
                        mergedDictionaries.Add(new Resources.Styles.LightTheme());
                        break;
                }
                CurrentTheme = theme;
                Preferences.Set("AppTheme", theme.ToString());
            }
        }

        public void ToggleTheme()
        {
            CurrentTheme = CurrentTheme == Theme.Light ? Theme.Dark : Theme.Light;
            SetTheme(CurrentTheme);
        }
    }
}