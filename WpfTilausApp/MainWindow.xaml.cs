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

namespace WpfTilausApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Alustukset
        DateTime tanaan = DateTime.Today;
        Decimal TilauksenSummaYht = 0;

        TilausDBEntities db = new TilausDBEntities();
        
        public MainWindow()
        {
            InitializeComponent();

            #region rutiinit
            HaeAsiakkaat();                                             //Täytetään Asiakas -comboboxin sisältö    
            HaeTuotteet();                                              //Täytetään Tuote -comboboxin sisältö
            dpTilausPvm.SelectedDate = tanaan;
            dpToimitusPvm.SelectedDate = tanaan.AddDays(14);
            AsetaTilausRiviGridinSarakkeet();                           //Asetetaan tilausrivitaulukon sarakkeet
            #endregion
        }

        #region 1. Hae asiakkaat
        private void HaeAsiakkaat()
        {
            List<cbPairAsiakas> cbpairAsiakkaat = new List<cbPairAsiakas>(); //Luodaan olemassa olevien asiakasnimi-asiakasnumero -parien lista

            var asiakkaat = from asiak in db.Asiakkaat
                            select asiak;                               //Haetaan asiakkaan tietokannasta LINQ -kyselyllä

            foreach (var asiakas in asiakkaat)                          //Käydään kaikki asiakkaat läpi silmukassa
            {
                cbpairAsiakkaat.Add(new cbPairAsiakas(asiakas.Nimi, asiakas.AsiakasID, asiakas.Osoite, asiakas.Postinumero)); //Viedään asiakkaat listalle
            }
            cbAsiakas.DisplayMemberPath = "asiakasNimi";                //Kerrotaan pudotusvalikolle (ComboBox) mikä kenttä näytetään
            cbAsiakas.SelectedValuePath = "asiakasNro";
            cbAsiakas.SelectedValuePath = "asiakasOsoite";              //Kerrotaan pudotusvalikolle (ComboBox) missä kentässä on arvotieto (joka siis viedään myöhemmin tkantaan tilaukselle)
            cbAsiakas.SelectedValuePath = "asiakasPostinro";
            cbAsiakas.ItemsSource = cbpairAsiakkaat;                    //Kerrotaan, että pudotusvalikon tietolähde on cbpairAsiakkaat-lista
        }
        #endregion

        #region 2. Asiakas dropdown
        private void CbAsiakas_DropDownClosed(object sender, EventArgs e) //Tämä eventti laukeaa aina, kun pudotusvalikko sulkeutuu
        {
            cbPairAsiakas cbp = (cbPairAsiakas)cbAsiakas.SelectedItem;
            txtAsiakasNumero.Text = cbp.asiakasNro.ToString();          //Asiakasnumero on kannassa int
            txtAsiakasOsoite.Text = cbp.asiakasOsoite;                  //Asiakkaan osoite lisätty 22.11.2019 JSO
            txtPostinro.Text = cbp.asiakasPostinro;                     //Asiakkaan postinro lisätty 28.4.2020 JM
        }
        #endregion


        #region 3. Tallenna asiakas ja tilaus
        private void BtnTallenna_Click(object sender, RoutedEventArgs e)
        {
            TilausOtsikko Tilaus = new TilausOtsikko();
            Tilaus.AsiakasNumero = int.Parse(txtAsiakasNumero.Text);    //Tämä on poimittu ComboBoxista (cbPairAsiakas-olion avulla)
            Tilaus.ToimitusOsoite = "";                                 //Lisää toimitusosoite käyttöliittymään

            //Tilaus.Postinumero = "53810";                             //Lisää textbox käyttöliittymään (huomaa, että tk vaatii olemassa olevan postinumeron viite-eheyssääntöjen takia)
            Tilaus.Postinumero = txtPostinro.Text;
            Tilaus.TilausPvm = dpTilausPvm.SelectedDate.Value;          //lisää DatePicker -komponentti käyttöliittymään
            Tilaus.ToimitusPvm = dpToimitusPvm.SelectedDate.Value;      //lisää DatePicker -komponentti käyttöliittymään
            //Tilaus.TilausPvm = DateTime.Today;                        //korvaa tämä alla olevalla esimerkilla, kun olet lisännyt DatePickerin
            //Tilaus.ToimitusPvm = DateTime.Now.AddDays(2);             //korvaa tämä alla olevalla esimerkilla, kun olet lisännyt DatePickerin

            txtToimitusAika.Text = Tilaus.LaskeToimitusAika();

            txtTilausNumero.Text = VieTilausKantaan(Tilaus);            //Tässä tietojen vienti kantaan. Katso VieTilausKantaan() -rutiinin koodi alempana
        }
        #endregion

        #region 4. Rutiini, joka vie tilausdatan kantaan.
        private string VieTilausKantaan(TilausOtsikko Tilaus)           //Huomaa, että rutiini palauttaa stringin (uuden tilauksen numeron)
        {
            try
            {
                Tilaukset dbItem = new Tilaukset()
                {
                    AsiakasID = Tilaus.AsiakasNumero,                   //Tilaus on tilausotisikko-tyyppinen olio, jonka tämä rutiini saa parametrinä kutsuvasta ohjelmasta
                    Toimitusosoite = Tilaus.ToimitusOsoite,
                    Postinumero = Tilaus.Postinumero,
                    Tilauspvm = Tilaus.TilausPvm,
                    Toimituspvm = Tilaus.ToimitusPvm
                };

                db.Tilaukset.Add(dbItem);
                db.SaveChanges();

                int id = dbItem.TilausID;                               //Haetaan juuri lisätyn rivin IDENTITEETTIsarakkeen arvo (eli PK)
                return id.ToString();                                   //Palautetaan onnistuneen lisäyksen merkiksi uuden tilauksen numero
            }
            catch (Exception)
            {
                return "0";                                             //Jos tallennus tietokantaan epäonnistuu, tämä rutiini palauttaa nollan
            }
        }
        #endregion

        #region 5. Hae tuotteet
        private void HaeTuotteet()
        {
            List<cbPairTuote> cbpairTuotteet = new List<cbPairTuote>(); //Luodaan olemassa olevien asiakasnimi-asiakasnumero -parien lista
      
            var tuotteet = from prod in db.Tuotteet
                            orderby prod.Nimi
                            select prod;                                //Haetaan tuotteet tietokannasta LINQ -kyselyllä

            foreach (var tuote in tuotteet)                             //Käydään kaikki tuotteet läpi silmukassa
            {
                cbpairTuotteet.Add(new cbPairTuote(tuote.Nimi, tuote.TuoteID, (decimal)tuote.Ahinta)); //Viedään tuotteet listalle
            }
            cbTuote.DisplayMemberPath = "tuoteNimi";                    //Kerrotaan pudotusvalikolle (ComboBox) mikä kenttä näytetään
            cbTuote.SelectedValuePath = "tuoteNro";                     //Kerrotaan pudotusvalikolle (ComboBox) missä kentässä on arvotieto (joka siis viedään myöhemmin tkantaan tilaukselle)
            cbTuote.ItemsSource = cbpairTuotteet;                       //Kerrotaan, että pudotusvalikon tietolähde on cbpairTuotteet-lista
        }
        #endregion

        #region 6. Tuotteet dropdown
        private void CbTuote_DropDownClosed(object sender, EventArgs e)
        {
            cbPairTuote cbp = (cbPairTuote)cbTuote.SelectedItem;
            txtTuoteNumero.Text = cbp.tuoteNro.ToString();              //Tuotenumero on kannassa int
            txtAHinta.Text = cbp.aHinta.ToString();                     //Tuotteen a-hinta
        }
        #endregion

        #region 7. Tallenna tilausrivit
        private void BtnLisaaRivi_Click(object sender, RoutedEventArgs e)
        {
            try 
            { 
                    TilausRivi tilausRivi = new TilausRivi();
                    tilausRivi.TilausNumero = int.Parse(txtTilausNumero.Text);
                    tilausRivi.TuoteNumero = int.Parse(txtTuoteNumero.Text);
                    tilausRivi.TuoteNimi = cbTuote.Text;
                    tilausRivi.Maara = int.Parse(txtMaara.Text);
                    tilausRivi.AHinta = Convert.ToDecimal(txtAHinta.Text);
           
                    tilausRivi.TilausRiviNumero = VieTilausRiviKantaan(tilausRivi);

                    TilauksenSummaYht += tilausRivi.RiviSumma();        //Kuten tämä: TilauksenSummaYht = TilauksenSummaYht + TilausR.RiviSumma();
                    txtTilausSumma.Text = TilauksenSummaYht.ToString();
                    
                    //SUPERKOMENTO! Vie uuden rivin datagridiin ja kentät löytävät oikean paikan luokan 
                    //ominaisuuksien nimien perusteella
                    dgTilausRivit.Items.Add(tilausRivi);
            }
            catch (Exception ex)
            {
                    MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 8. Vie tilausrivit kantaan
        private int VieTilausRiviKantaan(TilausRivi tilausRivi)
        {
            Tilausrivit dbItem = new Tilausrivit()
            {
                TilausID = tilausRivi.TilausNumero,
                TuoteID = tilausRivi.TuoteNumero,
                Maara = tilausRivi.Maara,
                Ahinta = tilausRivi.AHinta
            };

            db.Tilausrivit.Add(dbItem);
            db.SaveChanges();

            int id = dbItem.TilausriviID;                               //Haetaan juuri lisätyn rivin IDENTITEETTIsarakkeen arvo (eli PK)
            return id;                                                  //Paluuarvo
        }

        private void TxtMaara_LostFocus(object sender, RoutedEventArgs e)
        {
            txtRiviSumma.Text = (decimal.Parse(txtAHinta.Text) * decimal.Parse(txtMaara.Text)).ToString();
        }
        #endregion

        #region AsetaTilausRiviGridinSarakkeet (lisää rutiini mainwindow)
        private void AsetaTilausRiviGridinSarakkeet()
        {
            //Luodaan ikäänkuin ilmaan DataGridTextColumn -tyyppisiä olioita
            DataGridTextColumn colTilausRiviNumero = new DataGridTextColumn();
            DataGridTextColumn colTilausNumero = new DataGridTextColumn();
            DataGridTextColumn colTuoteNumero = new DataGridTextColumn();
            DataGridTextColumn colTuoteNimi = new DataGridTextColumn();
            DataGridTextColumn colMaara = new DataGridTextColumn();
            DataGridTextColumn colAHinta = new DataGridTextColumn();
            DataGridTextColumn colRivinSumma = new DataGridTextColumn();            
            //Oliot sidotaan tietyn nimiseen sarakkeeseen < --kohdistuu myöhemmin olion ominaisuuksiin, kunhan olio on ensin viety listalle
            colTilausRiviNumero.Binding = new Binding("TilausRiviNumero");
            colTilausNumero.Binding = new Binding("TilausNumero");
            colTuoteNumero.Binding = new Binding("TuoteNumero");
            colTuoteNimi.Binding = new Binding("TuoteNimi");
            colMaara.Binding = new Binding("Maara");
            colAHinta.Binding = new Binding("AHinta");
            colRivinSumma.Binding = new Binding("Summa");
            //DataGridin otsikot 
            colTilausRiviNumero.Header = "Tilausrivin numero";
            colTilausNumero.Header = "Tilauksen numero";
            colTuoteNumero.Header = "Tuotenumero";
            colTuoteNimi.Header = "Tuotenimi";
            colMaara.Header = "Määrä";
            colAHinta.Header = "A-Hinta";
            colRivinSumma.Header = "Rivin summa";
            //Edellä "ilmaan" luotujen sarakkeiden sijoitus konkreettiseen DataGridiin, joka on luotu formille
            dgTilausRivit.Columns.Add(colTilausRiviNumero);
            dgTilausRivit.Columns.Add(colTilausNumero);
            dgTilausRivit.Columns.Add(colTuoteNumero);
            dgTilausRivit.Columns.Add(colTuoteNimi);
            dgTilausRivit.Columns.Add(colMaara);
            dgTilausRivit.Columns.Add(colAHinta);
            dgTilausRivit.Columns.Add(colRivinSumma);
        }
        #endregion


        private void BtnSulje_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region newWindow
        //https://youtu.be/7vEpc2Vjxo4
        private void BtnPostitoimiPaikat_Click(object sender, RoutedEventArgs e)
        {
            formPostitoimiPaikat frmPostmp = new formPostitoimiPaikat();
            frmPostmp.Show();
        }

        private void BtnTuotteet_Click(object sender, RoutedEventArgs e)
        {
            formTuotteet frmTuotteet = new formTuotteet();
            frmTuotteet.Show();
        }

        #endregion

       
    }
}
