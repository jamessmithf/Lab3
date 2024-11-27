using System.Collections.ObjectModel;
using System.Text.Json;

namespace JSONEditor;

public partial class EditPage : ContentPage
{
    private Bike Bike { get; set; }

    public EditPage(Bike bike)
    {
        InitializeComponent();
        Bike = bike;
        BindingContext = Bike;
    }

    /* Дія кнопки "Зберегти" */
    private async void SaveButtonHandler(object sender, EventArgs e)
    {
        try
        {
            if (Application.Current.MainPage is NavigationPage navigationPage &&
                navigationPage.RootPage is MainPage mainPage)
            {
                var bikeInCollection = mainPage.BikesCollection.FirstOrDefault(b => b.Model == Bike.Model);

                if (new[] { ModelEntry.Text, FrameMaterialEntry.Text, WheelDiameterEntry.Text, WeightEntry.Text, DescriptionEntry.Text }
                    .All(string.IsNullOrWhiteSpace))
                {
                    bool confirmDelete = await DisplayAlert("Підтвердження", "Усі поля порожні. Видалити цей велосипед?", "Так", "Ні");
                    if (confirmDelete)
                    {
                        mainPage.BikesCollection.Remove(bikeInCollection);

                        RefactoryHelper.RewriteJson(mainPage.BikesCollection);

                        await DisplayAlert("Успіх", "Велосипед видалено!", "OK");
                        await Navigation.PopAsync();
                        return;
                    }
                    else
                    {
                        await Navigation.PopAsync();
                    }
                }

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

                // Збереження даних
                bikeInCollection.Model = ModelEntry.Text?.Trim() ?? string.Empty;
                bikeInCollection.FrameMaterial = FrameMaterialEntry.Text?.Trim() ?? string.Empty;
                bikeInCollection.WheelDiameter = WheelDiameterEntry.Text?.Trim() ?? string.Empty;
                bikeInCollection.Weight = WeightEntry.Text?.Trim() ?? string.Empty;
                bikeInCollection.Type = TypeEntry.Text?.Trim() ?? string.Empty;
                bikeInCollection.Description = DescriptionEntry.Text?.Trim() ?? string.Empty;

                // Збереження оновленого списку у файл
                RefactoryHelper.RewriteJson(mainPage.BikesCollection);

                await DisplayAlert("Успіх", "Велосипед успішно збережено!", "OK");

                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Помилка", "Головну сторінку програми не знайдено.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", $"Не вдалося зберегти велосипед через несподівану помилку:\n{ex.Message}", "OK");
        }
    }

    /* Дія кпопки "Скасувати" */
    private async void CancelButtonHandler(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
