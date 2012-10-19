using System;
using System.Collections.Generic;

namespace BookKeeper
{
  public class AuditEntry
  {
    public AuditEntry()
    {
      Id = ShortGuid.NewGuid();
      TimeStamp = DateTime.UtcNow;
    }
    /// <summary>
    /// Id derived from ShortGuid
    /// </summary>
    public string Id { get; private set; }
    /// <summary>
    /// List of strings representing path to delete.
    /// </summary>
    public List<string> Deletes { get; set; }
    /// <summary>
    /// List of K/V pairs, where Key is the path to the property and Value is the updated property value.
    /// </summary>
    public List<KeyValuePair<string, string>> Updates { get; set; }
    /// <summary>
    /// DateTime when the AuditEntry was created.
    /// </summary>
    public DateTime TimeStamp { get; private set; }
    /// <summary>
    /// UserId of the user that made changes.
    /// </summary>
    public Dictionary<string,string> Meta { get; set; }
  }
}
