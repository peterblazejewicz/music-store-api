using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace MusicStore.Api.Models.Dto
{
    [ModelMetadataType(typeof(Album))]
    public class AlbumChangeDto : IValidatableObject
    {
        public int GenreId { get; set; }

        public int ArtistId { get; set; }

        public string Title { get; set; }

        public decimal Price { get; set; }

        public string AlbumArtUrl { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // An example of object-level (i.e., multi-property) validation
            if (this.GenreId == 13 /* Indie */)
            {
                switch (SentimentAnalysis.GetSentiment(Title))
                {
                    case SentimentAnalysis.SentimentResult.Positive:
                        yield return new ValidationResult("Sounds too positive. Indie music requires more ambiguity.");
                        break;
                    case SentimentAnalysis.SentimentResult.Negative:
                        yield return new ValidationResult("Sounds too negative. Indie music requires more ambiguity.");
                        break;
                }
            }
        }
    }
}
