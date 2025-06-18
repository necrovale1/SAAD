using SAAD.Resources.Styles;

namespace SAAD
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!Preferences.Get("UsuarioLogado", false))
            {
                // Substituição do Device.BeginInvokeOnMainThread obsoleto
                Dispatcher.Dispatch(async () =>
                {
                    await DisplayAlert("Aviso", "Você precisa fazer login", "OK");

                    // Verificação segura para Application.Current
                    if (Application.Current != null)
                    {
                        Application.Current.MainPage = new MainPage();
                    }
                });
            }
        }

        private void btnMaterias_Clicked(object sender, EventArgs e)
        {
            var materias = new Materias
            {
                Nome = "Programação Web",
                Descricao = "Programação web é o processo de desenvolvimento de websites e aplicações web utilizando diversas linguagens de programação e tecnologias para criar funcionalidades e design interativos na internet.",
                Categoria = "Web"
            };

            Navigation.PushAsync(new MateriasPage() { BindingContext = materias });
        }

        private void btnFaltas_Clicked(object sender, EventArgs e)
        {
            var faltas = new Faltas
            {
                Materia = "Programação Web",
                Falta = 5,
                Presenca = 20
            };

            Navigation.PushAsync(new FaltasPage() { BindingContext = faltas });
        }
    }
}