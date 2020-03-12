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
    public partial class UrunlerForm : Form
    {
        KafeContext db;

        //List<Urun> siraliUrun = new List<Urun>();
        public UrunlerForm(KafeContext kafeVeri)
        {
            db = kafeVeri;
            InitializeComponent();
            dgvUrunler.AutoGenerateColumns = false;//otmatik column eklemeyi iptal ettik

            //siraliUrun = blurunler.OrderBy(x => x.UrunAd).ToList();
            //blurunler = new BindingList<Urun>(siraliUrun);
            dgvUrunler.DataSource = new BindingSource(db.Urunler.OrderBy(x => x.UrunAd).ToList(), null);

        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            string urunAd = txtUrunAdi.Text.Trim();
            if (urunAd == "")
            {
                MessageBox.Show("Lütfen bir ürün adı giriniz.");
                return;
            }
            db.Urunler.Add(new Urun { UrunAd = urunAd, BirimFiyat = nudBirimFiyat.Value });
            db.SaveChanges();
            dgvUrunler.DataSource = new BindingSource(db.Urunler.OrderBy(x => x.UrunAd).ToList(), null);


            //blurunler = new BindingList<Urun>(siraliUrun);

        }

        private void dgvUrunler_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Geçersiz giriş");
        }

        private void dgvUrunler_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            //UrunAd'ı düzenliyorsa
            if (e.ColumnIndex == 0)
            {
                if (((string)e.FormattedValue).Trim() == "")
                {
                    dgvUrunler.Rows[e.RowIndex].ErrorText = "Ürün boş geçilemez";
                    e.Cancel = true;
                }
                else
                {
                    dgvUrunler.Rows[e.RowIndex].ErrorText = "";
                }
            }
            db.SaveChanges();
        }

        private void dgvUrunler_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            Urun secili = (Urun)e.Row.DataBoundItem;
            if (secili.SiparisDetaylar.Count > 0)
            {
                MessageBox.Show("Bu ürün stokta yok olarak silindi");
                secili.StoktaYok = true;
                e.Cancel = true;
                return;
            }
            db.Urunler.Remove(secili);
            db.SaveChanges();
        }

        private void UrunlerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            txtUrunAdi.Focus();
        }

        private void UrunlerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            db.SaveChanges();
        }
    }
}
