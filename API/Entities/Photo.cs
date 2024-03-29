using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }

        public string PublicId { get; set; }    // This is the ID that Cloudinary returns to us when we upload a photo to Cloudinary
        
    }
}