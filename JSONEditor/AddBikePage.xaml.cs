using System.Collections.ObjectModel;
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
        // Перевірка на валідність значень в обов'язкових полях
        if (!RefactoryHelper.IsInvalidInputFieldsAlert(this, ModelEntry, WheelDiameterEntry, WeightEntry)[0])
        {
            await DisplayAlert("Попередження!", "Необхідно заповнити кожне з трьох обов'язкових полів:" +
            "\n\'Модель\', \'Діаметр коліс\' та \'Вага\'", "ОК");
            return;
        }
        else if (!RefactoryHelper.IsInvalidInputFieldsAlert(this, ModelEntry, WheelDiameterEntry, WeightEntry)[1])
        {
            await DisplayAlert("Попередження!", "Поля: \'Діаметр коліс\' та \'Вага\' повинні містити числові значення більші 0", "ОК");
            return;
        }

        // Створення нового велосипеда
        var newBike = new Bike
        {
            Model = ModelEntry.Text?.Trim(),
            FrameMaterial = FrameMaterialEntry.Text?.Trim(),
            WheelDiameter = WheelDiameterEntry.Text?.Trim(),
            Weight = WeightEntry.Text?.Trim(),
            Type = TypeEntry.Text?.Trim(),
            Description = DescriptionEntry.Text?.Trim()
        };

        // Завантаження існуючих велосипедів з файлу
        var filePath = Path.Combine(FileSystem.AppDataDirectory, MainPage.FilePath);
        ObservableCollection<Bike> bikesList;

        if (File.Exists(filePath))
        {
            var json = await File.ReadAllTextAsync(filePath);
            bikesList = JsonSerializer.Deserialize<ObservableCollection<Bike>>(json);
        }
        else bikesList = new ObservableCollection<Bike>();

        // Додавання нового велосипеда до списку
        bikesList.Add(newBike);

        // Збереження оновленого списку у файл
        try
        {
            RefactoryHelper.RewriteJson(bikesList);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помлкка", "Під час " +
                                   "\nПеревірте чи не пошкоджений файл та спробуйте ще раз", "ОК");
        }

        // Повернення до головної сторінки
        await Navigation.PopAsync();
    }

    /* Дія кнопки "Відмінити" */
    private async void CancelButtonHandler(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
