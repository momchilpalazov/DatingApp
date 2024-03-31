using System.ComponentModel.DataAnnotations.Schema;


namespace API.Entities
{
    [Table("Photos")]  // This is the name of the table in the database
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }

        public string PublicId { get; set; }    // This is the ID that Cloudinary returns to us when we upload a photo to Cloudinary

        public int AppUserId { get; set; }    // This is the foreign key that we use to link the photo to the user
        public AppUser AppUser { get; set; }    // This is the navigation property that we use to link the photo to the user
        
    }
}