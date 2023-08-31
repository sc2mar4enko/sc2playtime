namespace sc2playtime.Models;

public class PlaytimeModel
{
    public PlaytimeModel(int games, double hours)
    {
        Games = games;
        Hours = hours;
    }

    private int Games { get; set; }

    private double Hours { get; set; }

    public int GetGames()
    {
        return Games;
    }

    public double GetHours()
    {
        const double timeMultiplier = 0.7138;
        var hours = Hours / 3600 * timeMultiplier;
        return Math.Round(hours, 1);
    }
}