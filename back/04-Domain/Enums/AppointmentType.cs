namespace NoPrumo.Domain.Enums;

// Project vem primeiro de propósito: a coluna tem DEFAULT 'project', e o EF não
// manda o primeiro valor do enum, deixa o banco preencher. Se o primeiro fosse
// outro, o banco gravaria 'project' e a leitura não acharia o valor no enum.
public enum AppointmentType
{
    Project,
    Visit,
    Delivery,
    Inspection,
    Training,
    Meeting
}
