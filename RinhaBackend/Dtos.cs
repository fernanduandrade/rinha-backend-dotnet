namespace RinhaBackend;

public sealed record TransactionRequest(float Valor, char Tipo, string Descricao);
public sealed record TransactionResponse(int Limite, int Saldo);

public sealed record SaldoResponse
{
  public int Total {get; init;}
  public int Limite {get; init;}
  public DateTime Data_Extrato {get; init;} = DateTime.Now;
}

public sealed record UltimaTransacaoResponse
{
  public int Valor {get; init;}
  public char Tipo {get; init;}
  public string? Descricao {get; init;}
  public DateTime RealizadaEm {get; init;}

}
public sealed record ExtratoResponse(SaldoResponse Saldo, IEnumerable<UltimaTransacaoResponse> UltimasTransacoes);
public sealed record TransactionDbResponse
{
  public int Novo_Saldo {get; init;}
  public int Limite_Cliente { get; init; }
  public bool Possui_Erro {get; init;}
  public string? Mesagem {get; init;}
}