namespace NoPrumo.Tests.Fakes;

// Relógio parado num dia conhecido, em UTC, para o "hoje" do teste não depender de quando ele roda.
public sealed class FixedTimeProvider(DateTime now) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => new(DateTime.SpecifyKind(now, DateTimeKind.Utc));

    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
}
