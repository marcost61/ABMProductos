using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ABMProductos
{
    public partial class frmProducto : Form
    {
        accesoDatos datos = new accesoDatos(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source= C:\Users\marce\Documents\Visual Studio 2017\Projects\AMBProductoAspen1\DBFProducto.mdb");
        const int tam = 30;
        Producto[] vp = new Producto[tam];
        bool nuevo = false; 

        public frmProducto()
        {
            InitializeComponent();
        }

        private void frmProducto_Load(object sender, EventArgs e)
        {
            //datos.pCadenaConexion = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\Oscar\UTN\ProgramacionII\ABMProductos\DBFProducto.mdb";
            cargarCombo(cboMarca, "marca");
            cargarLista("producto");
            habilitar(false);
        }
        private void cargarLista(string nombreTabla)
        {
            datos.leerTabla(nombreTabla);
            int c = 0;
            while (datos.pDr.Read())
            {
                Producto p = new Producto();
                if (!datos.pDr.IsDBNull(0))
                    p.pCodigo = datos.pDr.GetInt32(0);
                if (!datos.pDr.IsDBNull(1))
                    p.pDetalle = datos.pDr.GetString(1);
                if (!datos.pDr.IsDBNull(2))
                    p.pTipo = datos.pDr.GetInt32(2);
                if (!datos.pDr.IsDBNull(3))
                    p.pMarca = datos.pDr.GetInt32(3);
                if (!datos.pDr.IsDBNull(4))
                    p.pPrecio = datos.pDr.GetDouble(4);
                if (!datos.pDr.IsDBNull(5))
                    p.pFecha = datos.pDr.GetDateTime(5);
                vp[c] = p;
                c++;
            }
            datos.pDr.Close();
            datos.desconectar();
            lstProducto.Items.Clear();
            for (int i = 0; i < c; i++)
                lstProducto.Items.Add(vp[i].ToString());
            lstProducto.SelectedIndex = lstProducto.Items.Count - 1;

        }
        private void cargarCombo(ComboBox combo, string nombreTabla)
        {
            DataTable tabla = new DataTable();
            tabla = datos.consultarTabla(nombreTabla);
            combo.DataSource = tabla;
            combo.ValueMember = tabla.Columns[0].ColumnName;
            combo.DisplayMember = tabla.Columns[1].ColumnName;
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.SelectedIndex = 0;
        }
        
        private void btnGrabar_Click(object sender, EventArgs e)
        {
            if (validar())
            {
                Producto p = new Producto();
                p.pCodigo = Convert.ToInt32(txtCodigo.Text);
                p.pDetalle = txtDetalle.Text;
                if (rbtNoteBook.Checked)
                    p.pTipo = 1;
                else
                    p.pTipo = 2;
                p.pMarca = Convert.ToInt32(cboMarca.SelectedValue);
                p.pPrecio = Convert.ToDouble(txtPrecio.Text);
                p.pFecha = dtpFecha.Value;

                string sql;

                if (nuevo)
                    if (!existe(p.pCodigo))
                    {
                        sql = "insert into producto (codigo,detalle,tipo,marca,precio,fecha) values ("
                            + p.pCodigo + ",'"
                            + p.pDetalle + "',"
                            + p.pTipo + ","
                            + p.pMarca + ","
                            + p.pPrecio + ",'"
                            + p.pFecha + "')";
                        datos.actualizar(sql); 
                        cargarLista("producto");
                    }
                    else
                        MessageBox.Show("Este producto ya se encuentra registrado...");
                else
                {
                    sql = "update producto set detalle='" + p.pDetalle + "',"
                                               + "tipo=" + p.pTipo + ","
                                               + "marca=" + p.pMarca + ","
                                               + "precio=" + p.pPrecio + ","
                                               + "fecha='" + p.pFecha + "' "
                                               + "Where codigo=" + p.pCodigo;
                    datos.actualizar(sql);
                    cargarLista("producto");
                }
                habilitar(false);
                nuevo = false;
            }
        }
        private bool existe(int pk)
        {
            for (int i = 0; i < lstProducto.Items.Count; i++)
                if (vp[i].pCodigo == pk)
                    return true;
            return false;
        }
        private bool validar()
        {
            if (string.IsNullOrEmpty(txtCodigo.Text))
            {
                MessageBox.Show("Debe completar el código...");
                txtCodigo.Focus();
                return false;
            }
            else
            {
                try
                {
                    Int32.Parse(txtCodigo.Text);
                }
                catch
                {
                    MessageBox.Show("Coloque sólo números.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    txtCodigo.Focus();
                    return false;
                }
            }
            if (txtDetalle.Text == "")
            {
                MessageBox.Show("Debe completar el detalle...");
                txtDetalle.Focus();
                return false;
            }
            if (cboMarca.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar una marca...");
                cboMarca.Focus();
                return false;
            }
            if (!rbtNetBook.Checked && !rbtNoteBook.Checked)
            {
                MessageBox.Show("Debe seleccionar un tipo...");
                rbtNoteBook.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtPrecio.Text))
            {
                MessageBox.Show("Debe completar el precio...");
                txtPrecio.Focus();
                return false;
            }
            if (dtpFecha.Value > DateTime.Now)
            {
                MessageBox.Show("La fecha no puede ser posterior a hoy...");
                dtpFecha.Focus();
                return false;
            }
          
            return true;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            nuevo = true;
            habilitar(true);
            limpiar();
            txtCodigo.Focus();
        }
        private void limpiar()
        {
            txtCodigo.Text = "";
            txtDetalle.Clear();
            txtPrecio.Clear();
            rbtNoteBook.Checked = false;
            rbtNetBook.Checked = false;
            cboMarca.SelectedIndex = -1;
            dtpFecha.Value = DateTime.Today;
        }
        private void habilitar(bool x)
        {
            txtCodigo.Enabled = x;
            txtDetalle.Enabled = x;
            txtPrecio.Enabled = x;
            rbtNoteBook.Enabled = x;
            rbtNetBook.Enabled = x;
            cboMarca.Enabled = x;
            dtpFecha.Enabled = x;
            btnGrabar.Enabled = x;
            btnCancelar.Enabled = x;

            btnNuevo.Enabled = !x;
            btnEditar.Enabled = !x;
            btnBorrar.Enabled = !x;
            btnSalir.Enabled = !x;
            lstProducto.Enabled = !x;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            limpiar();
            habilitar(false);
            nuevo = false;
            cargarCampos(lstProducto.SelectedIndex);
        }

        private void lstProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            cargarCampos(lstProducto.SelectedIndex);
        }
        private void cargarCampos(int posicion)
        {
            txtCodigo.Text = vp[posicion].pCodigo.ToString();
            txtDetalle.Text = vp[posicion].pDetalle;
            cboMarca.SelectedValue = vp[posicion].pMarca;
            if (vp[posicion].pTipo == 1)
                rbtNoteBook.Checked = true;
            else
                rbtNetBook.Checked = true;
            txtPrecio.Text = vp[posicion].pPrecio.ToString();
            dtpFecha.Value = vp[posicion].pFecha;
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            habilitar(true);
            txtCodigo.Enabled = false;
            nuevo = false;
            txtDetalle.Focus();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmProducto_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Desea abandonar la aplicación ?", "Salir", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                e.Cancel = false;
            else
                e.Cancel = true;
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Seguro de eliminar el producto " + vp[lstProducto.SelectedIndex].pDetalle + " ?"
                , "Borrar"
                , MessageBoxButtons.YesNo
                , MessageBoxIcon.Warning
                , MessageBoxDefaultButton.Button2)
                == DialogResult.Yes)
            {
                string sql = "Delete from producto where codigo=" + vp[lstProducto.SelectedIndex].pCodigo;
                datos.actualizar(sql);
                cargarLista("producto");
            }
        }
    }
}
