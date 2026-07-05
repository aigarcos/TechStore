using System;
using System.Drawing;
using System.Windows.Forms;
using TechStore.Servicios.Interfaces;
using System.Linq;
using TechStore.Modelos;

namespace TechStore.Formularios
{
    public partial class FrmCMDB : BaseForm
    {
        private readonly IServicioCMDB _servicioCMDB;

        private DataGridView dgvItems, dgvRelaciones;
        private ComboBox cmbTipo, cmbEstado;
        private Label lblItemSeleccionado, lblActivos, lblEnMantenimiento;
        private Button btnIniciarMantenimiento, btnFinalizarMantenimiento, btnDarDeBaja;
        
        private ItemConfiguracion _itemSeleccionado;

        public FrmCMDB(IServicioCMDB servicioCMDB)
        {
            _servicioCMDB = servicioCMDB;
            InitializeComponent();
            CargarItems();
            ActualizarResumen();
        }

        private void InitializeComponent()
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(1000, 600);
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9F);
            this.Text = "TechStore - CMDB";

            // Panel Superior Titulo
            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(245, 247, 250) };
            Label lblTitulo = new Label { Text = "Gestión de Infraestructura (CMDB)", Font = new Font("Segoe UI", 16F, FontStyle.Bold), ForeColor = Color.FromArgb(15, 23, 42), AutoSize = true, Location = new Point(15, 15) };
            pnlTop.Controls.Add(lblTitulo);
            this.Controls.Add(pnlTop);

            // Panel Izquierdo
            GroupBox gbItems = new GroupBox { Text = "Ítems de Configuración", Location = new Point(20, 80), Size = new Size(550, 420) };
            
            cmbTipo = new ComboBox { Location = new Point(15, 30), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTipo.Items.AddRange(new string[] { "Todos", "Hardware", "Software", "Red" });
            cmbTipo.SelectedIndex = 0;
            cmbTipo.SelectedIndexChanged += (s, e) => CargarItems();
            
            cmbEstado = new ComboBox { Location = new Point(145, 30), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbEstado.Items.AddRange(new string[] { "Todos", "Activo", "EnMantenimiento", "Baja" });
            cmbEstado.SelectedIndex = 0;
            cmbEstado.SelectedIndexChanged += (s, e) => CargarItems();

            dgvItems = new DataGridView
            {
                Location = new Point(15, 70), Size = new Size(520, 290),
                BackgroundColor = Color.White, BorderStyle = BorderStyle.FixedSingle,
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, Font = new Font("Segoe UI", 9F, FontStyle.Bold) },
                RowHeadersVisible = false, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvItems.SelectionChanged += DgvItems_SelectionChanged;

            btnIniciarMantenimiento = new Button { Text = "Iniciar Mantenimiento", Location = new Point(15, 375), Size = new Size(160, 30), BackColor = Color.FromArgb(234, 88, 12), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Enabled = false };
            btnIniciarMantenimiento.FlatAppearance.BorderSize = 0;
            btnIniciarMantenimiento.Click += BtnIniciarMantenimiento_Click;

            btnFinalizarMantenimiento = new Button { Text = "Finalizar Mantenimiento", Location = new Point(185, 375), Size = new Size(160, 30), BackColor = Color.FromArgb(22, 163, 74), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Enabled = false };
            btnFinalizarMantenimiento.FlatAppearance.BorderSize = 0;
            btnFinalizarMantenimiento.Click += BtnFinalizarMantenimiento_Click;

            btnDarDeBaja = new Button { Text = "Dar de baja", Location = new Point(355, 375), Size = new Size(100, 30), BackColor = Color.FromArgb(220, 38, 38), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Enabled = false };
            btnDarDeBaja.FlatAppearance.BorderSize = 0;
            btnDarDeBaja.Click += BtnDarDeBaja_Click;

            gbItems.Controls.AddRange(new Control[] { cmbTipo, cmbEstado, dgvItems, btnIniciarMantenimiento, btnFinalizarMantenimiento, btnDarDeBaja });
            this.Controls.Add(gbItems);

            // Panel Derecho
            GroupBox gbRelaciones = new GroupBox { Text = "Relaciones del Ítem", Location = new Point(590, 80), Size = new Size(380, 420) };
            
            lblItemSeleccionado = new Label { Text = "Seleccione un ítem", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(37, 99, 235), Location = new Point(15, 30), AutoSize = true };
            
            dgvRelaciones = new DataGridView
            {
                Location = new Point(15, 70), Size = new Size(350, 300),
                BackgroundColor = Color.White, BorderStyle = BorderStyle.FixedSingle,
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(100, 116, 139), ForeColor = Color.White, Font = new Font("Segoe UI", 9F, FontStyle.Bold) },
                RowHeadersVisible = false, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(248, 250, 252) },
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            gbRelaciones.Controls.AddRange(new Control[] { lblItemSeleccionado, dgvRelaciones });
            this.Controls.Add(gbRelaciones);

            // Panel Inferior Resumen
            Panel pnlResumen = new Panel { Location = new Point(20, 520), Size = new Size(950, 60), BackColor = Color.FromArgb(248, 250, 252), BorderStyle = BorderStyle.FixedSingle };
            lblActivos = new Label { Text = "CIs Activos: 0", Font = new Font("Segoe UI", 12F, FontStyle.Bold), ForeColor = Color.FromArgb(22, 163, 74), Location = new Point(20, 18), AutoSize = true };
            lblEnMantenimiento = new Label { Text = "CIs En Mantenimiento: 0", Font = new Font("Segoe UI", 12F, FontStyle.Bold), ForeColor = Color.FromArgb(234, 88, 12), Location = new Point(200, 18), AutoSize = true };
            
            pnlResumen.Controls.Add(lblActivos);
            pnlResumen.Controls.Add(lblEnMantenimiento);
            this.Controls.Add(pnlResumen);
        }

        private void CargarItems()
        {
            var items = _servicioCMDB.ObtenerTodos();

            if (cmbTipo.SelectedItem != null && cmbTipo.SelectedItem.ToString() != "Todos")
                items = items.Where(i => i.Tipo == cmbTipo.SelectedItem.ToString()).ToList();

            if (cmbEstado.SelectedItem != null && cmbEstado.SelectedItem.ToString() != "Todos")
                items = items.Where(i => i.Estado == cmbEstado.SelectedItem.ToString()).ToList();

            dgvItems.DataSource = items;
            
            // Ocultar algunas columnas que ocupan mucho espacio para que se vea mejor
            if (dgvItems.Columns["Id"] != null) dgvItems.Columns["Id"].Visible = false;
            if (dgvItems.Columns["Descripcion"] != null) dgvItems.Columns["Descripcion"].Visible = false;
            if (dgvItems.Columns["InicioMantenimiento"] != null) dgvItems.Columns["InicioMantenimiento"].Visible = false;
            if (dgvItems.Columns["FinMantenimiento"] != null) dgvItems.Columns["FinMantenimiento"].Visible = false;
            if (dgvItems.Columns["MotivoMantenimiento"] != null) dgvItems.Columns["MotivoMantenimiento"].Visible = false;
            if (dgvItems.Columns["FechaBaja"] != null) dgvItems.Columns["FechaBaja"].Visible = false;
            if (dgvItems.Columns["MotivoBaja"] != null) dgvItems.Columns["MotivoBaja"].Visible = false;

            ColorearFilas();
            ValidarBotones(null);
        }

        private void ColorearFilas()
        {
            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                string estado = row.Cells["Estado"].Value.ToString();
                if (estado == "Activo")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(220, 252, 231);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(22, 163, 74);
                }
                else if (estado == "EnMantenimiento")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 237, 213);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(234, 88, 12);
                }
                else if (estado == "Baja")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(148, 163, 184); // Gris
                    // Podría tacharse cambiando el Font si es necesario
                }
            }
        }

        private void DgvItems_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvItems.SelectedRows[0].Cells["Id"].Value);
                _itemSeleccionado = _servicioCMDB.ObtenerTodos().FirstOrDefault(i => i.Id == id);

                if (_itemSeleccionado != null)
                {
                    lblItemSeleccionado.Text = $"Relaciones: {_itemSeleccionado.Nombre}";
                    var relaciones = _servicioCMDB.ObtenerRelaciones(id);
                    dgvRelaciones.DataSource = relaciones;
                    
                    ValidarBotones(_itemSeleccionado);
                }
            }
            else
            {
                _itemSeleccionado = null;
                ValidarBotones(null);
            }
        }

        private void ValidarBotones(ItemConfiguracion item)
        {
            if (item == null)
            {
                btnIniciarMantenimiento.Enabled = false;
                btnFinalizarMantenimiento.Enabled = false;
                btnDarDeBaja.Enabled = false;
            }
            else
            {
                btnIniciarMantenimiento.Enabled = (item.Estado == "Activo");
                btnFinalizarMantenimiento.Enabled = (item.Estado == "EnMantenimiento");
                btnDarDeBaja.Enabled = (item.Estado == "Activo" || item.Estado == "EnMantenimiento");
            }
        }

        private void BtnIniciarMantenimiento_Click(object sender, EventArgs e)
        {
            if (_itemSeleccionado != null)
            {
                using (var frm = new FrmIniciarMantenimiento(_servicioCMDB, _itemSeleccionado.Id, _itemSeleccionado.Nombre))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        CargarItems();
                        ActualizarResumen();
                    }
                }
            }
        }

        private void BtnFinalizarMantenimiento_Click(object sender, EventArgs e)
        {
            if (_itemSeleccionado != null)
            {
                using (var frm = new FrmFinalizarMantenimiento(_servicioCMDB, _itemSeleccionado.Id, _itemSeleccionado.Nombre, _itemSeleccionado.InicioMantenimiento))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        CargarItems();
                        ActualizarResumen();
                    }
                }
            }
        }

        private void BtnDarDeBaja_Click(object sender, EventArgs e)
        {
            if (_itemSeleccionado != null)
            {
                using (var frm = new FrmDarDeBaja(_servicioCMDB, _itemSeleccionado.Id, _itemSeleccionado.Nombre))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        CargarItems();
                        ActualizarResumen();
                    }
                }
            }
        }

        private void ActualizarResumen()
        {
            lblActivos.Text = $"CIs Activos: {_servicioCMDB.ObtenerConteoActivos()}";
            lblEnMantenimiento.Text = $"CIs En Mantenimiento: {_servicioCMDB.ObtenerConteoEnMantenimiento()}";
        }
    }
}
