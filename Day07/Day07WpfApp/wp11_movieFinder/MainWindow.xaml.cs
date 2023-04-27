using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using wp11_movieFinder.Logics;
using wp11_movieFinder.Models;

namespace wp11_movieFinder
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        bool isFavorite = false;
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
        private void SearchMovie(string MovieName)
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
                    Adult = Convert.ToBoolean(val["adult"]),
                    Id = Convert.ToInt32(val["id"]),
                    Original_Language = Convert.ToString(val["original_language"]),
                    Original_Title = Convert.ToString(val["original_title"]),
                    Overview = Convert.ToString(val["overview"]),
                    Popularity = Convert.ToDouble(val["popularity"]),
                    Poster_Path = Convert.ToString(val["poster_path"]),
                    Release_Date = Convert.ToString(val["release_date"]),
                    Title = Convert.ToString(val["title"]),
                    Vote_Average = Convert.ToDouble(val["vote_average"])

                };

                movieItems.Add(MovieItem);
            }
            this.DataContext = movieItems; // 이게 있어야 되는구만


        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TxtMovieName.Focus();
        }


        //그리드에서 셀을 선택하면
        private void GrdResult_SelectedCellsChanged(object sender, System.Windows.Controls.SelectedCellsChangedEventArgs e)
        {
            try
            {
                var movie = GrdResult.SelectedItem as MovieItem; //이미지
                Debug.WriteLine(movie.Poster_Path);
                if (string.IsNullOrEmpty(movie.Poster_Path)) // 이미지가 없으면 no_picture
                {
                    ImgPoster.Source = new BitmapImage(new Uri("No_Picture.png", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    ImgPoster.Source = new BitmapImage(new Uri($"https://image.tmdb.org/t/p/w300_and_h450_bestv2/{movie.Poster_Path}", UriKind.RelativeOrAbsolute));
                }

            }
            catch
            {

            }
        }

        private async void BtnWatchTrailer_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("유튜브", "영화를 선택하세요");
                return;
            }
            if (GrdResult.SelectedItems.Count > 1)
            {
                await Commons.ShowMessageAsync("유튜브", "영화를 하나만 선택해주세요");
                return;
            }

            string movieName = string.Empty;

            var movie = GrdResult.SelectedItem as MovieItem;

            movieName = movie.Title;

            // await Commons.ShowMessageAsync("유튜브", $"예고편 재생 {movieName}");
            var trailerWindow = new TrailerWindow(movieName);
            trailerWindow.Owner = this; // TrailerWindow의 부모는 MainWindow
            trailerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            // trailerWindow.Show(); // 모달리스로 창을 열면 부모창을 손 댈수 있기 때문에 쓰면 안됨
            trailerWindow.ShowDialog(); // dialog로 열어야 함

        }


        //검색 결과 중에서 좋아하는 영화 저장
        private async void BtnAddFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "즐겨찾기에 추가할 영화를 선택하세요!");
                return;
            }

            if (isFavorite)
            {
                await Commons.ShowMessageAsync("오류", "이미 즐겨찾기한 영화입니다!");
                return;
            }

            List<FavoriteMovieItem> list = new List<FavoriteMovieItem>();

            foreach (MovieItem item in GrdResult.SelectedItems)
            {
                var favoriteMovie = new FavoriteMovieItem
                {
                    Id = item.Id,
                    Title = item.Title,
                    Orignal_Language = item.Original_Language,
                    Adult = item.Adult,
                    Vote_Average = item.Vote_Average,
                    Original_Title = item.Original_Title,
                    Overview = item.Overview,
                    Popularity = item.Popularity,
                    Poster_Path = item.Poster_Path,
                    Release_Date = item.Release_Date,
                    Reg_Date = DateTime.Now
                };

                list.Add(favoriteMovie);
            }


            try
            {
                // DB 연결 확인
                using (SqlConnection conn = new SqlConnection(Commons.connString))
                {
                    if (conn.State == System.Data.ConnectionState.Closed) conn.Open();

                    // 
                    var query = @"INSERT INTO [dbo].[FavoriteMovieItem]
                                                ([Id]           
                                                ,[Title]           
                                                ,[Original_Title]           
                                                ,[Release_Date]           
                                                ,[Orignal_Language]           
                                                ,[Adult]           
                                                ,[Vote_Average]           
                                                ,[Poster_Path]           
                                                ,[Overview]           
                                                ,[Reg_Date]           
                                                ,[Popularity])     
                                            VALUES           
                                                (@Id           
                                                ,@Title           
                                                ,@Original_Title           
                                                ,@Release_Date           
                                                ,@Orignal_Language           
                                                ,@Adult           
                                                ,@Vote_Average           
                                                ,@Poster_Path           
                                                ,@Overview           
                                                ,@Reg_Date           
                                                ,@Popularity)";
                    var insRes = 0;
                    foreach (var item in list)
                    {
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Id", item.Id);
                        cmd.Parameters.AddWithValue("@Title", item.Title);
                        cmd.Parameters.AddWithValue("@Original_Title", item.Original_Title);
                        cmd.Parameters.AddWithValue("@Release_Date", item.Release_Date);
                        cmd.Parameters.AddWithValue("@Orignal_Language", item.Orignal_Language);
                        cmd.Parameters.AddWithValue("@Adult", item.Adult);
                        cmd.Parameters.AddWithValue("@Vote_Average", item.Vote_Average);
                        cmd.Parameters.AddWithValue("@Poster_Path", item.Poster_Path);
                        cmd.Parameters.AddWithValue("@Overview", item.Overview);
                        cmd.Parameters.AddWithValue("@Reg_Date", item.Reg_Date);
                        cmd.Parameters.AddWithValue("@Popularity", item.Popularity);

                        insRes += cmd.ExecuteNonQuery();
                    }

                    if (list.Count == insRes)
                    {
                        await Commons.ShowMessageAsync("저장", $"저장 완료 : {insRes}개");

                    }

                }
            }
            catch
            {
                await Commons.ShowMessageAsync("오류", "DB저장 실패 및 중복 저장 오류, 관리자에게 문의하세용!");
            }
        }

        private async void BtnViewFavorite_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;
            TxtMovieName.Text = string.Empty;

            List<FavoriteMovieItem> list = new List<FavoriteMovieItem>();

            try
            {
                // DB 연결 확인
                using (SqlConnection conn = new SqlConnection(Commons.connString))
                {
                    if (conn.State == System.Data.ConnectionState.Closed) conn.Open();

                    // 
                    var query = @"SELECT Id
                                  ,Title
                                  ,Original_Title
                                  ,Release_Date
                                  ,Orignal_Language
                                  ,Adult
                                  ,Vote_Average
                                  ,Poster_Path
                                  ,Overview
                                  ,Reg_Date
                                  ,Popularity
                              FROM FavoriteMovieItem
                              ORDER BY Id ASC";
                    var cmd = new SqlCommand(query, conn);
                    var adapter = new SqlDataAdapter(cmd);
                    var dSet = new DataSet();
                    adapter.Fill(dSet, "FavoriteMovieItem");

                    foreach (DataRow dr in dSet.Tables["FavoriteMovieItem"].Rows)
                    {
                        list.Add(new FavoriteMovieItem
                        {
                            Id = Convert.ToInt32(dr["id"]),
                            Title = Convert.ToString(dr["title"]),
                            Original_Title = Convert.ToString(dr["title"]),
                            Release_Date = Convert.ToString(dr["Release_Date"]),
                            Orignal_Language = Convert.ToString(dr["Orignal_Language"]),
                            Adult = Convert.ToBoolean(dr["Adult"]),
                            Vote_Average = Convert.ToDouble(dr["Vote_Average"]),
                            Poster_Path = Convert.ToString(dr["Poster_Path"]),
                            Overview = Convert.ToString(dr["Overview"]),
                            Reg_Date = Convert.ToDateTime(dr["Reg_Date"]),
                            Popularity = Convert.ToDouble(dr["Popularity"]),

                        });
                    }
                    this.DataContext = list;
                    isFavorite = true;
                    StsResult.Content = $"즐겨찾기 {list.Count}건 조회 완료";
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB 조회 오류 : {ex.Message}");
            }
        }




        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (isFavorite == false)
            {
                await Commons.ShowMessageAsync("오류", "즐겨찾기만 삭제할 수 있습니다");
                return;
            }

            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "삭제 할 영화를 선택하세요");
                return;
            }

            try
            {
                // DB 연결 확인
                using (SqlConnection conn = new SqlConnection(Commons.connString))
                {
                    if (conn.State == System.Data.ConnectionState.Closed) conn.Open();

                    // 
                    var query = "DELETE FROM FavoriteMovieItem WHERE Id = @Id";
                    var delRes = 0;

                    foreach (FavoriteMovieItem item in GrdResult.SelectedItems)
                    {
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Id", item.Id);

                        delRes += cmd.ExecuteNonQuery();
                    }

                    if (delRes == GrdResult.SelectedItems.Count)
                    {
                        StsResult.Content = $"즐겨찾기 {delRes}건 삭제 완료";
                        await Commons.ShowMessageAsync("삭제", "DB 삭제 성공 !!");
                    }
                    else
                    {
                        await Commons.ShowMessageAsync("삭제", "DB 삭제 일부 성공 !!");
                    }
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB 삭제 오류 {ex.Message}");
            }
            BtnViewFavorite_Click(sender, e);
        }
    }
}

