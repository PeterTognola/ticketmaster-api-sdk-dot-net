﻿
using System.Collections.Generic;

namespace TM.Discovery.V2.Models
{
    public class Genre
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Links Links { get; set; }

        public Embedded _embedded { get; set; }

        public class Embedded
        {
            public Embedded()
            {
                SubGenres = new List<SubGenre>();
            }

            public List<SubGenre> SubGenres { get; set; }
        }
    }
}
