using Rinha_de_backend.Dtos;
using System.Text.Json.Serialization;

namespace Rinha_de_backend;

[JsonSerializable(typeof(TransacaoDto))]
[JsonSerializable(typeof(ClienteDto))]
[JsonSerializable(typeof(List<UltimasTrasacoesDto>))]
[JsonSerializable(typeof(ExtratoDto))]
[JsonSerializable(typeof(ClienteExtratoDto))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(DateTime))]
partial class JsonContextSerialization : JsonSerializerContext
{
}
