using SAAD.Enums;
using SAAD.Views;

namespace SAAD
{
    public partial class App : Application
    {
        public Theme CurrentTheme { get; private set; }

        // ▼▼ 1. Crie uma variável privada para guardar a SplashPage ▼▼
        private readonly SplashPage _splashPage;

        // ▼▼ 2. Modifique o construtor para "pedir" a SplashPage ▼▼
        // (Em vez de "public App()", ele agora recebe a página)
        public App(SplashPage splashPage) // 👈 A MUDANÇA ESTÁ AQUI
        {
            InitializeComponent();

            // ▼▼ 3. Salve a página que o sistema lhe deu ▼▼
            _splashPage = splashPage;

            // (Todo o seu código de Tema continua aqui, intacto)
            string themePreference = Preferences.Get("AppTheme", nameof(Theme.Light));
            CurrentTheme = (Theme)Enum.Parse(typeof(Theme), themePreference);
            SetTheme(CurrentTheme);
        }

        // ▼▼ 4. Use a página que você salvou ▼▼
        protected override Window CreateWindow(IActivationState activationState)
        {
            // (Não use mais 'new Views.SplashPage()')
            return new Window(_splashPage); // 👈 A MUDANÇA ESTÁ AQUI
        }

        //
        // O RESTO DO SEU ARQUIVO (SetTheme, ToggleTheme)
        // PERMANECE EXATAMENTE IGUAL
        //
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