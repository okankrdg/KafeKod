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
        KafeContext db = new KafeContext();
        

        public Form1()
        {
            InitializeComponent();
            MasalariOlustur();
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
            lvwMasalar.Items.Clear();
            ListViewItem lvi = new ListViewItem();
            for (int i = 1; i <= Properties.Settings.Default.MasaAdet; i++)
            {
                lvi = new ListViewItem("Masa" + i);
                //i masa noyla kayıtlı masa no var mi
                Siparis sip = db.Siparisler.FirstOrDefault(x => x.MasaNo == i && x.Durum==SiparisDurum.Aktif);
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
                    sip.Durum = SiparisDurum.Aktif;
                    sip.MasaNo = (int)lvi.Tag;
                    sip.AcilisZamani = DateTime.Now;
                    lvi.Tag = sip;
                    db.Siparisler.Add(sip);
                    db.SaveChanges();
                }
                //sipariş formu oluşma anı
                SiparisForm frmSiparis = new SiparisForm(db,sip);
                frmSiparis.MasaTasinyor += FrmSiparis_MasaTasindi;
                frmSiparis.ShowDialog();
                if (sip.Durum!=SiparisDurum.Aktif)
                {
                    lvi.Tag = sip.MasaNo;
                    lvi.ImageKey = "boş";
                }
            }
        }

        private void FrmSiparis_MasaTasindi(object sender, MasaTasimaEventArgs e)
        {
            ListViewItem lviEskiMasa = MasaBul(e.EskiMasaNo);
            lviEskiMasa.Tag = e.EskiMasaNo;
            lviEskiMasa.ImageKey = "boş";


            ListViewItem lviYeniMasa = MasaBul(e.YeniMasaNo);
            lviYeniMasa.Tag = e.TasinanSiparis;
            lviYeniMasa.ImageKey = "dolu";
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
            db.Dispose();
        }
        //public void MasaTasi(int kaynakNo,int hedefNo)
        //{
        //    lvwMasalar.Items[kaynakNo].ImageKey = "bos";
        //    lvwMasalar.Items[kaynakNo].Tag = hedefNo;
        //    lvwMasalar.Items[hedefNo].ImageKey = "dolu";
        //    lvwMasalar.Items[hedefNo].Tag = kaynakNo;
        //}
        private ListViewItem MasaBul(int masaNo)
        {
            foreach (ListViewItem item in lvwMasalar.Items)
            {
                if (item.Tag is int && (int)item.Tag==masaNo)
                {
                    return item;
                }
                else if (item.Tag is Siparis && ((Siparis)item.Tag).MasaNo==masaNo)
                {
                    return item;
                }
            }
            return null;
        }

        private void tsmiAyarlar_Click(object sender, EventArgs e)
        {
            var frm = new AyarlarForm();
            DialogResult dr = frm.ShowDialog();
            if (dr==DialogResult.OK)
            {
                MasalariOlustur();
            }
        }
    }
}
