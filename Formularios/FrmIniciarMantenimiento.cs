using System;
using System.Drawing;
using System.Windows.Forms;
using TechStore.Servicios.Interfaces;

namespace TechStore.Formularios
{
    public partial class FrmIniciarMantenimiento : BaseForm
    {
        private readonly IServicioCMDB _servicioCMDB;
        private readonly int _itemId;
        private readonly string _nombreItem;

        private Label lblCI;
        private TextBox txtMotivo;
        private DateTimePicker dtpInicio;
        private Button btnConfirmar, btnCancelar;

        public FrmIniciarMantenimiento(IServicioCMDB servicioCMDB, int itemId, string nombreItem)
        {
            _servicioCMDB = servicioCMDB;
            _itemId = itemId;
            _nombreItem = nombreItem;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(400, 250);
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9F);
            this.Text = "Iniciar Mantenimiento";
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            lblCI = new Label { Text = $"CI: {_nombreItem}", Location = new Point(20, 20), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            
            Label lblMotivo = new Label { Text = "Motivo del mantenimiento:", Location = new Point(20, 60), AutoSize = true };
            txtMotivo = new TextBox { Location = new Point(20, 80), Size = new Size(340, 50), Multiline = true, BorderStyle = BorderStyle.FixedSingle };
            
            Label lblInicio = new Label { Text = "Fecha de inicio:", Location = new Point(20, 140), AutoSize = true };
            dtpInicio = new DateTimePicker { Location = new Point(130, 135), Size = new Size(230, 30), Format = DateTimePickerFormat.Short };

            btnConfirmar = new Button { Text = "Confirmar", Location = new Point(160, 190), Size = new Size(90, 35), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnConfirmar.FlatAppearance.BorderSize = 0;
            btnConfirmar.Click += BtnConfirmar_Click;

            btnCancelar = new Button { Text = "Cancelar", Location = new Point(270, 190), Size = new Size(90, 35), BackColor = Color.FromArgb(100, 116, 139), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[] { lblCI, lblMotivo, txtMotivo, lblInicio, dtpInicio, btnConfirmar, btnCancelar });
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMotivo.Text))
            {
                MessageBox.Show("El motivo es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _servicioCMDB.IniciarMantenimiento(_itemId, txtMotivo.Text.Trim(), dtpInicio.Value);
                MessageBox.Show("Mantenimiento iniciado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
