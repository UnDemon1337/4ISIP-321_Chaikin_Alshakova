using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Логика взаимодействия для Requests.xaml
    /// </summary>
    public partial class Requests : Page
    {
        public Requests()
        {
            InitializeComponent();

            DataGridRequests.ItemsSource = TechnoServiceEntities.GetContext().Request.ToList();

            var priorityList = TechnoServiceEntities.GetContext().Priority.Select(x => x.priority_name).ToList();
            priorityList.Insert(0, "Не выбрано");
            FindForPriority.ItemsSource = priorityList;

            var statusList = TechnoServiceEntities.GetContext().Status.Select(x => x.status_name).ToList();
            statusList.Insert(0, "Не выбрано");
            FindForStatus.ItemsSource = statusList;
            FindForPriority.SelectedIndex = 0;
            FindForStatus.SelectedIndex = 0;

        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddRequests(null, 0));
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddRequests((sender as Button).DataContext as Request, Technik.positionID));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                TechnoServiceEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DataGridRequests.ItemsSource = TechnoServiceEntities.GetContext().Request.ToList();
            }
        }

        private void FindForNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateRequests();
        }

        private void FindForStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateRequests();
        }

        private void FindForPriority_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateRequests();
        }

        private void ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            FindForNumber.Text = string.Empty;
            FindForStatus.SelectedIndex = 0;
            FindForPriority.SelectedIndex = 0;

            UpdateRequests();
        }

        private void UpdateRequests()
        {
            var currentRequests = TechnoServiceEntities.GetContext().Request.ToList();
                
            currentRequests = currentRequests.Where(x => x.request_id.ToString().Contains(FindForNumber.Text)).ToList();
           
            if (FindForPriority.SelectedIndex != 0)
            {
                //currentRequests = currentRequests.Where(x => x.priority_id == ReturnPriority(FindForPriority.Text)).ToList();
                currentRequests = currentRequests.Where(x => x.priority_id ==  FindForPriority.SelectedIndex).ToList();
            }

            if (FindForStatus.SelectedIndex != 0)
            {
                //int currentStatusID = ReturnStatus(FindForStatus.SelectedItem.ToString());
                //currentRequests = currentRequests.Where(x => x.status_id == ReturnStatus(FindForStatus.Text)).ToList();
                //FindForNumber.Text = (ReturnStatus(FindForStatus.Text)).ToString();
                //currentRequests = currentRequests.Where(x => x.status_id == currentStatusID).ToList();

                currentRequests = currentRequests.Where(x => x.status_id == FindForStatus.SelectedIndex).ToList();
            }
            // Присваиваем итоговый список в источник данных
            DataGridRequests.ItemsSource = currentRequests;
        }

        private int ReturnPriority(string addPriority)
        {
            return TechnoServiceEntities.GetContext().Priority.Where(x => x.priority_name == addPriority).Select(u => u.priority_id).FirstOrDefault();
        }

        private int ReturnStatus(string addStatus)
        {
            return TechnoServiceEntities.GetContext().Status.Where(x => x.status_name == addStatus).Select(u => u.status_id).FirstOrDefault();
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ReportPage());
        }
    }
}
