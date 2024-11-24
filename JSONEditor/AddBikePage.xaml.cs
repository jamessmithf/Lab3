using System.Text.Json;

namespace JSONEditor;

public partial class AddBikePage : ContentPage
{
    public AddBikePage()
    {
        InitializeComponent();
    }

    /* Дія кнопки "Зберегти" */
    private async void SaveButtonHandler(object sender, EventArgs e)
    {
        // Перевірка на створення "пустого велосипеда"
        if (new[] { ModelEntry.Text, FrameMaterialEntry.Text, WheelDiameterEntry.Text, WeightEntry.Text, DescriptionEntry.Text }
            .All(string.IsNullOrWhiteSpace))
        {
            await DisplayAlert("Попередження!", "Не можливо додати велосипед без даних.\nНеобхідно заповнити принаймні одне поле", "ОК");
            return;
        }

        // Створення нового велосипеда
        var newBike = new Bike
        {
            Model = ModelEntry.Text?.Trim(),
            FrameMaterial = FrameMaterialEntry.Text?.Trim(),
            WheelDiameter = WheelDiameterEntry.Text?.Trim(),
            Weight = WeightEntry.Text?.Trim(),
            Description = DescriptionEntry.Text?.Trim()
        };

        // Завантаження існуючих велосипедів з файлу
        var filePath = Path.Combine(FileSystem.AppDataDirectory, MainPage.FilePath);
        List<Bike> bikesList;

        if (File.Exists(filePath))
        {
            var json = await File.ReadAllTextAsync(filePath);
            bikesList = JsonSerializer.Deserialize<List<Bike>>(json);
        }
        else bikesList = new List<Bike>();

        // Додавання нового велосипеда до списку
        bikesList.Add(newBike);

        // Збереження оновленого списку у файл
        var updatedJson = JsonSerializer.Serialize(bikesList, new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
        await File.WriteAllTextAsync(filePath, updatedJson);

        // Повернення до головної сторінки
        await Navigation.PopAsync();
    }

    /* Дія кнопки "Відмінити" */
    private async void CancelButtonHandler(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
