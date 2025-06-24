using Microsoft.Maui.Controls;

namespace SAAD;

public partial class MateriasPage : ContentPage
{
    public MateriasPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadMateriaData();
    }

    private void LoadMateriaData()
    {
        try
        {
            if (BindingContext == null) // Only load if not already set
            {
                if (Shell.Current?.CurrentState?.Location?.OriginalString is string location &&
                    location.Contains(nameof(MateriasPage)))
                {
                    var query = location.Split('?');
                    if (query.Length > 1)
                    {
                        var queryParams = System.Web.HttpUtility.ParseQueryString(query[1]);
                        BindingContext = new Materias
                        {
                            Nome = queryParams["Nome"] ?? "Nome não especificado",
                            Descricao = queryParams["Descricao"] ?? "Descrição não disponível",
                            Categoria = queryParams["Categoria"] ?? "Categoria não definida"
                        };
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading materia data: {ex.Message}");
            BindingContext = new Materias
            {
                Nome = "Erro ao carregar",
                Descricao = "Dados não disponíveis",
                Categoria = "Erro"
            };
        }
    }

    private async void btnVoltar_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}