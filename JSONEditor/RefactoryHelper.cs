using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JSONEditor
{
    static class RefactoryHelper
    {
        /// <summary>
        /// Відкриває діалог вибору файлу, де всі доступні файли типу <paramref name="expectedTypes"/>, 
        /// і перевіряє вміст цього файлу, щоб переконатися, що це валідний JSON-файл. 
        /// Якщо файл некоректний, виводить повідомлення про помилку на сторінку <paramref name="page"/>.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="expectedTypes"></param>
        /// <returns></returns>
        public static async Task<string?> ChooseFile(Page page, string[] expectedTypes)
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, expectedTypes }
            });

            var result = await FilePicker.PickAsync(new PickOptions { FileTypes = customFileType });
            try
            {
                // пробна десеріалізація з метою виявити потенційну помилку
                var json = await File.ReadAllTextAsync(result.FullPath);
                var Bikes = JsonSerializer.Deserialize<ObservableCollection<Bike>>(json);
                return result.FullPath;
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Помилка", "Файл відкрився некоректно", "ОК");
                return null;
            }
        }

        /// <summary>
        /// Не дозволяє:
        /// 1) Вводити порожні <paramref name="ModelEntry"/>, <paramref name="WheelDiameterEntry"/> та <paramref name="WeightEntry"/> поля; 
        /// 2) Вводити числові значення не більші за 0 або нечислові значення у поля <paramref name="WheelDiameterEntry"/> та <paramref name="WeightEntry"/>
        /// </summary>
        /// <param name="ModelEntry"></param>
        /// <param name="WheelDiameterEntry"></param>
        /// <param name="WeightEntry"></param>
        public static bool[] IsInvalidInputFieldsAlert(Page page, Entry ModelEntry, Entry WheelDiameterEntry, Entry WeightEntry) 
        {
            // Робимо заміну крапок на коми у числових значеннях задля коректного парсиингу
            if (!string.IsNullOrEmpty(WheelDiameterEntry.Text))
                WheelDiameterEntry.Text = WheelDiameterEntry.Text.Replace('.', ',');
            if (!string.IsNullOrEmpty(WeightEntry.Text))
                WeightEntry.Text = WeightEntry.Text.Replace('.', ',');

            double wheelDiameter;
            bool isDoubleDiameter = Double.TryParse(WheelDiameterEntry.Text, out wheelDiameter);

            double weight;
            bool isDoubleWeight = Double.TryParse(WeightEntry.Text, out weight);


            if (new[] { ModelEntry.Text, WheelDiameterEntry.Text, WeightEntry.Text }.Any(string.IsNullOrWhiteSpace))
            {
                return [false, true];
            }
            else if (!(wheelDiameter > 0 && weight > 0 || string.IsNullOrWhiteSpace(WeightEntry.Text)))
            {
                
                return [true, false];
            }
            else
            {
                return [true, true];
            }
        }

        /// <summary>
        /// Допоміжний метод, який перезаписує *.json файл враховуючи оновлений список <paramref name="data"/>
        /// </summary>
        /// <param name="data"></param>
        public static void RewriteJson(ObservableCollection<Bike> data)
        {
            string updatedJson = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            File.WriteAllText(MainPage.FilePath, updatedJson);
        }

        /// <summary>
        /// Парсить *.json файл та оновлює <paramref name="BikesCollection"/> на основі отриманих даних
        /// </summary>
        public static async void UpdateBikeList(Page page, ObservableCollection<Bike> BikesCollection)
        {
            if (string.IsNullOrEmpty(MainPage.FilePath)) return;

            try
            {
                string jsonContent = File.ReadAllText(MainPage.FilePath);

                var bikes = JsonSerializer.Deserialize<List<Bike>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (bikes == null) return;

                bikes.RemoveAll(bike =>
                    string.IsNullOrWhiteSpace(bike.Model) ||
                    string.IsNullOrWhiteSpace(bike.WheelDiameter) ||
                    string.IsNullOrWhiteSpace(bike.Weight));

                if (bikes != null)
                {
                    BikesCollection.Clear();
                    foreach (var bike in bikes)
                    {
                        BikesCollection.Add(bike);
                    }
                }
            }
            catch (Exception)
            {
                await page.DisplayAlert("Помилка", "Помилка під час обробки файлу.", "OK");
            }
        }
    }
}
