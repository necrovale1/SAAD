using Microsoft.Maui.Controls;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SAAD.Views;

public partial class CadastroFacialPage : ContentPage
{
    FileResult imagemSelecionada;
    readonly string apiKey;
    readonly string endpoint;

    public CadastroFacialPage()
    {
        InitializeComponent();

        var config = App.Current.Handler.MauiContext.Services.GetService<IConfiguration>();
        apiKey = config["AzureFaceApi:Key"];
        endpoint = config["AzureFaceApi:Endpoint"];
    }

    async void OnSelecionarImagemClicked(object sender, EventArgs e)
    {
        try
        {
            imagemSelecionada = await MediaPicker.PickPhotoAsync();

            if (imagemSelecionada != null)
            {
                ImagemSelecionada.Source = ImageSource.FromFile(imagemSelecionada.FullPath);
                ResultadoLabel.Text = "";
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Falha ao selecionar imagem: {ex.Message}", "OK");
        }
    }

    async void OnDetectarFacesClicked(object sender, EventArgs e)
    {
        if (imagemSelecionada == null)
        {
            await DisplayAlert("Erro", "Selecione uma imagem primeiro.", "OK");
            return;
        }

        try
        {
            using var stream = await imagemSelecionada.OpenReadAsync();
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

            var url = $"{endpoint}/face/v1.0/detect?returnFaceLandmarks=true&returnFaceAttributes=age,gender,smile";

            using var content = new StreamContent(stream);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var response = await client.PostAsync(url, content);
            var json = await response.Content.ReadAsStringAsync();

            var resultado = JsonSerializer.Deserialize<JsonElement>(json);
            var totalFaces = resultado.GetArrayLength();

            ResultadoLabel.Text = $"{totalFaces} face(s) detectada(s).";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Falha na detecção facial: {ex.Message}", "OK");
        }
    }
}
