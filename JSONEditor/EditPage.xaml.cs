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

    /* ĳ� ������ "��������" */
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
                    bool confirmDelete = await DisplayAlert("ϳ�����������", "�� ���� ������. �������� ��� ���������?", "���", "ͳ");
                    if (confirmDelete)
                    {
                        mainPage.BikesCollection.Remove(bikeInCollection);

                        RefactoryHelper.RewriteJson(mainPage.BikesCollection);

                        await DisplayAlert("����", "��������� ��������!", "OK");
                        await Navigation.PopAsync();
                        return;
                    }
                    else
                    {
                        await Navigation.PopAsync();
                    }
                }

                // �������� �� �������� ������� � ����'������� �����
                if (!RefactoryHelper.IsInvalidInputFieldsAlert(this, ModelEntry, WheelDiameterEntry, WeightEntry)[0])
                {
                    await DisplayAlert("������������!", "��������� ��������� ����� � ����� ����'������� ����:" +
                    "\n\'������\', \'ĳ����� ����\' �� \'����\'", "��");
                    return;
                }
                else if (!RefactoryHelper.IsInvalidInputFieldsAlert(this, ModelEntry, WheelDiameterEntry, WeightEntry)[1])
                {
                    await DisplayAlert("������������!", "����: \'ĳ����� ����\' �� \'����\' ������ ������ ������ �������� ����� 0", "��");
                    return;
                }

                // ���������� �����
                bikeInCollection.Model = ModelEntry.Text?.Trim() ?? string.Empty;
                bikeInCollection.FrameMaterial = FrameMaterialEntry.Text?.Trim() ?? string.Empty;
                bikeInCollection.WheelDiameter = WheelDiameterEntry.Text?.Trim() ?? string.Empty;
                bikeInCollection.Weight = WeightEntry.Text?.Trim() ?? string.Empty;
                bikeInCollection.Type = TypeEntry.Text?.Trim() ?? string.Empty;
                bikeInCollection.Description = DescriptionEntry.Text?.Trim() ?? string.Empty;

                // ���������� ���������� ������ � ����
                RefactoryHelper.RewriteJson(mainPage.BikesCollection);

                await DisplayAlert("����", "��������� ������ ���������!", "OK");

                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("�������", "������� ������� �������� �� ��������.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("�������", $"�� ������� �������� ��������� ����� ���������� �������:\n{ex.Message}", "OK");
        }
    }

    /* ĳ� ������ "���������" */
    private async void CancelButtonHandler(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
