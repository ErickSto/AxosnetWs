using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Xml;
using System.Data.SqlTypes;
using System.Runtime.InteropServices;

namespace Axosnet.BD
{
    public class Cls_BD
    {
        public Cls_BD()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        /// ADO.NET data access using the SQL Server Managed Provider.
        /// </summary>
        public class Cls_Coneccion : IDisposable
        {
            public string esquema = ConfigurationManager.AppSettings["Esquema"].ToString();

            #region Conections
            // connection to data source
            private SqlConnection con;
            private SqlCommand command;
            public int result_number = -1;
            public string result_msg = "";


            public SqlParameterCollection Parameters
            {
                get
                {
                    return command.Parameters;
                }
            }


            /// <summary>
            /// Open the connection.
            /// </summary>
            public void Open()
            {
                string strKey = ConfigurationManager.AppSettings["CADENACONEXIONSQL"].ToString();
                var conString = ConfigurationManager.ConnectionStrings[strKey];
                string strConnString = conString.ConnectionString;

                // open connection
                if (con == null)
                {
                    con = new SqlConnection(strConnString);

                    con.Open();
                    con.InfoMessage += new SqlInfoMessageEventHandler(SqlInfoMessage);
                }
            }

            /// <summary>
            /// Regresa los mensajes de el sql sin warnings
            /// </summary>
            private void SqlInfoMessage(object sender, SqlInfoMessageEventArgs e)
            {
                string[] strMensaje;
                strMensaje = e.Message.Split('§');

                if (e.Errors[0].Number == 8153)
                {
                    result_number = -1;
                    result_msg = "";
                    return;
                }

                if (strMensaje.Length == 1)
                {
                    result_number = 0;
                    result_msg = strMensaje[0];
                }
                else
                {
                    result_number = Convert.ToInt32(strMensaje[0]);
                    result_msg = strMensaje[1];
                }
            }
            /// <summary>
            /// Close the connection.
            /// </summary>
            public void Close()
            {
                if (con != null)
                    con.Close();
            }

            static void connection_InfoMessage(object sender, SqlInfoMessageEventArgs e)
            {
                // this gets the print statements (maybe the error statements?)
                var outputFromStoredProcedure = e.Message;
                var outputFromStoredProcedureE = e.Source;
            }

            /// <summary>
            /// Release resources.
            /// </summary>
            public void Dispose()
            {
                // make sure connection is closed
                if (con != null)
                {
                    con.Dispose();
                    con = null;
                }
            }

            /// <summary>
            /// Asigna Comando a ejecutar
            /// </summary>
            /// <param name="Comando"></param>
            public void SetCommand(string Comando)
            {
                if (command == null)
                {
                    this.Open();
                    command = new SqlCommand(Comando, con);
                    command.CommandTimeout = 36000;
                }
                else
                {
                    try
                    { command.Cancel(); }
                    catch (Exception)
                    { }
                    try
                    { command.Parameters.Clear(); }
                    catch (Exception)
                    { }
                }
                command.CommandText = Comando;
                command.CommandType = CommandType.StoredProcedure;
            }

            #endregion
            #region Parametros
            #region Parametros de Salida
            /// <summary>
            /// Crea Parametro de Salida de tipo Caracter
            /// </summary>
            /// <param name="strParameterName"></param>
            /// <param name="strParameter"></param>
            /// <param name="nLenght"></param>
            /// <returns></returns>
            public SqlParameter CreateOutputParameter(string strParameterName, string strParameter, int nLenght)
            {
                SqlParameter parameter = new SqlParameter(strParameterName, System.Data.SqlDbType.NVarChar, nLenght);
                parameter.Value = strParameter;
                parameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(parameter);
                return parameter;
            }

            /// <summary>
            /// Crea Parametro de Salida de tipo Entero
            /// </summary>
            /// <param name="strParameterName"></param>
            /// <param name="nParameter"></param>
            /// <returns></returns>
            public SqlParameter CreateOutputParameter(string strParameterName, int nParameter)
            {
                SqlParameter parameter = new SqlParameter(strParameterName, System.Data.SqlDbType.Int, 4);
                parameter.Value = nParameter;
                parameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(parameter);
                return parameter;
            }

            /// <summary>
            /// Crea Parametro de Salida de tipo Lógico
            /// </summary>
            /// <param name="strParameterName"></param>
            /// <param name="bParameter"></param>
            /// <returns></returns>
            public SqlParameter CreateOutputParameter(string strParameterName, bool bParameter)
            {
                SqlParameter parameter = new SqlParameter(strParameterName, System.Data.SqlDbType.Bit, 1);
                parameter.Value = bParameter;
                parameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(parameter);
                return parameter;
            }

            /// <summary>
            /// Crea Valor de Retorno de tipo Entero
            /// </summary>
            /// <param name="strParameterName"></param>
            /// <returns></returns>
            public SqlParameter CreateReturnValueParameter(string strParameterName)
            {
                SqlParameter parameter = new SqlParameter(strParameterName, System.Data.SqlDbType.Int);
                parameter.Direction = ParameterDirection.ReturnValue;
                command.Parameters.Add(parameter);
                return parameter;
            }
            #endregion
            #region Creacion Parametros

            /// <summary>
            /// Crea Parametro de entrada de tipo Entero
            /// </summary>
            /// <param name="strParameterName"></param>
            /// <param name="nParameter"></param>
            public void CreateParameter(string strParameterName, int nParameter, bool isOutput = false)
            {
                if (isOutput)
                {
                    var prm = new SqlParameter(strParameterName, SqlDbType.Int, 4);
                    prm.Direction = ParameterDirection.Output;
                    command.Parameters.Add(prm);
                }
                else
                    command.Parameters.Add(strParameterName, System.Data.SqlDbType.Int, 4).Value = nParameter;
            }

            /// <summary>
            /// Crea Parametro de entrada de tipo Entero
            /// </summary>
            /// <param name="strParameterName"></param>
            /// <param name="nParameter"></param>
            public void CreateParameter(string strParameterName, DateTime? nParameter, bool isOutput = false)
            {
                if (isOutput)
                {
                    var prm = new SqlParameter(strParameterName, SqlDbType.DateTime, 4);
                    prm.Direction = ParameterDirection.Output;
                    command.Parameters.Add(prm);
                }
                else
                    command.Parameters.Add(strParameterName, System.Data.SqlDbType.DateTime, 4).Value = nParameter;
            }


            /// <summary>
            /// Crea Parametro de entrada de tipo Entero
            /// </summary>
            /// <param name="strParameterName"></param>
            /// <param name="nParameter"></param>
            public void CreateParameter(string strParameterName, int? nParameter, bool isOutput = false)
            {
                if (isOutput)
                {
                    var prm = new SqlParameter(strParameterName, SqlDbType.Int, 4);
                    prm.Direction = ParameterDirection.Output;
                    command.Parameters.Add(prm);
                }
                else
                    command.Parameters.Add(strParameterName, System.Data.SqlDbType.Int, 4).Value = nParameter;
            }
            /// <summary>
            /// Crea Parametro de entrada de tipo Entero largo
            /// </summary>
            /// <param name="strParameterName"></param>
            /// <param name="nParameter"></param>
            public void CreateParameter(string strParameterName, double nParameter)
            {
                command.Parameters.Add(strParameterName, System.Data.SqlDbType.Money, 8).Value = nParameter;
            }

            /// <summary>
            /// Crea Parametro de entrada de tipo Caracter
            /// </summary>
            /// <param name="strParameterName"></param>
            /// <param name="strParameter"></param>
            /// <param name="nLenght"></param>
            public void CreateParameter(string strParameterName, string strParameter, int nLenght)
            {
                //Si la longitud es Cero ponerlo como Text

                if (nLenght == 0)
                {
                    command.Parameters.Add(strParameterName, System.Data.SqlDbType.NText).Value = strParameter;
                }
                else
                {
                    command.Parameters.Add(strParameterName, System.Data.SqlDbType.NVarChar, nLenght).Value = strParameter;
                }
            }

            /// <summary>
            /// Crea Parametro de entrada de tipo String
            /// </summary>
            /// <param name="strParameterName"></param>
            /// <param name="strParameter"></param>
            public void CreateParameter(string strParameterName, string strParameter, bool isOutput = false)
            {
                if (isOutput)
                {
                    var prm = new SqlParameter(strParameterName, SqlDbType.VarChar, 1024);
                    prm.Direction = ParameterDirection.Output;
                    command.Parameters.Add(prm);
                }
                else
                {
                    command.Parameters.Add(strParameterName, System.Data.SqlDbType.NText).Value = strParameter;
                }
            }


            /// <summary>
            /// Crea Parametro de entrada de tipo Fecha
            /// </summary>
            /// <param name="strParameterName"></param>
            /// <param name="dateTimeParameter"></param>
            public void CreateParameter(string strParameterName, DateTime dateTimeParameter)
            {
                command.Parameters.Add(strParameterName, System.Data.SqlDbType.DateTime, 8).Value = dateTimeParameter;
            }

            /// <summary>
            /// Crea Parametro de entrada de tipo Flotante
            /// </summary>
            /// <param name="strParameterName"></param>
            /// <param name="fParameter"></param>
            public void CreateParameter(string strParameterName, float fParameter)
            {
                command.Parameters.Add(strParameterName, System.Data.SqlDbType.Float, 8).Value = fParameter;
            }

            /// <summary>
            /// Crea Parametro de entrada de tipo Decimal
            /// </summary>
            /// <param name="strParameterName"></param>
            /// <param name="fParameter"></param>
            public void CreateParameter(string strParameterName, decimal fParameter, bool isOutput = false)
            {
                if (isOutput)
                {
                    var prm = new SqlParameter(strParameterName, SqlDbType.Float, 8);
                    prm.Direction = ParameterDirection.Output;
                    command.Parameters.Add(prm);
                }
                else
                    command.Parameters.Add(strParameterName, System.Data.SqlDbType.Float, 8).Value = fParameter;
            }

            /// <summary>
            /// Crea Parametro de entrada de tipo Lógico
            /// </summary>
            /// <param name="strParameterName"></param>
            /// <param name="bParameter"></param>
            public void CreateParameter(string strParameterName, bool bParameter, bool isOutput = false)
            {
                command.Parameters.Add(strParameterName, System.Data.SqlDbType.Bit, 1).Value = bParameter;
                if (isOutput)
                {
                    command.Parameters[strParameterName].Direction = ParameterDirection.Output;
                }
            }

            /// <summary>
            /// Crea Parametro de entrada de tipo Imagen
            /// </summary>
            /// <param name="strParameterName"></param>
            /// <param name="imgParameter"></param>
            public void CreateParameter(string strParameterName, byte[] imgParameter)
            {
                command.Parameters.Add(strParameterName, SqlDbType.Image).Value = imgParameter;
            }

            /// <summary>
            /// Parametro XML
            /// </summary>
            /// <param name="strParameterName"></param>
            /// <param name="nParameter"></param>
            public void CreateParameter(string strParameterName, XmlDocument nParameter)
            {
                command.Parameters.Add(strParameterName, System.Data.SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(nParameter.InnerXml, XmlNodeType.Document, null));

            }

            /// <summary>
            /// Parametro XML
            /// </summary>
            /// <param name="strParameterName"></param>
            /// <param name="nParameter"></param>
            public void CreateParameter(string strParameterName, SqlXml nParameter)
            {
                command.Parameters.Add(strParameterName, System.Data.SqlDbType.Xml).Value = nParameter;

            }

            #endregion

            #region Agregar Parametros y seguridad
            /// <summary>
            /// Parametros default esta desactivado
            /// </summary>
            public void AddParametersSys([Optional] string sql)
            {
                if (sql == null) { sql = ""; }
            }
            #endregion

            #endregion

            #region Resultados

            #region Scalar

            /// <summary>
            /// Ejecuta Escalar
            /// </summary>
            public object getScalar(string sql)
            {

                this.Open();
                AddParametersSys();
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                da.SelectCommand.CommandTimeout = 3600;
                DataSet ds = new DataSet();
                da.Fill(ds);
                ds.Dispose();
                this.Close();
                return ds.Tables[0].Rows[0][0].ToString();


            }

            /// <summary>
            /// Ejecuta Escalar
            /// </summary>
            public object getScalar(string sql, int to)
            {
                this.Open();
                AddParametersSys();
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                da.SelectCommand.CommandTimeout = to;
                DataSet ds = new DataSet();
                da.Fill(ds);
                ds.Dispose();
                this.Close();
                return ds.Tables[0].Rows[0][0].ToString();

            }

            /// <summary>
            /// Ejecuta Escalar
            /// </summary>
            public object getScalar()
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                AddParametersSys();
                return command.ExecuteScalar();
            }
            #endregion
            #region NonQuery
            /// <summary>
            /// Run stored procedure.
            /// </summary>
            /// <param name="sql">Name of stored procedure.</param>
            /// <returns>Stored procedure return value.</returns>
            public bool ExecuteNonQuery(string sql)
            {
                this.Open();
                AddParametersSys();
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.CommandTimeout = 36000;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                this.Close();
                return true;
            }

            /// <summary>
            /// Run stored procedure.
            /// </summary>
            /// <param name="sql">Name of stored procedure.</param>
            /// <returns>Stored procedure return value.</returns>
            public bool ExecuteNonQuery(string sql, int to)
            {
                this.Open();
                AddParametersSys();
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.CommandTimeout = to;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                this.Close();
                return true;
            }

            /// <summary>
            /// Ejecuta comando
            /// </summary>
            public void ExecuteNonQuery()
            {

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                AddParametersSys();
                command.ExecuteNonQuery();
            }

            #endregion
            #region DataTable
            /// <summary>
            /// Run stored procedure.
            /// </summary>
            /// <param name="sql">Name of stored procedure.</param>
            /// <param name="DataTable">Return result of procedure.</param>
            public DataTable getDataTable(string sql)
            {
                this.Open();

                AddParametersSys();
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                da.SelectCommand.CommandTimeout = 3600;
                DataTable dt = new DataTable();
                int rows = da.Fill(dt);
                dt.Dispose();
                this.Close();
                return dt;
            }
            /// <summary>
            /// Run stored procedure.
            /// </summary>
            /// <param name="sql">Name of stored procedure.</param>
            /// <param name="DataTable">Return result of procedure.</param>
            public DataTable getDataTable(string sql, int to)
            {
                this.Open();

                AddParametersSys();
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                da.SelectCommand.CommandTimeout = to;
                DataTable dt = new DataTable();
                int rows = da.Fill(dt);
                dt.Dispose();
                this.Close();
                return dt;
            }

            /// <summary>
            /// Regresa DataSet
            /// </summary>
            /// <param name="DataTable">Return result of procedure.</param>
            public DataTable getDataTable()
            {
                DataTable data = new DataTable();
                try
                {

                    if (con.State == ConnectionState.Closed)
                    { con.Open(); }


                    AddParametersSys();
                    SqlDataAdapter dataAdapterSearch = new SqlDataAdapter();

                    dataAdapterSearch.SelectCommand = command;
                    dataAdapterSearch.Fill(data);
                    dataAdapterSearch.Dispose();


                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    con.Close();
                }

                return data;


            }

            #endregion
            #region DataSet
            /// <summary>
            /// Run stored procedure.
            /// </summary>
            /// <param name="sql">Name of stored procedure.</param>
            /// <param name="DataSet">Return result of procedure.</param>
            public DataSet getDataSet(string sql)
            {
                this.Open();
                AddParametersSys();
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                da.SelectCommand.CommandTimeout = 3600;
                DataSet ds = new DataSet();
                da.Fill(ds);
                ds.Dispose();
                this.Close();
                return ds;

            }

            /// <summary>
            /// Run stored procedure.
            /// </summary>
            /// <param name="sql">Name of stored procedure.</param>
            /// <param name="DataSet">Return result of procedure.</param>
            public DataSet getDataSet(string sql, int to)
            {
                this.Open();
                AddParametersSys();
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                da.SelectCommand.CommandTimeout = to;
                DataSet ds = new DataSet();
                da.Fill(ds);
                ds.Dispose();
                this.Close();
                return ds;

            }


            /// <summary>
            /// Regresa DataSet
            /// </summary>
            /// <param name="DataSet">Return result of procedure.</param>
            public DataSet getDataSet()
            {
                DataSet data = new DataSet();
                try
                {
                    if (con.State == ConnectionState.Closed)
                    { con.Open(); }

                    AddParametersSys();
                    SqlDataAdapter dataAdapterSearch = new SqlDataAdapter();

                    dataAdapterSearch.SelectCommand = command;
                    dataAdapterSearch.Fill(data);
                    dataAdapterSearch.Dispose();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    con.Close();
                }

                return data;


            }

            #endregion
            #region DataReader
            /// <summary>
            /// Run stored procedure.
            /// </summary>
            /// <param name="sql">Name of stored procedure.</param>
            /// <param name="dataReader">Return result of procedure.</param>
            public SqlDataReader getDataReader(string sql)
            {
                this.Open();
                AddParametersSys();
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.CommandTimeout = 3600;
                SqlDataReader dr;
                dr = cmd.ExecuteReader(); //System.Data.CommandBehavior.CloseConnection);
                cmd.Dispose();
                this.Close();

                return dr;
            }
            /// <summary>
            /// Run stored procedure.
            /// </summary>
            /// <param name="sql">Name of stored procedure.</param>
            /// <param name="dataReader">Return result of procedure.</param>
            public SqlDataReader getDataReader(string sql, int to)
            {
                this.Open();
                AddParametersSys();
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.CommandTimeout = to;
                SqlDataReader dr;
                dr = cmd.ExecuteReader(); //System.Data.CommandBehavior.CloseConnection);
                cmd.Dispose();
                return dr;
            }
            /// <summary>
            /// Regresa Reader
            /// </summary>
            /// <param name="dataReader">Return result of procedure.</param>
            public SqlDataReader getDataReader()
            {

                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                AddParametersSys();
                con.Close();
                return command.ExecuteReader();
            }
            #endregion
            #endregion

            #region BulkCopy
            /// <summary>
            /// Hace un Insert Masivo con la información dada
            /// </summary>
            /// <param name="destinationTable">Destination Table To Insert.</param>
            /// <param name="dt">Data to be Inserted</param>
            public void BulkCopy(string destinationTable, DataTable dt)
            {
                try
                {
                    con.Open();

                    SqlBulkCopy bulkCopy = new SqlBulkCopy(
                            con,
                            SqlBulkCopyOptions.TableLock |
                            SqlBulkCopyOptions.FireTriggers |
                            SqlBulkCopyOptions.UseInternalTransaction,
                            null
                            );

                    foreach (DataColumn col in dt.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }


                    bulkCopy.DestinationTableName = destinationTable;
                    bulkCopy.WriteToServer(dt);

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
            #endregion
        }
    }
}