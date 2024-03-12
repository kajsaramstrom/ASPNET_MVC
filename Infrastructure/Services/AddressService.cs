using Infrastructure.Context;
using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class AddressService(DataContext context)
{
    private readonly DataContext _context = context;

    public async Task<AddressEntity> GetAddressAsync(string UserId)
    {
        var addressEntity = await _context.Addresses.FirstOrDefaultAsync(x => x.UserId == UserId);
        return addressEntity!;
    }

    public async Task<bool> CreateAddressAsync(AddressEntity entity)
    {
        _context.Addresses.Add(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAddressAsync(AddressEntity entity)
    {
        var existing = await _context.Addresses.FirstOrDefaultAsync(x => x.UserId == entity.UserId);
        if (existing != null)
        {
            _context.Entry(existing).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        return false;
    }

    public async Task<bool> RemoveAddressAsync(string userId)
    {
        var existing = await _context.Addresses.FirstOrDefaultAsync(x => x.UserId == userId);
        if (existing != null)
        {
            _context.Addresses.Remove(existing);
            await _context.SaveChangesAsync();

            return true;
        }

        return false;
    }
}
