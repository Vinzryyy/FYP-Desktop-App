using FYP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace FYP
{
    /// <summary>
    /// Interaction logic for ProfileSelectorWindow.xaml
    /// </summary>
    public partial class ProfileSelectorWindow : Window
    {
        public InputProfile SelectedProfile { get; private set; }

        public ProfileSelectorWindow(ObservableCollection<InputProfile> profiles)
        {
            InitializeComponent();
            ProfileComboBox.ItemsSource = profiles;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (ProfileComboBox.SelectedItem is InputProfile selected)
            {
                SelectedProfile = selected;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Please select a profile to continue.");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    
    }
}
