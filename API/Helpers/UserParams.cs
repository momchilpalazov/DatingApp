namespace API.Helpers;

public class UserParams: PaginationParams
{

    public string CurrentUsername { get; set; }

    public  string Gender { get; set; } 

    public int MinAge { get; set; } = 18;

    public int MaxAge { get; set; } = 100;

    public string OrderBy { get; set; } = "lastActive";

    public bool Likees { get; set; } = false;

    public bool Likers { get; set; } = false;

    public bool IsLiked { get; set; } = false;

    public bool IsMessage { get; set; } = false;

    public bool IsPhoto { get; set; } = false;

    public bool IsUser { get; set; } = false;

    public bool IsUserForUpdate { get; set; } = false;

    public bool IsUserForRegister { get; set; } = false;
    

}
