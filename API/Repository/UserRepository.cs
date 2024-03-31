using API.Data;
using API.DTOs;
using API.Entities;
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

        public async Task<IEnumerable<MemberDto>> GetMembersDtosAsync()
        {
            return  await _context.Users
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
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