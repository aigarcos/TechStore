using System;
using System.Drawing;
using System.Windows.Forms;
using TechStore.Servicios.Implementaciones;
using TechStore.Servicios.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace TechStore.Formularios
{
    public partial class FrmPrincipal : Form
    {
        private readonly IServicioVenta _servicioVenta;
        private readonly IServicioProducto _servicioProducto;
        private readonly IServicioCMDB _servicioCMDB;
        private readonly IServiceProvider _serviceProvider;

        private Label lblVentasHoy;
        private Label lblTransacciones;
        private Label lblSinStock;
        private Label lblStockBajo;
        private DataGridView dgvUltimasVentas;
        private DataGridView dgvStockCritico;
        private DataGridView dgvCMDB;
        private ToolStripStatusLabel statusUser;
        private ToolStripStatusLabel statusDate;
        private Timer timer;

        public FrmPrincipal(IServicioVenta servicioVenta, IServicioProducto servicioProducto, IServicioCMDB servicioCMDB, IServiceProvider serviceProvider)
        {
            _servicioVenta = servicioVenta;
            _servicioProducto = servicioProducto;
            _servicioCMDB = servicioCMDB;
            _serviceProvider = serviceProvider;
            InitializeComponent();
            ConfigurarSegunRol();
            CargarDatosDashboard();
        }

        private void InitializeComponent()
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(1100, 650);
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Font = new Font("Segoe UI", 9F);
            this.Text = "TechStore - Dashboard Principal";

            // MenuStrip
            var menuStrip = new MenuStrip { BackColor = Color.White };
            var menuVentas = new ToolStripMenuItem("Ventas", null, (s, e) => AbrirFormulario<FrmVenta>());
            var menuClientes = new ToolStripMenuItem("Clientes", null, (s, e) => AbrirFormulario<FrmClientes>());
            var menuInventario = new ToolStripMenuItem("Inventario", null, (s, e) => AbrirFormulario<FrmInventario>()) { Name = "MenuInventario" };
            var menuReportes = new ToolStripMenuItem("Reportes", null, (s, e) => AbrirFormulario<FrmReportes>()) { Name = "MenuReportes" };
            var menuCMDB = new ToolStripMenuItem("CMDB", null, (s, e) => AbrirFormulario<FrmCMDB>()) { Name = "MenuCMDB" };
            
            menuStrip.Items.AddRange(new ToolStripItem[] { menuVentas, menuClientes, menuInventario, menuReportes, menuCMDB });
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // StatusStrip
            var statusStrip = new StatusStrip { BackColor = Color.White };
            statusUser = new ToolStripStatusLabel($"Usuario: {SesionActual.Usuario?.NombreUsuario} | Rol: {SesionActual.Usuario?.Rol}");
            statusDate = new ToolStripStatusLabel(DateTime.Now.ToString("g")) { Spring = true, TextAlign = ContentAlignment.MiddleRight };
            statusStrip.Items.Add(statusUser);
            statusStrip.Items.Add(statusDate);
            this.Controls.Add(statusStrip);

            // Timer
            timer = new Timer { Interval = 60000 };
            timer.Tick += (s, e) => statusDate.Text = DateTime.Now.ToString("g");
            timer.Start();

            // Botón Refrescar
            var btnRefrescar = new Button
            {
                Text = "Actualizar Dashboard",
                Location = new Point(900, 40),
                Size = new Size(150, 30),
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRefrescar.FlatAppearance.BorderSize = 0;
            btnRefrescar.Click += (s, e) => CargarDatosDashboard();
            this.Controls.Add(btnRefrescar);

            // Tarjetas (Y = 80)
            this.Controls.Add(CrearTarjeta("Ventas Hoy", out lblVentasHoy, new Point(20, 80)));
            this.Controls.Add(CrearTarjeta("Transacciones", out lblTransacciones, new Point(280, 80)));
            this.Controls.Add(CrearTarjeta("Sin Stock", out lblSinStock, new Point(540, 80)));
            this.Controls.Add(CrearTarjeta("Stock Bajo", out lblStockBajo, new Point(800, 80)));

            // DataGridViews
            Label lblGridVentas = new Label { Text = "Últimas Transacciones", Font = new Font("Segoe UI", 10F, FontStyle.Bold), Location = new Point(20, 200), AutoSize = true };
            dgvUltimasVentas = CrearGrid(new Point(20, 225), new Size(500, 350));
            
            Label lblGridStock = new Label { Text = "Stock Crítico", Font = new Font("Segoe UI", 10F, FontStyle.Bold), Location = new Point(540, 200), AutoSize = true };
            dgvStockCritico = CrearGrid(new Point(540, 225), new Size(520, 150));

            Label lblGridCMDB = new Label { Text = "Estado Infraestructura (CMDB)", Font = new Font("Segoe UI", 10F, FontStyle.Bold), Location = new Point(540, 395), AutoSize = true };
            dgvCMDB = CrearGrid(new Point(540, 420), new Size(520, 155));

            this.Controls.Add(lblGridVentas);
            this.Controls.Add(dgvUltimasVentas);
            this.Controls.Add(lblGridStock);
            this.Controls.Add(dgvStockCritico);
            this.Controls.Add(lblGridCMDB);
            this.Controls.Add(dgvCMDB);
        }

        private Panel CrearTarjeta(string titulo, out Label valorLabel, Point location)
        {
            Panel panel = new Panel
            {
                Location = location,
                Size = new Size(240, 100),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label titleLabel = new Label
            {
                Text = titulo,
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(15, 15),
                AutoSize = true
            };

            valorLabel = new Label
            {
                Text = "0",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(15, 40),
                AutoSize = true
            };

            panel.Controls.Add(titleLabel);
            panel.Controls.Add(valorLabel);
            return panel;
        }

        private DataGridView CrearGrid(Point location, Size size)
        {
            return new DataGridView
            {
                Location = location,
                Size = size,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                EnableHeadersVisualStyles = false,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, Font = new Font("Segoe UI", 9F, FontStyle.Bold) },
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(248, 250, 252) },
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
        }

        private void ConfigurarSegunRol()
        {
            if (SesionActual.Usuario?.Rol == "Cajero")
            {
                this.MainMenuStrip.Items["MenuInventario"].Visible = false;
                this.MainMenuStrip.Items["MenuReportes"].Visible = false;
                this.MainMenuStrip.Items["MenuCMDB"].Visible = false;
                
                // Ocultar paneles que no debe ver
                dgvStockCritico.Visible = false;
                dgvCMDB.Visible = false;
            }
        }

        private void CargarDatosDashboard()
        {
            try
            {
                // Tarjetas
                lblVentasHoy.Text = $"S/ {_servicioVenta.ObtenerTotalDelDia():0.00}";
                lblVentasHoy.ForeColor = Color.FromArgb(37, 99, 235);

                lblTransacciones.Text = _servicioVenta.ObtenerCantidadTransaccionesDelDia().ToString();

                var sinStock = _servicioProducto.ObtenerConStockCritico(1).Count;
                lblSinStock.Text = sinStock.ToString();
                lblSinStock.ForeColor = sinStock > 0 ? Color.FromArgb(220, 38, 38) : Color.FromArgb(15, 23, 42);

                var stockBajo = _servicioProducto.ObtenerConStockCritico(5).Count - sinStock;
                lblStockBajo.Text = stockBajo.ToString();
                lblStockBajo.ForeColor = stockBajo > 0 ? Color.FromArgb(234, 88, 12) : Color.FromArgb(15, 23, 42);

                // Grids
                var ultimasVentas = _servicioVenta.ObtenerUltimasVentas(10);
                dgvUltimasVentas.DataSource = ultimasVentas;

                if (SesionActual.Usuario?.Rol == "Administrador")
                {
                    dgvStockCritico.DataSource = _servicioProducto.ObtenerConStockCritico(5);
                    ColorearStock();

                    var cmdbItems = _servicioCMDB.ObtenerTodos();
                    if(cmdbItems.Count > 5) cmdbItems = cmdbItems.GetRange(0, 5); // Tomar solo top 5 para el dashboard
                    dgvCMDB.DataSource = cmdbItems;
                    ColorearCMDB();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar dashboard: " + ex.Message);
            }
        }

        private void ColorearStock()
        {
            foreach (DataGridViewRow row in dgvStockCritico.Rows)
            {
                int stock = Convert.ToInt32(row.Cells["Stock"].Value);
                if (stock == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(254, 226, 226); // Light red
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(220, 38, 38);
                }
                else if (stock < 5)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 237, 213); // Light orange
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(234, 88, 12);
                }
            }
        }

        private void ColorearCMDB()
        {
            foreach (DataGridViewRow row in dgvCMDB.Rows)
            {
                string estado = row.Cells["Estado"].Value.ToString();
                if (estado == "Activo")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(220, 252, 231); // Light green
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(22, 163, 74);
                }
                else if (estado == "Mantenimiento")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 237, 213); // Light orange
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(234, 88, 12);
                }
                else if (estado == "Baja")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(254, 226, 226); // Light red
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(220, 38, 38);
                }
            }
        }

        private void AbrirFormulario<T>() where T : Form
        {
            var form = _serviceProvider.GetRequiredService<T>();
            form.ShowDialog();
            CargarDatosDashboard(); // Actualizar al volver
        }
    }
}
