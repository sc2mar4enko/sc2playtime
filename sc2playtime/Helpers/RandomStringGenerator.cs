namespace sc2playtime.Helpers
{
    public interface IRandomStringGenerator
    {
        string GenerateRandomString();
    }

    public class RandomStringGenerator : IRandomStringGenerator
    {
        public string GenerateRandomString()
        {
            const string chars = "qwertyuiopasdfghjklzxcvbnm1234567890";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, 5)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}



