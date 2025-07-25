using SAAD.Services;
using SAAD.Models;

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
        LoadMaterias();
    }

    private void LoadMaterias()
    {
        // Define a fonte de dados da CollectionView como a lista do servi�o
        MateriasCollectionView.ItemsSource = MateriaService.Instance.Materias;
    }

    private async void OnRegistrarMateriaClicked(object sender, EventArgs e)
    {
        // Navega para a p�gina de registro
        await Shell.Current.GoToAsync(nameof(RegistroMateriasPage));
    }
}