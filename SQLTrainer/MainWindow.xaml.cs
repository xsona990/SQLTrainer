using System;
using System.Collections.Generic;
using System.Data;
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
        private GeneralFunctionality func;
        private int index = 0;

        public MainWindow()
        {   
            InitializeComponent();
            func = new GeneralFunctionality();
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> data = func.GetAllTableNames();
            // ... Get the ComboBox reference.
            var comboBox = sender as ComboBox;

            // ... Assign the ItemsSource to the List.
            comboBox.ItemsSource = data;

            // ... Make the first item selected.
            comboBox.SelectedIndex = index;
            index++;
        }

        /// <summary>
        /// Метод обновляющий данные в таблицах.
        /// </summary>
        public void ReloadTables()
        {
            try
            {
                DataTable DT = func.GetTable(Table1CB.SelectedItem.ToString());
                Table1DG.ItemsSource = DT.DefaultView;
                DT = func.GetTable(Table2CB.SelectedItem.ToString());
                Table2DG.ItemsSource = DT.DefaultView;
            }
            catch (NullReferenceException)
            {
                ShowException("Нет возможности работать с БД!");
            }
            

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {

            DataTable DT = func.ExecuteStatement(SQLStatement.Text);
            //Если запрос не предполагает возвращения данных, значит он что-то изменяет и нужно обновить таблицы
            if (DT.Rows.Count == 0)
            {
                ReloadTables();
            }
            else//В противном случае, нужно вывести результат запроса в 3 таблицу
            {
                Table3DG.ItemsSource = DT.DefaultView;
            }
        }

        /// <summary>
        /// Обработчик кнопки который вызывает пересоздание таблиц в базе, и обновление ЮИ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReturnToDefault_Click(object sender, RoutedEventArgs e)
        {
            func.ReturnToDefault();
            ReloadTables();
        }

        private void Table2CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
     
            DataTable DT = func.GetTable(comboBox.SelectedItem.ToString());

            Table2DG.ItemsSource = DT.DefaultView;
        }

        private void Table1CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            DataTable DT = func.GetTable(comboBox.SelectedItem.ToString());

            Table1DG.ItemsSource = DT.DefaultView;
        }

        public static void ShowException(string ex)
        {
            MessageBox.Show("Внимание! " + ex.ToString());
        }
    }
}
