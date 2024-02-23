using System.Text.Json.Serialization;

namespace RinhaBackend.Handlers;

[JsonSerializable(typeof(TransactionDbResponse))]
[JsonSerializable(typeof(SaldoResponse))]
[JsonSerializable(typeof(ExtratoResponse))]
[JsonSerializable(typeof(UltimaTransacaoResponse))]
[JsonSerializable(typeof(IEnumerable<UltimaTransacaoResponse>))]
[JsonSerializable(typeof(TransactionResponse))]
[JsonSerializable(typeof(TransactionRequest))]
public partial class ContextSerializer : JsonSerializerContext { }