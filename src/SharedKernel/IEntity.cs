namespace SharedKernel;


public interface IEntity
{
  List<IDomainEvent> DomainEvents { get; }
  void ClearDomainEvents();
  void Raise(IDomainEvent domainEvent);

}
