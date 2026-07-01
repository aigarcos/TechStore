using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TechStore.Modelos;
using TechStore.Servicios.Interfaces;
using TechStore.Servicios.Implementaciones;

namespace TechStore.Formularios
{
    public partial class FrmVenta : Form
    {
        private readonly IServicioVenta _servicioVenta;
        private readonly IServicioProducto _servicioProducto;
        private readonly IServicioCliente _servicioCliente;

        private Producto productoActual;
        private int _clienteId = 0;
        private List<DetalleVenta> carrito = new List<DetalleVenta>();

        // UI Producto
        private TextBox txtBuscarProducto;
        private ListBox lstProductos;
        private Label lblNombreSeleccionado;
        private Label lblPrecioSeleccionado;
        private Label lblStockDisponible;
        private NumericUpDown nudCantidad;
        private Button btnAgregar;

        // UI Carrito
        private DataGridView dgvCarrito;
        private Label lblTotal;
        private Button btnLimpiarCarrito;

        // UI Cliente
        private TextBox txtBuscarCliente;
        private ListBox lstClientes;
        private Label lblClienteSeleccionado;
        private Button btnConfirmarVenta;

        public FrmVenta(IServicioVenta servicioVenta, IServicioProducto servicioProducto, IServicioCliente servicioCliente)
        {
            _servicioVenta = servicioVenta;
            _servicioProducto = servicioProducto;
            _servicioCliente = servicioCliente;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(950, 650);
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9F);
            this.Text = "TechStore - Nueva Venta";

            // Panel Superior Izquierdo: Buscar Producto
            GroupBox gbProducto = new GroupBox { Text = "Buscar producto", Location = new Point(20, 20), Size = new Size(400, 300) };
            
            txtBuscarProducto = new TextBox { Location = new Point(15, 30), Size = new Size(370, 30), BorderStyle = BorderStyle.FixedSingle };
            txtBuscarProducto.Text = "Escriba nombre o código...";
            txtBuscarProducto.ForeColor = Color.Gray;
            txtBuscarProducto.Enter += (s, e) => { if (txtBuscarProducto.Text == "Escriba nombre o código...") { txtBuscarProducto.Text = ""; txtBuscarProducto.ForeColor = Color.Black; } };
            txtBuscarProducto.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtBuscarProducto.Text)) { txtBuscarProducto.Text = "Escriba nombre o código..."; txtBuscarProducto.ForeColor = Color.Gray; } };
            txtBuscarProducto.TextChanged += TxtBuscarProducto_TextChanged;

            lstProductos = new ListBox { Location = new Point(15, 65), Size = new Size(370, 100), Font = new Font("Segoe UI", 9F) };
            lstProductos.SelectedIndexChanged += LstProductos_SelectedIndexChanged;

            lblNombreSeleccionado = new Label { Text = "-", Location = new Point(15, 180), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            lblPrecioSeleccionado = new Label { Text = "Precio: S/ 0.00", Location = new Point(15, 210), AutoSize = true };
            lblStockDisponible = new Label { Text = "Stock: 0", Location = new Point(150, 210), AutoSize = true };
            
            Label lblCant = new Label { Text = "Cant:", Location = new Point(15, 250), AutoSize = true };
            nudCantidad = new NumericUpDown { Location = new Point(60, 245), Size = new Size(60, 30), Minimum = 1, Value = 1 };
            
            btnAgregar = new Button { Text = "Agregar al carrito", Location = new Point(150, 240), Size = new Size(235, 35), BackColor = Color.FromArgb(22, 163, 74), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Enabled = false };
            btnAgregar.FlatAppearance.BorderSize = 0;
            btnAgregar.Click += BtnAgregar_Click;

            gbProducto.Controls.AddRange(new Control[] { txtBuscarProducto, lstProductos, lblNombreSeleccionado, lblPrecioSeleccionado, lblStockDisponible, lblCant, nudCantidad, btnAgregar });
            this.Controls.Add(gbProducto);

            // Panel Superior Derecho: Carrito de Compra
            GroupBox gbCarrito = new GroupBox { Text = "Carrito de compra", Location = new Point(440, 20), Size = new Size(490, 300) };
            
            dgvCarrito = new DataGridView
            {
                Location = new Point(15, 30), Size = new Size(460, 200),
                BackgroundColor = Color.White, BorderStyle = BorderStyle.FixedSingle,
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, Font = new Font("Segoe UI", 9F, FontStyle.Bold) },
                RowHeadersVisible = false, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvCarrito.CellClick += DgvCarrito_CellClick;
            
            lblTotal = new Label { Text = "TOTAL: S/ 0.00", Location = new Point(15, 250), Font = new Font("Segoe UI", 14F, FontStyle.Bold), ForeColor = Color.FromArgb(37, 99, 235), AutoSize = true };
            
            btnLimpiarCarrito = new Button { Text = "Vaciar carrito", Location = new Point(355, 245), Size = new Size(120, 35), BackColor = Color.FromArgb(100, 116, 139), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnLimpiarCarrito.FlatAppearance.BorderSize = 0;
            btnLimpiarCarrito.Click += (s, e) => LimpiarCarrito();

            gbCarrito.Controls.AddRange(new Control[] { dgvCarrito, lblTotal, btnLimpiarCarrito });
            this.Controls.Add(gbCarrito);

            // Panel Inferior: Cliente y confirmación
            GroupBox gbCliente = new GroupBox { Text = "Cliente y confirmación", Location = new Point(20, 340), Size = new Size(910, 280) };
            
            txtBuscarCliente = new TextBox { Location = new Point(15, 30), Size = new Size(370, 30), BorderStyle = BorderStyle.FixedSingle };
            txtBuscarCliente.Text = "Buscar por nombre o DNI...";
            txtBuscarCliente.ForeColor = Color.Gray;
            txtBuscarCliente.Enter += (s, e) => { if (txtBuscarCliente.Text == "Buscar por nombre o DNI...") { txtBuscarCliente.Text = ""; txtBuscarCliente.ForeColor = Color.Black; } };
            txtBuscarCliente.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtBuscarCliente.Text)) { txtBuscarCliente.Text = "Buscar por nombre o DNI..."; txtBuscarCliente.ForeColor = Color.Gray; } };
            txtBuscarCliente.TextChanged += TxtBuscarCliente_TextChanged;

            lstClientes = new ListBox { Location = new Point(15, 65), Size = new Size(370, 150), Font = new Font("Segoe UI", 9F) };
            lstClientes.SelectedIndexChanged += LstClientes_SelectedIndexChanged;

            lblClienteSeleccionado = new Label { Text = "Sin cliente seleccionado", Location = new Point(410, 120), AutoSize = true, Font = new Font("Segoe UI", 12F, FontStyle.Bold), ForeColor = Color.FromArgb(220, 38, 38) };
            
            btnConfirmarVenta = new Button { Text = "✓ Confirmar venta", Location = new Point(410, 160), Size = new Size(480, 55), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12F, FontStyle.Bold), Enabled = false };
            btnConfirmarVenta.FlatAppearance.BorderSize = 0;
            btnConfirmarVenta.Click += BtnConfirmarVenta_Click;

            gbCliente.Controls.AddRange(new Control[] { txtBuscarCliente, lstClientes, lblClienteSeleccionado, btnConfirmarVenta });
            this.Controls.Add(gbCliente);
        }

        private void TxtBuscarProducto_TextChanged(object sender, EventArgs e)
        {
            if (txtBuscarProducto.Text == "Escriba nombre o código...") return;
            string texto = txtBuscarProducto.Text.Trim();
            if (texto.Length >= 2)
            {
                var resultados = _servicioProducto.BuscarPorNombreOCodigo(texto);
                lstProductos.Items.Clear();
                foreach (var p in resultados)
                {
                    lstProductos.Items.Add(new ComboBoxItem { Text = $"{p.Codigo} - {p.Nombre} - S/{p.Precio:0.00} - Stock: {p.Stock}", Value = p });
                }
            }
            else if (texto.Length == 0)
            {
                lstProductos.Items.Clear();
            }
        }

        private void LstProductos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstProductos.SelectedItem is ComboBoxItem item)
            {
                productoActual = (Producto)item.Value;
                lblNombreSeleccionado.Text = productoActual.Nombre;
                lblPrecioSeleccionado.Text = $"Precio: S/ {productoActual.Precio:0.00}";
                lblStockDisponible.Text = $"Stock: {productoActual.Stock}";
                
                if (productoActual.Stock == 0)
                {
                    lblStockDisponible.ForeColor = Color.FromArgb(220, 38, 38);
                    btnAgregar.Enabled = false;
                }
                else
                {
                    lblStockDisponible.ForeColor = productoActual.Stock < 5 ? Color.FromArgb(249, 115, 22) : Color.FromArgb(22, 163, 74);
                    btnAgregar.Enabled = true;
                    nudCantidad.Maximum = productoActual.Stock;
                    nudCantidad.Value = 1;
                }
            }
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            if (productoActual == null || productoActual.Stock == 0) return;
            int cant = (int)nudCantidad.Value;
            
            var existente = carrito.FirstOrDefault(d => d.ProductoId == productoActual.Id);
            if (existente != null)
            {
                if (existente.Cantidad + cant > productoActual.Stock)
                {
                    MessageBox.Show("No hay suficiente stock para la cantidad solicitada.");
                    return;
                }
                existente.Cantidad += cant;
                existente.Subtotal = existente.Cantidad * existente.PrecioUnitario;
            }
            else
            {
                carrito.Add(new DetalleVenta
                {
                    ProductoId = productoActual.Id,
                    Cantidad = cant,
                    PrecioUnitario = productoActual.Precio,
                    Subtotal = cant * productoActual.Precio,
                    // Se usa temporalmente el Nombre en una propiedad custom si se requiriera, pero para dgv:
                });
            }

            ActualizarCarrito();
            ValidarConfirmacion();
            
            // Limpiar
            productoActual = null;
            lblNombreSeleccionado.Text = "-";
            lblPrecioSeleccionado.Text = "Precio: S/ 0.00";
            lblStockDisponible.Text = "Stock: 0";
            lblStockDisponible.ForeColor = Color.Black;
            btnAgregar.Enabled = false;
        }

        private void ActualizarCarrito()
        {
            dgvCarrito.DataSource = null;
            dgvCarrito.Columns.Clear();
            
            // Obtener info completa para el grid
            var source = carrito.Select(c => {
                var p = _servicioProducto.ObtenerTodos().FirstOrDefault(x => x.Id == c.ProductoId);
                return new {
                    Codigo = p?.Codigo ?? "",
                    Producto = p?.Nombre ?? "",
                    Cant = c.Cantidad,
                    PrecioUnit = c.PrecioUnitario,
                    Subtotal = c.Subtotal,
                    Id = c.ProductoId // hidden
                };
            }).ToList();
            
            dgvCarrito.DataSource = source;
            if (dgvCarrito.Columns["Id"] != null) dgvCarrito.Columns["Id"].Visible = false;

            // Añadir botón eliminar
            DataGridViewButtonColumn btnDelete = new DataGridViewButtonColumn();
            btnDelete.HeaderText = "";
            btnDelete.Name = "colEliminar";
            btnDelete.Text = "X";
            btnDelete.UseColumnTextForButtonValue = true;
            btnDelete.Width = 30;
            btnDelete.DefaultCellStyle.BackColor = Color.FromArgb(220, 38, 38);
            btnDelete.DefaultCellStyle.ForeColor = Color.White;
            btnDelete.FlatStyle = FlatStyle.Flat;
            dgvCarrito.Columns.Add(btnDelete);

            decimal total = carrito.Sum(d => d.Subtotal);
            lblTotal.Text = $"TOTAL: S/ {total:0.00}";
        }

        private void DgvCarrito_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvCarrito.Columns["colEliminar"].Index)
            {
                int prodId = (int)dgvCarrito.Rows[e.RowIndex].Cells["Id"].Value;
                var item = carrito.FirstOrDefault(c => c.ProductoId == prodId);
                if (item != null)
                {
                    carrito.Remove(item);
                    ActualizarCarrito();
                    ValidarConfirmacion();
                }
            }
        }

        private void LimpiarCarrito()
        {
            carrito.Clear();
            ActualizarCarrito();
            ValidarConfirmacion();
        }

        private void TxtBuscarCliente_TextChanged(object sender, EventArgs e)
        {
            if (txtBuscarCliente.Text == "Buscar por nombre o DNI...") return;
            string texto = txtBuscarCliente.Text.Trim();
            if (texto.Length >= 2)
            {
                var resultados = _servicioCliente.BuscarPorNombreODocumento(texto);
                lstClientes.Items.Clear();
                foreach (var c in resultados)
                {
                    lstClientes.Items.Add(new ComboBoxItem { Text = $"{c.Documento} - {c.Nombre} - {c.Telefono}", Value = c });
                }
            }
            else if (texto.Length == 0)
            {
                lstClientes.Items.Clear();
            }
        }

        private void LstClientes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstClientes.SelectedItem is ComboBoxItem item)
            {
                var cliente = (Cliente)item.Value;
                _clienteId = cliente.Id;
                lblClienteSeleccionado.Text = cliente.Nombre;
                lblClienteSeleccionado.ForeColor = Color.FromArgb(37, 99, 235);
                ValidarConfirmacion();
            }
        }

        private void ValidarConfirmacion()
        {
            btnConfirmarVenta.Enabled = (carrito.Count > 0 && _clienteId > 0);
        }

        private void BtnConfirmarVenta_Click(object sender, EventArgs e)
        {
            try
            {
                Venta venta = new Venta
                {
                    ClienteId = _clienteId,
                    UsuarioId = SesionActual.Usuario?.Id ?? 1,
                    Total = carrito.Sum(d => d.Subtotal),
                    Detalles = carrito
                };

                int idVenta = _servicioVenta.RegistrarVenta(venta);
                string numeroFactura = $"FAC-{DateTime.Now.Year}-{idVenta:D6}";
                MessageBox.Show($"Venta registrada.\nFactura: {numeroFactura}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                LimpiarCarrito();
                _clienteId = 0;
                lblClienteSeleccionado.Text = "Sin cliente seleccionado";
                lblClienteSeleccionado.ForeColor = Color.FromArgb(220, 38, 38);
                ValidarConfirmacion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private class ComboBoxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }
            public override string ToString() => Text;
        }
    }
}
