namespace truename;

public class MulliganDecision
{
  public bool Keep { get; set; }
  public int Taken { get; set; }
  public List<string> PutBack { get; set; } = new();
}