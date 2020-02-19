using KafeKod.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KafeKod
{
    public partial class Form1 : Form
    {
        KafeVeri db;
        
        public Form1()
        {
            VerileriOku();
            
            
            InitializeComponent();
            MasalariOlustur();
            
        }

        private void VerileriOku()
        {
            try
            {
                string json = File.ReadAllText("veri.json");
                db = JsonConvert.DeserializeObject<KafeVeri>(json);
            }
            catch (Exception)
            {

                db = new KafeVeri();
            }
        }

        private void OrnekVerileriYukle()
        {
            db.Urunler = new List<Urun> { new Urun { UrunAd = "Kola", BirimFiyat = 6.99m }, new Urun { UrunAd = "Çay", BirimFiyat = 2.99m } };
            db.Urunler.Sort();
        }

        private void MasalariOlustur()
        {
            #region LisView Imajlarını Hazırlanması
            ImageList il = new ImageList();
            il.Images.Add("Boş", Properties.Resources.masabos);
            il.Images.Add("Dolu", Properties.Resources.masadolu);
            il.ImageSize = new Size(64, 64);
            lvwMasalar.LargeImageList = il;

            #endregion
            ListViewItem lvi = new ListViewItem();
            for (int i = 1; i <= db.MasaAdet; i++)
            {
                lvi = new ListViewItem("Masa" + i);
                //i masa noyla kayıtlı masa no var mi
                Siparis sip = db.AktifSiparisler.FirstOrDefault(x => x.MasaNo == i);
                if (sip == null)
                {
                    lvi.Tag = i;
                    lvi.ImageKey = "boş";

                }
                else
                {
                    lvi.Tag = sip;
                    lvi.ImageKey = "dolu";
                }
                lvwMasalar.Items.Add(lvi);
            }
            
        }

        private void lvwMasalar_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button==MouseButtons.Left)
            {
                var lvi = lvwMasalar.SelectedItems[0];
                lvi.ImageKey = "dolu";

                Siparis sip;
                //masa boşsa yeni sipariş oluştur
                if (lvi.Tag is Siparis)
                {
                    sip = (Siparis)lvi.Tag;
                }
                else
                {
                    sip = new Siparis();
                    sip.MasaNo = (int)lvi.Tag;
                    sip.AcilisZamani = DateTime.Now;
                    lvi.Tag = sip;
                    db.AktifSiparisler.Add(sip);
                }
                //sipariş formu oluşma anı
                SiparisForm frmSiparis = new SiparisForm(db,sip);
                frmSiparis.MasaTasindi += FrmSiparis_MasaTasindi;
                frmSiparis.ShowDialog();
                if (sip.Durum!=SiparisDurum.Aktif)
                {
                    lvi.Tag = sip.MasaNo;
                    lvi.ImageKey = "boş";
                    db.AktifSiparisler.Remove(sip);
                    db.GecmisSiparisler.Add(sip);
                }
            }
        }

        private void FrmSiparis_MasaTasindi(object sender, MasaTasimaEventArgs e)
        {
            ListViewItem lviEskiMasa = null;
            foreach (ListViewItem item in lvwMasalar.Items)
            {
                if (item.Tag==e.TasinanSiparis)
                {
                    lviEskiMasa = item;
                    break;
                }
            }
            lviEskiMasa.Tag = e.EskiMasaNo;
            lviEskiMasa.ImageKey = "boş";
        }

        private void tsmiGecmisSiparisler_Click(object sender, EventArgs e)
        {
            var frm = new GecmisSiparislerForm(db);
            frm.ShowDialog();
        }

        private void tsmiUrunler_Click(object sender, EventArgs e)
        {
            var frm = new UrunlerForm(db);
            frm.ShowDialog();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string json=JsonConvert.SerializeObject(db);
            File.WriteAllText("veri.json", json);
        }
        //public void MasaTasi(int kaynakNo,int hedefNo)
        //{
        //    lvwMasalar.Items[kaynakNo].ImageKey = "bos";
        //    lvwMasalar.Items[kaynakNo].Tag = hedefNo;
        //    lvwMasalar.Items[hedefNo].ImageKey = "dolu";
        //    lvwMasalar.Items[hedefNo].Tag = kaynakNo;
        //}
    }
}
