using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TechStore.Servicios.Interfaces;

namespace TechStore.Formularios
{
    public partial class FrmReportes : BaseForm
    {
        private readonly IServicioVenta _servicioVenta;
        
        private DateTimePicker dtpDesde, dtpHasta;
        private DataGridView dgvResultados;
        private Label lblTotalPeriodo, lblCantidadVentas, lblPromedioVenta;

        public FrmReportes(IServicioVenta servicioVenta)
        {
            _servicioVenta = servicioVenta;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(900, 550);
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9F);
            this.Text = "TechStore - Reportes de Ventas";

            // Panel Superior
            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.FromArgb(245, 247, 250) };
            
            Label lblDesde = new Label { Text = "Desde:", Location = new Point(20, 30), AutoSize = true };
            dtpDesde = new DateTimePicker { Location = new Point(70, 26), Format = DateTimePickerFormat.Short, Width = 100 };
            
            Label lblHasta = new Label { Text = "Hasta:", Location = new Point(190, 30), AutoSize = true };
            dtpHasta = new DateTimePicker { Location = new Point(240, 26), Format = DateTimePickerFormat.Short, Width = 100 };

            Button btnGenerar = new Button { Text = "Generar Reporte", Location = new Point(360, 24), Size = new Size(130, 30), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnGenerar.FlatAppearance.BorderSize = 0;
            btnGenerar.Click += BtnGenerar_Click;

            Button btnExportar = new Button { Text = "Exportar CSV", Location = new Point(500, 24), Size = new Size(110, 30), BackColor = Color.FromArgb(22, 163, 74), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnExportar.FlatAppearance.BorderSize = 0;
            btnExportar.Click += BtnExportar_Click;

            pnlTop.Controls.AddRange(new Control[] { lblDesde, dtpDesde, lblHasta, dtpHasta, btnGenerar, btnExportar });
            this.Controls.Add(pnlTop);

            // Grid
            dgvResultados = new DataGridView
            {
                Location = new Point(20, 100), Size = new Size(840, 320),
                BackgroundColor = Color.White, BorderStyle = BorderStyle.FixedSingle,
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, Font = new Font("Segoe UI", 9F, FontStyle.Bold) },
                RowHeadersVisible = false, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(248, 250, 252) },
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            this.Controls.Add(dgvResultados);

            // Panel Resumen Inferior
            Panel pnlResumen = new Panel { Location = new Point(20, 440), Size = new Size(840, 90), BackColor = Color.FromArgb(248, 250, 252), BorderStyle = BorderStyle.FixedSingle };
            
            lblTotalPeriodo = new Label { Text = "Total del Período: S/ 0.00", Font = new Font("Segoe UI", 16F, FontStyle.Bold), ForeColor = Color.FromArgb(37, 99, 235), Location = new Point(20, 30), AutoSize = true };
            lblCantidadVentas = new Label { Text = "Cantidad de ventas: 0", Location = new Point(400, 20), AutoSize = true, Font = new Font("Segoe UI", 10F) };
            lblPromedioVenta = new Label { Text = "Promedio por venta: S/ 0.00", Location = new Point(400, 50), AutoSize = true, Font = new Font("Segoe UI", 10F) };

            pnlResumen.Controls.AddRange(new Control[] { lblTotalPeriodo, lblCantidadVentas, lblPromedioVenta });
            this.Controls.Add(pnlResumen);
        }

        private void BtnGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                var ventas = _servicioVenta.ObtenerVentasPorFecha(dtpDesde.Value, dtpHasta.Value);
                
                var dataSource = ventas.Select(v => new {
                    ID_Venta = v.Id,
                    Fecha = v.Fecha,
                    ID_Cliente = v.ClienteId,
                    ID_Usuario = v.UsuarioId,
                    Total = v.Total
                }).ToList();

                dgvResultados.DataSource = dataSource;

                decimal total = ventas.Sum(v => v.Total);
                int count = ventas.Count;
                decimal promedio = count > 0 ? total / count : 0;

                lblTotalPeriodo.Text = $"Total del Período: S/ {total:0.00}";
                lblCantidadVentas.Text = $"Cantidad de ventas: {count}";
                lblPromedioVenta.Text = $"Promedio por venta: S/ {promedio:0.00}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnExportar_Click(object sender, EventArgs e)
        {
            if (dgvResultados.Rows.Count == 0) return;

            SaveFileDialog sfd = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = "Reporte.csv" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName))
                    {
                        sw.WriteLine("ID_Venta,Fecha,ID_Cliente,ID_Usuario,Total");
                        foreach (DataGridViewRow row in dgvResultados.Rows)
                        {
                            sw.WriteLine($"{row.Cells[0].Value},{row.Cells[1].Value},{row.Cells[2].Value},{row.Cells[3].Value},{row.Cells[4].Value}");
                        }
                    }
                    MessageBox.Show("Exportado exitosamente.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al exportar: " + ex.Message);
                }
            }
        }
    }
}
