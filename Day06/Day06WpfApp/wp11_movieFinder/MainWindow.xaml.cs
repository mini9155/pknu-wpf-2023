using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Xml.Linq;
using wp11_movieFinder.Logics;
using wp11_movieFinder.Models;

namespace wp11_movieFinder
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

        // 네이버 영화 클릭
        private async void BtnNaverMovie_Click(object sender, RoutedEventArgs e)
        {
            // 네이버 영화 사이트로 간다 라고 창 띄워줌
            await Commons.ShowMessageAsync("네이버영화", "네이버영화 사이트로 갑니다");
        }

        // 검색,네이버API 영화 검색
        private async void BtnSearchMovie_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtMovieName.Text)) // 만약에 검색박스가 비어있으면
            {
                await Commons.ShowMessageAsync("검색", "영화를 검색합니다");
                return;
            }

            try
            {
                SearchMovie(TxtMovieName.Text);
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"오류발생 : {ex.Message}");
            }
        }

        //검색창에서 키를 입력할 때 엔터를 누르면 검색시작
        private void TxtMovieName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) // 엔터를 누르면
            {
                BtnSearchMovie_Click(sender, e);
            }
        }

        //실제 검색 메서드
        private  void SearchMovie(string MovieName)
        {
            string tmdb_apikey = "b044cdf716f1bf45fac864f2cd1ca931";
            string encoding_movieName = HttpUtility.UrlEncode(MovieName, Encoding.UTF8);
            string openApiUri = $"https://api.themoviedb.org/3/search/movie?api_key={tmdb_apikey}" +
                                $"&language=ko-KR&page=1&include_adult=false&query={encoding_movieName}";
            string result = string.Empty; // 결과 값

            //api 실행할 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            try
            {
                req = WebRequest.Create(openApiUri); // url을 넣어서 객체를 생성

                res = req.GetResponse(); // 요청한 결과를 응답에 할당
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd();

                Debug.WriteLine(result);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                res.Close();
            }

            var jsonResult = JObject.Parse(result);

            var total = Convert.ToInt32(jsonResult["total_results"]);

            var items = jsonResult["results"];

            var json_array = items as JArray;

            var movieItems = new List<MovieItem>();
            foreach (var val in json_array)
            {
                var MovieItem = new MovieItem()
                {
                    Id = Convert.ToInt32(val["id"]),
                    Title = Convert.ToString(val["title"]),
                    Original_Title = Convert.ToString(val["original_title"]),
                    Release_Date = Convert.ToString(val["realse_date"]),
                    Vote_Average = Convert.ToDouble(val["vote_average"]),
                };

                movieItems.Add(MovieItem);
            }
            this.DataContext = movieItems; // 이게 있어야 되는구만


        }
    }
}
