using Microsoft.Maui.Controls;

namespace SAAD;

public partial class FaltasPage : ContentPage
{
    public FaltasPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadFaltasData();
    }

    private void LoadFaltasData()
    {
        try
        {
            if (Shell.Current?.CurrentState?.Location?.OriginalString is string location &&
                location.Contains(nameof(FaltasPage)))
            {
                var query = location.Split('?');
                if (query.Length > 1)
                {
                    var queryParams = System.Web.HttpUtility.ParseQueryString(query[1]);
                    BindingContext = new Faltas
                    {
                        Materia = queryParams["Materia"] ?? "Matéria não especificada",
                        Falta = int.TryParse(queryParams["Falta"], out int falta) ? falta : 0,
                        Presenca = int.TryParse(queryParams["Presenca"], out int presenca) ? presenca : 0
                    };
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading faltas data: {ex.Message}");
            BindingContext = new Faltas
            {
                Materia = "Erro ao carregar",
                Falta = 0,
                Presenca = 0
            };
        }
    }

    private async void btnVoltar_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}