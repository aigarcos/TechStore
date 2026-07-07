using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TechStore.Modelos;
using TechStore.Servicios.Interfaces;

namespace TechStore.Formularios
{
    public partial class FrmInventario : BaseForm
    {
        private readonly IServicioProducto _servicioProducto;
        
        private int _productoSeleccionadoId = 0;

        // Búsqueda
        private TextBox txtBuscar;
        private ComboBox cmbCategoriaFiltro;
        private Button btnNuevo;

        // Grid
        private DataGridView dgvProductos;

        // Edición
        private Label lblModoActual;
        private TextBox txtCodigo, txtNombre, txtPrecio, txtStock;
        private ComboBox cmbCategoriaProducto;
        private Button btnGuardar, btnEliminar, btnCancelar;

        // Resumen
        private Label lblTotalProductos, lblSinStock, lblStockBajo;

        public FrmInventario(IServicioProducto servicioProducto)
        {
            _servicioProducto = servicioProducto;
            InitializeComponent();
            CargarDatos();
        }

        private void InitializeComponent()
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(1000, 600);
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9F);
            this.Text = "TechStore - Inventario";

            // Barra superior
            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(245, 247, 250) };
            
            txtBuscar = new TextBox { Location = new Point(20, 15), Size = new Size(300, 30), BorderStyle = BorderStyle.FixedSingle };
            txtBuscar.Text = "Buscar por nombre o código...";
            txtBuscar.ForeColor = Color.Gray;
            txtBuscar.Enter += (s, e) => { if (txtBuscar.Text == "Buscar por nombre o código...") { txtBuscar.Text = ""; txtBuscar.ForeColor = Color.Black; } };
            txtBuscar.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtBuscar.Text)) { txtBuscar.Text = "Buscar por nombre o código..."; txtBuscar.ForeColor = Color.Gray; } };
            txtBuscar.TextChanged += Filtros_Changed;

            cmbCategoriaFiltro = new ComboBox { Location = new Point(340, 15), Size = new Size(150, 30), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbCategoriaFiltro.Items.AddRange(new object[] { "Todos", "Laptops", "Periféricos", "Monitores", "Audio", "Accesorios" });
            cmbCategoriaFiltro.SelectedIndex = 0;
            cmbCategoriaFiltro.SelectedIndexChanged += Filtros_Changed;

            btnNuevo = new Button { Text = "＋ Nuevo producto", Location = new Point(510, 12), Size = new Size(140, 35), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnNuevo.FlatAppearance.BorderSize = 0;
            btnNuevo.Click += BtnNuevo_Click;

            pnlTop.Controls.AddRange(new Control[] { txtBuscar, cmbCategoriaFiltro, btnNuevo });
            this.Controls.Add(pnlTop);

            // Grid
            dgvProductos = new DataGridView
            {
                Location = new Point(20, 80), Size = new Size(620, 450),
                BackgroundColor = Color.White, BorderStyle = BorderStyle.FixedSingle,
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, Font = new Font("Segoe UI", 9F, FontStyle.Bold) },
                RowHeadersVisible = false, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(248, 250, 252) },
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvProductos.CellClick += DgvProductos_CellClick;
            dgvProductos.CellFormatting += DgvProductos_CellFormatting;
            this.Controls.Add(dgvProductos);

            // Panel Derecho (Edición)
            GroupBox gbDetalle = new GroupBox { Text = "Datos del producto", Location = new Point(660, 80), Size = new Size(320, 380) };
            
            lblModoActual = new Label { Text = "Modo: Nuevo producto", Location = new Point(15, 30), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(37, 99, 235) };
            
            Label lblCod = new Label { Text = "Código:", Location = new Point(15, 70), AutoSize = true };
            txtCodigo = new TextBox { Location = new Point(90, 65), Size = new Size(210, 30), BorderStyle = BorderStyle.FixedSingle };
            
            Label lblNom = new Label { Text = "Nombre:", Location = new Point(15, 110), AutoSize = true };
            txtNombre = new TextBox { Location = new Point(90, 105), Size = new Size(210, 30), BorderStyle = BorderStyle.FixedSingle };
            
            Label lblCat = new Label { Text = "Categoría:", Location = new Point(15, 150), AutoSize = true };
            cmbCategoriaProducto = new ComboBox { Location = new Point(90, 145), Size = new Size(210, 30), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbCategoriaProducto.Items.AddRange(new object[] { "Laptops", "Periféricos", "Monitores", "Audio", "Accesorios" });
            cmbCategoriaProducto.SelectedIndex = 0;

            Label lblPre = new Label { Text = "Precio (S/):", Location = new Point(15, 190), AutoSize = true };
            txtPrecio = new TextBox { Location = new Point(90, 185), Size = new Size(210, 30), BorderStyle = BorderStyle.FixedSingle };
            txtPrecio.Leave += ValidarDecimal;
            
            Label lblStk = new Label { Text = "Stock:", Location = new Point(15, 230), AutoSize = true };
            txtStock = new TextBox { Location = new Point(90, 225), Size = new Size(210, 30), BorderStyle = BorderStyle.FixedSingle };
            txtStock.Leave += ValidarEntero;

            btnGuardar = new Button { Text = "Guardar", Location = new Point(15, 280), Size = new Size(90, 35), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.Click += BtnGuardar_Click;

            btnEliminar = new Button { Text = "Eliminar", Location = new Point(115, 280), Size = new Size(90, 35), BackColor = Color.FromArgb(220, 38, 38), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Enabled = false };
            btnEliminar.FlatAppearance.BorderSize = 0;
            btnEliminar.Click += BtnEliminar_Click;

            btnCancelar = new Button { Text = "Cancelar", Location = new Point(215, 280), Size = new Size(85, 35), BackColor = Color.FromArgb(100, 116, 139), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => LimpiarFormulario();

            gbDetalle.Controls.AddRange(new Control[] { lblModoActual, lblCod, txtCodigo, lblNom, txtNombre, lblCat, cmbCategoriaProducto, lblPre, txtPrecio, lblStk, txtStock, btnGuardar, btnEliminar, btnCancelar });
            this.Controls.Add(gbDetalle);

            // Resumen Inferior
            Panel pnlResumen = new Panel { Location = new Point(660, 470), Size = new Size(320, 60), BorderStyle = BorderStyle.FixedSingle };
            lblTotalProductos = new Label { Text = "Total: 0", Location = new Point(10, 10), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            lblSinStock = new Label { Text = "Sin stock: 0", Location = new Point(10, 35), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            lblStockBajo = new Label { Text = "Stock bajo: 0", Location = new Point(150, 35), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            
            pnlResumen.Controls.AddRange(new Control[] { lblTotalProductos, lblSinStock, lblStockBajo });
            this.Controls.Add(pnlResumen);
        }

        private void CargarDatos()
        {
            try
            {
                var texto = txtBuscar.Text == "Buscar por nombre o código..." ? "" : txtBuscar.Text.Trim();
                var productos = string.IsNullOrEmpty(texto) 
                    ? _servicioProducto.ObtenerTodos() 
                    : _servicioProducto.BuscarPorNombreOCodigo(texto);

                if (cmbCategoriaFiltro.SelectedIndex > 0) // Si no es "Todos"
                {
                    string cat = cmbCategoriaFiltro.SelectedItem.ToString();
                    productos = productos.Where(p => p.Categoria == cat).ToList();
                }

                var source = productos.Select(p => new {
                    Id = p.Id,
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    Categoria = p.Categoria,
                    Precio = p.Precio,
                    Stock = p.Stock,
                    Estado = p.Stock == 0 ? "Sin stock" : (p.Stock < 5 ? "Stock bajo" : "OK")
                }).ToList();

                dgvProductos.DataSource = source;
                if (dgvProductos.Columns["Id"] != null) dgvProductos.Columns["Id"].Visible = false;

                ColorearFilas();
                ActualizarEstadisticas(productos);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar inventario: " + ex.Message);
            }
        }

        private void ColorearFilas()
        {
            foreach (DataGridViewRow row in dgvProductos.Rows)
            {
                string estado = row.Cells["Estado"].Value.ToString();
                if (estado == "Sin stock")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(254, 226, 226); // Light red
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(220, 38, 38);
                }
                else if (estado == "Stock bajo")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 237, 213); // Light orange
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(234, 88, 12);
                }
            }
        }

        private void ActualizarEstadisticas(System.Collections.Generic.List<Producto> productos)
        {
            lblTotalProductos.Text = $"Total productos: {productos.Count}";
            int sinStock = productos.Count(p => p.Stock == 0);
            int stockBajo = productos.Count(p => p.Stock > 0 && p.Stock < 5);
            
            lblSinStock.Text = $"Sin stock: {sinStock}";
            lblSinStock.ForeColor = sinStock > 0 ? Color.FromArgb(220, 38, 38) : Color.Black;

            lblStockBajo.Text = $"Stock bajo: {stockBajo}";
            lblStockBajo.ForeColor = stockBajo > 0 ? Color.FromArgb(234, 88, 12) : Color.Black;
        }

        private void Filtros_Changed(object sender, EventArgs e)
        {
            if (txtBuscar.Text != "Buscar por nombre o código..." || sender == cmbCategoriaFiltro)
            {
                CargarDatos();
            }
        }

        // Filtrado rápido por categoría desde código (ej: "Laptops", "Accesorios")
        public void FiltrarPorCategoria(string categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria)) return;
            for (int i = 0; i < cmbCategoriaFiltro.Items.Count; i++)
            {
                if (cmbCategoriaFiltro.Items[i].ToString().Equals(categoria, StringComparison.OrdinalIgnoreCase))
                {
                    cmbCategoriaFiltro.SelectedIndex = i;
                    CargarDatos();
                    return;
                }
            }
            cmbCategoriaFiltro.SelectedIndex = 0; // "Todos"
            CargarDatos();
        }

        // Calcula el valor monetario total del inventario: suma de (Stock * Precio)
        public decimal CalcularValorInventario()
        {
            try
            {
                var productos = _servicioProducto.ObtenerTodos();
                decimal total = 0m;
                foreach (var p in productos)
                {
                    total += p.Stock * p.Precio;
                }
                return total;
            }
            catch
            {
                return 0m;
            }
        }

        private void DgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                _productoSeleccionadoId = (int)dgvProductos.Rows[e.RowIndex].Cells["Id"].Value;
                txtCodigo.Text = dgvProductos.Rows[e.RowIndex].Cells["Codigo"].Value?.ToString();
                txtNombre.Text = dgvProductos.Rows[e.RowIndex].Cells["Nombre"].Value?.ToString();
                cmbCategoriaProducto.SelectedItem = dgvProductos.Rows[e.RowIndex].Cells["Categoria"].Value?.ToString();
                txtPrecio.Text = dgvProductos.Rows[e.RowIndex].Cells["Precio"].Value?.ToString();
                txtStock.Text = dgvProductos.Rows[e.RowIndex].Cells["Stock"].Value?.ToString();
                
                lblModoActual.Text = $"Modo: Editando — {txtNombre.Text}";
                btnEliminar.Enabled = true;
            }
        }

        private void DgvProductos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;
                var row = dgvProductos.Rows[e.RowIndex];
                var cell = row.Cells["Stock"];
                if (cell?.Value != null && int.TryParse(cell.Value.ToString(), out int stock))
                {
                    const int minimoPermitido = 5; // Ajustar según configuración
                    if (stock < minimoPermitido)
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(254, 226, 226);
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(220, 38, 38);
                    }
                }
            }
            catch { }
        }

        private void BtnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void LimpiarFormulario()
        {
            _productoSeleccionadoId = 0;
            txtCodigo.Text = "";
            txtNombre.Text = "";
            txtPrecio.Text = "";
            txtStock.Text = "";
            lblModoActual.Text = "Modo: Nuevo producto";
            btnEliminar.Enabled = false;
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text) || string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Código y Nombre son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var codigoIngresado = txtCodigo.Text.Trim();
                var nombreIngresado = txtNombre.Text.Trim();
                var productosExistentes = _servicioProducto.ObtenerTodos();

                // Verificar duplicados (cuando se crea uno nuevo se compara contra todos,
                // cuando se edita se excluye el propio registro)
                if (_productoSeleccionadoId == 0)
                {
                    if (productosExistentes.Any(p => p.Codigo.Equals(codigoIngresado, StringComparison.OrdinalIgnoreCase)))
                    {
                        MessageBox.Show("El código ingresado ya existe en otro producto.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (productosExistentes.Any(p => p.Nombre.Equals(nombreIngresado, StringComparison.OrdinalIgnoreCase)))
                    {
                        MessageBox.Show("El nombre ingresado ya existe en otro producto.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else
                {
                    if (productosExistentes.Any(p => p.Id != _productoSeleccionadoId && p.Codigo.Equals(codigoIngresado, StringComparison.OrdinalIgnoreCase)))
                    {
                        MessageBox.Show("El código ingresado ya existe en otro producto.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (productosExistentes.Any(p => p.Id != _productoSeleccionadoId && p.Nombre.Equals(nombreIngresado, StringComparison.OrdinalIgnoreCase)))
                    {
                        MessageBox.Show("El nombre ingresado ya existe en otro producto.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                var producto = new Producto
                {
                    Id = _productoSeleccionadoId,
                    Codigo = txtCodigo.Text,
                    Nombre = txtNombre.Text,
                    Categoria = cmbCategoriaProducto.SelectedItem.ToString(),
                    Precio = string.IsNullOrWhiteSpace(txtPrecio.Text) ? 0 : Convert.ToDecimal(txtPrecio.Text),
                    Stock = string.IsNullOrWhiteSpace(txtStock.Text) ? 0 : Convert.ToInt32(txtStock.Text)
                };

                if (_productoSeleccionadoId == 0)
                {
                    _servicioProducto.Registrar(producto);
                    MessageBox.Show("Producto guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _servicioProducto.Actualizar(producto);
                    MessageBox.Show("Producto actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
                LimpiarFormulario();
                CargarDatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (_productoSeleccionadoId > 0)
            {
                if (MessageBox.Show($"¿Desea eliminar el producto {txtNombre.Text}?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        _servicioProducto.Eliminar(_productoSeleccionadoId);
                        MessageBox.Show("Producto eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarFormulario();
                        CargarDatos();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error al eliminar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ValidarDecimal(object sender, EventArgs e)
        {
            var txt = (TextBox)sender;
            if (!string.IsNullOrWhiteSpace(txt.Text) && !decimal.TryParse(txt.Text, out _))
            {
                MessageBox.Show("Ingrese un valor decimal válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt.Text = "0";
                txt.Focus();
            }
        }

        private void ValidarEntero(object sender, EventArgs e)
        {
            var txt = (TextBox)sender;
            if (!string.IsNullOrWhiteSpace(txt.Text) && !int.TryParse(txt.Text, out _))
            {
                MessageBox.Show("Ingrese un valor entero válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt.Text = "0";
                txt.Focus();
            }
        }
    }
}
