namespace MusicStore.Api.Models.Dto
{
    public class AlbumResultDto : AlbumChangeDto
    {
        public AlbumResultDto()
        {
            Artist = new ArtistResultDto();
            Genre = new GenreResultDto();
        }

        public int AlbumId { get; set; }

        public ArtistResultDto Artist { get; private set; }

        public GenreResultDto Genre { get; private set; }
    }
}
