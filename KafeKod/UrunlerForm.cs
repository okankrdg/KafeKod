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
        BindingList<Urun> blurunler;
        //List<Urun> siraliUrun = new List<Urun>();
        public UrunlerForm(KafeContext kafeVeri)
        {
            db = kafeVeri;
            InitializeComponent();
            dgvUrunler.AutoGenerateColumns = false;//otmatik column eklemeyi iptal ettik
            blurunler = new BindingList<Urun>(db.Urunler);

            //siraliUrun = blurunler.OrderBy(x => x.UrunAd).ToList();
            //blurunler = new BindingList<Urun>(siraliUrun);
            dgvUrunler.DataSource = blurunler;

        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            string urunAd = txtUrunAdi.Text.Trim();
            if (urunAd=="")
            {
                MessageBox.Show("Lütfen bir ürün adı giriniz.");
                return;
            }
            blurunler.Add(new Urun { UrunAd = urunAd, BirimFiyat = nudBirimFiyat.Value });
            db.Urunler.Sort();
            
            //blurunler = new BindingList<Urun>(siraliUrun);

        }

        private void dgvUrunler_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Geçersiz giriş");
        }

        private void dgvUrunler_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            //UrunAd'ı düzenliyorsa
            if (e.ColumnIndex==0)
            {
                if (((string)e.FormattedValue).Trim()=="")
                {
                    dgvUrunler.Rows[e.RowIndex].ErrorText = "Ürün boş geçilemez";
                    e.Cancel = true;
                }
                else
                {
                    dgvUrunler.Rows[e.RowIndex].ErrorText = "";
                }
            }
        }
    }
}
