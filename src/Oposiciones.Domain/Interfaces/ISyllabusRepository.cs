using Oposiciones.Domain.Entities;

namespace Oposiciones.Domain.Interfaces
{
    public interface ISyllabusRepository
    {
        Task<IReadOnlyList<SyllabusBlock>> GetBlocksAsync(CancellationToken cancellationToken = default);

        /// <summary>Temas de un bloque; si blockId es nulo devuelve el temario completo.</summary>
        Task<IReadOnlyList<SyllabusTopic>> GetTopicsByBlockAsync(int? blockId, CancellationToken cancellationToken = default);
    }
}
