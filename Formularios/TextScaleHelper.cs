using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TechStore.Formularios
{
    public static class TextScaleHelper
    {
        private const int MinScalePercent = 80;
        private const int MaxScalePercent = 180;
        private static int _currentScalePercent = 100;

        public static int CurrentScalePercent => _currentScalePercent;

        public static void Ajustar(int deltaPercent)
        {
            _currentScalePercent = Math.Max(MinScalePercent, Math.Min(MaxScalePercent, _currentScalePercent + deltaPercent));
            AplicarATodasLasVentanas();
        }

        public static void Aplicar(Form form)
        {
            if (form == null)
                return;

            AplicarControl(form);
            AplicarStripItems(form);

            foreach (Form openForm in Application.OpenForms.Cast<Form>().Where(f => f != form).ToList())
            {
                AplicarControl(openForm);
                AplicarStripItems(openForm);
            }
        }

        private static void AplicarATodasLasVentanas()
        {
            foreach (Form form in Application.OpenForms.Cast<Form>().ToList())
            {
                AplicarControl(form);
                AplicarStripItems(form);
            }
        }

        private static void AplicarControl(Control control)
        {
            if (control == null)
                return;

            if (control.Font != null)
            {
                float baseSize = ObtenerTamanioBase(control);
                float nuevoTamanio = baseSize * (GetFactorEscala());
                control.Font = new Font(control.Font.FontFamily, nuevoTamanio, control.Font.Style);
            }

            foreach (Control child in control.Controls.Cast<Control>().ToList())
            {
                AplicarControl(child);
            }
        }

        private static void AplicarStripItems(Control control)
        {
            if (control is ToolStrip strip)
            {
                foreach (ToolStripItem item in strip.Items.Cast<ToolStripItem>().ToList())
                {
                    if (item.Font != null)
                    {
                        float baseSize = ObtenerTamanioBase(item);
                        float nuevoTamanio = baseSize * GetFactorEscala();
                        item.Font = new Font(item.Font.FontFamily, nuevoTamanio, item.Font.Style);
                    }

                    if (item is ToolStripDropDownItem dropDownItem)
                    {
                        foreach (ToolStripItem child in dropDownItem.DropDownItems.Cast<ToolStripItem>().ToList())
                        {
                            if (child.Font != null)
                            {
                                float baseSize = ObtenerTamanioBase(child);
                                float nuevoTamanio = baseSize * GetFactorEscala();
                                child.Font = new Font(child.Font.FontFamily, nuevoTamanio, child.Font.Style);
                            }
                        }
                    }
                }
            }
        }

        private static float ObtenerTamanioBase(object elemento)
        {
            if (elemento is Control control)
            {
                if (control.Tag is float tamanioBase)
                    return tamanioBase;

                float tamanio = control.Font?.Size ?? 9f;
                control.Tag = tamanio;
                return tamanio;
            }

            if (elemento is ToolStripItem item)
            {
                if (item.Tag is float tamanioBase)
                    return tamanioBase;

                float tamanio = item.Font?.Size ?? 9f;
                item.Tag = tamanio;
                return tamanio;
            }

            return 9f;
        }

        private static float GetFactorEscala()
        {
            return _currentScalePercent / 100f;
        }
    }
}
