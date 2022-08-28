using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WinCarpiteriaEjercitacion
{
    public partial class Form1 : Form
    {
        private HelperDB helper;
        private Presupuesto nuevo;

        public Form1()
        {
            InitializeComponent();
            helper = new HelperDB();
            CargarProductos();
            nuevo= new Presupuesto();
        }

        private void CargarProductos()
        {
            DataTable tabla = helper.ConsultarSQL("SP_CONSULTAR_PRODUCTOS");
            cmbProducto.DataSource = tabla;
            cmbProducto.ValueMember = "id_producto";
            cmbProducto.DisplayMember = "n_producto";
            cmbProducto.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (cmbProducto.Text.Equals(string.Empty))//validaciones de los campos
            {
                MessageBox.Show("No selecciono un producto","error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                
            }
            if(txtCantidad.Text.Equals(string.Empty) || !int.TryParse(txtCantidad.Text, out _))
            {
                MessageBox.Show("Debe indicar una cantidad", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            foreach(DataGridViewRow row in dgvDetalles.Rows)
            {
                if (row.Cells["colProducto"].Value.ToString().Equals(cmbProducto.Text))
                {
                    MessageBox.Show("Producto: "+cmbProducto.Text+ "ya fue ingresado como detalle", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            DataRowView item = (DataRowView)cmbProducto.SelectedItem;
            int nro = Convert.ToInt32(item.Row.ItemArray[0]);//es la columna ID del datagridview
            string nom=item.Row.ItemArray[1].ToString();//columna producto
            double pre=Convert.ToDouble(item.Row.ItemArray[2].ToString());//columna precio
            Producto p = new Producto(nro, nom, pre);//se crea el producto
            int cant = Convert.ToInt32(item.Row.ItemArray[3].ToString());//columna de cantidad de cada producto
            DetallePresupuesto detalle = new DetallePresupuesto(p,cant);//creo el detalle que va en cada fila y se la agrego al DGV
            nuevo.AgregarDetalle(detalle);//creo el vector de objetos 
            dgvDetalles.Rows.Add(new object[] { item.Row.ItemArray[0], item.Row.ItemArray[1], item.Row.ItemArray[2], item.Row.ItemArray[3] });
            

            
        }

        private void dgvDetalles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(dgvDetalles.CurrentCell.ColumnIndex==4)
            {

                nuevo.QuitarDetalle(dgvDetalles.CurrentRow.Index);//click boton
                //quitar detalle 
                dgvDetalles.Rows.Remove(dgvDetalles.CurrentRow);
                nuevo.CalcularTotal();
                if(txtDescuento.Text!="")
                {
                    nuevo.CalcularTotal().Equals(lblSubTotal.Text);
                }

            }
        }

        
    }
}
