using System;
using System.Drawing;
using System.Windows.Forms;
using TechStore.Servicios.Implementaciones;
using TechStore.Servicios.Interfaces;

namespace TechStore.Formularios
{
    public partial class FrmLogin : Form
    {
        private readonly IServicioAutenticacion _servicioAutenticacion;
        private readonly IServiceProvider _serviceProvider;

        private TextBox txtUsuario;
        private TextBox txtContrasena;
        private Button btnIngresar;
        private Label lblError;
        private Label lblIntentos;

        public FrmLogin(IServicioAutenticacion servicioAutenticacion, IServiceProvider serviceProvider)
        {
            _servicioAutenticacion = servicioAutenticacion;
            _serviceProvider = serviceProvider;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(400, 300);
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9F);
            this.Text = "TechStore - Login";

            Label lblTitulo = new Label
            {
                Text = "TechStore",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(37, 99, 235), // Primary Blue
                AutoSize = true,
                Location = new Point(130, 30)
            };

            Label lblSubtitulo = new Label
            {
                Text = "Sistema de gestión",
                ForeColor = Color.FromArgb(100, 116, 139), // Secondary Text
                AutoSize = true,
                Location = new Point(145, 70)
            };

            txtUsuario = new TextBox
            {
                Location = new Point(50, 110),
                Size = new Size(300, 30),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10F)
            };

            txtContrasena = new TextBox
            {
                Location = new Point(50, 150),
                Size = new Size(300, 30),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10F),
                PasswordChar = '*'
            };

            btnIngresar = new Button
            {
                Text = "Ingresar",
                Location = new Point(50, 200),
                Size = new Size(300, 36),
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnIngresar.FlatAppearance.BorderSize = 0;
            btnIngresar.Click += BtnIngresar_Click;

            lblError = new Label
            {
                ForeColor = Color.FromArgb(220, 38, 38), // Red error
                AutoSize = true,
                Location = new Point(50, 245),
                Visible = false
            };

            this.Controls.Add(lblTitulo);
            this.Controls.Add(lblSubtitulo);
            this.Controls.Add(txtUsuario);
            this.Controls.Add(txtContrasena);
            this.Controls.Add(btnIngresar);
            this.Controls.Add(lblError);
        }

        private void BtnIngresar_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;

            try
            {
                var usuario = _servicioAutenticacion.Autenticar(txtUsuario.Text, txtContrasena.Text);

                if (usuario != null)
                {
                    this.Hide();
                    // Resolvemos el FrmPrincipal a través de DI
                    var frmPrincipal = (FrmPrincipal)_serviceProvider.GetService(typeof(FrmPrincipal));
                    frmPrincipal.FormClosed += (s, args) => this.Show(); // Volver al login al cerrar
                    frmPrincipal.Show();
                    
                    // Limpiar campos
                    txtUsuario.Text = "";
                    txtContrasena.Text = "";
                }
                else
                {
                    lblError.Text = "Credenciales incorrectas.";
                    lblError.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                lblError.Visible = true;
                if (ex.Message.Contains("bloqueado"))
                {
                    btnIngresar.Enabled = false;
                }
            }
        }
    }
}
