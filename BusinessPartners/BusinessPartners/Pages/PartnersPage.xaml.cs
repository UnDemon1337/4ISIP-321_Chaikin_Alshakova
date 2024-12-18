using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// Логика взаимодействия для PartnersPage.xaml
    /// </summary>
    public partial class PartnersPage : Page
    {
        public PartnersPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                MainStackPanel.Children.Clear();

                using (var context = new Entities())
                {
                    var partners = context.Partners
                        .Join(context.PartnerTypes,
                              partner => partner.PartnerTypeID,
                              type => type.PartnerTypeID,
                              (partner, type) => new
                              {
                                  PartnerID = partner.PartnerID,
                                  PartnerName = partner.PartnerName,
                                  PartnerTypeName = type.PartnerTypeName,
                                  FirstName = partner.FirstName,
                                  LastName = partner.LastName,
                                  MiddleName = partner.MiddleName,
                                  Phone = partner.Phone,
                                  Rating = partner.Rating,
                                  LegalAddress = partner.LegalAddress,
                                  Email = partner.Email
                              }).ToList();

                    foreach (var partner in partners)
                    {
                        var totalProducts = context.PartnerProducts
                            .Where(pp => pp.PartnerID == partner.PartnerID)
                            .Sum(pp => (int?)pp.Quantity) ?? 0;
                        int discount = CalculateDiscount(totalProducts);

                        Border border = new Border
                        {
                            Margin = new Thickness(10),
                            BorderBrush = System.Windows.Media.Brushes.Gray,
                            BorderThickness = new Thickness(1),
                            Padding = new Thickness(10),
                            DataContext = partner
                        };

                        StackPanel stackPanel = new StackPanel { Orientation = Orientation.Vertical };

                        Grid topGrid = new Grid();
                        topGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
                        topGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                        TextBlock partnerNameAndType = new TextBlock
                        {
                            Text = $"{partner.PartnerTypeName} | {partner.PartnerName}",
                            FontSize = 16,
                            FontWeight = FontWeights.Bold
                        };

                        TextBlock discountText = new TextBlock
                        {
                            Text = $"{discount}%",
                            FontSize = 18,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Margin = new Thickness(0, 0, 60, 0)
                        };

                        Grid.SetColumn(partnerNameAndType, 0);
                        Grid.SetColumn(discountText, 1);

                        topGrid.Children.Add(partnerNameAndType);
                        topGrid.Children.Add(discountText);

                        stackPanel.Children.Add(topGrid);

                        TextBlock details = new TextBlock
                        {
                            Text = $"{partner.LastName} {partner.FirstName} {partner.MiddleName}\n+7 {partner.Phone}\nРейтинг: {partner.Rating}",
                            FontSize = 14
                        };

                        stackPanel.Children.Add(details);

                        Button historyButton = new Button
                        {
                            Content = "История",
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Margin = new Thickness(0, 5, 0, 0)
                        };
                        historyButton.Click += (s, e) =>
                        {
                            NavigationService.Navigate(new PartnerHistoryPage(partner.PartnerID));
                        };

                        stackPanel.Children.Add(historyButton);

                        border.Child = stackPanel;

                        border.MouseLeftButtonDown += Border_MouseLeftButtonDown;

                        MainStackPanel.Children.Add(border);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Получаем объект партнёра из DataContext
            var partner = (sender as Border)?.DataContext;

            // Получаем PartnerID из анонимного типа
            int partnerID = (int)partner.GetType().GetProperty("PartnerID").GetValue(partner);

            using (var context = new Entities())
            {
                var fullPartner = context.Partners.FirstOrDefault(p => p.PartnerID == partnerID);
                NavigationService.Navigate(new AddEditPartnerPage(fullPartner));
            }
        }

        private int CalculateDiscount(int totalProducts)
        {
            if (totalProducts > 300000)
                return 15;
            else if (totalProducts >= 50000)
                return 10;
            else if (totalProducts >= 10000)
                return 5;
            else
                return 0;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddEditPartnerPage());
        }
    }
}
