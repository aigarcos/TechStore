using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TechStore.Modelos;
using TechStore.Servicios.Interfaces;

namespace TechStore.Formularios
{
    public partial class FrmClientes : BaseForm
    {
        private readonly IServicioCliente _servicioCliente;
        private readonly IServicioVenta _servicioVenta;
        
        private int _clienteSeleccionadoId = 0;

        // Búsqueda
        private TextBox txtBuscar;
        private Button btnNuevoCliente;

        // Lista de clientes
        private DataGridView dgvClientes;

        // Datos del cliente
        private Label lblModoActual;
        private TextBox txtNombre, txtDocumento, txtTelefono, txtCorreo;
        private Button btnGuardar, btnEliminar, btnCancelar;

        // Historial
        private DataGridView dgvHistorial;
        private Label lblTotalCompras;

        public FrmClientes(IServicioCliente servicioCliente, IServicioVenta servicioVenta)
        {
            _servicioCliente = servicioCliente;
            _servicioVenta = servicioVenta;
            InitializeComponent();
            CargarClientes();
        }

        private void InitializeComponent()
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(950, 600);
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9F);
            this.Text = "TechStore - Gestión de Clientes";

            // Barra superior
            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(245, 247, 250) };
            
            txtBuscar = new TextBox { Location = new Point(20, 15), Size = new Size(300, 30), BorderStyle = BorderStyle.FixedSingle };
            txtBuscar.Text = "Buscar por nombre o DNI...";
            txtBuscar.ForeColor = Color.Gray;
            txtBuscar.Enter += (s, e) => { if (txtBuscar.Text == "Buscar por nombre o DNI...") { txtBuscar.Text = ""; txtBuscar.ForeColor = Color.Black; } };
            txtBuscar.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtBuscar.Text)) { txtBuscar.Text = "Buscar por nombre o DNI..."; txtBuscar.ForeColor = Color.Gray; } };
            txtBuscar.TextChanged += TxtBuscar_TextChanged;

            btnNuevoCliente = new Button { Text = "＋ Nuevo cliente", Location = new Point(340, 12), Size = new Size(130, 35), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnNuevoCliente.FlatAppearance.BorderSize = 0;
            btnNuevoCliente.Click += BtnNuevoCliente_Click;

            pnlTop.Controls.AddRange(new Control[] { txtBuscar, btnNuevoCliente });
            this.Controls.Add(pnlTop);

            // Panel Izquierdo: DataGridView
            dgvClientes = new DataGridView
            {
                Location = new Point(20, 80), Size = new Size(450, 480),
                BackgroundColor = Color.White, BorderStyle = BorderStyle.FixedSingle,
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, Font = new Font("Segoe UI", 9F, FontStyle.Bold) },
                RowHeadersVisible = false, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(248, 250, 252) }
            };
            dgvClientes.CellClick += DgvClientes_CellClick;
            this.Controls.Add(dgvClientes);

            // Panel Derecho Superior: Datos del Cliente
            GroupBox gbDatos = new GroupBox { Text = "Datos del cliente", Location = new Point(490, 80), Size = new Size(430, 260) };
            
            lblModoActual = new Label { Text = "Modo: Nuevo cliente", Location = new Point(15, 25), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(37, 99, 235) };
            
            Label lblNom = new Label { Text = "Nombre:", Location = new Point(15, 55), AutoSize = true };
            txtNombre = new TextBox { Location = new Point(100, 50), Size = new Size(310, 30), BorderStyle = BorderStyle.FixedSingle };
            
            Label lblDoc = new Label { Text = "DNI/Doc:", Location = new Point(15, 95), AutoSize = true };
            txtDocumento = new TextBox { Location = new Point(100, 90), Size = new Size(310, 30), BorderStyle = BorderStyle.FixedSingle };
            
            Label lblTel = new Label { Text = "Teléfono:", Location = new Point(15, 135), AutoSize = true };
            txtTelefono = new TextBox { Location = new Point(100, 130), Size = new Size(310, 30), BorderStyle = BorderStyle.FixedSingle };
            
            Label lblCor = new Label { Text = "Correo:", Location = new Point(15, 175), AutoSize = true };
            txtCorreo = new TextBox { Location = new Point(100, 170), Size = new Size(310, 30), BorderStyle = BorderStyle.FixedSingle };

            btnGuardar = new Button { Text = "Guardar", Location = new Point(100, 215), Size = new Size(90, 30), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.Click += BtnGuardar_Click;

            btnEliminar = new Button { Text = "Eliminar", Location = new Point(200, 215), Size = new Size(90, 30), BackColor = Color.FromArgb(220, 38, 38), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Visible = false };
            btnEliminar.FlatAppearance.BorderSize = 0;
            btnEliminar.Click += BtnEliminar_Click;

            btnCancelar = new Button { Text = "Cancelar", Location = new Point(300, 215), Size = new Size(90, 30), BackColor = Color.FromArgb(100, 116, 139), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => LimpiarFormulario();

            gbDatos.Controls.AddRange(new Control[] { lblModoActual, lblNom, txtNombre, lblDoc, txtDocumento, lblTel, txtTelefono, lblCor, txtCorreo, btnGuardar, btnEliminar, btnCancelar });
            this.Controls.Add(gbDatos);

            // Panel Derecho Inferior: Historial de compras
            GroupBox gbHistorial = new GroupBox { Text = "Historial de compras", Location = new Point(490, 350), Size = new Size(430, 210) };
            
            dgvHistorial = new DataGridView
            {
                Location = new Point(15, 25), Size = new Size(400, 140),
                BackgroundColor = Color.White, BorderStyle = BorderStyle.FixedSingle,
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(100, 116, 139), ForeColor = Color.White, Font = new Font("Segoe UI", 9F) },
                RowHeadersVisible = false, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            
            lblTotalCompras = new Label { Text = "Total comprado: S/ 0.00", Location = new Point(15, 175), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };

            gbHistorial.Controls.AddRange(new Control[] { dgvHistorial, lblTotalCompras });
            this.Controls.Add(gbHistorial);
        }

        private static bool EsDocumentoValido(string documento)
        {
            return !string.IsNullOrWhiteSpace(documento) && Regex.IsMatch(documento, @"^(?:\d{8}|\d{11})$");
        }

        private void CargarClientes()
        {
            var texto = txtBuscar.Text == "Buscar por nombre o DNI..." ? "" : txtBuscar.Text.Trim();
            if (string.IsNullOrEmpty(texto))
            {
                // Cargar todos. Asumiendo que el repositorio tiene un ObtenerTodos o BuscarPorNombre con "".
                dgvClientes.DataSource = _servicioCliente.BuscarPorNombreODocumento("");
            }
            else
            {
                dgvClientes.DataSource = _servicioCliente.BuscarPorNombreODocumento(texto);
            }

            if (dgvClientes.Columns["Id"] != null) dgvClientes.Columns["Id"].Visible = false;
        }

        private void TxtBuscar_TextChanged(object sender, EventArgs e)
        {
            if (txtBuscar.Text != "Buscar por nombre o DNI...")
            {
                CargarClientes();
            }
        }

        private void DgvClientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                _clienteSeleccionadoId = (int)dgvClientes.Rows[e.RowIndex].Cells["Id"].Value;
                txtNombre.Text = dgvClientes.Rows[e.RowIndex].Cells["Nombre"].Value?.ToString();
                txtDocumento.Text = dgvClientes.Rows[e.RowIndex].Cells["Documento"].Value?.ToString();
                txtTelefono.Text = dgvClientes.Rows[e.RowIndex].Cells["Telefono"].Value?.ToString();
                txtCorreo.Text = dgvClientes.Rows[e.RowIndex].Cells["Correo"].Value?.ToString();
                
                lblModoActual.Text = $"Modo: Editando — {txtNombre.Text}";
                btnEliminar.Visible = true;
                
                CargarHistorial(_clienteSeleccionadoId);
            }
        }

        private void CargarHistorial(int clienteId)
        {
            var ventas = _servicioVenta.ObtenerVentasPorCliente(clienteId);
            if (ventas.Count == 0)
            {
                dgvHistorial.DataSource = null;
                lblTotalCompras.Text = "Sin historial de compras aún.";
            }
            else
            {
                var source = ventas.Select(v => new {
                    Factura = v.Id, // Mostrar el Id o algo como "FAC-" + v.Id
                    Fecha = v.Fecha.ToString("dd/MM/yyyy HH:mm"),
                    Total = v.Total,
                    Cajero = v.UsuarioId // Mostraríamos el nombre, pero Id sirve para este prototipo
                }).ToList();
                
                dgvHistorial.DataSource = source;
                lblTotalCompras.Text = $"Total comprado: S/ {ventas.Sum(v => v.Total):0.00}";
            }
        }

        private void BtnNuevoCliente_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void LimpiarFormulario()
        {
            _clienteSeleccionadoId = 0;
            txtNombre.Text = "";
            txtDocumento.Text = "";
            txtTelefono.Text = "";
            txtCorreo.Text = "";
            lblModoActual.Text = "Modo: Nuevo cliente";
            btnEliminar.Visible = false;
            dgvHistorial.DataSource = null;
            lblTotalCompras.Text = "Total comprado: S/ 0.00";
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtDocumento.Text))
            {
                txtNombre.BackColor = string.IsNullOrWhiteSpace(txtNombre.Text) ? Color.FromArgb(254, 226, 226) : Color.White;
                txtDocumento.BackColor = string.IsNullOrWhiteSpace(txtDocumento.Text) ? Color.FromArgb(254, 226, 226) : Color.White;
                MessageBox.Show("Nombre y Documento son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            txtNombre.BackColor = Color.White;
            txtDocumento.BackColor = Color.White;

            if (!EsDocumentoValido(txtDocumento.Text))
            {
                txtDocumento.BackColor = Color.FromArgb(254, 226, 226);
                MessageBox.Show("El documento debe tener 8 dígitos para DNI o 11 dígitos para RUC.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var cliente = new Cliente
                {
                    Id = _clienteSeleccionadoId,
                    Nombre = txtNombre.Text,
                    Documento = txtDocumento.Text,
                    Telefono = txtTelefono.Text,
                    Correo = txtCorreo.Text
                };

                if (_clienteSeleccionadoId == 0)
                {
                    _servicioCliente.Registrar(cliente);
                    MessageBox.Show("Cliente registrado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _servicioCliente.Actualizar(cliente);
                    MessageBox.Show("Cliente actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
                LimpiarFormulario();
                CargarClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (_clienteSeleccionadoId > 0)
            {
                if (MessageBox.Show($"¿Desea eliminar al cliente {txtNombre.Text}?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        _servicioCliente.Eliminar(_clienteSeleccionadoId);
                        MessageBox.Show("Cliente eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarFormulario();
                        CargarClientes();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error al eliminar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
