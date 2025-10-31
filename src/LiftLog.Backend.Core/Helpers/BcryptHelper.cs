using LiftLog.Backend.Core.Options;
using Microsoft.Extensions.Options;

namespace LiftLog.Backend.Core.Helpers;

public class BcryptHelper(IOptions<BcryptOptions> options)
{
    private readonly int _workFactor = options.Value.WorkFactor;

    public string Encrypt(string input) => BCrypt.Net.BCrypt.HashPassword(input, _workFactor);

    public bool Verify(string input, string hash) => BCrypt.Net.BCrypt.Verify(input, hash);
}
