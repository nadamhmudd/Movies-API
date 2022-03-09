namespace Movies.Services.Extensions;

//Extension Methods for String
public static class StringExtension
{
    public static string CapitalizeFistLitter(this String text)
    {
        return string.Join(" ", text.Split(" ").ToList()
            .ConvertAll(word => word.Substring(0, 1).ToUpper() + word.Substring(1))
                );
    }
}
