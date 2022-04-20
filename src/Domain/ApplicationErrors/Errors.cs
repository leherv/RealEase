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
        public static Error UnsubscribeFailedError(string mediaName)
            => new(UnsubscribeFailedErrorCode, $"Unsubscribing from '{mediaName}' failed.");
    }

    public static class Media
    {
        public const string PublishNewReleaseErrorCode = "publish.release.failed";
        public static Error PublishNewReleaseFailedError(Release release)
            => new(PublishNewReleaseErrorCode, $"Publishing new release {release} failed.");
    }

    public static class Scraper
    {
        public const string ScrapeFailedErrorCode = "scrape.failed";
        public static Error ScrapeFailedError() =>
            new Error(ScrapeFailedErrorCode, "Scraping for new media release failed.");
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