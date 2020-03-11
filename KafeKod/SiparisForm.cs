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
    public partial class SiparisForm : Form
    {
        public event EventHandler<MasaTasimaEventArgs> MasaTasinyor;
        KafeContext db;
        Siparis siparis;
        
        public SiparisForm(KafeContext kafeVeri,Siparis siparis)
        {
            db = kafeVeri;
            this.siparis = siparis;
            
            InitializeComponent();
            dgvSiparisDetaylari.AutoGenerateColumns = false;
            MasaNoGuncelle();
            cboUrun.DataSource = db.Urunler.ToList();
            TutarGuncelle();
            dgvSiparisDetaylari.DataSource = siparis.SiparisDetaylar;
            MasaNolariYukle();
            
        }

        private void MasaNolariYukle()
        {
            cboMasaNo.Items.Clear();
            for (int i = 1; i <= Properties.Settings.Default.MasaAdet; i++)
            {
                if (!db.Siparisler.Any(x=>x.MasaNo==i && x.Durum==SiparisDurum.Aktif))
                {
                    cboMasaNo.Items.Add(i);

                }
            }
        }

        private void TutarGuncelle()
        {
            lblTutar.Text = siparis.SiparisDetaylar.Sum(x=>x.Adet*x.BirimFiyat).ToString("0:0.00");
        }

        private void MasaNoGuncelle()
        {
            Text = "Masa " + siparis.MasaNo;
            lblMasaNo.Text = siparis.MasaNo.ToString("00");
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (cboUrun.SelectedItem==null)
            {
                MessageBox.Show("Lütfen bir ürün seçiniz!");
                return;
            }
            Urun seciliUrun = (Urun)cboUrun.SelectedItem;
            SiparisDetay sd = new SiparisDetay
            {
                UrunId=seciliUrun.Id,
                UrunAd = seciliUrun.UrunAd,
                BirimFiyat = seciliUrun.BirimFiyat,
                Adet = (int)nudAdet.Value
            };
            siparis.SiparisDetaylar.Add(sd);
            db.SaveChanges();
            dgvSiparisDetaylari.DataSource = null;
            dgvSiparisDetaylari.DataSource = siparis.SiparisDetaylar.ToList();
            //cboUrun.SelectedItem = null;
            nudAdet.Value = 1;
            TutarGuncelle();
            
        }

        private void btnAnaSayfa_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSiparisIptal_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("Sipariş iptal edilecektir. Onaylıyor musunuz?","Sipariş Onayı",MessageBoxButtons.YesNo,MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            //siparis.Durum = SiparisDurum.Iptal;
            if (dr==DialogResult.Yes)
            {
                siparis.Durum = SiparisDurum.Iptal;
                siparis.KapanisZamani = DateTime.Now;
                db.SaveChanges();
                Close();
            }
        }

        private void btnOdemeAl_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("Ödeme alındıysa masa kapatılacaktır. Onaylıyor musunuz", "Ödeme Alma", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            //siparis.Durum = SiparisDurum.Iptal;
            if (dr == DialogResult.Yes)
            {
                siparis.Durum = SiparisDurum.Odendi;
                siparis.KapanisZamani = DateTime.Now;
                siparis.OdenenTutar = siparis.SiparisDetaylar.Sum(x=>x.Adet * x.BirimFiyat);
                db.SaveChanges();
                Close();
            }
        }

        private void btnTasi_Click(object sender, EventArgs e)
        {
            if (cboMasaNo.SelectedItem==null)
            {
                MessageBox.Show("Lütfen hedef masa noyu seçiniz");
                return;
            }

            int hedefMasaNo = (int)cboMasaNo.SelectedItem;
            int eskiMasaNo = siparis.MasaNo;
            if (MasaTasinyor != null)
            {
                var args = new MasaTasimaEventArgs
                {
                    TasinanSiparis = siparis,
                    EskiMasaNo = eskiMasaNo,
                    YeniMasaNo = hedefMasaNo
                };
                MasaTasinyor(this, args);
            }
            siparis.MasaNo = hedefMasaNo;
            db.SaveChanges();
            MasaNoGuncelle();
            MasaNolariYukle();
          
            //Form1 form1 = new Form1();
            //form1.MasaTasi(suAnMasaNo, hedefMasaNo);

        }

        private void dgvSiparisDetaylari_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button==MouseButtons.Right)
            {
                int rowIndex = dgvSiparisDetaylari.HitTest(e.X, e.Y).RowIndex;
                if (rowIndex>-1)
                {
                dgvSiparisDetaylari.ClearSelection();
                dgvSiparisDetaylari.Rows[rowIndex].Selected = true;
                cmsSiparisDetay.Show(MousePosition);
                }
            }
        }

        private void tsmiSiparisDetaySil_Click(object sender, EventArgs e)
        {
            if (dgvSiparisDetaylari.SelectedRows.Count > 0)
            {
                var seciliSatir = dgvSiparisDetaylari.SelectedRows[0];
                var sipDetay = (SiparisDetay)seciliSatir.DataBoundItem;
                db.SiparisDetaylar.Remove(sipDetay);
                db.SaveChanges();
            }
            TutarGuncelle();
        }
    }

    public class MasaTasimaEventArgs : EventArgs
    {
        public Siparis TasinanSiparis { get; set; }
        public int EskiMasaNo { get; set; }
        public int YeniMasaNo { get; set; }
    }
}
