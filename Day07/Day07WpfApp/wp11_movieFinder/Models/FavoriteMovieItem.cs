﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wp11_movieFinder.Models
{
    public class FavoriteMovieItem
    {
        public string Title { get; set; }
        public string Original_Title { get; set; }
        public string Release_Date { get; set; }
        public double Popularity { get; set; }
        public double Vote_Average { get; set; }
        public int Id { get; set; }
        public string Overview { get; set; }
        public string Orignal_Language { get; set; }
        public string Poster_Path { get; set; }
        public bool Adult { get; set; }
        public DateTime Reg_Date { get; set; }
    }
}
