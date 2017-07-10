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

namespace SQLTrainer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DBWorker DB;
        private int index = 0;

        public MainWindow()
        {   
            InitializeComponent();
            DB = new DBWorker();
           


        }


        private List<string> Initalize()
        {
            List<string> a = DB.GetAllTableNames();
            foreach (var item in a)
            {
                MessageBox.Show(item);
                //Table1CB.ItemsSource = item;
            }
            return a;
        }


      


        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            
            List<string> data = DB.GetAllTableNames();

            // ... Get the ComboBox reference.
            var comboBox = sender as ComboBox;

            // ... Assign the ItemsSource to the List.
            comboBox.ItemsSource = data;

            // ... Make the first item selected.
            comboBox.SelectedIndex = index;
            index++;
        }





        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }


        private void Execute_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ReturnToDefault_Click(object sender, RoutedEventArgs e)
        {
            DB.ReturnToDefault();
        }

        private void Table2CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void Table1CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
