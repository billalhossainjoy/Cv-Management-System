// using CVMS.Application.Services;
// using CVMS.Domain.Entities;
// using CVMS.Infrastructure.Persistence;
// using Microsoft.EntityFrameworkCore;
//
// namespace CVMS.Infrastructure.Services;
//
// public class ProfileService: IProfileService
//
// {
//     private readonly ApplicationDbContext  _context;
//
//     public ProfileService(ApplicationDbContext context)
//     {
//         _context = context;
//     }
//     
//     public async Task<Profile?> GetProfileAsync(Guid userId)
//     {
//         return await _context.Profiles.FirstAsync(x => x.UserId == userId);
//     }
// }