namespace SAAD;

public partial class RecuperarSenha : ContentPage
{
    public RecuperarSenha()
    {
        InitializeComponent();
    }

    private async void OnEnviarClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            await DisplayAlert("Enviado!", $"Um link de recuperação foi enviado para o e-mail: {EmailEntry.Text}", "OK");
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await DisplayAlert("Erro", "Por favor, digite um e-mail válido.", "OK");
        }
    }

    private async void OnVoltarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}