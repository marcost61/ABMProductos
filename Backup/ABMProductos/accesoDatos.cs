using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace ABMProductos
{
    class accesoDatos
    {
        OleDbConnection conexion;
        OleDbCommand comando;
        OleDbDataReader dr;
        DataTable dt;
        string cadenaConexion;

        public OleDbDataReader pDr
        {
            get { return dr; }
            set { dr = value; }
        }

        public string pCadenaConexion
        {
            get { return cadenaConexion; }
            set { cadenaConexion = value; }
        }
        public accesoDatos()
        {
            conexion = new OleDbConnection();
            comando = new OleDbCommand();
            dt = new DataTable();
            dr = null;
            cadenaConexion = "";
        }
        public accesoDatos(string strConexion)
        {
            conexion = new OleDbConnection(strConexion);
            comando = new OleDbCommand();
            dt = new DataTable();
            dr = null;
            cadenaConexion = strConexion;
        }
        public void conectar()
        {
            conexion.ConnectionString=cadenaConexion;
            conexion.Open();
            comando.Connection = conexion;
            comando.CommandType = CommandType.Text;
        }
        public void desconectar()
        {
            conexion.Close();
            conexion.Dispose();
        }
        public DataTable consultarTabla(string tabla)
        {
            dt = new DataTable();
            conectar();
            comando.CommandText = "Select * from " + tabla;
            dt.Load(comando.ExecuteReader());
            desconectar();
            return dt;
        }
        public DataTable consultar(string sql)
        {
            dt = new DataTable(); 
            conectar();
            comando.CommandText = sql;
            dt.Load(comando.ExecuteReader());
            desconectar();
            return dt;
        }
        public void leerTabla(string tabla)
        {
            conectar();
            comando.CommandText = "Select * from " + tabla;
            dr = comando.ExecuteReader();
        }
        public void actualizar(string sql)
        {
            conectar();
            comando.CommandText = sql;
            comando.ExecuteNonQuery();
            desconectar();
        }
    }
}
