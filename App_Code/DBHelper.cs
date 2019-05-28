using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections;


    public class DBHelper
    {
        public MySqlConnection conn;
        private MySqlDataAdapter da;
        private MySqlCommandBuilder cb;
        public DBHelper(string serverPath, string userName, string pwd, string dbName)
        {
            string connectionstring = "Character Set=utf8;server=" + serverPath + ";User Id=" + userName + ";password=" + pwd + ";database=" + dbName + ";Allow Zero Datetime=True;port=3306;";
            conn = new MySqlConnection(connectionstring);
        }

        public DataTable getTB(string cmd)
        {
            DataTable tb = new DataTable();
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                da = new MySqlDataAdapter(cmd, conn);
                cb = new MySqlCommandBuilder(da);
                da.Fill(tb);
                conn.Close();
                da.Dispose();
                cb.Dispose();
                return tb;
            }
            catch (Exception e)
            {
                conn.Close();
                da.Dispose();
                cb.Dispose();
             
                return null;
            }
        }
        public bool SendCommand(string cmd)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            MySqlCommand cmdCtl = new MySqlCommand();
            cmdCtl = conn.CreateCommand();
            cmdCtl.CommandText = cmd;
            try
            {
                int a = cmdCtl.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
             
                return false;
            }
            finally
            {
                cmdCtl.Dispose();
                conn.Close();
            }
        }

        public bool SetTB(string cmd, DataTable tb)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                da = new MySqlDataAdapter(cmd, conn);
                cb = new MySqlCommandBuilder(da);
                da.Update(tb);
                return true;
            }
            catch (Exception e)
            {
             
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 执行语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">mysql语句</param>
        /// <param name="ps"></param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string SQLString, params MySqlParameter[] ps)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            MySqlTransaction myTran = conn.BeginTransaction();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, conn))
                {

                    cmd.Transaction = myTran;
                    cmd.Parameters.AddRange(ps);
                    int rows = cmd.ExecuteNonQuery();
                    myTran.Commit();
                    conn.Close();
                    return rows;
                }
            }
            catch (Exception e)
            {
                conn.Close();
                myTran.Rollback();
             
                return 0;
            }
        }
        /// <summary>
        /// 执行查询语句,返回MySqlDataReader
        /// </summary>
        /// <param name="strSQL">mysql语句</param>
        /// <param name="ps"></param>
        /// <returns>MySqlDataReader</returns>
        public MySqlDataReader ExecuteReader(string strSQL, params MySqlParameter[] ps)
        {

            MySqlCommand cmd = new MySqlCommand(strSQL, conn);
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                cmd.Parameters.AddRange(ps);
                MySqlDataReader myReader = cmd.ExecuteReader();
                return myReader;
            }
            catch (Exception e)
            {
                conn.Close();
             
                return null;
            }

        }
        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string SQLString, params MySqlParameter[] ps)
        {
            using (MySqlCommand cmd = new MySqlCommand(SQLString, conn))
            {

                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    cmd.Parameters.AddRange(ps);
                    object obj = cmd.ExecuteScalar();

                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return 0;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (Exception e)
                {
                    conn.Close();
                 
                    return 0;
                }

            }
        }
        /// <summary>
        /// 执行SQL语句,带数组参数.
        /// </summary>
        /// <param name="sqlstr">SQL语句</param>
        /// <param name="p">SQL变量名数组</param>
        /// <param name="pv">替换值数组</param>
        /// <returns></returns>
        public int ExecuteSQL(string sqlstr, ArrayList p, ArrayList pv)
        {
            int count = 0;
            try
            {
                MySqlCommand comm = new MySqlCommand(sqlstr, conn);

                for (int i = 0; i < p.Count; i++)
                {
                    if (pv[i].ToString().Length < 4000)
                    {
                        comm.Parameters.AddWithValue(p[i].ToString(), MySqlDbType.VarChar).Value = pv[i].ToString();
                    }
                    else
                    {
                        comm.Parameters.AddWithValue(p[i].ToString(), MySqlDbType.Text).Value = pv[i].ToString();
                    }
                }
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                count = comm.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();
                return count;
            }
            catch (Exception e)
            {
                conn.Close();
             
                return -1;
            }
        }
        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <param name="sqlstr">SQL语句</param>
        /// <param name="intStarId">游标起始位置</param>
        /// <param name="intMaxLen">读取数量</param>
        /// <param name="p">SQL变量名数组</param>
        /// <param name="pv">替换值数组</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataTable(string sqlstr, int intStarId, int intMaxLen, ArrayList p, ArrayList pv)
        {
            try
            {
                MySqlCommand comm = new MySqlCommand(sqlstr, conn);
                for (int i = 0; i < p.Count; i++)
                {
                    comm.Parameters.AddWithValue(p[i].ToString(), MySqlDbType.VarChar).Value = pv[i].ToString();
                }
                MySqlDataAdapter da = new MySqlDataAdapter(comm);
                DataSet ds = new DataSet();
                da.Fill(ds, intStarId, intMaxLen, "table1");
                comm.Dispose();
                conn.Dispose();
                da.Dispose();
                return ds.Tables[0];
            }
            catch (Exception e)
            {
                conn.Close();
             
                return null;
            }
        }
        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <param name="sqlstr">mysql语句</param>
        /// <param name="intStarId">游标起始位置</param>
        /// <param name="intMaxLen">读取数量</param>
        /// <param name="ps">params</param>
        /// <returns></returns>
        public DataTable GetDataTable(string sqlstr, int intStarId, int intMaxLen, params MySqlParameter[] ps)
        {
            try
            {
                MySqlCommand comm = new MySqlCommand(sqlstr, conn);
                //for (int i = 0; i < p.Count; i++)
                //{
                //    comm.Parameters.AddWithValue(p[i].ToString(), MySqlDbType.VarChar).Value = pv[i].ToString();
                //}
                comm.Parameters.AddRange(ps);
                MySqlDataAdapter da = new MySqlDataAdapter(comm);
                DataSet ds = new DataSet();
                da.Fill(ds, intStarId, intMaxLen, "table1");
                comm.Dispose();
                conn.Dispose();
                da.Dispose();
                return ds.Tables[0];
            }
            catch (Exception e)
            {
                conn.Close();
             
                return null;
            }
        }
        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <param name="sqlstr">SQL语句</param>
        /// <param name="p1">SQL变量名1</param>
        /// <param name="pv1">替换值1</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataTable(string sqlstr, string p1, string pv1)
        {
            try
            {
                MySqlCommand comm = new MySqlCommand(sqlstr, conn);
                comm.Parameters.AddWithValue(p1, MySqlDbType.VarChar).Value = pv1;
                MySqlDataAdapter da = new MySqlDataAdapter(comm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                comm.Dispose();
                conn.Dispose();
                return dt;
            }
            catch (Exception e)
            {
                conn.Close();
             
                return null;
            }
        }

    }
