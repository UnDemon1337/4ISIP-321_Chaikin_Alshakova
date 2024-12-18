using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логика взаимодействия для AuthorizePage.xaml
    /// </summary>
    public partial class AuthorizePage : Page
    {
        public AuthorizePage()
        {
            InitializeComponent();
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Login.Text) || string.IsNullOrEmpty(Password.Password))
            {
                MessageBox.Show("Вы не заполнели все поля");
            }
            else
            {
                string login = Login.Text;
                string password = Password.Password;

                try
                {
                    using (var db = new TechnoServiceEntities())
                    {
                        var user = db.Users
                            .AsNoTracking().FirstOrDefault(u => u.Login == login && u.Password == password);
                        if (user == null)
                        {
                            MessageBox.Show("Пользователь не зарегистрирован");
                            return;
                        }
                        MessageBox.Show("Вы успешно авторизовались");
                        NavigationService.Navigate(new Requests());

                        //Technik.positionID = ReturnPositionID(user.TechnicianID);
                        //switch (Technik.positionID)
                        //{
                        //    case 2:
                        //        NavigationService.Navigate(new Requests());
                        //        break;
                        //    case 3:
                        //        NavigationService.Navigate(new Requests());
                        //        break;
                        //}


                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }        
            }           
        }

        private int ReturnPositionID(int? TechnicianID)
        {
            var db = new TechnoServiceEntities();
            var technician = db.Technician.AsNoTracking().FirstOrDefault(u => u.technician_id == TechnicianID);

            return (int)technician.position_id;
        }
    }
}
