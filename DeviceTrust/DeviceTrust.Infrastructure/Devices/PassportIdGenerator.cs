using DeviceTrust.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DeviceTrust.Infrastructure.Devices;

public class PassportIdGenerator
{
    // Excludes 0, O, 1, I, L — visually ambiguous characters, bad for a code someone reads off a QR/label.
    private const string Alphabet = "23456789ABCDEFGHJKMNPQRSTUVWXYZ";
    private readonly Random _random = new();
    private readonly DeviceTrustDbContext _context;

    public PassportIdGenerator(DeviceTrustDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateAsync()
    {
        string candidate;
        bool exists;

        do
        {
            var suffix = new string(Enumerable.Range(0, 6)
                .Select(_ => Alphabet[_random.Next(Alphabet.Length)])
                .ToArray());
            candidate = $"DVT-{suffix}";

            exists = await _context.Devices.AnyAsync(d => d.PublicPassportId == candidate);
        }
        while (exists);

        return candidate;
    }
}