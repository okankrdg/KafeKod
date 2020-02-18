using KafeKod.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KafeKod
{
    public partial class Form1 : Form
    {
        KafeVeri db;
        int masaAdet = 20;
        public Form1()
        {
            db = new KafeVeri();
            
            OrnekVerileriYukle();
            InitializeComponent();
            MasalariOlustur();
        }

        private void OrnekVerileriYukle()
        {
            db.Urunler = new List<Urun> { new Urun { UrunAd = "Kola", BirimFiyat = 6.99m }, new Urun { UrunAd = "Çay", BirimFiyat = 2.99m } };
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
            for (int i = 1; i <= masaAdet; i++)
            {
                lvi = new ListViewItem("Masa" + i);
                lvi.Tag = i;
                lvi.ImageKey = "boş";
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

                SiparisForm frmSiparis = new SiparisForm(db,sip);
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

        private void tsmiGecmisSiparisler_Click(object sender, EventArgs e)
        {
            var frm = new GecmisSiparislerForm(db);
            frm.ShowDialog();
        }
    }
}
