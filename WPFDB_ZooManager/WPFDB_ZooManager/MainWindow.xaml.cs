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
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace WPFDB_ZooManager
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection sqlConnection;
        public MainWindow()
        {

            

            InitializeComponent();
            string connectionString = ConfigurationManager.ConnectionStrings["WPFDB_ZooManager.Properties.Settings.timmyDBConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);
            ShowZoos(); // Methode muss aufgerufen werden
            ShowAnimals();

        }

        public void ShowZoos()
        {
            try
            {
                string query = "select * from Zoo"; // gibt uns ALLES aus "Zoo" zurück
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection); // Interface um unsere Tabellen als C# Objekte nutzbart zu machen (DataTable)

                using (sqlDataAdapter)
                {
                    DataTable zooTable = new DataTable(); // Ein Objekt das wir unserer Tabelle machen (Ergebnis mit den Daten aus Zoo)
                    sqlDataAdapter.Fill(zooTable); // Fülle den ZooTable mit dem sqlAdapter

                    // Welche Informationen der Tabelle in unserem DataTable sollen in unserer ListBox angezeigt werden
                    listZoos.DisplayMemberPath = "Location";
                    // Welcher WErt soll gegeben werden, wenn eines unserer Items von der Listbox ausgewählt wird
                    listZoos.SelectedValuePath = "Id";
                    //
                    listZoos.ItemsSource = zooTable.DefaultView;
                }

            }
            catch (Exception e)
            {

                MessageBox.Show(e.ToString());  //Try & Catch damit statt Programmabsturz der komplette Fehler angezeigt wird
            }

           
        }

        public void ShowAssociatedAnimals()
        {
            if(listZoos.SelectedValue == null)
            {
                return;
            }
            else { 
            try
            {
                string query = "select * from Animal a inner join ZooAnimal za on a.Id = za.AnimalId where za.ZooId = @ZooId"; // gibt uns ALLES aus "Zoo" zurück

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand); 

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue); //Ausgewählter Wert wird übergeben

                    DataTable animalTable = new DataTable(); // Ein Objekt das wir unserer Tabelle machen (Ergebnis mit den Daten aus Zoo)
                    sqlDataAdapter.Fill(animalTable); // Fülle den ZooTable mit dem sqlAdapter

                    // Welche Informationen der Tabelle in unserem DataTable sollen in unserer ListBox angezeigt werden
                    listAssociatedAnimals.DisplayMemberPath = "Name";
                    // Welcher WErt soll gegeben werden, wenn eines unserer Items von der Listbox ausgewählt wird
                    listAssociatedAnimals.SelectedValuePath = "Id";
                    //
                    listAssociatedAnimals.ItemsSource = animalTable.DefaultView;
                }

            }
            catch (Exception e)
            {

                MessageBox.Show(e.ToString());  //Try & Catch damit statt Programmabsturz der komplette Fehler angezeigt wird
            }
            }


        }

        private void listZoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowAssociatedAnimals();
            ShowSelectedZooInTextBox();
        }



        public void ShowAnimals()
        {
            try
            {
                string query = "select * from Animal"; // gibt uns ALLES aus "Zoo" zurück
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection); // Interface um unsere Tabellen als C# Objekte nutzbart zu machen (DataTable)

                using (sqlDataAdapter)
                {
                    DataTable allAnimalTable = new DataTable(); // Ein Objekt das wir unserer Tabelle machen (Ergebnis mit den Daten aus Zoo)
                    sqlDataAdapter.Fill(allAnimalTable); // Fülle den ZooTable mit dem sqlAdapter

                    // Welche Informationen der Tabelle in unserem DataTable sollen in unserer ListBox angezeigt werden
                    listTiere.DisplayMemberPath = "Name";
                    // Welcher Wert soll gegeben werden, wenn eines unserer Items von der Listbox ausgewählt wird
                    listTiere.SelectedValuePath = "Id";
                    //
                    listTiere.ItemsSource = allAnimalTable.DefaultView;
                }

            }
            catch (Exception e)
            {

                MessageBox.Show(e.ToString());  //Try & Catch damit statt Programmabsturz der komplette Fehler angezeigt wird
            }
        }

        private void DeleteZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Zoo where id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                sqlCommand.ExecuteScalar();  // Führt eine Abfrage aus und gibt die erste Spalte der ersten Zeile im Resultset zurück
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }finally
            {

                sqlConnection.Close(); // Verbindung wieder schließen
                ShowZoos(); // Methode zum aktualisieren der DB
            }

           
        }

        public void AddZoo_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string query = "insert into Zoo values (@Location)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Location", myTextBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Fehler beim hinzufügen!");
            }
            finally
            {
                sqlConnection.Close();
                ShowZoos();
            }
            
        }

        public void AddAnimalToZoo_Click(object sender, RoutedEventArgs e)
        {
            if(listZoos.SelectedValue == null || listTiere.SelectedValue == null)
            {
                return;
            }
            else { 
            try
            {
                string query = "insert into ZooAnimal values (@ZooId, @AnimalId)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@AnimalId", listTiere.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {

                    MessageBox.Show(ex.ToString());
            }
                finally
                {
                    sqlConnection.Close();
                    ShowAssociatedAnimals();
                }
            }
        }


        public void AddAnimal_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string query = "insert into Animal values (@Name)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Name", myTextBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Fehler beim hinzufügen eines Tieres zur Tiertabelle!");
            }
            finally
            {
                sqlConnection.Close();
                ShowAnimals();
            }

        }

        private void DeleteAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Animal where id = @Name";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Name", listTiere.SelectedValue);
                sqlCommand.ExecuteScalar();  // Führt eine Abfrage aus und gibt die erste Spalte der ersten Zeile im Resultset zurück
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
            finally
            {

                sqlConnection.Close(); // Verbindung wieder schließen
                ShowAnimals(); // Methode zum aktualisieren der DB
                ShowAssociatedAnimals();
            }


        }

        private void ShowSelectedZooInTextBox()
        {
            try
            {
                string query = "select location from Zoo where Id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                    DataTable zooDataTable = new DataTable();
                    sqlDataAdapter.Fill(zooDataTable);

                    myTextBox.Text = zooDataTable.Rows[0]["Location"].ToString();
                }
            }
            catch (Exception e)
            {

                MessageBox.Show(e.ToString());
            }
        }

        private void ShowSelectedAnimalInTextBox()
        {
            try
            {
                string query = "select Name from Animal where Id = @AnimalId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@AnimalId", listTiere.SelectedValue);
                    DataTable animalDataTable = new DataTable();
                    sqlDataAdapter.Fill(animalDataTable);

                    myTextBox.Text = animalDataTable.Rows[0]["Name"].ToString();
                }
            }
            catch (Exception e)
            {

                MessageBox.Show("Fehler ShowSelectedAnimalInTextBox" );
            }
        }

        private void UpdateZoo_Click(object sender, RoutedEventArgs e)
        {
            if(listZoos.SelectedValue == null)
            {
                return;
            }
            try
            {
                string query = "update Zoo Set Location = @Location where Id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@Location", myTextBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Fehler beim aktualisieren eines Zoos");
            }
            finally
            {
                sqlConnection.Close();
                ShowZoos();
            }
        }

        private void UpdateAnimal_Click(object sender, RoutedEventArgs e)
        {
            if (listTiere.SelectedValue == null)
            {
                return;
            }
            try
            {
                string query = "update Animal Set Name = @Name where Id = @AnimalId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@AnimalId", listTiere.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@Name", myTextBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Fehler beim aktualisieren eines Zoos");
            }
            finally
            {
                sqlConnection.Close();
                ShowAnimals();
            }
        }



        private void listTiere_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowSelectedAnimalInTextBox();
        }
    }
}
