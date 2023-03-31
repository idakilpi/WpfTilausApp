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
using System.Windows.Shapes;

namespace WpfTilausApp
{
    /// <summary>
    /// Interaction logic for formTuotteet.xaml
    /// </summary>
    public partial class formTuotteet : Window
    {
        TilausDBEntities db = new TilausDBEntities();
        string dbMode = "";

        public formTuotteet()
        {
            InitializeComponent();
            HaeTuotteet();
        }

        private void HaeTuotteet()
        {
            List<Tuote> lstTuotteet = new List<Tuote>();

            var tuoteLuettelo = from tuot in db.Tuotteet
                                select tuot;

            foreach (Tuotteet tuote in tuoteLuettelo)
            {
                Tuote yksiTuote = new Tuote();
                yksiTuote.TuoteID = tuote.TuoteID.ToString();
                yksiTuote.Nimi = tuote.Nimi;
                yksiTuote.Ahinta = tuote.Ahinta?.ToString();
                yksiTuote.Kuva = tuote.Kuva;
                lstTuotteet.Add(yksiTuote);
            }

            dgTuotteet.ItemsSource = lstTuotteet.ToList();

            txtTuoteID.IsEnabled = false;
            txtTuoteNimi.IsEnabled = false;
            txtAhinta.IsEnabled = false;
            btnTallenna.IsEnabled = false;
            dbMode = "";
        }

        private void DgTuotteet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgTuotteet.SelectedIndex >= 0)
            {
                TextBlock TuoteID = dgTuotteet.Columns[0].GetCellContent(dgTuotteet.Items[dgTuotteet.SelectedIndex]) as TextBlock;
                if (TuoteID != null)
                    txtTuoteID.Text = TuoteID.Text;
                TextBlock TuoteNimi = dgTuotteet.Columns[1].GetCellContent(dgTuotteet.Items[dgTuotteet.SelectedIndex]) as TextBlock;
                if (TuoteNimi != null)
                    txtTuoteNimi.Text = TuoteNimi.Text;
                TextBlock Ahinta = dgTuotteet.Columns[2].GetCellContent(dgTuotteet.Items[dgTuotteet.SelectedIndex]) as TextBlock;
                if (Ahinta != null)
                    txtAhinta.Text = Ahinta.Text;
            }
            //db.SaveChanges();
        }

        private void BtnTallenna_Click(object sender, RoutedEventArgs e)
        {
            if (dbMode == "EDIT")
            {
                MuokkaaTietokantaa();
            }
            else if (dbMode == "ADD")
            {
                LisaaTietokantaan();
            }
            else
            {
                //
            }
            db.SaveChanges();
            HaeTuotteet();
        }


        private void BtnLisaa_Click(object sender, RoutedEventArgs e)
        {
            txtTuoteID.IsEnabled = false;
            txtTuoteNimi.IsEnabled = true;
            txtAhinta.IsEnabled = true;
            btnTallenna.IsEnabled = true;
            //tyhjennetään kentät, jotta päästään lisäämään uusi tuote
            txtTuoteID.Text = "";
            txtTuoteNimi.Text = "";
            txtAhinta.Text = "";
            dbMode = "ADD";
        }
        private void LisaaTietokantaan()
        {
            Tuotteet prod = new Tuotteet();
            prod.Nimi = txtTuoteNimi.Text;
            prod.Ahinta = decimal.Parse(txtAhinta.Text);
            db.Tuotteet.Add(prod);
            db.SaveChanges();
            HaeTuotteet();
        }

        private void BtnMuokkaa_Click(object sender, RoutedEventArgs e)
        {
            txtTuoteID.IsEnabled = false;
            txtTuoteNimi.IsEnabled = true;
            txtAhinta.IsEnabled = true;
            btnTallenna.IsEnabled = true;
            dbMode = "EDIT";
        }

        private void MuokkaaTietokantaa()
        {
            Tuotteet prod = db.Tuotteet.Find(int.Parse(txtTuoteID.Text));
            if (prod != null)
            {
                prod.Nimi = txtTuoteNimi.Text;
                prod.Ahinta = decimal.Parse(txtAhinta.Text);
                db.SaveChanges();
            }
            HaeTuotteet();
        }
        private void BtnPoista_Click(object sender, RoutedEventArgs e)
        {
            Tuotteet prod = db.Tuotteet.Find(int.Parse(txtTuoteID.Text));
            if (prod != null)
            {
                db.Tuotteet.Remove(prod);
                db.SaveChanges();
            }
            HaeTuotteet();
        }

        private void BtnSulje_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

 
    }
}
