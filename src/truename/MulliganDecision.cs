namespace truename;

public class MulliganDecision
{
  public bool Keep { get; set; }
  public int Taken { get; set; }
  public bool Done { get; set; }
  public List<Card> PutBack { get; set; } = new();
}