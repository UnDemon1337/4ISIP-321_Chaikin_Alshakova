using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BusinessPartners.Pages
{
    public partial class AddEditPartnerPage : Page
    {
        private BusinessPartners.Partners _currentPartner;

        public AddEditPartnerPage(BusinessPartners.Partners selectedPartner = null)
        {
            InitializeComponent();

            _currentPartner = selectedPartner ?? new BusinessPartners.Partners();

            DataContext = _currentPartner;

            using (var context = new Entities())
            {
                ComboBoxPartnerType.ItemsSource = context.PartnerTypes.ToList();
                ComboBoxPartnerType.DisplayMemberPath = "PartnerTypeName";
                ComboBoxPartnerType.SelectedValuePath = "PartnerTypeID";

                if (_currentPartner.PartnerID != 0)
                    ComboBoxPartnerType.SelectedValue = _currentPartner.PartnerTypeID;
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentPartner.PartnerName))
                errors.AppendLine("Укажите наименование партнёра!");
            if (ComboBoxPartnerType.SelectedValue == null)
                errors.AppendLine("Выберите тип партнёра!");
            if (!int.TryParse(TextBoxRating.Text, out int rating) || rating < 0)
                errors.AppendLine("Рейтинг должен быть целым положительным числом!");
            if (string.IsNullOrWhiteSpace(_currentPartner.LegalAddress))
                errors.AppendLine("Укажите адрес!");
            if (string.IsNullOrWhiteSpace(_currentPartner.LastName))
                errors.AppendLine("Укажите фамилию директора!");
            if (string.IsNullOrWhiteSpace(_currentPartner.FirstName))
                errors.AppendLine("Укажите имя директора!");
            if (string.IsNullOrWhiteSpace(_currentPartner.MiddleName))
                errors.AppendLine("Укажите отчество директора!");
            if (string.IsNullOrWhiteSpace(_currentPartner.Phone))
                errors.AppendLine("Укажите телефон!");
            if (string.IsNullOrWhiteSpace(_currentPartner.Email))
                errors.AppendLine("Укажите email компании!");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            _currentPartner.PartnerTypeID = (int)ComboBoxPartnerType.SelectedValue;

            using (var context = new Entities())
            {
                if (_currentPartner.PartnerID == 0)
                    context.Partners.Add(_currentPartner);
                else
                    context.Entry(_currentPartner).State = System.Data.Entity.EntityState.Modified;

                try
                {
                    context.SaveChanges();
                    MessageBox.Show("Данные успешно сохранены!");
                    NavigationService.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}");
                }
            }
        }
    }
}
