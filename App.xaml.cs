using SAAD2.Enums; // Adicione esta linha

namespace SAAD2
{
    public partial class App : Application
    {
        public Theme CurrentTheme { get; private set; }

        public App()
        {
            InitializeComponent();

            // Carrega o tema salvo ou usa o padrão (Claro)
            string themePreference = Preferences.Get("AppTheme", nameof(Theme.Light));
            CurrentTheme = (Theme)Enum.Parse(typeof(Theme), themePreference);

            // Primeiro, define o tema
            SetTheme(CurrentTheme);

            // DEPOIS, cria a página principal
            MainPage = new Views.SplashPage(); ; // DEPOIS DA CORREÇÃO
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
                // Salva a preferência do usuário
                Preferences.Set("AppTheme", theme.ToString());
            }

        }
    }
}