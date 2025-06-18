using SAAD.Resources.Styles;

namespace SAAD;

public partial class MateriasPage : ContentPage
{
	public MateriasPage()
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