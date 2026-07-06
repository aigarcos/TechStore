using System;
using System.Drawing;
using System.Windows.Forms;

namespace TechStore.Formularios
{
    public static class TemaOscuroHelper
    {
        private static bool _activo = false;

        public static bool Activo => _activo;

        public static void Alternar(Form form)
        {
            _activo = !_activo;
            Aplicar(form);
        }

        public static void Aplicar(Control control)
        {
            if (control == null)
                return;

            if (control is Form form)
            {
                if (_activo)
                {
                    form.BackColor = Color.FromArgb(24, 24, 24);
                    form.ForeColor = Color.White;
                }
                else
                {
                    form.BackColor = Color.White;
                    form.ForeColor = Color.Black;
                }
            }

            foreach (Control child in control.Controls)
            {
                if (child is TextBox txt)
                {
                    txt.BackColor = _activo ? Color.FromArgb(40, 40, 40) : Color.White;
                    txt.ForeColor = _activo ? Color.White : Color.Black;
                }
                else if (child is Label lbl)
                {
                    lbl.ForeColor = _activo ? Color.White : Color.Black;
                }
                else if (child is Button btn)
                {
                    if (btn.Text == "Ingresar" || btn.Text == "Actualizar Dashboard" || btn.Text == "A+" || btn.Text == "A-" || btn.Text == "+" || btn.Text == "-")
                    {
                        btn.BackColor = _activo ? Color.FromArgb(37, 99, 235) : btn.BackColor;
                        btn.ForeColor = Color.White;
                    }
                    else
                    {
                        btn.BackColor = _activo ? Color.FromArgb(40, 40, 40) : SystemColors.Control;
                        btn.ForeColor = _activo ? Color.White : Color.Black;
                    }
                }
                else if (child is Panel panel)
                {
                    panel.BackColor = _activo ? Color.FromArgb(35, 35, 35) : Color.White;
                }
                else if (child is DataGridView dgv)
                {
                    dgv.BackgroundColor = _activo ? Color.FromArgb(35, 35, 35) : Color.White;
                    dgv.ForeColor = _activo ? Color.White : Color.Black;
                    dgv.GridColor = _activo ? Color.FromArgb(70, 70, 70) : Color.LightGray;
                    dgv.EnableHeadersVisualStyles = false;
                    dgv.DefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = _activo ? Color.FromArgb(40, 40, 40) : Color.White,
                        ForeColor = _activo ? Color.White : Color.Black,
                        SelectionBackColor = _activo ? Color.FromArgb(59, 130, 246) : SystemColors.Highlight,
                        SelectionForeColor = Color.White
                    };
                    dgv.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = _activo ? Color.FromArgb(50, 50, 50) : Color.FromArgb(248, 250, 252),
                        ForeColor = _activo ? Color.White : Color.Black,
                        SelectionBackColor = _activo ? Color.FromArgb(59, 130, 246) : SystemColors.Highlight,
                        SelectionForeColor = Color.White
                    };
                    dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = _activo ? Color.FromArgb(32, 32, 32) : Color.FromArgb(37, 99, 235),
                        ForeColor = Color.White,
                        Font = new Font(dgv.Font, FontStyle.Bold)
                    };
                    dgv.RowHeadersDefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = _activo ? Color.FromArgb(32, 32, 32) : Color.White,
                        ForeColor = _activo ? Color.White : Color.Black
                    };
                }
                else if (child is MenuStrip menu)
                {
                    menu.BackColor = _activo ? Color.FromArgb(35, 35, 35) : Color.White;
                    menu.ForeColor = _activo ? Color.White : Color.Black;
                }
                else if (child is StatusStrip status)
                {
                    status.BackColor = _activo ? Color.FromArgb(35, 35, 35) : Color.White;
                    status.ForeColor = _activo ? Color.White : Color.Black;
                }

                if (child.Controls.Count > 0)
                {
                    Aplicar(child);
                }
            }
        }
    }
}
