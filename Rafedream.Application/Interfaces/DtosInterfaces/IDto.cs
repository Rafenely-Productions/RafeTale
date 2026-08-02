namespace Rafedream.Application.Interfaces.DtosInterfaces
{
    public interface IDto
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
    }
}