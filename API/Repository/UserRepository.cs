using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DaitingAppDbContext   _context;

        private readonly IMapper _mapper;

        public UserRepository(DaitingAppDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper=mapper;
        }

        public async Task<MemberDto> GetMemberDtoAsync(string username)
        {
            return await _context.Users.Where(u=>u.UserName==username)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersDtosAsync(UserParams userParams)
        {

            var queries= _context.Users.AsQueryable();

            queries=queries.Where(u=>u.UserName!=userParams.CurrentUsername);
            queries=queries.Where(u=>u.Gender==userParams.Gender);

            var minDob=DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge-1));
            var maxDob=DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            queries=queries.Where(u=>u.DateOfBirth>=minDob && u.DateOfBirth<=maxDob);

            queries= userParams.OrderBy
             switch
            {
                "created" => queries.OrderByDescending(u=>u.Created),
                _ => queries.OrderByDescending(u=>u.LastActive)
            };
            
            return await PagedList<MemberDto>.CreateAsync(
            queries.AsNoTracking().ProjectTo<MemberDto>(_mapper.ConfigurationProvider),
            userParams.PageNumber,
            userParams.PageSize);
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.
            Include(p=>p.Photos).
            SingleOrDefaultAsync(u=>u.UserName==username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users.
            Include(p=>p.Photos).
            ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync()>0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State=EntityState.Modified;
        }
    }
}