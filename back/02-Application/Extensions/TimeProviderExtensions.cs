namespace NoPrumo.Application.Extensions;

public static class TimeProviderExtensions
{
    // Horário local, o mesmo do CURRENT_TIMESTAMP que o MySQL grava em
    // created_at e updated_at. Em UTC, depois das 21h o dia já virou e a
    // obra apareceria atrasada um dia antes.
    public static DateTime Now(this TimeProvider timeProvider) =>
        timeProvider.GetLocalNow().DateTime;

    public static DateOnly Today(this TimeProvider timeProvider) =>
        DateOnly.FromDateTime(timeProvider.Now());
}