﻿using System;
using System.Windows.Forms;

namespace aplicacion_musica
{
    public partial class CrearCD : Form
    {
        private Album album;
        private DiscoCompacto cd;
        private short numDiscos;
        private short NDisco;
        private short NC;
        private bool edit = false;
        /// <summary>
        /// Constructor para crear un sólo CD, para álbumes con menos de 80 minutos de duración
        /// </summary>
        /// <param name="a"></param>
        public CrearCD(ref Album a)
        {
            InitializeComponent();
            album = a;
            Console.WriteLine("Creando sólo un CD, duración: " + album.duracion);
            numericUpDownNumCanciones.Hide();
            labelNumCanciones.Hide();
            NDisco = 1;
            NC = a.numCanciones;
            PonerTextos();
        }
        /// <summary>
        /// Constructor para crear el primer cd de varios.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="nd">número de discos</param>
        public CrearCD(ref Album a, short nd)
        {
            InitializeComponent();
            Console.WriteLine("Creando primer CD");
            album = a;
            NDisco = 1;
            numDiscos = nd;
            PonerTextos();
        }
        /// <summary>
        /// Constructor para crear un segundo, tercer CD
        /// </summary>
        /// <param name="cdd">CD original</param>
        /// <param name="n">número del disco, en el caso de editar es disco 1, 2...</param>
        /// <param name="edit">configura el formulario para la edición</param>
        public CrearCD(ref DiscoCompacto cdd, short n, bool edit = false)
        {
            InitializeComponent();
            NDisco = n;
            album = cdd.Album;
            cd = cdd;
            if(n > 1 && !edit)
            {
                labelFormato.Hide();
                comboBoxFormatoCD.Hide();
                labelAñoPublicacion.Hide();
                comboBoxEstadoExterior.Hide();
                labelEstadoExterior.Hide();
                labelAñoPublicacion.Hide();
                labelPaisPublicacion.Hide();
                textBoxPais.Hide();
                textBoxAño.Hide();
                numericUpDownNumCanciones.Maximum = album.numCanciones - cdd.Discos[0].NumCanciones;
                numericUpDownNumCanciones.Value = numericUpDownNumCanciones.Maximum;
                Log.Instance.ImprimirMensaje("Creando otro CD con un máximo de "+numericUpDownNumCanciones.Maximum, TipoMensaje.Info);
            }
            else if(edit)
            {
                Log.Instance.ImprimirMensaje("Editando CD", TipoMensaje.Info);
                this.edit = true;
                comboBoxFormatoCD.SelectedItem = cdd.FormatoCD;
                comboBoxEstadoMedio.SelectedItem = cdd.Discos[n-1].EstadoDisco;
                comboBoxEstadoExterior.SelectedItem = cdd.EstadoExterior;
                numericUpDownNumCanciones.Value = cdd.Discos[n-1].NumCanciones;
                textBoxAño.Text = cd.YearRelease.ToString();
                textBoxPais.Text = cd.PaisPublicacion;
            }
            PonerTextos();
        }
        private void PonerTextos()
        {
            labelEstadoExterior.Text = Programa.textosLocal.GetString("estado_exterior");
            labelEstadoMedio.Text = Programa.textosLocal.GetString("estado_medio");
            labelFormato.Text = Programa.textosLocal.GetString("formato");
            labelAñoPublicacion.Text = Programa.textosLocal.GetString("añoPublicacion");
            labelPaisPublicacion.Text = Programa.textosLocal.GetString("paisPublicacion");
            labelNumCanciones.Text = Programa.textosLocal.GetString("numcanciones");
            String[] eeT = new string[7];
            String[] fT = new string[4];
            for (int i = 0; i < eeT.Length; i++)
                eeT[i] = Programa.textosLocal.GetString(Enum.GetName(typeof(EstadoMedio), i));
            for (int i = 0; i < fT.Length; i++)
                fT[i] = Programa.textosLocal.GetString(Enum.GetName(typeof(FormatoCD), i));
            comboBoxEstadoMedio.Items.AddRange(eeT);
            comboBoxEstadoExterior.Items.AddRange(eeT);
            comboBoxFormatoCD.Items.AddRange(fT);
            comboBoxEstadoMedio.SelectedIndex = 0;
            comboBoxEstadoExterior.SelectedIndex = 0;
            comboBoxFormatoCD.SelectedIndex = 0;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            album.ProtegerBorrado();
            EstadoMedio exterior = (EstadoMedio)Enum.Parse(typeof(EstadoMedio), comboBoxEstadoExterior.SelectedIndex.ToString());
            EstadoMedio medio = (EstadoMedio)Enum.Parse(typeof(EstadoMedio), comboBoxEstadoMedio.SelectedIndex.ToString());
            FormatoCD formato = (FormatoCD)Enum.Parse(typeof(FormatoCD), comboBoxFormatoCD.SelectedIndex.ToString());
            string s = album.artista + "_" + album.nombre;
            if(edit)
            {
                cd.FormatoCD = formato;
                cd.Discos[NDisco - 1].EstadoDisco = medio;
                cd.EstadoExterior = exterior;
                cd.Discos[NDisco - 1].NumCanciones=(short)numericUpDownNumCanciones.Value;
                cd.YearRelease = Convert.ToInt16(textBoxAño.Text);
                cd.PaisPublicacion = textBoxPais.Text;
                visualizarAlbum nuevo = new visualizarAlbum(ref cd);
                Programa.refrescarVista();
                nuevo.Show();
            }
            else if (NC != album.numCanciones)
            {
                if(NDisco > 1)
                {
                    Disco nuevo = new Disco(Convert.ToInt16(numericUpDownNumCanciones.Value), medio);
                    cd.Discos[NDisco-1] = nuevo;
                }
                else
                {
                    DiscoCompacto cd = new DiscoCompacto(s, Convert.ToInt16(numericUpDownNumCanciones.Value), medio, exterior, formato, numDiscos);
                    Programa.miColeccion.AgregarCD(ref cd);
                }
            }
            else
            {
                DiscoCompacto cd = null;
                try
                {
                     cd = new DiscoCompacto(s, album.numCanciones, medio, exterior, formato, Convert.ToInt16(textBoxAño.Text), textBoxPais.Text);

                }
                catch (Exception)
                {
                     cd = new DiscoCompacto(s, album.numCanciones, medio, exterior, formato, 0, textBoxPais.Text);
                }
                Programa.miColeccion.AgregarCD(ref cd);
                visualizarAlbum v = new visualizarAlbum(ref cd);
                v.Show();
            }
            Dispose();
        }
    }
}
