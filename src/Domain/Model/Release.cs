namespace Domain.Model;

public class Release
{
    public ReleaseNumber ReleaseNumber { get; }
    public Media Media { get; }

    public Release(ReleaseNumber releaseNumber, Media media)
    {
        ReleaseNumber = releaseNumber;
        Media = media;
    }

    public static Release Create(ReleaseNumber releaseNumber, Media media)
    {
        return new Release(releaseNumber, media);
    }
}