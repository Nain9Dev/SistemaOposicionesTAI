using Oposiciones.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Oposiciones.Domain.Interfaces
{
    public interface ISyllabusRepository
    {
        Task<IEnumerable<SyllabusBlock>> GetBlocksAsync();
        Task<IEnumerable<SyllabusTopic>> GetTopicsByBlockAsync(int blockId);
    }
}