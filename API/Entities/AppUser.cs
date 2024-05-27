
using Microsoft.AspNetCore.Identity;


namespace API.Entities
{
    public class AppUser :IdentityUser<int>
    {

        public DateOnly DateOfBirth { get; set; }   

        public string KnownAs { get; set; }

        private DateTime created;

        public DateTime Created
        {
            get => created;
            set => created = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        private DateTime lastActive;
        public DateTime LastActive
        {
            get => lastActive;
            set => lastActive = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
        public string Gender { get; set; }  

        public string Introduction { get; set; }

        public string LookingFor { get; set; }

        public string Interests { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public ICollection<Photo> Photos { get; set; }= new List<Photo>();

        public ICollection<UserLike> LikedByUsers { get; set; }

        public ICollection<UserLike> LikedUsers { get; set; }

        public ICollection<Message> MessagesSent { get; set; }

        public ICollection<Message> MessagesReceived { get; set; }

        public ICollection<AppUserRole> UserRoles { get; set; }

                
    }

    
}


