namespace SAAD
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(Views.CadastroAlunoPage), typeof(Views.CadastroAlunoPage));
            Routing.RegisterRoute(nameof(Views.CameraCapturePage), typeof(Views.CameraCapturePage));
            Routing.RegisterRoute(nameof(Views.CadastroAlunoPage), typeof(Views.CadastroAlunoPage));
        }
    }
}