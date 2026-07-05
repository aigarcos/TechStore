using System;
using System.Drawing;
using System.Windows.Forms;
using TechStore.Servicios.Interfaces;

namespace TechStore.Formularios
{
    public partial class FrmFinalizarMantenimiento : BaseForm
    {
        private readonly IServicioCMDB _servicioCMDB;
        private readonly int _itemId;
        private readonly string _nombreItem;
        private readonly DateTime? _fechaInicio;

        private Label lblCI, lblInicio;
        private DateTimePicker dtpFin;
        private Button btnConfirmar, btnCancelar;

        public FrmFinalizarMantenimiento(IServicioCMDB servicioCMDB, int itemId, string nombreItem, DateTime? fechaInicio)
        {
            _servicioCMDB = servicioCMDB;
            _itemId = itemId;
            _nombreItem = nombreItem;
            _fechaInicio = fechaInicio;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(380, 200);
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9F);
            this.Text = "Finalizar Mantenimiento";
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            lblCI = new Label { Text = $"CI: {_nombreItem}", Location = new Point(20, 20), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            
            lblInicio = new Label { Text = $"Inicio de Mantenimiento: {(_fechaInicio.HasValue ? _fechaInicio.Value.ToShortDateString() : "Desconocido")}", Location = new Point(20, 60), AutoSize = true };
            
            Label lblFin = new Label { Text = "Fecha de fin:", Location = new Point(20, 100), AutoSize = true };
            dtpFin = new DateTimePicker { Location = new Point(130, 95), Size = new Size(200, 30), Format = DateTimePickerFormat.Short };

            btnConfirmar = new Button { Text = "Confirmar", Location = new Point(130, 150), Size = new Size(90, 35), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnConfirmar.FlatAppearance.BorderSize = 0;
            btnConfirmar.Click += BtnConfirmar_Click;

            btnCancelar = new Button { Text = "Cancelar", Location = new Point(240, 150), Size = new Size(90, 35), BackColor = Color.FromArgb(100, 116, 139), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[] { lblCI, lblInicio, lblFin, dtpFin, btnConfirmar, btnCancelar });
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                _servicioCMDB.FinalizarMantenimiento(_itemId, dtpFin.Value);
                MessageBox.Show("Mantenimiento finalizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
