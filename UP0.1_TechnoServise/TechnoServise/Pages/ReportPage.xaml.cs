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

namespace TechnoServise.Pages
{
    /// <summary>
    /// Логика взаимодействия для ReportPage.xaml
    /// </summary>
    public partial class ReportPage : Page
    {
        string defaultQCPContent;
        int FaultTypeCount;
        int RequestCount;
        public ReportPage()
        {
            InitializeComponent();

            defaultQCPContent = QuantityCompletedRequestsButton.Content.ToString();

        }

        private void QuantityCompletedRequestsButton_Click(object sender, RoutedEventArgs e)
        {
            ClearPage();
            QuantityCompletedRequestsLabel.Visibility = Visibility.Visible;
            QuantityCompletedRequestsLabel.Content = defaultQCPContent + " " + (TechnoServiceEntities.GetContext().Request.Where(x => x.status_id == 3).Count().ToString());
        }

        private void StatisticsOnTypesOfFaultsButton_Click(object sender, RoutedEventArgs e)
        {
            ClearPage();

            FaultTypeCount = TechnoServiceEntities.GetContext().FaultType.Count();

            for (int i = 1; i <= FaultTypeCount; i++)
            {
                FillingLabelsContent(i.ToString());
            }
        }

        private void FillingLabelsContent(string labelName)
        {
            int id = int.Parse(labelName);
            string FaultTypeName = TechnoServiceEntities.GetContext().FaultType.Where(x => x.fault_type_id == id).Select(u => u.description).FirstOrDefault();
            string FaultTypeCount = TechnoServiceEntities.GetContext().Request.Where(x => x.fault_type_id == id).Count().ToString();
            Label label = new Label()
            {
                Name = "FaultTypeQuantity" + labelName,
                Content = FaultTypeName + " " + FaultTypeCount,
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            StatisticsLabelsContainer.Children.Add(label);          
        }

        private void ClearPage()
        {
            StatisticsLabelsContainer.Children.Clear();
            QuantityCompletedRequestsLabel.Visibility= Visibility.Hidden;
            AVGTimeCompletedRequestsLabel.Visibility= Visibility.Hidden;
        }

        private void AVGTimeCompletedRequestsButton_Click(object sender, RoutedEventArgs e)
        {
            ClearPage();
            AVGTimeCompletedRequestsLabel.Visibility= Visibility.Visible;

            RequestCount = TechnoServiceEntities.GetContext().Request.Count();
            float counter = 0;
            float time = 0;

            for (int i = 1; i <= RequestCount; i++)
            {
                DateTime startDate = (DateTime)TechnoServiceEntities.GetContext().Request.Where(x => x.request_id == i).Select(u => u.date_created).FirstOrDefault();
                var middleDate = TechnoServiceEntities.GetContext().Request.Where(x => x.request_id == i && x.status_id == 3).Select(u => u.date_ended).FirstOrDefault();
                DateTime endDate;
                if (middleDate == null)
                {
                    endDate = DateTime.Now;
                }
                else { endDate = (DateTime)middleDate; }
                time += (endDate - startDate).Days;
                counter++;
            }
            AVGTimeCompletedRequestsLabel.Content = "Среднее время выполнение заявки " + (time / counter).ToString() + " дней";

        }
    }
}
