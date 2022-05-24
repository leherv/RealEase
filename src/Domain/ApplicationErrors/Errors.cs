using Domain.Model;

namespace Domain.ApplicationErrors;

public static class Errors
{
    public static class Validation
    {
        public const string InvariantViolationErrorCode = "invariant.violation";
        public static Error InvariantViolationError(
            string entityName,
            IEnumerable<string> violationList
        ) =>
            new(
                InvariantViolationErrorCode,
                $"Invariants of Entity '{entityName}' were violated.",
                violationList
            );

        public const string RuleViolationErrorCode = "rule.violation";
        public static Error RuleViolationError(
            string message
        ) =>
            new(
                RuleViolationErrorCode,
                message
            );
    }

    public static class General
    {
        public const string NotFoundErrorCode = "entity.not.found";
        public static Error NotFound(string entityName) => 
            new(NotFoundErrorCode, $"'{entityName}' not found.");
    }

    public static class Subscriber
    {
        public const string UnsubscribeFailedErrorCode = "unsubsribe.failed";
        public static Error UnsubscribeFailedError(Guid mediaId)
            => new(UnsubscribeFailedErrorCode, $"Unsubscribing from media with id'{mediaId}' failed.");
    }

    public static class Media
    {
        public const string PublishNewReleaseErrorCode = "publish.release.failed";
        public static Error PublishNewReleaseFailedError(Release release)
            => new(PublishNewReleaseErrorCode, $"Publishing new release {release} failed.");

        public const string MediaWithScrapeTargetExistsErrorCode = "media.with.scrapetarget.exists";
        public static Error MediaWithScrapeTargetExistsError(string mediaName) =>
            new Error(MediaWithScrapeTargetExistsErrorCode,$"Media with name {mediaName} already configured for URI.");
        
        public const string MediaWithNameExistsErrorCode = "media.with.name.exists";
        public static Error MediaWithNameExistsError(string mediaName) =>
            new Error(MediaWithNameExistsErrorCode,$"Media with name {mediaName} already exists.");
        
        public const string ScrapeTargetExistsErrorCode = "scrapeTarget.exists";
        public static Error ScrapeTargetExistsError(string mediaName) =>
            new Error(ScrapeTargetExistsErrorCode,$"ScrapeTarget already exists for media with name {mediaName}.");
    }

    public static class Scraper
    {
        public const string ScrapeFailedErrorCode = "scrape.failed";
        public static Error ScrapeFailedError(string reason) =>
            new Error(ScrapeFailedErrorCode, $"Scraping for new media release failed due to: {reason}.");
        
        public const string ScrapeMediaNameFailedErrorCode = "scrape.medianame.failed";
        public static Error ScrapeMediaNameFailedError(string reason) =>
            new Error(ScrapeMediaNameFailedErrorCode, $"Scraping for media name failed due to: {reason}.");
    }

    public static class Notification
    {
        public const string NoSubscriberForExternalIdentifierFoundErrorCode = "no.subscriber.for.externalIdentifier";
        public static Error NoSubscriberForExternalIdentifierError(string externalIdentifier) =>
            new Error(NoSubscriberForExternalIdentifierFoundErrorCode,
                $"No subscriber for externalIdentifier {externalIdentifier} found.");

        public const string MalformedExternalIdentifierErrorCode = "malformed.externalIdentifier";
        public static Error MalformedExternalIdentifierError(string externalIdentifier) =>
            new Error(MalformedExternalIdentifierErrorCode,
                $"ExternalIdentifier {externalIdentifier} does not have the expected format.");

        public const string NotifyingSubscriberFailedErrorCode = "notify.subscriber.failed";

        public static Error NotifyingSubscriberFailedError(string externalIdentifier) =>
            new Error(NotifyingSubscriberFailedErrorCode,
                $"Subscriber with {externalIdentifier} could not be notified.");
    }
}