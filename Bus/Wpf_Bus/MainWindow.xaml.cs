using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace Wpf_Bus
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BtnBus_Click(object sender, RoutedEventArgs e)
        {
            string openApiUri = "https://apis.data.go.kr/6260000/BusanLifeInfoService/getLifeInfo?serviceKey=HVto27eamBEo1E7tjkDl7GVSuHLlwZcM2XpUQB691yS14zX9Wu1OK5ZNqnOzK5REs8adTJvOTuUNABJgDw8x7Q%3D%3D&pageNo=1&numOfRows=1000&resultType=json";
            string result = string.Empty;

            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            try
            {
                req = WebRequest.Create(openApiUri);
                res = await req.GetResponseAsync();
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd();

            }
            catch
            {
            }
            var jsonResult = JObject.Parse(result);

            try
            {
                var data = jsonResult["getLifeInfo"]["body"]["items"]["item"]; // 폴더 처럼 [] [] 차례대로 하위로 가주면 된다
                var json_array = data as JArray;

                var MartItem = new List<martItem>();
                foreach (var sensor in json_array)
                {
                    MartItem.Add(new martItem
                    {
                        pumNm = Convert.ToString(sensor["pumNm"]),
                        itemName = Convert.ToString(sensor["itemName"]),
                        gugunNm = Convert.ToString(sensor["gugunNm"]),
                        bsshNm = Convert.ToString(sensor["bsshNm"]),
                        unitprice = Convert.ToInt32(sensor["unitprice"]),
                        adres = Convert.ToString(sensor["adres"]),
                        unit = Convert.ToDouble(sensor["unit"])
                    });
                }
                this.DataContext = MartItem;
            }
            catch
            {
            }


        }
            


        #region<SQL 데이터 저장>
        //private void BtnSave_Click(object sender, RoutedEventArgs e)
        //{

        //    // 저장
        //    // PK있으면 중복 안됨
        //    try
        //    {
        //        using (MySqlConnection conn = new MySqlConnection(Myconn.conn))
        //        {
        //            if (conn.State == System.Data.ConnectionState.Closed) conn.Open();
        //            var query = @"INSERT INTO mart     (pumNm,
        //                                                itemName,
        //                                                gugunNm,
        //                                                bsshNm,
        //                                                unitprice,
        //                                                adres,
        //                                                unit)
        //                                                VALUES
        //                                                (@pumNm,
        //                                                @itemName,
        //                                                @gugunNm,
        //                                                @bsshNm,
        //                                                @unitprice,
        //                                                @adres,
        //                                                @unit)";
        //            var insRes = 0;
        //            foreach (var temp in DgbMart.Items)
        //            {
        //                if (temp is martItem)
        //                {
        //                    var item = temp as martItem;

        //                    MySqlCommand cmd = new MySqlCommand(query, conn);
        //                    cmd.Parameters.AddWithValue("@pumNm", item.pumNm);
        //                    cmd.Parameters.AddWithValue("@itemName", item.itemName);
        //                    cmd.Parameters.AddWithValue("@gugunNm", item.gugunNm);
        //                    cmd.Parameters.AddWithValue("@bsshNm", item.bsshNm);
        //                    cmd.Parameters.AddWithValue("@unitprice", item.unitprice);
        //                    cmd.Parameters.AddWithValue("@adres", item.adres);
        //                    cmd.Parameters.AddWithValue("@unit", item.unit);

        //                    insRes += cmd.ExecuteNonQuery();
        //                }
        //            }
        //            txt.Text = $"{insRes}";
        //        }

        //    }
        //    catch(Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }

        //}
        #endregion


        // 콤보 박스 안에 중복된 걸로 리스트를 만들어줌
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(Myconn.conn))
            {
                conn.Open();
                var query = @"SELECT gugunNm AS Save_Data
                              FROM mart
                              GROUP BY 1
                              ORDER BY 1";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                List<string> saveDataList = new List<string>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    saveDataList.Add(Convert.ToString(row["Save_Data"]));
                }
                Cbogugun.ItemsSource = saveDataList;
            }

            using (MySqlConnection conn1 = new MySqlConnection(Myconn.conn))
            {
                conn1.Open();
                var query1 = @"SELECT pumNm AS Save_Data
                              FROM mart
                              GROUP BY 1
                              ORDER BY 1";
                MySqlCommand cmd = new MySqlCommand(query1, conn1);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                List<string> saveDataList = new List<string>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    saveDataList.Add(Convert.ToString(row["Save_Data"]));
                }
                CbopumNm.ItemsSource = saveDataList;
            }
        }

        private void Cbogugun_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(Myconn.conn)) 
            {
                var query = $@"SELECT pumNm,
                                    itemName,
                                    gugunNm,
                                    bsshNm,
                                    unitprice,
                                    adres,
                                    unit
                                    FROM
                                    mart
                                    WHERE gugunNm = @gugunNm";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@gugunNm", Cbogugun.SelectedValue.ToString());
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataSet mi = new DataSet();
                adapter.Fill(mi, "mart");
                List<martItem> MartItems = new List<martItem>();

                foreach (DataRow row in mi.Tables["mart"].Rows)
                {
                    MartItems.Add(new martItem
                    {
                        pumNm = Convert.ToString(row["pumNm"]),
                        itemName = Convert.ToString(row["itemName"]),
                        gugunNm = Convert.ToString(row["gugunNm"]),
                        unitprice = Convert.ToInt32(row["unitprice"]),
                        adres = Convert.ToString(row["adres"]),
                        unit = Convert.ToDouble(row["unit"])
                    });
                }
                this.DataContext = MartItems;
            }

        }

        private void CbopumNm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(Myconn.conn))
            {
                var query = $@"SELECT pumNm,
                                    itemName,
                                    gugunNm,
                                    bsshNm,
                                    unitprice,
                                    adres,
                                    unit
                                    FROM
                                    mart
                                    WHERE gugunNm = @gugunNm";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@gugunNm", CbopumNm.SelectedValue.ToString());
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataSet mi = new DataSet();
                adapter.Fill(mi, "mart");
                List<martItem> MartItems = new List<martItem>();

                foreach (DataRow row in mi.Tables["mart"].Rows)
                {
                    MartItems.Add(new martItem
                    {
                        pumNm = Convert.ToString(row["pumNm"]),
                        itemName = Convert.ToString(row["itemName"]),
                        gugunNm = Convert.ToString(row["gugunNm"]),
                        unitprice = Convert.ToInt32(row["unitprice"]),
                        adres = Convert.ToString(row["adres"]),
                        unit = Convert.ToDouble(row["unit"])
                    });
                }
                this.DataContext = MartItems;
            }
        }
    }
}
