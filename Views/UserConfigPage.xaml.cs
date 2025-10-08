using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using SAAD2.Models;
using System.Threading.Tasks;

namespace SAAD2.Views;

// Nome da classe corrigido para UserConfigPage
public partial class UserConfigPage : ContentPage
{
    private readonly FirebaseClient firebaseClient = new FirebaseClient("https://saad-1fd38-default-rtdb.firebaseio.com/");
    private readonly FirebaseStorage firebaseStorage = new FirebaseStorage("saad-1fd38.appspot.com");
    private string userUid;
    private FileResult photo;

    public UserConfigPage()
    {
        InitializeComponent();
        ObservacoesEditor.IsVisible = false;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        userUid = Preferences.Get("UserUid", string.Empty);

        if (string.IsNullOrEmpty(userUid))
        {
            await DisplayAlert("Erro", "Sessão expirada. Faça login novamente.", "OK");
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }
        await LoadUserProfile();
    }

    private async Task LoadUserProfile()
    {
        try
        {
            // Usando o nome completo para desambiguação
            var user = await firebaseClient.Child("Users").Child(userUid).OnceSingleAsync<SAAD2.Models.User>();
            if (user != null)
            {
                NomeEntry.Text = user.Nome;
                IdadeEntry.Text = user.Idade > 0 ? user.Idade.ToString() : string.Empty;
                TelefoneEntry.Text = user.Telefone;
                RgEntry.Text = user.Rg;
                CpfEntry.Text = user.Cpf;
                SexoPicker.SelectedItem = user.Sexo;
                RegistroAcademicoEntry.Text = user.RegistroAcademico;
                InstituicaoEntry.Text = user.Instituicao;
                CursoEntry.Text = user.Curso;
                SemestreEntry.Text = user.Semestre > 0 ? user.Semestre.ToString() : string.Empty;
                PeriodoPicker.SelectedItem = user.Periodo;
                NecessidadesPicker.SelectedItem = user.TipoNecessidadeEspecial;
                ObservacoesEditor.Text = user.Observacoes;

                if (!string.IsNullOrEmpty(user.FotoPerfilUrl))
                {
                    ProfileImage.Source = ImageSource.FromUri(new Uri(user.FotoPerfilUrl));
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Perfil não encontrado ou erro ao carregar: {ex.Message}");
        }
    }

    private async void OnAlterarFotoClicked(object sender, EventArgs e)
    {
        photo = await MediaPicker.PickPhotoAsync();
        if (photo != null)
        {
            ProfileImage.Source = ImageSource.FromFile(photo.FullPath);
        }
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(userUid)) return;

        var existingUser = await firebaseClient.Child("Users").Child(userUid).OnceSingleAsync<SAAD2.Models.User>();
        string fotoUrl = existingUser?.FotoPerfilUrl;

        if (photo != null)
        {
            var stream = await photo.OpenReadAsync();
            fotoUrl = await firebaseStorage.Child("profile_pictures").Child($"{userUid}.jpg").PutAsync(stream);
        }

        // Usando o nome completo para desambiguação
        var userProfile = new SAAD2.Models.User
        {
            Nome = NomeEntry.Text,
            Idade = int.TryParse(IdadeEntry.Text, out var idade) ? idade : 0,
            Telefone = TelefoneEntry.Text,
            Rg = RgEntry.Text,
            Cpf = CpfEntry.Text,
            Sexo = SexoPicker.SelectedItem as string,
            FotoPerfilUrl = fotoUrl,
            RegistroAcademico = RegistroAcademicoEntry.Text,
            Instituicao = InstituicaoEntry.Text,
            Curso = CursoEntry.Text,
            Semestre = int.TryParse(SemestreEntry.Text, out var semestre) ? semestre : 0,
            Periodo = PeriodoPicker.SelectedItem as string,
            TipoNecessidadeEspecial = NecessidadesPicker.SelectedItem as string,
            Observacoes = ObservacoesEditor.Text,
            Email = Preferences.Get("UserEmail", string.Empty)
        };

        await firebaseClient.Child("Users").Child(userUid).PutAsync(userProfile);
        await DisplayAlert("Sucesso", "Seu perfil foi atualizado!", "OK");
    }

    private void OnNecessidadesPickerChanged(object sender, EventArgs e)
    {
        var selected = NecessidadesPicker.SelectedItem as string;
        ObservacoesEditor.IsVisible = !string.IsNullOrEmpty(selected) && selected != "Nenhuma";
    }
}