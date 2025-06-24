using SAAD.Services;

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
        FaltasCollectionView.ItemsSource = FaltaService.Instance.ListaFaltas;
    }

    private async void OnRegistrarFaltasClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RegistroFaltasPage));
    }
}