using SAAD.Resources.Styles;

namespace SAAD;

public partial class FaltasPage : ContentPage
{
    public FaltasPage()
    {
        InitializeComponent();
    }

    private void btnVoltar_Clicked(object sender, EventArgs e)
    {
        {
            if (Application.Current != null)
            {
                Application.Current.MainPage = new AppShell();
            }
        }
    }
}