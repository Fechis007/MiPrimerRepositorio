using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PELOTA_TRABAJO_FINAL
{
    public partial class Form1 : Form
    {
        int dx = 3;
        int dy = 3;

        List<Button> botonesExtra = new List<Button>();
        List<Panel> formasExtra = new List<Panel>();
        int maxFormas = 3;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Botón redondo inicial
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(0, 0, btnPelota.Width, btnPelota.Height);
            btnPelota.Region = new Region(gp);

            btnPelota.FlatStyle = FlatStyle.Flat;
            btnPelota.FlatAppearance.BorderSize = 0;

            // Click en botón → cerrar programa
            btnPelota.Click += (s, ev) => this.Close();

            // Activar timer
            timer1.Interval = 20;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            btnPelota.Left += dx;
            btnPelota.Top += dy;

            int formRight = this.ClientSize.Width;
            int formBottom = this.ClientSize.Height;

            bool esquina = false;

            // --- ESQUINAS ---
            if (btnPelota.Right >= formRight && btnPelota.Top <= 0)
            {
                CambiarTamanyoYColor(btnPelota, true);
                dx = -Math.Abs(dx);
                dy = Math.Abs(dy);
                esquina = true;
            }

            if (btnPelota.Left <= 0 && btnPelota.Bottom >= formBottom)
            {
                CambiarTamanyoYColor(btnPelota, false);
                dx = Math.Abs(dx);
                dy = -Math.Abs(dy);
                esquina = true;
            }

            if (btnPelota.Left <= 0 && btnPelota.Top <= 0)
            {
                CambiarTamanyoYColor(btnPelota, true);
                dx = Math.Abs(dx);
                dy = Math.Abs(dy);
                esquina = true;
            }

            if (btnPelota.Right >= formRight && btnPelota.Bottom >= formBottom)
            {
                CambiarTamanyoYColor(btnPelota, false);
                dx = -Math.Abs(dx);
                dy = -Math.Abs(dy);
                esquina = true;
            }

            // Rebote normal y acciones de bordes
            if (!esquina)
            {
                // Parte derecha → botón clon
                if (btnPelota.Right >= formRight)
                {
                    btnPelota.Left = formRight - btnPelota.Width;
                    dx = -Math.Abs(dx);
                    CrearBotonClon();
                }

                // Parte izquierda → forma clon
                if (btnPelota.Left <= 0)
                {
                    btnPelota.Left = 0;
                    dx = Math.Abs(dx);
                    CrearFormaClon();
                }

                // Parte superior → quitar una forma extra
                if (btnPelota.Top <= 0)
                {
                    btnPelota.Top = 0;
                    dy = Math.Abs(dy);
                    EliminarFormaExtra();
                }

                // Parte inferior → quitar un botón extra
                if (btnPelota.Bottom >= formBottom)
                {
                    btnPelota.Top = formBottom - btnPelota.Height;
                    dy = -Math.Abs(dy);
                    EliminarBotonExtra();
                }
            }
        }

        private void CambiarTamanyoYColor(Button b, bool aumentar)
        {
            int cambio = 10;
            int nuevoW = b.Width + (aumentar ? cambio : -cambio);
            int nuevoH = b.Height + (aumentar ? cambio : -cambio);

            if (nuevoW < 20) nuevoW = 20;
            if (nuevoH < 20) nuevoH = 20;

            b.Size = new Size(nuevoW, nuevoH);

            Random rnd = new Random();
            b.BackColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));

            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(0, 0, b.Width, b.Height);
            b.Region = new Region(gp);
        }

        private void CrearBotonClon()
        {
            Button clon = new Button();
            clon.Size = btnPelota.Size;
            clon.Left = 50 + botonesExtra.Count * 60;
            clon.Top = 50;
            clon.BackColor = btnPelota.BackColor;
            clon.FlatStyle = FlatStyle.Flat;
            clon.FlatAppearance.BorderSize = 0;

            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(0, 0, clon.Width, clon.Height);
            clon.Region = new Region(gp);

            clon.Click += (s, ev) => this.Close();

            this.Controls.Add(clon);
            botonesExtra.Add(clon);
        }

        private void CrearFormaClon()
        {
            if (formasExtra.Count >= maxFormas) return;

            Panel forma = new Panel();
            forma.Size = btnPelota.Size;
            forma.Left = 50 + formasExtra.Count * 60;
            forma.Top = 50;
            forma.BackColor = btnPelota.BackColor;

            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(0, 0, forma.Width, forma.Height);
            forma.Region = new Region(gp);

            // Click en forma → reinicia todo
            forma.Click += (s, ev) => ReiniciarPrograma();

            this.Controls.Add(forma);
            formasExtra.Add(forma);
        }

        private void EliminarFormaExtra()
        {
            if (formasExtra.Count > 0)
            {
                Panel ultima = formasExtra[formasExtra.Count - 1];
                this.Controls.Remove(ultima);
                formasExtra.Remove(ultima);
            }
        }

        private void EliminarBotonExtra()
        {
            if (botonesExtra.Count > 0)
            {
                Button ultimo = botonesExtra[botonesExtra.Count - 1];
                this.Controls.Remove(ultimo);
                botonesExtra.Remove(ultimo);
            }
        }

        private void ReiniciarPrograma()
        {
            // Quitar todas las formas extra
            foreach (var f in formasExtra)
                this.Controls.Remove(f);
            formasExtra.Clear();

            // Quitar todos los botones extra
            foreach (var b in botonesExtra)
                this.Controls.Remove(b);
            botonesExtra.Clear();

            // Restaurar botón inicial
            btnPelota.Size = new Size(50, 50);
            btnPelota.BackColor = Color.Red;
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(0, 0, btnPelota.Width, btnPelota.Height);
            btnPelota.Region = new Region(gp);

            // Reposicionar en el centro
            btnPelota.Left = (this.ClientSize.Width - btnPelota.Width) / 2;
            btnPelota.Top = (this.ClientSize.Height - btnPelota.Height) / 2;
        }
    }
}