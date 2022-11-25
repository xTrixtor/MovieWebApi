namespace MyAwesomeWebApi.Models.Movie
{
    public class MovieModel
    {
        public int movie_id { get; set; }
        public string? title { get; set; }
        public string? overview { get; set; }
        public int popularity { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }

    }
}
