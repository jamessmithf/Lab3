using System.Collections.ObjectModel;
using System.Text.Json;

namespace JSONEditor
{
    public partial class MainPage : ContentPage
    {
        public static string FilePath { get; set; }

        public ObservableCollection<Bike> BikesCollection { get; set; }

        public MainPage()
        {
            InitializeComponent();
            BikeList.HeightRequest = App.WindowHeight * 0.6667;
            BikesCollection = new ObservableCollection<Bike>();
            BindingContext = this;
        }

        /* Метод, що викликається на початку життєвого циклу програми */
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (FilePath != null)
            {
                RefactoryHelper.UpdateBikeList(this, BikesCollection);
            }
        }

        /* Дія кнопки "Обрати файл" */
        private async void OpenFileHandler(object sender, EventArgs e)
        {
            try
            {
                FilePath = await RefactoryHelper.ChooseFile(this, new[] { ".json" });
                if (FilePath == null)
                {
                    return;
                }
                else
                {
                    FilePathLabel.Text = $"Обрано: {FilePath}";
                    RefactoryHelper.UpdateBikeList(this, BikesCollection);
                }
            }
            catch(Exception ex) 
            {
                await DisplayAlert("Помилка", "Файл відкрився некоректно", "ОК");
                return;
            }
        }

        /* Дія кнопки "Інформація" */
        private async void InfoButtonHandler(object sender, EventArgs e)
        {
            string studentInfo = "Лабораторна робота Сікори Віктора, студента групи К-26" +
                                 "\n\nПрограма створена для роботи з файлами у форматі JSON. Вона дозволяє відкривати, редагувати, видаляти та додавати дані," +
                                 " а також швидко виконувати пошук за різними параметрами." +
                                 "\n\nПредметна область даних — велосипеди";
            await DisplayAlert("Про програму", studentInfo, "ОК");
        }

        /* Дія кнопки "+Велосипед" */
        private async void AddBikeHandler(object sender, EventArgs e)
        {
            if (FilePath == null)
                await DisplayAlert("Помилка", "Неможливо додати велосипед у ще не обраний *.json файл!", "OK");
            else
            {
                await Navigation.PushAsync(new AddBikePage());
            }
        }

        /* Дія кнопки "Пошук" */
        private async void SearchHandler(object sender, EventArgs e)
        {
            if (BikesCollection == null || !BikesCollection.Any())
            {
                await DisplayAlert("Попередження", "Список велосипедів порожній.", "OK");
                return;
            }

            string modelFilter = ModelEntry.Text?.Trim().ToLower() ?? string.Empty;
            string frameMaterialFilter = FrameMaterialEntry.Text?.Trim().ToLower() ?? string.Empty;
            string wheelDiameterFilter = (WheelDiameterEntry.Text?.Trim().ToLower() ?? string.Empty).Replace('.', ',');
            string weightFilter = (WeightEntry.Text?.Trim().ToLower() ?? string.Empty).Replace('.', ',');
            string typeFilter = TypeEntry.Text?.Trim().ToLower() ?? string.Empty;
            string descriptionFilter = DescriptionEntry.Text?.Trim().ToLower() ?? string.Empty;

            var filteredBikes = BikesCollection.Where(bike =>
                (string.IsNullOrEmpty(modelFilter) || bike.Model.ToLower().Contains(modelFilter)) &&
                (string.IsNullOrEmpty(frameMaterialFilter) || bike.FrameMaterial.ToLower().Contains(frameMaterialFilter)) &&
                (string.IsNullOrEmpty(wheelDiameterFilter) || Double.TryParse(wheelDiameterFilter, out var wheelDiameterFilterValue) && 
                Double.TryParse(bike.WheelDiameter, out var wheelDiameter) && wheelDiameter == wheelDiameterFilterValue) &&
                (string.IsNullOrEmpty(weightFilter) || Double.TryParse(weightFilter, out var weightFilterValue) && 
                Double.TryParse(bike.Weight, out var weight) && weight == weightFilterValue) &&
                (string.IsNullOrEmpty(typeFilter) || bike.Type.ToLower().Contains(typeFilter)) &&
                (string.IsNullOrEmpty(descriptionFilter) || bike.Description.ToLower().Contains(descriptionFilter))
            ).ToList();

            if (filteredBikes.Any())
            {
                BikesCollectionView.ItemsSource = new ObservableCollection<Bike>(filteredBikes);
            }
            else
            {
                await DisplayAlert("Результат", "Жоден велосипед не відповідає заданим критеріям пошуку.", "OK");
                return;
            }
        }

        /* Дія кнопки "Очистити" */
        private void ClearFiltersHander(object sender, EventArgs e)
        {
            ModelEntry.Text = string.Empty;
            FrameMaterialEntry.Text = string.Empty;
            WheelDiameterEntry.Text = string.Empty;
            WeightEntry.Text = string.Empty;
            TypeEntry.Text = string.Empty;
            DescriptionEntry.Text = string.Empty;
        }

        /* Дія кнопки "Глянути" */
        private async void ViewDescriptionHandler(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Bike bike)
            {
                string description = string.IsNullOrWhiteSpace(bike.Description) ? "Тут пусто" : bike.Description;
                await DisplayAlert("Опис велосипеда", description, "OK");
            }
        }
        
        /* Дія кнопки "Редагувати"*/
        private async void EditBikeHandler(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Bike bike)
            {
                await Navigation.PushAsync(new EditPage(bike));
            }
        }
    }
}
